using NBB.Core.Abstractions;
using System;

namespace NBB.Messaging.Abstractions.Hadlers
{
    public class DefaultTypeHandler : MessageTypeHandler<Type>
    {
        public override string Handle(Type request)
            => $"ch.messages.{request.GetLongPrettyName()}";
    }
}