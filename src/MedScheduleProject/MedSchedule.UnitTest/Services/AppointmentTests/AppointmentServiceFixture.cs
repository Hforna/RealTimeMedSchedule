using AutoMapper;
using MedSchedule.Application.Services;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.Hubs;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Services.AppointmentTests
{
    public class AppointmentServiceFixture
    {
        public AppointmentService Create(IUnitOfWork? uow = null, IEmailService? emailService = null,
            ITokenService? tokenService = null, ILogger<AppointmentService>? logger = null, 
            IQueueDomainService? queueDomain = null, IMapper? mapper = null)
        {
            uow = uow ?? new Mock<IUnitOfWork>().Object;
            emailService = emailService ?? new Mock<IEmailService>().Object;
            tokenService = tokenService ?? new Mock<ITokenService>().Object;
            logger = logger ?? new Mock<ILogger<AppointmentService>>().Object;
            queueDomain = queueDomain ?? new Mock<IQueueDomainService>().Object;
            mapper = mapper ?? new Mock<IMapper>().Object;
            var queueHub = new Mock<IQueueHubService>().Object;

            return new AppointmentService(uow, tokenService, logger, queueDomain, mapper, emailService, queueHub);
        }
    }
}
