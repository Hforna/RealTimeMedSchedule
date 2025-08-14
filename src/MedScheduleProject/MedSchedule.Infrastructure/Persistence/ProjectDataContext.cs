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

            builder.Entity<Specialty>().HasData(AddDefaultSpecialties());

            builder.Entity<Role>().HasData(
                new Role() { Id = new Guid("05c33ca5-2edd-478c-8472-aa51608c610f"), Name = StaffRoles.Admin, NormalizedName = StaffRoles.Admin.ToUpper() },
                new Role() { Id = new Guid("4a377ec7-d318-4e31-ae12-0342ab0f04d8"), Name = StaffRoles.Professional, NormalizedName = StaffRoles.Professional.ToUpper() },
                new Role() { Id = new Guid("78ea5b7f-45ec-4e9b-9522-9526034fa3b9"), Name = "patient", NormalizedName = "PATIENT" }
                );

            builder.ApplyConfigurationsFromAssembly(typeof(ProjectDataContext).Assembly);
        }

        private List<Specialty> AddDefaultSpecialties()
        {
            return new List<Specialty>()
    {
        new Specialty()
        {
            Id = new Guid("1126ccc3-2137-4a85-888d-b3ba4c73bf0f"),
            AvgConsultationTime = 30,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2606),
            MinEmergencySlots = 4,
            Name = "Ophthalmology",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new Specialty()
        {
            Id = new Guid("3e6b5514-5ab9-4410-8a15-ebfaaca453b5"),
            AvgConsultationTime = 20,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2602),
            MinEmergencySlots = 4,
            Name = "Orthopedics",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new Specialty()
        {
            Id = new Guid("6797cbd8-4b25-4274-8bc6-3f1550917bbc"),
            AvgConsultationTime = 30,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2589),
            MinEmergencySlots = 3,
            Name = "Endocrinology",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new Specialty()
        {
            Id = new Guid("7e8fc7a1-e211-42ea-9d7a-ae7166779c1f"),
            AvgConsultationTime = 90,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2588),
            MinEmergencySlots = 3,
            Name = "Oncology",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new Specialty()
        {
            Id = new Guid("9353101a-ab97-4144-a312-516b8d2cac0a"),
            AvgConsultationTime = 15,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2584),
            MinEmergencySlots = 4,
            Name = "Pediatrics",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        },
        new Specialty()
        {
            Id = new Guid("aa3c4f4a-d218-418e-858c-edc1720ecf59"),
            AvgConsultationTime = 20,
            CreatedAt = new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(1967),
            MinEmergencySlots = 2,
            Name = "Cardiology",
            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
        }
    };
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
