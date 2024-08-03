using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Interfaces.RepositoryInterfaces;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressalTests.RepositoryTests
{
    public class SolutionRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private SolutionRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _repository = new SolutionRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Solutions.Any())
            {
                _context.Solutions.AddRange(
                    new Solution
                    {
                        SolutionId = 1,
                        GrievanceId = 1,
                        SolutionTitle = "Solution 1",
                        Description = "Description 1",
                        DateProvided = DateTime.Now,
                        DocumentUrls = new List<DocumentUrl>
                        {
                            new DocumentUrl { DocumentUrlId = 1, GrievanceId = 1, Url = "url1" },
                            new DocumentUrl { DocumentUrlId = 2, GrievanceId = 1, Url = "url2" }
                        },
                        Solver = new User
                        {
                            UserId = 1,
                            Name = "Solver 1",
                            Phone = "1234567890",
                            Email = "solver1@example.com",
                            UserImage = "solver1.png",
                            DOB = DateTime.Now,
                            Role = UserRole.Solver,
                            IsApproved = true
                        }
                    },
                    new Solution
                    {
                        SolutionId = 2,
                        GrievanceId = 2,
                        SolutionTitle = "Solution 2",
                        Description = "Description 2",
                        DateProvided = DateTime.Now,
                        DocumentUrls = new List<DocumentUrl>
                        {
                            new DocumentUrl { DocumentUrlId = 3, GrievanceId = 2, Url = "url3" },
                            new DocumentUrl { DocumentUrlId = 4, GrievanceId = 2, Url = "url4" }
                        },
                        Solver = new User
                        {
                            UserId = 2,
                            Name = "Solver 2",
                            Phone = "2345678901",
                            Email = "solver2@example.com",
                            UserImage = "solver2.png",
                            DOB = DateTime.Now,
                            Role = UserRole.Solver,
                            IsApproved = true
                        }
                    },
                    new Solution
                    {
                        SolutionId = 3,
                        GrievanceId = 3,
                        SolutionTitle = "Solution 3",
                        Description = "Description 3",
                        DateProvided = DateTime.Now,
                        DocumentUrls = new List<DocumentUrl>
                        {
                            new DocumentUrl { DocumentUrlId = 5, GrievanceId = 3, Url = "url5" },
                            new DocumentUrl { DocumentUrlId = 6, GrievanceId = 3, Url = "url6" }
                        },
                        Solver = new User
                        {
                            UserId = 3,
                            Name = "Solver 3",
                            Phone = "3456789012",
                            Email = "solver3@example.com",
                            UserImage = "solver3.png",
                            DOB = DateTime.Now,
                            Role = UserRole.Solver,
                            IsApproved = true
                        }
                    }
                );
                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetAllSolutionswithSolver_ShouldReturnAllSolutionsWithSolvers()
        {
            // Act
            var solutions = await _repository.GetAllSolutionswithSolver();

            // Assert
            Assert.IsNotNull(solutions);
            Assert.AreEqual(3, solutions.Count);
            Assert.IsTrue(solutions.All(s => s.Solver != null));
        }

        [Test]
        public void GetAllSolutionswithSolver_ShouldThrowEntityNotFoundException_WhenNoSolutionsExist()
        {
            // Arrange
            _context.Solutions.RemoveRange(_context.Solutions);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetAllSolutionswithSolver());
            Assert.That(ex.Message, Is.EqualTo("No Solutions are found !!"));
        }

        [Test]
        public async Task GetAllSolutionswithSolver_ShouldHandleEmptyDocumentUrls()
        {
            // Arrange
            _context.Solutions.Add(new Solution
            {
                SolutionId = 4,
                GrievanceId = 4,
                SolutionTitle = "Solution 4",
                Description = "Description 4",
                DateProvided = DateTime.Now,
                DocumentUrls = new List<DocumentUrl>(),
                Solver = new User
                {
                    UserId = 4,
                    Name = "Solver 4",
                    Phone = "4567890123",
                    Email = "solver4@example.com",
                    UserImage = "solver4.png",
                    DOB = DateTime.Now,
                    Role = UserRole.Solver,
                    IsApproved = true
                }
            });
            _context.SaveChanges();

            // Act
            var solutions = await _repository.GetAllSolutionswithSolver();

            // Assert
            var solution = solutions.FirstOrDefault(s => s.SolutionId == 4);
            Assert.IsNotNull(solution);
            Assert.IsEmpty(solution.DocumentUrls);
        }

        [Test]
        public async Task GetSolutionByIdwithSolver_ShouldReturnSolutionWithSolver_WhenSolutionExists()
        {
            // Act
            var solution = await _repository.GetSolutionByIdwithSolver(1);

            // Assert
            Assert.IsNotNull(solution);
            Assert.AreEqual(1, solution.SolutionId);
            Assert.IsNotNull(solution.Solver);
        }

        [Test]
        public void GetSolutionByIdwithSolver_ShouldThrowEntityNotFoundException_WhenSolutionDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetSolutionByIdwithSolver(999));
            Assert.That(ex.Message, Is.EqualTo("No Solutions are found !!"));
        }

        [Test]
        public async Task GetSolutionByIdwithSolver_ShouldReturnSolutionWithEmptyDocumentUrls()
        {
            // Arrange
            var solution = new Solution
            {
                SolutionId = 5,
                GrievanceId = 5,
                SolutionTitle = "Solution 5",
                Description = "Description 5",
                DateProvided = DateTime.Now,
                DocumentUrls = new List<DocumentUrl>(),
                Solver = new User
                {
                    UserId = 5,
                    Name = "Solver 5",
                    Phone = "5678901234",
                    Email = "solver5@example.com",
                    UserImage = "solver5.png",
                    DOB = DateTime.Now,
                    Role = UserRole.Solver,
                    IsApproved = true
                }
            };
            _context.Solutions.Add(solution);
            _context.SaveChanges();

            // Act
            var retrievedSolution = await _repository.GetSolutionByIdwithSolver(5);

            // Assert
            Assert.IsNotNull(retrievedSolution);
            Assert.AreEqual(5, retrievedSolution.SolutionId);
            Assert.IsEmpty(retrievedSolution.DocumentUrls);
        }

        [Test]
        public async Task GetSolutionByIdwithSolver_ShouldHandleSolutionWithMultipleDocumentUrls()
        {
            // Act
            var solution = await _repository.GetSolutionByIdwithSolver(1);

            // Assert
            Assert.IsNotNull(solution);
            Assert.AreEqual(2, solution.DocumentUrls.Count);
        }

        [Test]
        public async Task GetAllSolutionswithSolver_ShouldHandleMultipleSolutionsCorrectly()
        {
            // Act
            var solutions = await _repository.GetAllSolutionswithSolver();

            // Assert
            Assert.AreEqual(3, solutions.Count);
        }

        [Test]
        public async Task GetSolutionByIdwithSolver_ShouldReturnCorrectSolverDetails()
        {
            // Act
            var solution = await _repository.GetSolutionByIdwithSolver(2);

            // Assert
            Assert.IsNotNull(solution);
            Assert.AreEqual("Solver 2", solution.Solver.Name);
            Assert.AreEqual("solver2@example.com", solution.Solver.Email);
        }
    }
}
