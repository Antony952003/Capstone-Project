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
    public class FeedbackRepositoryTest
    {
        private EmployeeGrievanceContext _context;
        private Repository<Feedback> _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EmployeeGrievanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EmployeeGrievanceContext(options);
            _repository = new Repository<Feedback>(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Feedbacks.Any())
            {
                _context.Feedbacks.Add(new Feedback
                {
                    FeedbackId = 1,
                    Comments = "Great service",
                    DateProvided = DateTime.Now
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
            Assert.AreEqual(1, entity.FeedbackId);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowEntityNotFoundException_WhenEntityDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<EntityNotFoundException>(async () => await _repository.GetByIdAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Feedback with ID 999 not found."));
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
            _context.Feedbacks.RemoveRange(_context.Feedbacks);
            _context.SaveChanges();

            // Act & Assert
            var requests = await _repository.GetAllAsync();

            Assert.AreEqual(0, requests.Count());
        }

        [Test]
        public async Task Add_ShouldAddEntitySuccessfully()
        {
            // Arrange
            var newFeedback = new Feedback
            {
                FeedbackId = 2,
                Comments = "Needs improvement",
                DateProvided = DateTime.Now
            };

            // Act
            await _repository.Add(newFeedback);

            // Assert
            var entity = _context.Feedbacks.Find(2);
            Assert.IsNotNull(entity);
            Assert.AreEqual("Needs improvement", entity.Comments);
        }

        [Test]
        public async Task Add_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var invalidFeedback = new Feedback(); // Missing required properties

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () => await _repository.Add(invalidFeedback));
            Assert.That(ex.Message, Is.EqualTo("Error adding entity"));
            Assert.That(ex.InnerException, Is.InstanceOf<DbUpdateException>());
        }

        [Test]
        public async Task Update_ShouldUpdateEntitySuccessfully()
        {
            // Arrange
            var feedback = _context.Feedbacks.Find(1);
            feedback.Comments = "Updated feedback";

            // Act
            _repository.Update(feedback);

            // Assert
            var updatedFeedback = _context.Feedbacks.Find(1);
            Assert.AreEqual("Updated feedback", updatedFeedback.Comments);
        }

        [Test]
        public async Task Update_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var newFeedback = new Feedback
            {
                FeedbackId = 2,
                Comments = "Needs improvement",
                DateProvided = DateTime.Now
            };// This will cause an exception if FeedbackId is not valid

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () =>  _repository.Update(newFeedback));
            Assert.That(ex.Message, Is.EqualTo("Error updating entity"));
        }

        [Test]
        public async Task Remove_ShouldRemoveEntitySuccessfully()
        {
            // Arrange
            var feedback = _context.Feedbacks.Find(1);

            // Act
            _repository.Remove(feedback);
            await _context.SaveChangesAsync();

            // Assert
            var removedFeedback = _context.Feedbacks.Find(1);
            Assert.IsNull(removedFeedback);
        }

        [Test]
        public async Task Remove_ShouldThrowRepositoryException_WhenExceptionIsThrown()
        {
            // Arrange
            var feedback = _context.Feedbacks.Find(1);
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            // Act & Assert
            var ex = Assert.ThrowsAsync<RepositoryException>(async () => _repository.Remove(feedback));
            Assert.That(ex.Message, Is.EqualTo("Error removing entity"));
            Assert.That(ex.InnerException, Is.InstanceOf<DbUpdateException>());
        }
    }
}
