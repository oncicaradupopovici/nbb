FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY NuGet.config .
COPY dependencies.props .
COPY Directory.Build.props .
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.Worker/NBB.Contracts.Worker.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.Worker/"]
COPY ["src/Messaging/NBB.Messaging.Host/NBB.Messaging.Host.csproj", "src/Messaging/NBB.Messaging.Host/"]
COPY ["src/Messaging/NBB.Messaging.Abstractions/NBB.Messaging.Abstractions.csproj", "src/Messaging/NBB.Messaging.Abstractions/"]
COPY ["src/Tenancy/NBB.Tenancy.Abstractions/NBB.Tenancy.Abstractions.csproj", "src/Tenancy/NBB.Tenancy.Abstractions/"]
COPY ["src/Application/NBB.Application.DataContracts/NBB.Application.DataContracts.csproj", "src/Application/NBB.Application.DataContracts/"]
COPY ["src/Messaging/NBB.Messaging.DataContracts/NBB.Messaging.DataContracts.csproj", "src/Messaging/NBB.Messaging.DataContracts/"]
COPY ["src/Core/NBB.Core.Abstractions/NBB.Core.Abstractions.csproj", "src/Core/NBB.Core.Abstractions/"]
COPY ["src/Resiliency/NBB.Resiliency/NBB.Resiliency.csproj", "src/Resiliency/NBB.Resiliency/"]
COPY ["src/Correlation/NBB.Correlation/NBB.Correlation.csproj", "src/Correlation/NBB.Correlation/"]
COPY ["src/Core/NBB.Core.Pipeline/NBB.Core.Pipeline.csproj", "src/Core/NBB.Core.Pipeline/"]
COPY ["src/EventStore/NBB.GetEventStore/NBB.GetEventStore.csproj", "src/EventStore/NBB.GetEventStore/"]
COPY ["src/EventStore/NBB.EventStore.Abstractions/NBB.EventStore.Abstractions.csproj", "src/EventStore/NBB.EventStore.Abstractions/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.Application/NBB.Contracts.Application.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.Application/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.ReadModel/NBB.Contracts.ReadModel.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.ReadModel/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.Application.Commands/NBB.Contracts.Application.Commands.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.Application.Commands/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.Domain/NBB.Contracts.Domain.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.Domain/"]
COPY ["src/Domain/NBB.Domain/NBB.Domain.csproj", "src/Domain/NBB.Domain/"]
COPY ["src/Domain/NBB.Domain.Abstractions/NBB.Domain.Abstractions.csproj", "src/Domain/NBB.Domain.Abstractions/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.PublishedLanguage/NBB.Contracts.PublishedLanguage.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.PublishedLanguage/"]
COPY ["src/Data/NBB.Data.Abstractions/NBB.Data.Abstractions.csproj", "src/Data/NBB.Data.Abstractions/"]
COPY ["src/Messaging/NBB.Messaging.Kafka/NBB.Messaging.Kafka.csproj", "src/Messaging/NBB.Messaging.Kafka/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.WriteModel.Data/NBB.Contracts.WriteModel.Data.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.WriteModel.Data/"]
COPY ["src/Data/NBB.Data.EventSourcing/NBB.Data.EventSourcing.csproj", "src/Data/NBB.Data.EventSourcing/"]
COPY ["src/Tenancy/NBB.Tenancy.Impl/NBB.Tenancy.Impl.csproj", "src/Tenancy/NBB.Tenancy.Impl/"]
COPY ["src/EventStore/NBB.EventStore/NBB.EventStore.csproj", "src/EventStore/NBB.EventStore/"]
COPY ["src/Messaging/NBB.Messaging.Nats/NBB.Messaging.Nats.csproj", "src/Messaging/NBB.Messaging.Nats/"]
COPY ["src/EventStore/NBB.EventStore.AdoNet/NBB.EventStore.AdoNet.csproj", "src/EventStore/NBB.EventStore.AdoNet/"]
COPY ["src/Correlation/NBB.Correlation.Serilog/NBB.Correlation.Serilog.csproj", "src/Correlation/NBB.Correlation.Serilog/"]
COPY ["samples/MicroServices/NBB.Contracts/NBB.Contracts.ReadModel.Data/NBB.Contracts.ReadModel.Data.csproj", "samples/MicroServices/NBB.Contracts/NBB.Contracts.ReadModel.Data/"]
COPY ["src/Data/NBB.Data.EntityFramework/NBB.Data.EntityFramework.csproj", "src/Data/NBB.Data.EntityFramework/"]
RUN dotnet restore "samples/MicroServices/NBB.Contracts/NBB.Contracts.Worker/NBB.Contracts.Worker.csproj"
COPY . .
WORKDIR "/src/samples/MicroServices/NBB.Contracts/NBB.Contracts.Worker"
RUN dotnet build "NBB.Contracts.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NBB.Contracts.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NBB.Contracts.Worker.dll"]