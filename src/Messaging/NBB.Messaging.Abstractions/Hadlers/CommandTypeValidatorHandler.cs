using NBB.Core.Abstractions;
using System;

namespace NBB.Messaging.Abstractions.Hadlers
{
    public class CommandTypeValidatorHandler : MessageTypeHandler<Type>
    {
        public override string Handle(Type request)
        {
            if (typeof(ICommand).IsAssignableFrom(request))
            {
                return $"ch.commands.{request.GetLongPrettyName()}";
            }
            return base.Handle(request);
        }
    }
}