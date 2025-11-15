using FluentAssertions;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.Enums;
using MedSchedule.Domain.Exceptions;
using MedSchedule.UnitTest.Commons.Fakers.Entities;
using MedSchedule.UnitTest.Commons.Mocks;
using Moq;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Services.AppointmentTests
{
    public class NextAppointmentTests
    {
        [Fact]
        public async Task NextAppointment_NullGuidFromToken_ThrowNotAuthenticatedException()
        {
            var tokenMock = new TokenServiceMock();
            tokenMock.GetMock().Setup(d => d.GetUserGuidByToken()).Returns<Guid?>(null);

            var service = new AppointmentServiceFixture().Create(tokenService: tokenMock.GetObject());

            var result = async () => await service.NextAppointment();

            await result.Should()
                .ThrowAsync<NotAuthenticatedException>("User must be authenticated for create an appointment");
        }

        [Fact]
        public async Task NextAppointment_UserNotStaff_ThrowUnauthorizedException()
        {
            var tokenMock = new TokenServiceMock();
            var userUid = Guid.NewGuid();
            tokenMock.GetMock().Setup(d => d.GetUserGuidByToken()).Returns(userUid);
            var userMock = new UserRepositoryMock();

            var staffMock = new StaffRepositoryMock();
            var uowMock = new UnitOfWorkMock().Create(userRepository: userMock.GetObject(),
                staffRepository: staffMock.GetObject());

            var service = new AppointmentServiceFixture().Create(uowMock, tokenService: tokenMock.GetObject());
            var result = async () => await service.NextAppointment();

            await result.Should().ThrowAsync<UnauthorizedException>("Staff assigned to user not found");
        }

        [Fact]
        public async Task NextAppointment_QueueRoot_ThrowNotFoundException()
        {
            var tokenMock = new TokenServiceMock();
            var userUid = Guid.NewGuid();
            tokenMock.GetMock().Setup(d => d.GetUserGuidByToken()).Returns(userUid);

            var userMock = new UserRepositoryMock();

            var staffMock = new StaffRepositoryMock();
            var staff = StaffEntityFaker.Generate();
            var profInfos = StaffEntityFaker.GenerateProfessionalInfos();
            staff.ProfessionalInfos = profInfos;
            staffMock.GetMock().Setup(d => d.GetStaffByUserId(userUid)).ReturnsAsync(staff);

            var uowMock = new UnitOfWorkMock().Create(userRepository: userMock.GetObject(),
                staffRepository: staffMock.GetObject());

            var service = new AppointmentServiceFixture().Create(uowMock, tokenService: tokenMock.GetObject());

            var result = async () => await service.NextAppointment();

            await result.Should().ThrowAsync<NotFoundException>("There aren't any appointment scheduled today");
        }

        [Fact]
        public async Task Should_Call_Update_InProgressIsNotNull()
        {
            var tokenMock = new TokenServiceMock();
            var userUid = Guid.NewGuid();
            tokenMock.GetMock().Setup(d => d.GetUserGuidByToken()).Returns(userUid);

            var userMock = new UserRepositoryMock();

            var staffMock = new StaffRepositoryMock();
            var staff = StaffEntityFaker.Generate();
            var profInfos = StaffEntityFaker.GenerateProfessionalInfos();
            staff.ProfessionalInfos = profInfos;
            staffMock.GetMock().Setup(d => d.GetStaffByUserId(userUid)).ReturnsAsync(staff);

            var queueMock = new QueueRepositoryMock();
            var queueRoot = QueueEntityFaker.GenerateRoot();
            queueRoot.QueuePositions = QueueEntityFaker.GeneratePositions(2, queueRoot.Id);
            queueRoot.QueuePositions[0].Appointment = new Appointment()
            {
                Id =
                    Guid.NewGuid(),
                AppointmentStatus =
                    Domain.Enums.EAppointmentStatus.InProgress,
                Schedule = new Domain.ValueObjects.ScheduleWork(1, 4, 6, 8)
            };

            queueRoot.QueuePositions[1].Appointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                AppointmentStatus = Domain.Enums.EAppointmentStatus.CheckedIn,
                Schedule = new Domain.ValueObjects.ScheduleWork(2, 4, 3, 4)
            };
            queueMock.GetMock().Setup(d => d.GetQueueRootToStaff(staff)).ReturnsAsync(queueRoot);
            var nextAppointment = queueRoot.QueuePositions[1].Appointment;
            nextAppointment.PatientId = Guid.NewGuid();

            var patient = UserEntityFaker.Generate();
            patient.Id = nextAppointment.PatientId;
            userMock.GetMock().Setup(d => d.GetUserById(nextAppointment.PatientId)).ReturnsAsync(patient);

            var uowMock = new UnitOfWorkMock();
            var uowObject = uowMock.Create(userRepository: userMock.GetObject(), staffRepository: staffMock.GetObject(),
                queueRepository: queueMock.GetObject());

            var service = new AppointmentServiceFixture().Create(uowObject, tokenService: tokenMock.GetObject(),
                mapper: MapperMock.Create());

            var result = await service.NextAppointment();

            Assert.Equal(EAppointmentStatus.Completed, queueRoot.QueuePositions[0].Appointment.AppointmentStatus);
            Assert.Equal(patient.Id, result.PatientId);
            uowMock
                .GetMock()
                .Verify(d => d.GenericRepository.Update<QueuePosition>(queueRoot.QueuePositions[0]), Times.Once);
        }

        [Fact]
        public async Task NextNull_Should_ReturnEmptyObject()
        {
            var tokenMock = new TokenServiceMock();
            var userMock = new UserRepositoryMock();
            var staffMock = new StaffRepositoryMock();
            var queueRootMock = new QueueRepositoryMock();

            var userUid = Guid.NewGuid();
            tokenMock.GetMock().Setup(d => d.GetUserGuidByToken()).Returns(userUid);
            var staff = StaffEntityFaker.Generate();
            staff.ProfessionalInfos = StaffEntityFaker.GenerateProfessionalInfos();
            staffMock.GetMock().Setup(d => d.GetStaffByUserId(userUid)).ReturnsAsync(staff);

            var queueRoot = QueueEntityFaker.GenerateRoot();
            queueRootMock.GetMock().Setup(d => d.GetQueueRootToStaff(staff)).ReturnsAsync(queueRoot);
            queueRoot.QueuePositions = QueueEntityFaker.GeneratePositions(1, queueRoot.Id);
            queueRoot.QueuePositions[0].Appointment = AppointmentEntityFaker.Generate();
            queueRoot.QueuePositions[0].Appointment.AppointmentStatus = EAppointmentStatus.InProgress;

            var uowMock = new UnitOfWorkMock();
            var uowObj = uowMock.Create(null, null, null, queueRootMock.GetObject(), staffMock.GetObject());

            var service = new AppointmentServiceFixture().Create(uowObj, tokenService: tokenMock.GetObject());

            var result = await service.NextAppointment();

            Assert.Equal(Guid.Empty, result.Id);

            uowMock.GetMock().Verify(d => d.Commit(), Times.Once);
        }

        [Fact]
        public async Task NextExists_Should_Return_FullResponse()
      
    }
}
