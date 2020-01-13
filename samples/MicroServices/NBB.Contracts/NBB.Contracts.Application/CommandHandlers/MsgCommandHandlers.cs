using MediatR;
using NBB.Contracts.Application.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contracts.Application.CommandHandlers
{
    public class MsgCommandHandlers :
        IRequestHandler<Msg1>,
        IRequestHandler<Msg2>,
        IRequestHandler<Msg3>,
        IRequestHandler<Msg4>,
        IRequestHandler<Msg5>,
        IRequestHandler<Msg6>,
        IRequestHandler<Msg7>,
        IRequestHandler<Msg8>,
        IRequestHandler<Msg9>,
        IRequestHandler<Msg10>,
        IRequestHandler<Msg11>,
        IRequestHandler<Msg12>,
        IRequestHandler<Msg13>,
        IRequestHandler<Msg14>,
        IRequestHandler<Msg15>
    {

        public Task Handle(Msg2 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            Thread.Sleep(100); //simulate some synchronuos work
            return Task.CompletedTask;
        }

        public async Task Handle(Msg3 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            Thread.Sleep(100); //simulate some synchronuos work
        }

        public Task Handle(Msg4 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            Thread.Sleep(100); //simulate some synchronuos work
            return Task.CompletedTask;
        }

        public async Task Handle(Msg11 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            Thread.Sleep(100); //simulate some synchronuos work
        }

        public Task Handle(Msg5 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            Thread.Sleep(100);//simulate some synchronuos work
            return Task.CompletedTask;
        }

        public Task Handle(Msg12 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            Thread.Sleep(100);//simulate some synchronuos work
            return Task.CompletedTask;
        }

        public async Task Handle(Msg6 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            Thread.Sleep(100); //simulate some synchronuos work
        }

        public async Task Handle(Msg13 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
        }

        public async Task Handle(Msg7 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
        }

        public async Task Handle(Msg14 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
        }

        public async Task Handle(Msg8 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            Thread.Sleep(100); //simulate some synchronuos work
        }

        public async Task Handle(Msg15 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            Thread.Sleep(100); //simulate some synchronuos work
        }

        public Task Handle(Msg9 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            return Task.CompletedTask;
        }

        public async Task Handle(Msg10 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100);//simulate some asynchronuos work
            Thread.Sleep(100);
        }

        public async Task Handle(Msg1 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100);//simulate some asynchronuos work
        }
    }
}