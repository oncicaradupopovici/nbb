using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

            return (_includePrefix ? GetTopicPrefix() : string.Empty) + text;
        }

        private string GetTopicPrefix()
            => _configuration.GetSection("Messaging")?["TopicPrefix"] ?? "";
    }
}