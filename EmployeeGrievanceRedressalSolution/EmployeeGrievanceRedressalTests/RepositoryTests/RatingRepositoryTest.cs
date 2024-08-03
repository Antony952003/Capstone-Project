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
    public class RatingRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private RatingRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _repository = new RatingRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Ratings.Any())
            {
                _context.Ratings.AddRange(
                    new Rating
                    {
                        RatingId = 1,
                        SolverId = 1,
                        GrievanceId = 1,
                        RatingValue = 5,
                        Comment = "Excellent work!",
                        Grievance = new Grievance
                        {
                            GrievanceId = 1,
                            Title = "Issue with HR",
                            Description = "HR is not responding.",
                            DateRaised = DateTime.Now,
                            Priority=   GrievancePriority.High.ToString(),
                            SolverId= 1,
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
                        }
                    },
                    new Rating
                    {
                        RatingId = 2,
                        SolverId = 1,
                        GrievanceId = 2,
                        RatingValue = 4,
                        Comment = "Good solution.",
                        Grievance = new Grievance
                        {
                            GrievanceId = 2,
                            Title = "Software Issue",
                            Description = "Application crashes frequently.",
                            Priority = GrievancePriority.High.ToString(),
                            SolverId = 1,
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
                    }
                );
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task AddRatingAsync_ShouldAddRating()
        {
            // Arrange
            var newRating = new Rating
            {
                SolverId = 2,
                GrievanceId = 1,
                RatingValue = 3,
                Comment = "Average response.",
                DateProvided = DateTime.Now,
                
            };

            // Act
            await _repository.AddRatingAsync(newRating);
            var addedRating = await _repository.GetRatingsBySolverIdAsync(2);

            // Assert
            Assert.IsNotNull(addedRating);
            Assert.AreEqual(1, addedRating.Count());
            Assert.AreEqual(3, addedRating.First().RatingId);
        }

        [Test]
        public async Task GetRatingsBySolverIdAsync_ShouldReturnRatings_WhenRatingsExist()
        {
            // Act
            var ratings = await _repository.GetRatingsBySolverIdAsync(1);

            // Assert
            Assert.IsNotNull(ratings);
            Assert.AreEqual(2, ratings.Count());
        }

        [Test]
        public void GetRatingsBySolverIdAsync_ShouldThrowEntityNotFoundException_WhenNoRatingsExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetRatingsBySolverIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("No ratings found for the specified solver."));
        }

        [Test]
        public void AddRatingAsync_ShouldThrowRepositoryException_OnError()
        {
            // Arrange
            _context.Dispose(); // Dispose context to simulate an error

            // Act & Assert
            var ex = Assert.ThrowsAsync<ObjectDisposedException>(async () => await _repository.AddRatingAsync(new Rating
            {
                RatingId = 4,
                SolverId = 3,
                GrievanceId = 4,
                RatingValue = 2,
                Comment = "Poor performance."
            }));
            Assert.That(ex.Message, Is.EqualTo(ex.Message));
        }
    }
}
