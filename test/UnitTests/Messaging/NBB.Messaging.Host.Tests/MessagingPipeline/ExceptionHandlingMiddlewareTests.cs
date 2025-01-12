// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NBB.Messaging.Abstractions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NBB.Messaging.Host.Tests.MessagingPipeline
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async void Should_logSuccessMessage()
        {
            //Arrange
            var mockedLogger = Mock.Of<ILogger<ExceptionHandlingMiddleware>>();
            var mockedMessagePub = Mock.Of<IMessageBusPublisher>();
            var deadLetterQueue = Mock.Of<IDeadLetterQueue>();
            var correlationMiddleWare = new ExceptionHandlingMiddleware(mockedLogger, deadLetterQueue);
            var sentMessage = new { Field = "value" };
            var envelope = new MessagingEnvelope(new System.Collections.Generic.Dictionary<string, string>(), sentMessage);

            Task next() => Task.CompletedTask;

            //Act
            await correlationMiddleWare.Invoke(new MessagingContext(envelope, string.Empty, null), default, next);

            //Assert
            VerifyLog(mockedLogger, LogLevel.Information, "processed in");
        }

        [Fact]
        public async void Should_logErrorMessageWhenExceptionIsThrown()
        {
            //Arrange
            var mockedLogger = Mock.Of<ILogger<ExceptionHandlingMiddleware>>();
            var deadLetterQueue = Mock.Of<IDeadLetterQueue>();
            var correlationMiddleWare = new ExceptionHandlingMiddleware(mockedLogger, deadLetterQueue);
            var sentMessage = new { Field = "value" };
            var envelope = new MessagingEnvelope(new System.Collections.Generic.Dictionary<string, string>(), sentMessage);

            Task next() => throw new ApplicationException();

            //Act
            try
            {
                await correlationMiddleWare.Invoke(new MessagingContext(envelope, string.Empty, null), default, next);
            }
            catch
            {
                // ignored
            }

            //Assert
            VerifyLog(mockedLogger, LogLevel.Error, nameof(ApplicationException));
        }

        [Fact]
        public async void Should_callNextPipelineMiddleware()
        {
            //Arrange

            var executionTimeMiddleware = new ExceptionHandlingMiddleware(Mock.Of<ILogger<ExceptionHandlingMiddleware>>(), Mock.Of<IDeadLetterQueue>());
            var sentMessage = new { Field = "value" };
            bool isNextMiddlewareCalled = false;
            var envelope = new MessagingEnvelope(new System.Collections.Generic.Dictionary<string, string>(), sentMessage);


            Task Next() { isNextMiddlewareCalled = true; return Task.CompletedTask; }

            //Act
            await executionTimeMiddleware.Invoke(new MessagingContext(envelope, string.Empty, null), default, Next);

            //Assert
            isNextMiddlewareCalled.Should().BeTrue();
        }

        private void VerifyLog(ILogger<ExceptionHandlingMiddleware> mockedLogger, LogLevel logLevel, string containedString)
        {
            Mock.Get(mockedLogger).Verify(x => x.Log(logLevel, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(containedString)),
                null, It.IsAny<Func<It.IsAnyType, Exception, string>>()));
        }
    }
}
