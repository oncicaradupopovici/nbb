namespace NBB.Messaging.Abstractions
{
    public class TopicPrefixConcatenateProcessor : ITopicProcesor
    {
        private readonly string _topicPrefix;

        public TopicPrefixConcatenateProcessor(string topicPrefix)
        {
            _topicPrefix = topicPrefix;
        }

        public string Execute(string text) =>
            _topicPrefix + "." + text;
    }
}
