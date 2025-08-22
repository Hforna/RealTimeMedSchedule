using AutoMapper;
using MedSchedule.Application.Services;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Services.Admin
{
    public class CreateAdminService
    {
        public AdminService Create(IUnitOfWork? uow = null, IEmailService? emailService = null, 
            ITokenService? tokenService = null, ILogger<AdminService>? logger = null, IMapper? mapper = null)
        {
            uow = uow ?? new Mock<IUnitOfWork>().Object;
            emailService = emailService ?? new Mock<IEmailService>().Object;
            tokenService = tokenService ?? new Mock<ITokenService>().Object;
            logger = logger ?? new Mock<ILogger<AdminService>>().Object;
            mapper = mapper ?? new Mock<IMapper>().Object;

            return new AdminService(mapper, uow, logger, emailService, tokenService);
        }
    }
}
