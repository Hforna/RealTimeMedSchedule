using AutoMapper;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.ValueObjects;
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
        }
    }
}
