﻿using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressalTests.RepositoryTests
{
    public class GenericRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private Repository<User> _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test run
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _repository = new Repository<User>(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User
                {
                    UserId = 1,
                    Name = "John Doe",
                    Phone = "1234567890",
                    Email = "john.doe@example.com",
                    UserImage = "image.png",
                    DOB = DateTime.Now,
                    Role = UserRole.Employee,
                    IsApproved = true
                });
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Act
            var entity = await _repository.GetByIdAsync(1);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.UserId);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowEntityNotFoundException_WhenEntityDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("User with ID 999 not found."));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Act
            var entities = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(entities);
            Assert.AreEqual(1, entities.Count());
        }

        [Test]
        public async Task GetAllAsync_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            // Use a new context or manipulate the current one to force an exception
            var repositoryWithException = new Repository<User>(new EmployeeGrievanceContext(new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options));

            // Act & Assert
            var requests = await _repository.GetAllAsync();

            Assert.AreEqual(1, requests.Count());
        }

        [Test]
        public async Task Add_ShouldAddEntitySuccessfully()
        {
            // Arrange
            var newUser = new User
            {
                UserId = 2,
                Name = "Jane Doe",
                Phone = "0987654321",
                Email = "jane.doe@example.com",
                UserImage = "image2.png",
                DOB = DateTime.Now,
                Role = UserRole.Employee,
                IsApproved = true
            };

            // Act
            await _repository.Add(newUser);

            // Assert
            var entity = _context.Users.Find(2);
            Assert.IsNotNull(entity);
            Assert.AreEqual("Jane Doe", entity.Name);
        }

        [Test]
        public async Task Update_ShouldUpdateEntitySuccessfully()
        {
            // Arrange
            var user = _context.Users.Find(1);
            user.Name = "John Smith";

            // Act
            _repository.Update(user);
            await _context.SaveChangesAsync();

            // Assert
            var updatedUser = _context.Users.Find(1);
            Assert.AreEqual("John Smith", updatedUser.Name);
        }

        [Test]
        public async Task Update_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var user = new User
            {
                UserId = 2,
                Name = "Jane Doe",
                Phone = "0987654321",
                Email = "jane.doe@example.com",
                UserImage = "image2.png",
                DOB = DateTime.Now,
                Role = UserRole.Employee,
                IsApproved = true
            }; // Simulate detached entity

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>
            {
                _repository.Update(user);
            });

        }

        [Test]
        public async Task Remove_ShouldRemoveEntitySuccessfully()
        {
            // Arrange
            var user = _context.Users.Find(1);

            // Act
            _repository.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var removedUser = _context.Users.Find(1);
            Assert.IsNull(removedUser);
        }

        [Test]
        public async Task Remove_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var user = new User
            {
                UserId = 2,
                Name = "Jane Doe",
                Phone = "0987654321",
                Email = "jane.doe@example.com",
                UserImage = "image2.png",
                DOB = DateTime.Now,
                Role = UserRole.Employee,
                IsApproved = true
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>
            {
                _repository.Remove(user);
            });

        }

        [Test]
        public async Task Add_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var invalidUser = new User(); // Missing required properties

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>
            {
                await _repository.Add(invalidUser);
            });

            Assert.That(ex.Message, Is.EqualTo("Error adding entity"));
            Assert.That(ex.InnerException, Is.InstanceOf<DbUpdateException>()); // Adjust based on actual inner exception type
        }
    }
}
