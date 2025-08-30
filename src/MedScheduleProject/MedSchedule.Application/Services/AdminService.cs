using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using MedSchedule.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MedSchedule.Application.Services
{
    public interface IAdminService
    {
        public Task<StaffResponse> CreateNewStaff(CreateNewStaffRequest request);
        public Task<StaffResponse> AssignSpecialtyToStaff(SetSpecialtyToStaffRequest request);
        public Task<StaffsResponse> GetAllStaffsPaginated(StaffsPaginatedRequest request);
    }

    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AdminService> _logger;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AdminService(IMapper mapper, IUnitOfWork uow, 
            ILogger<AdminService> logger, IEmailService emailService, ITokenService tokenService)
        {
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<StaffResponse> CreateNewStaff(CreateNewStaffRequest request)
        {
            if (string.IsNullOrEmpty(request.Role) || StaffRoles.IsValidRole(request.Role) == false)
                throw new RequestException("Cannot create staff with this role");

            var userStaff = await _uow.UserRepository.GetUserById(request.UserId)
                ?? throw new ResourceNotFoundException("The user by id was not found");

            var staffExists = await _uow.UserRepository.UserStaffExists(userStaff.Id);

            if (staffExists)
                throw new ConflictException("Staff already exists");

            var staff = new Staff()
            {
                WorkShift = _mapper.Map<WorkShift>(request.WorkShift),
                UserId = request.UserId,
                Role = request.Role.ToLower()
            };

            var professionalInfos = new ProfessionalInfos()
            {
                StaffId = staff.Id,
            };

            await _uow.GenericRepository.Add<ProfessionalInfos>(professionalInfos);
            await _uow.GenericRepository.Add<Staff>(staff);
            await _uow.Commit();

            return _mapper.Map<StaffResponse>(staff);
        }

        public async Task<StaffResponse> AssignSpecialtyToStaff(SetSpecialtyToStaffRequest request)
        {
            var userUid = _tokenService.GetUserGuidByToken() 
                ?? throw new NotAuthenticatedException("User must be authenticated for set specialty");

            var user = await _uow.UserRepository.GetUserById(userUid);

            var staff = await _uow.UserRepository.StaffById(request.StaffId);

            if (staff.Role != StaffRoles.Professional)
                throw new DomainException("Staff must be a professional to assign a specialty");

            if(staff is null)
            {
                _logger.LogError($"Attempt to get staff of an admin failed, staff not exists, staff id: {request.StaffId}");

                throw new RequestException("The staff was not found");
            }

            var specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName)
                ?? throw new RequestException("Specialty with this name was not found");

            var professionalInfos = await _uow.UserRepository.GetProfessionalInfosByStaffId(staff.Id);

            using var transaction = await _uow.BeginTransaction();

            try
            {            
                if(professionalInfos is null)
                {
                    professionalInfos = new ProfessionalInfos() { SpecialtyId = specialty.Id, StaffId = staff.Id };
                    await _uow.GenericRepository.Add<ProfessionalInfos>(professionalInfos);
                }
                else
                {
                    professionalInfos.SpecialtyId = specialty.Id;
                    _uow.GenericRepository.Update<ProfessionalInfos>(professionalInfos);
                }
                await _uow.Commit();
                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured");
                await transaction.RollbackAsync();

                throw new ExternalServerException("It was not possible send the email to staff, try again");
            }

            try 
            {
                await SendEmailToStaffWhenSpecialtyIsSet($"" +
                $"{staff.User!.FirstName} {staff.User.LastName}",
                            staff.User.Email!,
                $"{user.FirstName} {user.LastName}",
                specialty.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The email couldn't be sent to staff: {staff.User!.UserName}");

                throw new InternalServerException("An unexpected error occured while trying to set staff specialty");
            }

            return _mapper.Map<StaffResponse>(staff);
        }

        private async Task SendEmailToStaffWhenSpecialtyIsSet(string staffName, string staffEmail, string adminName, string specialty)
        {
            var body =  $@"<!DOCTYPE html>
            <html>
            <head>
                <style>
                    .card {{ border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; }}
                    .highlight {{ background-color: #f8f9fa; padding: 5px 10px; border-radius: 4px; }}
                </style>
            </head>
            <body>
                <p>Dear {staffName},</p>
    
                <p>Your professional specialty has been updated to:</p>
                <div class=""card"">
                    <h3>{specialty}</h3>
                </div>

                <div style=""margin-top: 25px;"">
                    <p><strong>Details:</strong></p>
                    <ul>
                        <li>Updated by: {adminName}</li>
                        <li>Effective date: {DateTime.UtcNow}</li>
                        <li>System: Employee Management Portal</li>
                    </ul>
                </div>
    
                <p>This change will affect your assigned cases and responsibilities.</p>
    
                <div style=""background-color: #fff8e1; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;"">
                    <p>If this was made in error, please contact HR immediately.</p>
                </div>
    
                <p>Regards,<br>The Administration Team</p>
            </body>
            </html>";

            var subject = "You staff specialty has been updated";

            await _emailService.SendEmail(body, subject, staffEmail, staffName);
        }

        public async Task<StaffsResponse> GetAllStaffsPaginated(StaffsPaginatedRequest request)
        {
            if (request.PerPage > 100)
                throw new RequestException("The max per page must be 100");

            if (!string.IsNullOrEmpty(request.SpecialtyName))
            {
                var specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName)
                    ?? throw new RequestException("Specialty with this name was not found");
            }

            var mapFilterDto = _mapper.Map<StaffPaginatedFilterDto>(request);

            var staffs = _uow.UserRepository.GetAllStaffsPaginated(mapFilterDto);

            var response = new StaffsResponse();
            response.Staffs = staffs.Select(staff => _mapper.Map<StaffShortResponse>(staff)).ToList();
            response.Count = staffs.Count;
            response.IsFirstPage = staffs.IsFirstPage;
            response.IsLastPage = staffs.IsLastPage;
            response.PageNumber = staffs.PageNumber;
            response.HasNextPage = staffs.HasNextPage;
            response.HasPreviousPage = staffs.HasPreviousPage;

            return response;
        }
    }
}
