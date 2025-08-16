using MedSchedule.Application.Services;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application
{
    public static class DependenciesConfiguration
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddServices(services);
            AddMapper(services);
        }

        static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<IQueueDomainService, QueueDomainService>();
        }

        static void AddMapper(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile(new MapperService()));
        }
    }
}
