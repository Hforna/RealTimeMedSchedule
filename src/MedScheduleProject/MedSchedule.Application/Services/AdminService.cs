using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
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
        public Task<StaffResponse> CreateNewProfessionalStaff(CreateNewStaffRequest request);
        public Task<StaffResponse> SetSpecialtyToStaff(SetSpecialtyToStaffRequest request);
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

        public async Task<StaffResponse> CreateNewProfessionalStaff(CreateNewStaffRequest request)
        {
            var userStaff = await _uow.UserRepository.GetUserById(request.UserId)
                ?? throw new ResourceNotFoundException("The user by id was not found");

            var specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName) 
                ?? throw new ResourceNotFoundException("The specialty was not found");

            var staff = new Staff()
            {
                WorkShift = _mapper.Map<WorkShift>(request.WorkShift),
                SpecialtyId = specialty.Id,
                UserId = request.UserId,
                Role = StaffRoles.Professional
            };

            await _uow.GenericRepository.Add<Staff>(staff);
            await _uow.Commit();

            return _mapper.Map<StaffResponse>(staff);
        }

        public async Task<StaffResponse> SetSpecialtyToStaff(SetSpecialtyToStaffRequest request)
        {
            var staff = await _uow.UserRepository.StaffById(request.StaffId);

            if(staff is null)
            {
                _logger.LogError($"Attempt to get staff of an admin failed, staff not exists, staff id: {request.StaffId}");

                throw new RequestException("The staff was not found");
            }

            if (staff.Specialty is not null)
                throw new DomainException("Staff already have a specialty set");

            var specialty = await _uow.UserRepository.SpecialtyByName(request.SpecialtyName)
                ?? throw new RequestException("Specialty with this name was not found");

            using var transaction = await _uow.BeginTransaction();

            if (staff.User is null)
                staff.User = await _uow.UserRepository.GetUserById(staff.UserId);

            var user = await _tokenService.GetUserByToken();

            try
            {
                staff.SpecialtyId = specialty.Id;

                if (string.IsNullOrEmpty(staff.Role))
                    staff.Role = StaffRoles.Professional;

                _uow.GenericRepository.Update<Staff>(staff);
                await _uow.Commit();
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
                await transaction.RollbackAsync();

                throw new InternalServerException("An unexpected error occured while trying set staff specialty");
            }
            await transaction.CommitAsync();

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
    }
}
