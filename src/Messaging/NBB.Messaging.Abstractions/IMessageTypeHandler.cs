namespace NBB.Messaging.Abstractions
{
    public interface IMessageTypeHandler<T> where T : class
    {
        IMessageTypeHandler<T> Then(IMessageTypeHandler<T> next);
        string Handle(T request);
    }

    public abstract class MessageTypeHandler<T> : IMessageTypeHandler<T> where T : class
    {
        private IMessageTypeHandler<T> Next { get; set; }

        public virtual string Handle(T request)
        {
            return Next?.Handle(request);
        }

        public IMessageTypeHandler<T> Then(IMessageTypeHandler<T> next)
        {
            Next = next;
            return Next;
        }
    }
}