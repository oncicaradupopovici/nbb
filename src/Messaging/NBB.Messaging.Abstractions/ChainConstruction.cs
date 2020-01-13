namespace NBB.Messaging.Abstractions
{
    static class ChainConstruction
    {
        public static ITopicProcesor Then(this ITopicProcesor first, ITopicProcesor next) =>
            new ChainedProcessor(first, next);
    }
}