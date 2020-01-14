using System.Collections.Generic;
using System.Linq;

namespace NBB.Messaging.Abstractions
{
    public class TopicRulesResolver : ITopicProcesor
    {
        private IEnumerable<RuleValue> Rules { get; } = new[]
        {
            new RuleValue("+", "."),
            new RuleValue("<", "_"),
            new RuleValue(">", "_"),
        };

        public string Execute(string text) =>
            Rules.Aggregate(text, (current, rule) => current.Replace(rule.OldValue, rule.NewValue));
    }
}