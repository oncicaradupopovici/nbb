using NBB.Core.Abstractions;
using System;

namespace NBB.Messaging.Abstractions.Hadlers
{
    public class EventTypeValidatorHandler : MessageTypeHandler<Type>
    {
        public override string Handle(Type request)
        {
            if (typeof(IEvent).IsAssignableFrom(request))
            {
                return $"ch.events.{request.GetLongPrettyName()}";
            }

            return base.Handle(request);
        }
    }
}