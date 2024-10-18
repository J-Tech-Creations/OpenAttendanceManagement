using ResultBoxes;
namespace OpenAttendanceManagement.Common.EventSourcing;

public interface IAggregatePayload;
public record EmptyAggregatePayload : IAggregatePayload;
public record Aggregate<TAggregatePayload>(Guid AggregateId, int Version, TAggregatePayload Payload)
    where TAggregatePayload : IAggregatePayload;
public interface IAggregateProjector
{
    public static abstract Func<IAggregatePayload, IEvent, IAggregatePayload> Projector();
}
public interface IEventPayload;
public interface IEvent
{
    public IEventPayload GetPayload();
}
public record Event<TPayload>(Guid AggregateId, int Version, TPayload Payload) : IEvent where TPayload : IEventPayload
{
    public IEventPayload GetPayload() => Payload;
}
public record UnconfirmedUser(string Name, string Email) : IAggregatePayload;
public record ConfirmedUser(string Name, string Email) : IAggregatePayload;
public record UserRegistered(string Name, string Email) : IEventPayload;
public record UserConfirmed : IEventPayload;
public record UserUnconfirmed : IEventPayload;
public class UserProjector : IAggregateProjector
{
    public static Func<IAggregatePayload, IEvent, IAggregatePayload> Projector() =>
        (payload, @event) => payload switch
        {
            EmptyAggregatePayload => @event.GetPayload() switch
            {
                UserRegistered registered => new UnconfirmedUser(registered.Name, registered.Email),
                _ => payload
            },
            UnconfirmedUser unconfirmedUser => @event.GetPayload() switch
            {
                UserConfirmed => new ConfirmedUser(unconfirmedUser.Name, unconfirmedUser.Email),
                _ => payload
            },
            ConfirmedUser confirmedUser => @event.GetPayload() switch
            {
                UserUnconfirmed => new UnconfirmedUser(confirmedUser.Name, confirmedUser.Email),
                _ => payload
            }
        };
}
public record Branch(string Name) : IAggregatePayload;
public record BranchCreated(string Name) : IEventPayload;
public class BranchProjector : IAggregateProjector
{
    public static Func<IAggregatePayload, IEvent, IAggregatePayload> Projector() =>
        (payload, @event) => payload switch
        {
            EmptyAggregatePayload => @event.GetPayload() switch
            {
                BranchCreated created => new Branch(created.Name),
                _ => payload
            },
            _ => payload
        };
}
public record PartitionKeys(Guid AggregateId, string Group, string RootPartitionKey)
{
    public static PartitionKeys Generate(string group = "default", string rootPartitionKey = "default") =>
        new(Guid.NewGuid(), group, rootPartitionKey);
    public static PartitionKeys Existing(
        Guid aggregateId,
        string group = "default",
        string rootPartitionKey = "default") =>
        new(aggregateId, group, rootPartitionKey);
}
public record EventOrNone(IEventPayload? EventPayload, bool HasEvent)
{
    public static EventOrNone Empty => new(default, false);
    public static ResultBox<EventOrNone> None => Empty;
    public static EventOrNone FromValue(IEventPayload value) => new(value, true);
    public static ResultBox<EventOrNone> Event(IEventPayload value) => ResultBox.FromValue(FromValue(value));
    public IEventPayload GetValue() => HasEvent && EventPayload is not null
        ? EventPayload
        : throw new ResultsInvalidOperationException("no value");
    public static implicit operator EventOrNone(UnitValue value) => Empty;
}
public interface IPureCommandCommon<TCommand, TProjector, TAggregatePayload>
    where TCommand : IPureCommandCommon<TCommand, TProjector, TAggregatePayload>, IEquatable<TCommand>
    where TProjector : IAggregateProjector
    where TAggregatePayload : IAggregatePayload;
