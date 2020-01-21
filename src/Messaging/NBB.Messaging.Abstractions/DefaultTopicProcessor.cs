using Microsoft.Extensions.Configuration;
using NBB.Messaging.Abstractions.Hadlers;
using NBB.Messaging.DataContracts;
using System;
using System.Linq;

namespace NBB.Messaging.Abstractions
{
    static class DefaultTopicProcessor
    {
        public static string GetTopic(Type messageType, IConfiguration configuration) =>
            GetTopicNameFromAttribute(messageType, configuration) ?? new CommandTypeValidatorHandler()
                .Then(new EventTypeValidatorHandler())
                .Then(new QueryTypeValidatorHandler())
                .Then(new DefaultTypeHandler()).Handle(messageType);

        private static string GetTopicNameFromAttribute(Type messageType, IConfiguration configuration)
        {
            var topicNameResolver = messageType.GetCustomAttributes(typeof(TopicNameResolverAttribute), true).FirstOrDefault() as TopicNameResolverAttribute;
            return topicNameResolver?.ResolveTopicName(messageType, configuration);
        }
    }
}