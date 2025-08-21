using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using MedSchedule.Infrastructure.Persistence;
using MedSchedule.Infrastructure.Repositories;
using MedSchedule.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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
            AddRepositories(services);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("sqlserver");

            services.AddDbContext<ProjectDataContext>(d => d.UseSqlServer(connection));
        }

        static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQueueRepository, QueueRepository>();
        }

        static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            //expires in minutes
            int expiresAt = int.Parse(configuration["services:auth:token:expiresAt"]!);
            string signKey = configuration["services:auth:token:signKey"]!;

            services.AddScoped<ITokenService, TokenService>(d => {
                using var scope = d.CreateScope();

                return new TokenService(
                    signKey,
                    expiresAt,
                    scope.ServiceProvider.GetRequiredService<IRequestService>(),
                    scope.ServiceProvider.GetRequiredService<IUserRepository>(),
                    scope.ServiceProvider.GetRequiredService<TokenValidationParameters>());
                });

            services.AddSingleton<IPasswordEncryptService, BCryptService>();
            services.AddScoped<IRequestService, RequestService>();

            //EmailConfiguration set on program.cs
            services.AddScoped<IEmailService, EmailService>();
        }

        static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentityCore<User>(d =>
            {
                d.User.RequireUniqueEmail = true;
                d.SignIn.RequireConfirmedEmail = true;
            }).AddRoles<Role>()
            .AddEntityFrameworkStores<ProjectDataContext>()
            .AddUserManager<UserManager<User>>();
        }
    }
}
