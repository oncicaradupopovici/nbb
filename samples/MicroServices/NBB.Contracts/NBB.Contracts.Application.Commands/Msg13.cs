using NBB.Application.DataContracts;
using System;

namespace NBB.Contracts.Application.Commands
{
    public class Msg13 : Command
    {
        public Guid ClientId { get; }

        public Msg13(Guid clientId, CommandMetadata metadata = null)
            : base(metadata)
        {
            ClientId = clientId;
        }

    }
}