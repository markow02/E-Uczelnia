using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using University.Data;
using University.Models;
using University.Services;
using Xunit;

namespace University.Tests
{
    public class ActivityClubServiceTests
    {
        private readonly Mock<UniversityContext> _mockContext;
        private readonly ActivityClubService _service;
        private readonly Mock<DbSet<ActivityClub>> _mockSet;

        public ActivityClubServiceTests()
        {
            _mockContext = new Mock<UniversityContext>();
            _mockSet = new Mock<DbSet<ActivityClub>>();


            var emptyData = new List<ActivityClub>().AsQueryable();
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<ActivityClub>(emptyData.Provider));
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Expression)
                .Returns(emptyData.Expression);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.ElementType)
                .Returns(emptyData.ElementType);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.GetEnumerator())
                .Returns(emptyData.GetEnumerator());


            _mockContext.Setup(m => m.Set<ActivityClub>()).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.ActivityClubs).Returns(_mockSet.Object);

            _service = new ActivityClubService(_mockContext.Object);
        }

        [Fact]
        public async Task LoadDataAsync_ReturnsAllActivityClubs_Positive()
        {
            // Arrange
            var data = new List<ActivityClub>
                {
                    new ActivityClub { ActivityClubId = 1, ActivityClubName = "Club1", MeetingDay = "Monday", ActivityClubDescription = "Description1" },
                    new ActivityClub { ActivityClubId = 2, ActivityClubName = "Club2", MeetingDay = "Tuesday", ActivityClubDescription = "Description2" }
                }.AsQueryable();

            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Act
            var result = await _service.LoadDataAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Club1", result[0].ActivityClubName);
            Assert.Equal("Club2", result[1].ActivityClubName);
        }

        [Fact]
        public async Task LoadDataAsync_ReturnsEmptyList_WhenNoActivityClubs_Negative()
        {
            // Arrange
            var data = new List<ActivityClub>().AsQueryable();

            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Act
            var result = await _service.LoadDataAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task IsValidAsync_ReturnsTrue_WhenActivityClubIsValid_Positive()
        {
            // Arrange
            var activityClub = new ActivityClub { ActivityClubName = "Club", MeetingDay = "Monday", ActivityClubDescription = "Description" };

            // Act
            var result = await _service.IsValidAsync(activityClub);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidAsync_ReturnsFalse_WhenActivityClubNameIsEmpty_Negative()
        {
            // Arrange
            var activityClub = new ActivityClub { ActivityClubName = "", MeetingDay = "Monday", ActivityClubDescription = "Description" };

            // Act
            var result = await _service.IsValidAsync(activityClub);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveDataAsync_AddsNewActivityClub_WhenActivityClubIdIsZero_Positive()
        {
            // Arrange
            var activityClub = new ActivityClub { ActivityClubId = 0, ActivityClubName = "Club", MeetingDay = "Monday", ActivityClubDescription = "Description" };

            // Act
            await _service.SaveDataAsync(activityClub);

            // Assert
            _mockSet.Verify(m => m.Add(It.IsAny<ActivityClub>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SaveDataAsync_AddsNewActivityClub_WhenActivityClubIdIsZero_Negative()
        {
            // Arrange
            var activityClub = new ActivityClub { ActivityClubId = 0, ActivityClubName = "Club", MeetingDay = "Monday", ActivityClubDescription = "Description" };

            _mockSet.Setup(m => m.Add(It.IsAny<ActivityClub>())).Throws(new DbUpdateException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _service.SaveDataAsync(activityClub));
            _mockSet.Verify(m => m.Add(It.IsAny<ActivityClub>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task GetActivityClubByIdAsync_ReturnsActivityClub_WhenIdExists_Positive()
        {
            // Arrange
            var activityClub = new ActivityClub { ActivityClubId = 1, ActivityClubName = "Club", MeetingDay = "Monday", ActivityClubDescription = "Description" };
            var data = new List<ActivityClub> { activityClub }.AsQueryable();

            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(activityClub);

            // Act
            var result = await _service.GetActivityClubByIdAsync(1);

            // Assert
            Assert.Equal(activityClub, result);
        }

        [Fact]
        public async Task GetActivityClubByIdAsync_ReturnsNull_WhenIdDoesNotExist_Negative()
        {
            // Arrange
            var data = new List<ActivityClub>().AsQueryable();

            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<ActivityClub>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((ActivityClub)null);

            // Act
            var result = await _service.GetActivityClubByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}

// Helper class to support async query provider
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return new ValueTask<TResult>(Execute<TResult>(expression));
    }

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;
}
