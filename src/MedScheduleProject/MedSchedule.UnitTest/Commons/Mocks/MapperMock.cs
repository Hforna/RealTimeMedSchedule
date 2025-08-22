using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedSchedule.Application;
using Microsoft.Extensions.Logging;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public static class MapperMock
    {
        public static IMapper Create()
        {
            var configurationExpression = new MapperConfigurationExpression();
            configurationExpression.AddProfile(new MapperService());

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });

            var config = new MapperConfiguration(configurationExpression, loggerFactory);

            return config.CreateMapper();
        }
    }
}
