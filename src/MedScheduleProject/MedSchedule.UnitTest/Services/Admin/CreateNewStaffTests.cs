using FluentAssertions;
using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.ValueObjects;
using MedSchedule.UnitTest.Commons;
using MedSchedule.UnitTest.Commons.Fakers.Entities;
using MedSchedule.UnitTest.Commons.Fakers.Requests;
using MedSchedule.UnitTest.Commons.Mocks;
using Microsoft.Identity.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Services.Admin
{
    public class CreateNewStaffTests
    {
        [Fact]
        public async Task CreateNewStaff_EmptyStaffRole_ThrowDomainException()
        {
            //Arrange
            var request = CreateNewStaffRequestFaker.Generate();
            request.Role = "invalid role";

            var service = new CreateAdminService().Create();

            //Act
            var result = async () => await service.CreateNewStaff(request);

            //Assert
            await result.Should().ThrowExactlyAsync<RequestException>("Cannot create staff with this role");
        }

        [Fact]
        public async Task CreateNewStaff_InvalidStaffRole_ThrowDomainException()
        {
            //Arrange
            var request = CreateNewStaffRequestFaker.Generate();
            request.Role = "invalid role";

            var service = new CreateAdminService().Create();

            //Act
            var result = async () => await service.CreateNewStaff(request);

            //Assert
            await result.Should().ThrowExactlyAsync<RequestException>("Cannot create staff with this role");
        }

        [Fact]
        public async Task CreateNewStaff_UserNotExists_ThrowResourceNotFound()
        {
            //Arrange
            var request = CreateNewStaffRequestFaker.Generate();

            var userRepos = new UserRepositoryMock();
            userRepos.SetUserById(null);
            var uow = new UnitOfWorkMock().Create(userRepos.GetObject());
            var service = new CreateAdminService().Create(uow);

            //Act
            var result = async () => await service.CreateNewStaff(request);

            //Assert
            await result
                .Should()
                .ThrowExactlyAsync<ResourceNotFoundException>("The user by id was not found");
        }

        [Fact]
        public async Task CreateNewStaff_StaffExists_ThrowConflictException()
        {
            //Arrange
            var request = CreateNewStaffRequestFaker.Generate();
            var user = UserEntityFaker.Generate();
            var staff = StaffEntityFaker.Generate();

            var userRepos = new UserRepositoryMock();
            userRepos.SetUserById(user);
            userRepos.GetMock().Setup(d => d.UserStaffExists(user.Id)).ReturnsAsync(true);
            var uow = new UnitOfWorkMock().Create(userRepos.GetObject());
            var service = new CreateAdminService().Create(uow);

            //Act
            var result = async () => await service.CreateNewStaff(request);

            //Assert
            await result
                .Should()
                .ThrowExactlyAsync<ConflictException>("Staff already exists");
        }

        [Theory]
        [MemberData(nameof(RolesRandomCase.GetRolesRandomlyCase), MemberType = typeof(RolesRandomCase))]
        public async Task CreateNewStaff_RolesIndependentFromCase_ReturnsOk(string role)
        {
            var request = CreateNewStaffRequestFaker.Generate();
            request.Role = role;

            var user = UserEntityFaker.Generate();
            user.Id = request.UserId;

            var userRepos = new UserRepositoryMock();
            userRepos.SetUserById(user);
            var uow = new UnitOfWorkMock().Create(userRepository: userRepos.GetObject());
            var mapper = MapperMock.Create();

            var service = new CreateAdminService().Create(uow, mapper: mapper);

            var result = await service.CreateNewStaff(request);

            result.Role.Should().Be(role.ToLower());
            result.UserId.Should().Be(request.UserId);
            result.WorkShift.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateNewStaff_InvalidWorkShift_ThrowDomainException()
        {
            var request = CreateNewStaffRequestFaker.Generate();
            request.WorkShift.StartHours = 70;

            var user = UserEntityFaker.Generate();
            user.Id = request.UserId;

            var userRepos = new UserRepositoryMock();
            userRepos.SetUserById(user);
            var uow = new UnitOfWorkMock().Create(userRepository: userRepos.GetObject());
            var mapper = MapperMock.Create();

            var service = new CreateAdminService().Create(uow, mapper: mapper);

            var result = async () => await service.CreateNewStaff(request);

            await result
                .Should()
                .ThrowExactlyAsync<DomainException>("Work time hours must be between 0 and 23"); 
        }
    }
}
