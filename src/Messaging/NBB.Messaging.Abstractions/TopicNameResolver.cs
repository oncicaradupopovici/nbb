using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace NBB.Messaging.Abstractions
{
    public class TopicNameResolver : ITopicProcesor
    {
        private readonly IConfiguration _configuration;
        private readonly bool _includePrefix;

        public TopicNameResolver(bool includePrefix, IConfiguration configuration)
        {
            _includePrefix = includePrefix;
            _configuration = configuration;
        }

        private IEnumerable<RuleValue> Rules { get; } = new[]
        {
            new RuleValue("+", "."),
            new RuleValue("<", "_"),
            new RuleValue(">", "_"),
        };

        public string Execute(string text)
        {
            if (text == null)
            {
                return null;
            }

            var topic = (_includePrefix ? GetTopicPrefix() : string.Empty) + text;
            return GetModifiedTopic(ref topic);
        }

        private string GetTopicPrefix()
            => _configuration.GetSection("Messaging")?["TopicPrefix"] ?? "";


        private string GetModifiedTopic(ref string topic) =>
            topic = Rules.Aggregate(topic, (current, rule) => current.Replace(rule.OldValue, rule.NewValue));
    }
}