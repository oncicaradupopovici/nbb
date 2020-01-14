namespace NBB.Messaging.Abstractions
{
    public class RuleValue
    {
        public string OldValue { get; }
        public string NewValue { get; }
        public RuleValue(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}