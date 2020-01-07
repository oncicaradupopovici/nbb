using Microsoft.Extensions.Configuration;
using NBB.Messaging.Abstractions.Hadlers;
using NBB.Messaging.DataContracts;
using System;
using System.Linq;

namespace NBB.Messaging.Abstractions
{
    public class DefaultTopicRegistry : ITopicRegistry
    {
        private readonly IConfiguration _configuration;

        public DefaultTopicRegistry(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetTopicForMessageType(Type messageType, bool includePrefix = true)
            => GetTopicForName(GetTopic(messageType), includePrefix);

        public string GetTopicForName(string topicName, bool includePrefix = true)
        {
            if (topicName == null)
            {
                return null;
            }

            var topic = (includePrefix ? GetTopicPrefix() : string.Empty) + topicName;
            return GetModifiedTopic(ref topic);
        }

        public string GetTopicForTopicPrefix(Type messageType, string topicPrefix)
          => topicPrefix + "." + GetTopic(messageType);

        public string GetSharedTopicPrefix()
            => _configuration.GetSection("Messaging")["SharedTopicPrefix"];

        public string GetTopicPrefixFromTopic(string topic)
            => topic.Split('.')[0] + ".";



        
        private string GetModifiedTopic(ref string topic)
           => topic.Replace("+", ".").Replace("<", "_").Replace(">", "_");

        private string GetTopicPrefix()
            => _configuration.GetSection("Messaging")?["TopicPrefix"] ?? "";

        private string GetTopicNameFromAttribute(Type messageType)
        {
            var topicNameResolver = messageType.GetCustomAttributes(typeof(TopicNameResolverAttribute), true).FirstOrDefault() as TopicNameResolverAttribute;
            return topicNameResolver?.ResolveTopicName(messageType, _configuration);
        }

        private string GetTopic(Type messageType)
        {
            var topic = GetTopicNameFromAttribute(messageType);
            if (topic == null)
            {
                topic = new CommandTypeValidatorHandler()
                    .Then(new EventTypeValidatorHandler())
                    .Then(new QueryTypeValidatorHandler())
                    .Then(new DefaultTypeHandler()).Handle(messageType);
            }

            return topic;
        }
    }
}