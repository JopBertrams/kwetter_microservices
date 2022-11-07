using HonkService.Application.Commands;
using HonkService.Application.Queries;
using HonkService.Domain.Entities;
using HonkService.Domain;
using NUnit.Framework;
using Moq;

namespace HonkService.Tests
{
    public static class DateTimeComparison
    {
        public static bool EqualsUptoSeconds(this DateTime dateTime, DateTime dateTimeToCompare)
        {
            if (dateTime.Date != dateTimeToCompare.Date) return false;
            if (dateTime.Hour != dateTimeToCompare.Hour) return false;
            if (dateTime.Minute != dateTimeToCompare.Minute) return false;
            if (dateTime.Second != dateTimeToCompare.Second) return false;

            return true;
        }
    }

    public class UnitTests
    {
        [Test]
        public async Task CheckIfHonkGetsCreated()
        {
            // Arrange
            var honkRepositoryMock = new Mock<IHonkRepository>();
            var postHonkHandler = new PostHonkCommandHandler(honkRepositoryMock.Object);
            var honk = new HonkEntity
            {
                UserId = "UniqueTestUserId",
                Message = "This honk is posted from the unit test!"
            };
            var command = new PostHonkCommand(honk.UserId, honk.Message);

            // Act
            var result = await postHonkHandler.Handle(command, CancellationToken.None);


            // Result
            Assert.NotNull(result);
            Assert.IsInstanceOf<Guid>(result.Id);
            Assert.That(result.UserId, Is.SameAs(honk.UserId));
            Assert.That(result.Message, Is.SameAs(honk.Message));
            Assert.That(DateTimeComparison.EqualsUptoSeconds(result.CreatedAt, DateTime.UtcNow), Is.True);
        }

        [Test]
        public async Task CheckIfHonkGetsDeleted()
        {
            // Arrange
            var honkRepositoryMock = new Mock<IHonkRepository>();
            var deleteHonkHandler = new DeleteHonkCommandHandler(honkRepositoryMock.Object);
            var deletedHonk = new HonkEntity
            {
                Id = Guid.NewGuid(),
            };
            var command = new DeleteHonkCommand(deletedHonk.Id);

            // Act
            var result = await deleteHonkHandler.Handle(command, CancellationToken.None);


            // Result
            Assert.NotNull(result);
            Assert.IsInstanceOf<Guid>(result.Id);
        }

        [Test]
        public async Task GetHonkById()
        {
            // Arrange
            var honkRepositoryMock = new Mock<IHonkRepository>();
            var honk = new HonkEntity
            {
                Id = Guid.NewGuid(),
                UserId = "UniqueTestUserId",
                Message = "This honk is posted from the unit test!",
                CreatedAt = new DateTime(2022, 07, 20)
            };

            honkRepositoryMock.Setup(r => r.GetHonkById(It.IsAny<Guid>())).ReturnsAsync(honk);

            var getHonkHandler = new GetHonkQueryHandler(honkRepositoryMock.Object);
            var query = new GetHonkQuery(honk.Id);

            // Act
            var result = await getHonkHandler.Handle(query, CancellationToken.None);


            // Result
            Assert.NotNull(result);
            Assert.IsInstanceOf<Guid>(result.Id);
            Assert.That(result.UserId, Is.SameAs(honk.UserId));
            Assert.That(result.Message, Is.SameAs(honk.Message));
            Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2022, 07, 20)));
        }
    }
}