using MediatR;
using NBB.Contracts.Application.Commands;
using NBB.Contracts.Domain.ContractAggregate;
using NBB.Data.Abstractions;
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
        private readonly IEventSourcedRepository<Contract> _repository;

        public MsgCommandHandlers(IEventSourcedRepository<Contract> repository)
        {
            _repository = repository;
        }

        public async Task Handle(Msg2 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            var contract = new Contract(request.ClientId);
            Thread.Sleep(10); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg3 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg4 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            var contract = new Contract(request.ClientId);
            Thread.Sleep(10); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg11 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public Task Handle(Msg5 request, CancellationToken cancellationToken)
        {
            Thread.Sleep(100);//simulate some synchronuos work
            return Task.CompletedTask;
        }

        public Task Handle(Msg12 request, CancellationToken cancellationToken)
        {
            Thread.Sleep(100);//simulate some synchronuos work
            return Task.CompletedTask;
        }

        public async Task Handle(Msg6 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg13 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg7 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg14 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg8 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg15 request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message received {request.ClientId}");
            await Task.Delay(100); //simulate some async wait - like a database read
            var contract = new Contract(request.ClientId);
            Thread.Sleep(100); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }

        public async Task Handle(Msg9 request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);//simulate some async wait - like a database read
        }

        public async Task Handle(Msg10 request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);//simulate some async wait - like a database read
        }

        public async Task Handle(Msg1 request, CancellationToken cancellationToken)
        {
            var contract = new Contract(request.ClientId);
            Thread.Sleep(10); //simulate some synchronuos work
            await _repository.SaveAsync(contract, cancellationToken);
        }
    }
}
