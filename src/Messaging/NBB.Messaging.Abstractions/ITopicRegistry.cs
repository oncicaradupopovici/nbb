using System;

namespace NBB.Messaging.Abstractions
{
    public interface ITopicRegistry
    {
        string GetTopicForMessageType(Type messageType, bool includePrefix = true);
        string GetTopicPrefixFromTopic(string topic);
        string GetTopicForTopicPrefix(Type messageType, string topicPrefix);
        string GetSharedTopicPrefix();
        string GetTopicForName(string topicName, bool includePrefix = true);
    }
}