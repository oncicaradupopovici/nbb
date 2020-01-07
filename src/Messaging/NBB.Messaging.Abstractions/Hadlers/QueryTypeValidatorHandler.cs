using NBB.Core.Abstractions;
using System;

namespace NBB.Messaging.Abstractions.Hadlers
{
    public class QueryTypeValidatorHandler : MessageTypeHandler<Type>
    {
        public override string Handle(Type request)
        {
            if (typeof(IQuery).IsAssignableFrom(request))
            {
                return $"ch.queries.{request.GetLongPrettyName()}";
            }
            return base.Handle(request);
        }
    }
}