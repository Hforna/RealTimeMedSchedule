using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.ValueObjects;
using Pagination.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application
{
    public class MapperService : Profile
    {
        public MapperService()
        {
            CreateMap<ScheduleWork, ScheduleWorkResponse>();

            CreateMap<Staff, StaffResponse>();

            CreateMap<WorkShiftRequest, WorkShift>();

            CreateMap<StaffsPaginatedRequest, StaffPaginatedFilterDto>();

            CreateMap<ProfessionalInfos, ProfessionalInfosResponse>();

            CreateMap<QueuePosition, QueueInProgressDto>()
                .ForMember(d => d.EstimatedMinutes, f => f.MapFrom(d => d.EstimatedMinutes))
                .ForMember(d => d.RawPosition, f => f.MapFrom(d => d.RawPosition))
                .ForMember(d => d.LastUpdate, f => f.MapFrom(d => d.LastUpdate))
                .ForMember(d => d.AppointmentId, f => f.MapFrom(d => d.AppointmentId));

            CreateMap<Pagination<Appointment>, AppointmentPaginatedResponse>();
        }
    }
}
