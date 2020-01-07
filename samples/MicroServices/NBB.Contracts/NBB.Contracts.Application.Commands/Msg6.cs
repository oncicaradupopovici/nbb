using NBB.Application.DataContracts;
using System;

namespace NBB.Contracts.Application.Commands
{
    public class Msg6 : Command
    {
        public Guid ClientId { get; }

        public Msg6(Guid clientId, CommandMetadata metadata = null)
            : base(metadata)
        {
            ClientId = clientId;
        }

    }
}