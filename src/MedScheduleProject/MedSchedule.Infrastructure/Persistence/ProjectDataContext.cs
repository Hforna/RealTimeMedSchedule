using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedSchedule.Domain;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.PriorityAssignment;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Entities;
using MedSchedule.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MedSchedule.Infrastructure.Persistence
{
    public class ProjectDataContext : IdentityDbContext<User, Role, Guid>
    {
        public ProjectDataContext(DbContextOptions<ProjectDataContext> dbContext) : base(dbContext)
        {

        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PriorityOverride> PriorityOverrides { get; set; }
        public DbSet<QueueRoot> QueueRoots { get; set; }
        public DbSet<QueuePosition> QueuePositions { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Notification>().HasMany(d => d.Channels).WithOne(d => d.Notification);

            builder.Owned(typeof(ScheduleWork));

            builder.Entity<Staff>().HasOne(d => d.User);

            builder.ApplyConfigurationsFromAssembly(typeof(ProjectDataContext).Assembly);
        }

    }

    public class ProjectDataContextFactory : IDesignTimeDbContextFactory<ProjectDataContext>
    {
        public ProjectDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectDataContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\MedSchedule.WebApi\appsettings.Development.json"), 
                optional: false, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("sqlserver"));

            return new ProjectDataContext(optionsBuilder.Options);
        }
    }
}
