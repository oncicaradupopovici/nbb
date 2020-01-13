namespace NBB.Messaging.Abstractions
{
    public class RuleValue
    {
        public RuleValue(string oldValue, string newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public string OldValue { get; }
        public string NewValue { get; }
    }
}