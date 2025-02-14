using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using University.Data;
using University.Models;
using University.Services;
using Xunit;
namespace University.Tests
{
    public class ClassroomServiceTests
    {
        [Fact]
        public async Task LoadDataAsync_ReturnsAllClassrooms_PositiveTest()
        {
            // Arrange
            var classroomsData = new List<Classroom>
                {
                    new Classroom { ClassroomId = 1, ClassroomNumber = "A101", Capacity = 30 },
                    new Classroom { ClassroomId = 2, ClassroomNumber = "B202", Capacity = 50 }
                }.AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(classroomsData.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(classroomsData.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(classroomsData.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(classroomsData.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            var result = await service.LoadDataAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ClassroomNumber == "A101");
        }

        [Fact]
        public async Task LoadDataAsync_ReturnsEmptyList_NegativeTest()
        {
            // Arrange
            var emptyData = new List<Classroom>().AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            var result = await service.LoadDataAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task IsValidAsync_ReturnsTrueForValidClassroom_PositiveTest()
        {
            // Arrange
            var classroom = new Classroom
            {
                ClassroomNumber = "C303",
                Capacity = 40
            };

            var mockContext = new Mock<UniversityContext>();
            var service = new ClassroomService(mockContext.Object);

            // Act
            var result = await service.IsValidAsync(classroom);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidAsync_ReturnsFalseForInvalidClassroom_NegativeTest()
        {
            // Arrange
            var emptyNumberClassroom = new Classroom
            {
                ClassroomNumber = "  ",
                Capacity = 20
            };

            var negativeCapacityClassroom = new Classroom
            {
                ClassroomNumber = "D404",
                Capacity = -1
            };

            var mockContext = new Mock<UniversityContext>();
            var service = new ClassroomService(mockContext.Object);

            // Act
            var result1 = await service.IsValidAsync(emptyNumberClassroom);
            var result2 = await service.IsValidAsync(negativeCapacityClassroom);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task SaveDataAsync_AddsNewClassroomWhenIdIsZero_PositiveTest()
        {
            // Arrange
            var classroom = new Classroom
            {
                ClassroomId = 0,
                ClassroomNumber = "E505",
                Capacity = 100
            };

            var mockSet = new Mock<DbSet<Classroom>>();
            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            await service.SaveDataAsync(classroom);

            // Assert
            mockSet.Verify(m => m.Add(It.Is<Classroom>(c => c.ClassroomNumber == "E505")), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task SaveDataAsync_UpdatesExistingClassroom_PositiveTest()
        {
            // Arrange
            var classroom = new Classroom
            {
                ClassroomId = 2,
                ClassroomNumber = "F606",
                Capacity = 120
            };

            var data = new List<Classroom>
                {
                    new Classroom { ClassroomId = 2, ClassroomNumber = "OldNumber", Capacity = 60 }
                }.AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            await service.SaveDataAsync(classroom);

            // Assert
            var updatedClassroom = data.First();
            Assert.Equal("F606", updatedClassroom.ClassroomNumber);
            Assert.Equal(120, updatedClassroom.Capacity);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task SaveDataAsync_DoesNotUpdateWhenClassroomNotFound_NegativeTest()
        {
            // Arrange
            var classroom = new Classroom
            {
                ClassroomId = 3,
                ClassroomNumber = "G707",
                Capacity = 200
            };

            var data = new List<Classroom> // no item with ClassroomId=3
                {
                    new Classroom { ClassroomId = 1, ClassroomNumber = "X", Capacity = 10 },
                    new Classroom { ClassroomId = 2, ClassroomNumber = "Y", Capacity = 20 }
                }.AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            await service.SaveDataAsync(classroom);

            // Assert
            // Classroom with Id=3 doesn't exist, so no updates should be made
            var updatedClassroom = data.FirstOrDefault(c => c.ClassroomId == 3);
            Assert.Null(updatedClassroom);
            // But SaveChanges still gets called in the method
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public async Task GetClassroomByIdAsync_FindsClassroomById_PositiveTest()
        {
            // Arrange
            var data = new List<Classroom>
                {
                    new Classroom { ClassroomId = 5, ClassroomNumber = "H808", Capacity = 40 }
                }.AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            var result = await service.GetClassroomByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("H808", result.ClassroomNumber);
        }

        [Fact]
        public async Task GetClassroomByIdAsync_ReturnsNullIfNotFound_NegativeTest()
        {
            // Arrange
            var data = new List<Classroom>
                {
                    new Classroom { ClassroomId = 1, ClassroomNumber = "TestRoom" }
                }.AsQueryable();

            var mockSet = new Mock<DbSet<Classroom>>();
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Classroom>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<UniversityContext>();
            mockContext.Setup(c => c.Classrooms).Returns(mockSet.Object);

            var service = new ClassroomService(mockContext.Object);

            // Act
            var result = await service.GetClassroomByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}
