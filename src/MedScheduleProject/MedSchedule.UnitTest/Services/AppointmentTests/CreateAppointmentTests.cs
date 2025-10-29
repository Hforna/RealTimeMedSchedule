using FluentAssertions;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Exceptions;
using MedSchedule.UnitTest.Commons.Fakers.Entities;
using MedSchedule.UnitTest.Commons.Fakers.Requests;
using MedSchedule.UnitTest.Commons.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Services.AppointmentTests
{
    public class CreateAppointmentTests
    {
        [Fact]
        public async Task CreateAppointment_DateToAppointmentOutOfRange_ThrowDomainException()
        {
            var request = CreateAppointmentRequestFaker.Generate();
            request.Time = DateTime.UtcNow.AddDays(-1);

            var service = new CreateAppointmentService().Create();

            var result = async () => await service.CreateAppointment(request);

            await result
                .Should()
                .ThrowExactlyAsync<DomainException>("Appointment time must be longer than today");
        }

        [Fact]
        public async Task CreateAppointment_RequestNotProvidedToken_ThrowRequestException()
        {
            var request = CreateAppointmentRequestFaker.Generate();

            var service = new CreateAppointmentService().Create();

            var result = async () => await service.CreateAppointment(request);

            await result
                .Should()
                .ThrowExactlyAsync<RequestException>("User must be authenticated for create an appointment");
        }

        [Fact]
        public async Task CreateAppointment_InvalidSpecialtyName_ThrowRequestException()
        {
            var request = CreateAppointmentRequestFaker.Generate();
            request.SpecialtyName = "Invalid specialty";

            var tokenServ = new TokenServiceMock();
            tokenServ.GetMock()
                .Setup(d => d.GetUserGuidByToken())
                .Returns(Guid.NewGuid());
            var userRepos = new UserRepositoryMock();
            userRepos.GetMock().Setup(d => d.GetUserById(It.IsAny<Guid>())).ReturnsAsync(UserEntityFaker.Generate());
            var uow = new UnitOfWorkMock().Create(userRepository: userRepos.GetObject());

            var service = new CreateAppointmentService().Create(tokenService: tokenServ.GetObject(), uow: uow);

            var result = async () => await service.CreateAppointment(request);

            await result
                .Should()
                .ThrowExactlyAsync<RequestException>("Specialty name not exists");
        }

        [Fact]
        public async Task CreateAppointment_StaffForSpecialtyNotExists_ThrowResourceNotFoundException()
        {
            var request = CreateAppointmentRequestFaker.Generate();
            var specialty = SpecialtyEntityFaker.Generate();
            specialty.Name = request.SpecialtyName;

            var tokenServ = new TokenServiceMock();
            tokenServ.GetMock()
                .Setup(d => d.GetUserGuidByToken())
                .Returns(Guid.NewGuid());
            var userRepos = new UserRepositoryMock();
            userRepos.GetMock().Setup(d => d.GetUserById(It.IsAny<Guid>())).ReturnsAsync(UserEntityFaker.Generate());
            userRepos.GetMock().Setup(d => d.SpecialtyByName(request.SpecialtyName)).ReturnsAsync(specialty);
            var uow = new UnitOfWorkMock().Create(userRepos.GetObject());
            var service = new CreateAppointmentService().Create(uow, tokenService: tokenServ.GetObject());

            var result = async () => await service.CreateAppointment(request);

            await result
                .Should()
                .ThrowExactlyAsync<ResourceNotFoundException>($"There aren't {specialty.Name} avaliable");
        }

        [Fact]
        public async Task CreateAppointment_NoAvaliableStaffAtMoment_ThrowUnavaliableException()
        {
            var request = CreateAppointmentRequestFaker.Generate();
            var specialty = SpecialtyEntityFaker.Generate();
            request.SpecialtyName = specialty.Name;

            var userRepos = new UserRepositoryMock();
            var tokenServ = new TokenServiceMock();
            tokenServ.GetMock()
                .Setup(d => d.GetUserGuidByToken())
                .Returns(Guid.NewGuid());
            userRepos.GetMock().Setup(d => d.GetUserById(It.IsAny<Guid>())).ReturnsAsync(UserEntityFaker.Generate());
            userRepos.GetMock().Setup(d => d.SpecialtyByName(request.SpecialtyName)).ReturnsAsync(specialty);
            userRepos
                .GetMock()
                .Setup(d => d.GetStaffsBySpecialty(It.IsAny<Guid>()))
                .ReturnsAsync(SpecialtyEntityFaker.GenerateSpecialtyStaffsInRange(20, request.SpecialtyName));

            var uow = new UnitOfWorkMock().Create(userRepos.GetObject());
            var service = new CreateAppointmentService().Create(uow, tokenService: tokenServ.GetObject());

            var result = async () => await service.CreateAppointment(request);

            await result
                .Should()
                .ThrowExactlyAsync<UnavaliableException>($"There aren't {specialty.Name} professionals avaliable at this time");
        }

        [Fact]
        public async Task CreateAppointment_StaffWithLessAppointments_ReturnAppointmentWithThem()
        {
            var request = CreateAppointmentRequestFaker.Generate();
            var specialty = SpecialtyEntityFaker.Generate();
            request.SpecialtyName = specialty.Name;

            var specialtyStaffs = SpecialtyEntityFaker.GenerateSpecialtyStaffsInRange(20, request.SpecialtyName);

            var userRepos = new UserRepositoryMock();
            var tokenServ = new TokenServiceMock();
            tokenServ.GetMock()
                .Setup(d => d.GetUserGuidByToken())
                .Returns(Guid.NewGuid());
            userRepos.GetMock().Setup(d => d.GetUserById(It.IsAny<Guid>())).ReturnsAsync(UserEntityFaker.Generate());
            userRepos.GetMock().Setup(d => d.SpecialtyByName(request.SpecialtyName)).ReturnsAsync(specialty);
            userRepos
                .GetMock()
                .Setup(d => d.GetStaffsBySpecialty(It.IsAny<Guid>()))
                .ReturnsAsync(specialtyStaffs);
            userRepos
                .GetMock()
                .Setup(d => d.GetAllSpecialtyStaffAvaliableByIds(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(specialtyStaffs.Take(5).ToList());
            var staffLessApp = StaffEntityFaker.Generate();
            var profInfos = StaffEntityFaker.GenerateProfessionalInfos();
            profInfos.Staff = staffLessApp;
            profInfos.StaffId = staffLessApp.Id;
            staffLessApp.ProfessionalInfos = profInfos;
            var appointmentRepos = new AppointmentRepositoryMock();
            appointmentRepos.GetMock().Setup(d => d.GetStaffWithLessAppointments(It.IsAny<List<Staff>>())).ReturnsAsync(staffLessApp);
            var queueRoot = QueueEntityFaker.GenerateRoot();
            var queueRepos = new QueueRepositoryMock();
            queueRepos.GetMock()
                .Setup(d => d.GetQueueRoot(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(queueRoot);
            var queueDomain = new QueueDomainMock();
            queueDomain
                .GetMock()
                .Setup(d => d.SetQueuePosition(It.IsAny<QueueRoot>(), It.IsAny<Appointment>()))
                .ReturnsAsync(QueueEntityFaker.GeneratePosition());


            var uow = new UnitOfWorkMock().Create(userRepos.GetObject(), 
                queueRepository: queueRepos.GetObject(), 
                appointmentRepository: appointmentRepos.GetObject());
            var service = new CreateAppointmentService().Create(uow, tokenService: tokenServ.GetObject(), queueDomain: queueDomain.GetObject());

            var result = async () => await service.CreateAppointment(request);
            var response = await result();
            await result.Should().NotThrowAsync();
            response.StaffId = staffLessApp.Id;
        }
    }
}
