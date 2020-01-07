using NBB.Application.DataContracts;
using System;

namespace NBB.Contracts.Application.Commands
{
    public class Msg4 : Command
    {
        public Guid ClientId { get; }

        public Msg4(Guid clientId, CommandMetadata metadata = null)
            : base(metadata)
        {
            ClientId = clientId;
        }

    }
}