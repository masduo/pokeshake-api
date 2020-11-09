using Microsoft.Extensions.Logging;
using Moq;
using System;

/// <summary>
/// Helper extension method to verify that a mocked <see cref="Microsoft.Extensions.Logging.ILogger<T>" /> has been called
/// </summary>
public static class MockLoggerExtensions
{
    public static void Verify<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string message) =>
        loggerMock.Verify(l =>
            l.Log(level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString() == message),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
}
