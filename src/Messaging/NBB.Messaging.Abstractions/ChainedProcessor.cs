namespace NBB.Messaging.Abstractions
{
    public class ChainedProcessor : ITopicProcesor
    {
        private ITopicProcesor Inner { get; }
        private ITopicProcesor Next { get; }

        public ChainedProcessor(ITopicProcesor inner, ITopicProcesor next)
        {
            Inner = inner;
            Next = next;
        }

        public string Execute(string text) =>
            this.Next.Execute(Inner.Execute(text));
    }
}