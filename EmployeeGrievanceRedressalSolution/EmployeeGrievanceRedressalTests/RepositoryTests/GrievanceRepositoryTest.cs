using EmployeeGrievanceRedressal.Exceptions;
using EmployeeGrievanceRedressal.Models;
using EmployeeGrievanceRedressal.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeGrievanceRedressalTests.RepositoryTests
{
    public class GrievanceRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private Repository<Grievance> _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _repository = new Repository<Grievance>(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Grievances.Any())
            {
                _context.Grievances.Add(new Grievance
                {
                    GrievanceId = 1,
                    Description = "Issue with system",
                    Status = GrievanceStatus.Open,
                    EmployeeId = 1,
                    DateRaised = DateTime.Now,
                    Priority = "high"
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
            Assert.AreEqual(1, entity.GrievanceId);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowEntityNotFoundException_WhenEntityDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Grievance with ID 999 not found."));
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
        public async Task GetAllAsync_ShouldThrowEntityNotFoundException_WhenNoEntitiesExist()
        {
            // Arrange
            _context.Grievances.RemoveRange(_context.Grievances);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () => await _repository.GetAllAsync());
            Assert.That(ex.Message, Is.EqualTo("Error getting all entities"));
        }

        [Test]
        public async Task Add_ShouldAddEntitySuccessfully()
        {
            // Arrange
            var newGrievance = new Grievance
            {
                GrievanceId = 2,
                Description = "Another issue",
                Status = GrievanceStatus.Open,
                EmployeeId = 1,
                DateRaised = DateTime.Now,
                Priority = "low"
                

            };

            // Act
            await _repository.Add(newGrievance);

            // Assert
            var entity = _context.Grievances.Find(2);
            Assert.IsNotNull(entity);
            Assert.AreEqual("Another issue", entity.Description);
        }

        [Test]
        public async Task Add_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var invalidGrievance = new Grievance(); // Missing required properties

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () => await _repository.Add(invalidGrievance));
        }

        [Test]
        public async Task Update_ShouldUpdateEntitySuccessfully()
        {
            // Arrange
            var grievance = _context.Grievances.Find(1);
            grievance.Description = "Updated issue";

            // Act
            _repository.Update(grievance);

            // Assert
            var updatedGrievance = _context.Grievances.Find(1);
            Assert.AreEqual("Updated issue", updatedGrievance.Description);
        }

        [Test]
        public async Task Update_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var grievance = new Grievance
            {
                GrievanceId = 5,
                Description = "Issue with system",
                Status = GrievanceStatus.Open,
                EmployeeId = 1,
                DateRaised = DateTime.Now,
                Priority = "high"
            }; // This will cause an exception if GrievanceId is not valid

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>  _repository.Update(grievance));
            Assert.That(ex.Message, Is.EqualTo("Error updating entity"));
            Assert.That(ex.InnerException, Is.InstanceOf<DbUpdateException>());
        }

        [Test]
        public async Task Remove_ShouldRemoveEntitySuccessfully()
        {
            // Arrange
            var grievance = _context.Grievances.Find(1);

            // Act
            _repository.Remove(grievance);
            await _context.SaveChangesAsync();

            // Assert
            var removedGrievance = _context.Grievances.Find(1);
            Assert.IsNull(removedGrievance);
        }

        [Test]
        public async Task Remove_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var grievance = _context.Grievances.Find(1);
            _context.Grievances.Remove(grievance);
            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>  _repository.Remove(grievance));
            Assert.That(ex.Message, Is.EqualTo("Error removing entity"));
            Assert.That(ex.InnerException, Is.InstanceOf<DbUpdateException>());
        }
    }
}
