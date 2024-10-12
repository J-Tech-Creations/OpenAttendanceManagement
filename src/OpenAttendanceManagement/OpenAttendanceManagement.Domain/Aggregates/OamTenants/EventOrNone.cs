using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Command;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants;

public record EventOrNone<TAggregatePayload>(IEventPayloadApplicableTo<TAggregatePayload>? Value, bool HasValue = true)
    where TAggregatePayload : IAggregatePayloadCommon
{
    public static EventOrNone<TAggregatePayload> Empty => new(default, false);
    public static ResultBox<EventOrNone<TAggregatePayload>> None => Empty;
    public static EventOrNone<TAggregatePayload> FromValue(IEventPayloadApplicableTo<TAggregatePayload> value) =>
        new(value);
    public static ResultBox<EventOrNone<TAggregatePayload>>
        Event(IEventPayloadApplicableTo<TAggregatePayload> value) => ResultBox.FromValue(FromValue(value));

    public IEventPayloadApplicableTo<TAggregatePayload> GetValue() =>
        HasValue && Value is not null ? Value : throw new ResultsInvalidOperationException("no value");

    public static implicit operator EventOrNone<TAggregatePayload>(UnitValue value) => Empty;
}
public static class EventOrNone
{
    public static EventOrNone<TAggregatePayload> FromValue<TAggregatePayload>(
        IEventPayloadApplicableTo<TAggregatePayload> value)
        where TAggregatePayload : IAggregatePayloadCommon => new(value);
    public static ResultBox<EventOrNone<TAggregatePayload>> Event<TAggregatePayload>(
        IEventPayloadApplicableTo<TAggregatePayload> value)
        where TAggregatePayload : IAggregatePayloadCommon =>
        ResultBox.FromValue(new EventOrNone<TAggregatePayload>(value));

    public static ResultBox<EventOrNone<TAggregatePayload>> None<TAggregatePayload>()
        where TAggregatePayload : IAggregatePayloadCommon =>
        EventOrNone<TAggregatePayload>.None;
}
public class TestEventOrNone
{
    public ResultBox<EventOrNone<OamTenant>> Handle1()
        => EventOrNone<OamTenant>.Event(new OamTenantDeleted());

    public ResultBox<EventOrNone<OamTenant>> Handle2()
        => EventOrNone.Event(new OamTenantDeleted());

    // 旧型最短 Event
    public static ResultBox<UnitValue> HandleCommand(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        context.AppendEvent(new TermAddedToTenant(command.Term, command.TermTenantId));
    // 新型最短 Event
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand2(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        EventOrNone.Event(new TermAddedToTenant(command.Term, command.TermTenantId));

    // 旧型処理後 Event
    public static ResultBox<UnitValue> HandleCommand3(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        ResultBox.Start
            .Conveyor(_ => context.AppendEvent(new TermAddedToTenant(command.Term, command.TermTenantId)));
    // 新型処理後 Event
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand4(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        ResultBox.Start
            .Conveyor(_ => EventOrNone.Event(new TermAddedToTenant(command.Term, command.TermTenantId)));

    // 旧型処理後 Event なし
    public static ResultBox<UnitValue> HandleCommand5(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        ResultBox.Start
            .Conveyor(_ => ResultBox.UnitValue);
    // 新型処理後 Event なし
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand6(AddTermToTenant command, ICommandContext<OamTenant> context) =>
        ResultBox.Start
            .Conveyor(_ => EventOrNone.None<OamTenant>());

    public ResultBox<EventOrNone<OamTenant>> Handle3()
        => ResultBox.Start.Conveyor(_ => EventOrNone.Event(new OamTenantDeleted()));

    public Task<ResultBox<EventOrNone<OamTenant>>> Handle4()
        => ResultBox.Start.ToTask().Conveyor(_ => EventOrNone.Event(new OamTenantDeleted()));

    public ResultBox<EventOrNone<OamTenant>> Handle5()
        => EventOrNone<OamTenant>.None;

    public ResultBox<EventOrNone<OamTenant>> Handle6()
        => ResultBox.Start.Conveyor(_ => EventOrNone<OamTenant>.None);

    public ResultBox<EventOrNone<OamTenant>> Handle7()
        => EventOrNone.None<OamTenant>();

    public ResultBox<EventOrNone<OamTenant>> Handle8()
        => ResultBox.Start.Conveyor(_ => EventOrNone.None<OamTenant>());
}
