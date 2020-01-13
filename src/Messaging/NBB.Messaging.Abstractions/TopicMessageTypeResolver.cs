using Microsoft.Extensions.Configuration;
using NBB.Messaging.Abstractions.Hadlers;
using NBB.Messaging.DataContracts;
using System;
using System.Linq;

namespace NBB.Messaging.Abstractions
{
    public class TopicMessageTypeResolver : IDefaultTopicProcesor
    {
        private readonly IConfiguration _configuration;
        private readonly Type _messageType;

        public TopicMessageTypeResolver(IConfiguration configuration, Type messageType)
        {
            _configuration = configuration;
            _messageType = messageType;
        }

        public string Execute() =>
            GetTopicNameFromAttribute(_messageType) ?? new CommandTypeValidatorHandler()
                .Then(new EventTypeValidatorHandler())
                .Then(new QueryTypeValidatorHandler())
                .Then(new DefaultTypeHandler()).Handle(_messageType);

        private string GetTopicNameFromAttribute(Type messageType)
        {
            var topicNameResolver = messageType.GetCustomAttributes(typeof(TopicNameResolverAttribute), true).FirstOrDefault() as TopicNameResolverAttribute;
            return topicNameResolver?.ResolveTopicName(messageType, _configuration);
        }

        public string Execute(string text)
        {
            return text;
        }
    }
}