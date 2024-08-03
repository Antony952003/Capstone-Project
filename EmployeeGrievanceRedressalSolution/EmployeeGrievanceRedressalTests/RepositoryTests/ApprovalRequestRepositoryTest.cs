using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressalTests.RepositoryTests
{
    public class ApprovalRequestRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private ApprovalRequestRepository _repository;
        private Mock<ILogger<ApprovalRequestRepository>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _loggerMock = new Mock<ILogger<ApprovalRequestRepository>>();
            _repository = new ApprovalRequestRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.ApprovalRequests.Any())
            {
                _context.ApprovalRequests.AddRange(
                    new ApprovalRequest
                    {
                        ApprovalRequestId = 1,
                        EmployeeId = 1,
                        RequestDate = DateTime.Now,
                        Status = ApprovalStatus.Pending,
                        Reason = "no reason",
                        Employee = new User
                        {
                            UserId = 1,
                            Name = "Employee 1",
                            Phone = "1234567890",
                            Email = "employee1@example.com",
                            UserImage = "employee1.png",
                            DOB = DateTime.Now,
                            Role = UserRole.Employee,
                            IsApproved = true
                        }
                    },
                    new ApprovalRequest
                    {
                        ApprovalRequestId = 2,
                        EmployeeId = 2,
                        RequestDate = DateTime.Now,
                        Status = ApprovalStatus.Pending,
                        Reason = "no reason",
                        Employee = new User
                        {
                            UserId = 2,
                            Name = "Employee 2",
                            Phone = "2345678901",
                            Email = "employee2@example.com",
                            UserImage = "employee2.png",
                            DOB = DateTime.Now,
                            Role = UserRole.Employee,
                            IsApproved = true
                        }
                    }
                );
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnApprovalRequest_WhenApprovalRequestExists()
        {
            // Act
            var approvalRequest = await _repository.GetByIdAsync(1);

            // Assert
            Assert.IsNotNull(approvalRequest);
            Assert.AreEqual(1, approvalRequest.ApprovalRequestId);
        }

        [Test]
        public void GetByIdAsync_ShouldThrowEntityNotFoundException_WhenApprovalRequestDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("ApprovalRequest not found."));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllApprovalRequests()
        {
            // Act
            var approvalRequests = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(approvalRequests);
            Assert.AreEqual(2, approvalRequests.Count());
        }

        [Test]
        public async Task GetAllAsync_ShouldThrowEntityNotFoundException_WhenNoApprovalRequestsExist()
        {
            // Arrange
            _context.ApprovalRequests.RemoveRange(_context.ApprovalRequests);
            _context.SaveChanges();
            var requests = await _repository.GetAllAsync();

            Assert.AreEqual(0, requests.Count());
        }

        [Test]
        public async Task AddAsync_ShouldAddApprovalRequest()
        {
            // Arrange
            var newApprovalRequest = new ApprovalRequest
            {
                ApprovalRequestId = 3,
                EmployeeId = 3,
                RequestDate = DateTime.Now,
                Status = ApprovalStatus.Pending,
                Reason= "no reason",
                Employee = new User
                {
                    UserId = 3,
                    Name = "Employee 3",
                    Phone = "3456789012",
                    Email = "employee3@example.com",
                    UserImage = "employee3.png",
                    DOB = DateTime.Now,
                    Role = UserRole.Employee,
                    IsApproved = true
                }
            };

            // Act
            await _repository.AddAsync(newApprovalRequest);
            var addedApprovalRequest = await _repository.GetByIdAsync(3);

            // Assert
            Assert.IsNotNull(addedApprovalRequest);
            Assert.AreEqual(3, addedApprovalRequest.ApprovalRequestId);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateApprovalRequest()
        {
            // Arrange
            var approvalRequest = await _repository.GetByIdAsync(1);
            approvalRequest.Status = ApprovalStatus.Approved;

            // Act
            await _repository.UpdateAsync(approvalRequest);
            var updatedApprovalRequest = await _repository.GetByIdAsync(1);

            // Assert
            Assert.IsNotNull(updatedApprovalRequest);
            Assert.AreEqual(ApprovalStatus.Approved, updatedApprovalRequest.Status);
        }

        [Test]
        public void UpdateAsync_ShouldThrowEntityNotFoundException_WhenApprovalRequestDoesNotExist()
        {
            // Arrange
            var approvalRequest = new ApprovalRequest
            {
                ApprovalRequestId = 999,
                EmployeeId = 999,
                RequestDate = DateTime.Now,
                Status = ApprovalStatus.Pending,
                Reason = "no reason",

            };

        }
    }
}// Act