public interface
    IPureCommand<TCommand, TProjector, TAggregatePayload> : IPureCommandCommon<TCommand, TProjector, TAggregatePayload>
    where TCommand : IPureCommand<TCommand, TProjector, TAggregatePayload>, IEquatable<TCommand>
    where TProjector : IAggregateProjector
    where TAggregatePayload : IAggregatePayload
{
    public static abstract PartitionKeys SpecifyPartitionKeys(TCommand input);
    public static abstract ResultBox<EventOrNone> Handle(TCommand input, Func<Aggregate<TAggregatePayload>> stateFunc);
}
public interface
    IPureCommandWithInjection<TCommand, TProjector, TAggregatePayload, TInjections> : IPureCommandCommon<TCommand,
    TProjector,
    TAggregatePayload>
    where TCommand : IPureCommandWithInjection<TCommand, TProjector, TAggregatePayload, TInjections>,
    IEquatable<TCommand>
    where TProjector : IAggregateProjector
    where TAggregatePayload : IAggregatePayload
{
    public static abstract PartitionKeys SpecifyPartitionKeys(TCommand input);
    public static abstract ResultBox<EventOrNone> Handle(
        TCommand input,
        Func<Aggregate<TAggregatePayload>> stateFunc,
        TInjections injections);
}
public record RegisterClient1(string Name, string Email)
    : IPureCommand<RegisterClient1, UserProjector, EmptyAggregatePayload>
{
    public static PartitionKeys SpecifyPartitionKeys(RegisterClient1 input) => PartitionKeys.Generate();
    public static ResultBox<EventOrNone> Handle(
        RegisterClient1 input,
        Func<Aggregate<EmptyAggregatePayload>> stateFunc) =>
        EventOrNone.Event(new UserRegistered(input.Name, input.Email));
}
public record RegisterClientWithInjection(string Name, string Email)
    : IPureCommandWithInjection<RegisterClientWithInjection, UserProjector, EmptyAggregatePayload,
        RegisterClientWithInjection.Injections>
{
    public static PartitionKeys SpecifyPartitionKeys(RegisterClientWithInjection input) => PartitionKeys.Generate();
    public static ResultBox<EventOrNone> Handle(
        RegisterClientWithInjection input,
        Func<Aggregate<EmptyAggregatePayload>> stateFunc,
        Injections injections) => injections.EmailAlreadyExists(input.Email)
        ? EventOrNone.None
        : EventOrNone.Event(new UserRegistered(input.Name, input.Email));
    public class Injections
    {
        public Func<string, bool> EmailAlreadyExists { get; init; }
    }
}
public record RegisterBranch(string Name)
    : IPureCommand<RegisterBranch, BranchProjector, EmptyAggregatePayload>
{
    public static PartitionKeys SpecifyPartitionKeys(RegisterBranch input) => PartitionKeys.Generate();
    public static ResultBox<EventOrNone> Handle(
        RegisterBranch input,
        Func<Aggregate<EmptyAggregatePayload>> stateFunc) =>
        EventOrNone.Event(new BranchCreated(input.Name));
}
public interface ISekibanUsecase<in TIn, TOut>
    where TIn : class, ISekibanUsecase<TIn, TOut>, IEquatable<TIn>
    where TOut : notnull
{
    public static abstract ResultBox<TOut> Execute(TIn input);
}
public interface ISekibanUsecaseWithInjection<in TIn, TOut, TInjection>
    where TIn : class, ISekibanUsecaseWithInjection<TIn, TOut, TInjection>, IEquatable<TIn>
    where TOut : notnull
{
    public static abstract Task<ResultBox<TOut>> Execute(TIn input, TInjection injection);
}
public record CommandResponse(Guid AggregateId);
public record RegisterBranchAndClient(string BranchName, string ClientName, string ClientEmail)
    : ISekibanUsecaseWithInjection<RegisterBranchAndClient, bool, RegisterBranchAndClient.Injections>
{
    public static Task<ResultBox<bool>> Execute(RegisterBranchAndClient input, Injections injection) =>
        ResultBox
            .FromValue(new RegisterBranch(input.BranchName))
            .Conveyor(injection.RegisterBranch)
            .Remap(result => new RegisterClientWithInjection(input.ClientName, input.ClientEmail))
            .Combine(
                _ => ResultBox.FromValue(
                    new RegisterClientWithInjection.Injections
                        { EmailAlreadyExists = injection.EmailAlreadyExists }))
            .Conveyor(injection.RegisterClient)
            .Conveyor(_ => ResultBox.FromValue(true));
    public class Injections
    {
        public Func<RegisterBranch, Task<ResultBox<CommandResponse>>> RegisterBranch { get; init; }
        public Func<RegisterClientWithInjection, RegisterClientWithInjection.Injections,
            Task<ResultBox<CommandResponse>>> RegisterClient { get; init; }
        public Func<string, bool> EmailAlreadyExists { get; init; }
    }
}
// public class Pr
// {
//     public async Task m()
//     {
//         dynamic a = 1;
//
// a.MapPost(
//     "api/bandc",
//     ([FromBody] RegisterBranchAndClient input, [FromServices] ISekibanExecutor sekibanExecutor) =>
//         RegisterBranchAndClient.Execute(input, new RegisterBranchAndClient.Injections()
//         {
//             RegisterBranch = (input) => sekibanExecutor.ExecuteCommand(input),
//             RegisterClient = (input) => sekibanExecutor.ExecuteCommand(input),
//             EmailAlreadyExists = (email) => sekibanExecutor.ExecuteQuery(new EmailExists(email))
//         })
//     );
//     }
//     
//     
//     
// }
