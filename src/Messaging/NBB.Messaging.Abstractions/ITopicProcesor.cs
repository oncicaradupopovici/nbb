namespace NBB.Messaging.Abstractions
{
    public interface ITopicProcesor
    {
        string Execute(string text);
    }
}