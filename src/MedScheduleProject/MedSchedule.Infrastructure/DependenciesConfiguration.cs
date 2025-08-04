using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Services;
using MedSchedule.Infrastructure.Persistence;
using MedSchedule.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure
{
    public static class DependenciesConfiguration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddIdentity(services);
            AddServices(services, configuration);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("sqlserver");

            services.AddDbContext<ProjectDataContext>(d => d.UseSqlServer(connection));
        }

        static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            //expires in minutes
            int expiresAt = int.Parse(configuration["services:auth:token:expiresAt"]!);
            string signKey = configuration["services:auth:token:signKey"]!;

            services.AddScoped<ITokenService, TokenService>(d => new TokenService(signKey, expiresAt));

            services.AddSingleton<IPasswordEncryptService, BCryptService>();
        }

        static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentityCore<User>(d =>
            {
                d.User.RequireUniqueEmail = true;
                d.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ProjectDataContext>();
        }
    }
}
