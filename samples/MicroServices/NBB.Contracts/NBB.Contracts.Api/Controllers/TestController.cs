using Microsoft.AspNetCore.Mvc;
using NBB.Application.DataContracts;
using NBB.Contracts.Application.Commands;
using NBB.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contracts.Api.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IMessageBusPublisher _messageBusPublisher;
        private List<Command> commands = new List<Command>();

        public TestController(IMessageBusPublisher messageBusPublisher)
        {
            _messageBusPublisher = messageBusPublisher;
            commands.Add(new Msg1(Guid.NewGuid()));
            commands.Add(new Msg2(Guid.NewGuid()));
            commands.Add(new Msg3(Guid.NewGuid()));
            commands.Add(new Msg4(Guid.NewGuid()));
            commands.Add(new Msg5(Guid.NewGuid()));
            commands.Add(new Msg6(Guid.NewGuid()));
            commands.Add(new Msg7(Guid.NewGuid()));
            commands.Add(new Msg8(Guid.NewGuid()));
            commands.Add(new Msg9(Guid.NewGuid()));
            commands.Add(new Msg10(Guid.NewGuid()));
            commands.Add(new Msg11(Guid.NewGuid()));
            commands.Add(new Msg12(Guid.NewGuid()));
            commands.Add(new Msg13(Guid.NewGuid()));
            commands.Add(new Msg14(Guid.NewGuid()));
            commands.Add(new Msg15(Guid.NewGuid()));
        }

        // POST api/test/OnlyAMessage sent
        [HttpPost("one")]
        public async Task ScenarioOne([FromBody]Msg1 command, CancellationToken cancellationToken)
        {
            var list = new List<Task>();
            for (int i = 0; i < 1000; i++)
                list.Add(_messageBusPublisher.PublishAsync(command, cancellationToken));

            await Task.WhenAll(list);
        }

        // POST api/test/OnlyAMessage sent
        [HttpPost("two")]
        public async Task ScenarioTwo([FromBody]Msg2 command, CancellationToken cancellationToken)
        {
            foreach (var cmd in commands)
                await PostCommand(cmd, cancellationToken);
        }

        public async Task PostCommand(Command command, CancellationToken cancellationToken)
        {
            var list = new List<Task>();

            for (int i = 0; i < 1000; i++)
                list.Add(_messageBusPublisher.PublishAsync(command, cancellationToken));

            await Task.WhenAll(list);
        }

        [HttpPost("three")]
        public async Task ScenarioThree(CancellationToken cancellationToken)
        {
            var list = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                foreach (var cmd in commands)
                    list.Add(_messageBusPublisher.PublishAsync(cmd, cancellationToken));
            }

            await Task.WhenAll(list);
        }
    }
}