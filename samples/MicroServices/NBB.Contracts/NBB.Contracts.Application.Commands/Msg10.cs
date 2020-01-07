using NBB.Application.DataContracts;
using System;

namespace NBB.Contracts.Application.Commands
{
    public class Msg10 : Command
    {
        public Guid ClientId { get; }

        public Msg10(Guid clientId, CommandMetadata metadata = null)
            : base(metadata)
        {
            ClientId = clientId;
        }

    }
}