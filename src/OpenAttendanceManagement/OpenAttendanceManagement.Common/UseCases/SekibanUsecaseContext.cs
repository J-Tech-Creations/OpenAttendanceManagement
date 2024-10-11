using ResultBoxes;
using Sekiban.Core;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Command;
using Sekiban.Core.History;
using Sekiban.Core.Query.QueryModel;
using Sekiban.Core.Query.SingleProjections;
namespace OpenAttendanceManagement.Common.UseCases;

public class SekibanUsecaseContext(ISekibanExecutor executor) : ISekibanUsecaseContext
{

    public ResultBox<T1> GetRequiredService<T1>() where T1 : class => executor.GetRequiredService<T1>();
    public Task<ResultBox<CommandExecutorResponse>> ExecuteCommand<TCommand>(
        TCommand command,
        List<CallHistory>? callHistories = null) where TCommand : ICommandCommon =>
        executor.ExecuteCommand(command, callHistories);
    public Task<ResultBox<CommandExecutorResponse>> ExecuteCommandWithoutValidation<TCommand>(
        TCommand command,
        List<CallHistory>? callHistories = null) where TCommand : ICommandCommon =>
        executor.ExecuteCommandWithoutValidation(command, callHistories);
    public Task<ResultBox<CommandExecutorResponseWithEvents>> ExecuteCommandWithoutValidationWithEvents<TCommand>(
        TCommand command,
        List<CallHistory>? callHistories = null) where TCommand : ICommandCommon =>
        executor.ExecuteCommandWithoutValidationWithEvents(command, callHistories);
    public Task<ResultBox<CommandExecutorResponseWithEvents>> ExecuteCommandWithEvents<TCommand>(
        TCommand command,
        List<CallHistory>? callHistories = null) where TCommand : ICommandCommon =>
        executor.ExecuteCommandWithEvents(command, callHistories);
    public Task<ResultBox<AggregateState<TAggregatePayloadCommon>>> GetAggregateState<TAggregatePayloadCommon>(
        Guid aggregateId,
        string rootPartitionKey = "default",
        int? toVersion = null,
        SingleProjectionRetrievalOptions? retrievalOptions = null)
        where TAggregatePayloadCommon : IAggregatePayloadCommon =>
        executor.GetAggregateState<TAggregatePayloadCommon>(aggregateId, rootPartitionKey, toVersion, retrievalOptions);
    public Task<ResultBox<AggregateState<TAggregatePayloadCommon>>> GetAggregateStateFromInitial<
        TAggregatePayloadCommon>(
        Guid aggregateId,
        string rootPartitionKey = "default",
        int? toVersion = null) where TAggregatePayloadCommon : IAggregatePayloadCommon =>
        executor.GetAggregateStateFromInitial<TAggregatePayloadCommon>(aggregateId, rootPartitionKey, toVersion);
    public Task<ResultBox<SingleProjectionState<TSingleProjectionPayload>>> GetSingleProjectionStateFromInitial<
        TSingleProjectionPayload>(
        Guid aggregateId,
        string rootPartitionKey = "default",
        int? toVersion = null) where TSingleProjectionPayload : class, ISingleProjectionPayloadCommon =>
        executor.GetSingleProjectionStateFromInitial<TSingleProjectionPayload>(
            aggregateId,
            rootPartitionKey,
            toVersion);
    public Task<ResultBox<SingleProjectionState<TSingleProjectionPayload>>> GetSingleProjectionState<
        TSingleProjectionPayload>(
        Guid aggregateId,
        string rootPartitionKey = "default",
        int? toVersion = null,
        SingleProjectionRetrievalOptions? retrievalOptions = null)
        where TSingleProjectionPayload : class, ISingleProjectionPayloadCommon =>
        executor.GetSingleProjectionState<TSingleProjectionPayload>(
            aggregateId,
            rootPartitionKey,
            toVersion,
            retrievalOptions);
    public Task<ResultBox<TOutput>> ExecuteQuery<TOutput>(INextQueryCommonOutput<TOutput> query)
        where TOutput : notnull => executor.ExecuteQuery(query);
    public Task<ResultBox<ListQueryResult<TOutput>>> ExecuteQuery<TOutput>(INextListQueryCommonOutput<TOutput> query)
        where TOutput : notnull => executor.ExecuteQuery(query);
    public Task<ResultBox<ListQueryResult<TOutput>>> ExecuteQuery<TOutput>(IListQueryInput<TOutput> param)
        where TOutput : IQueryResponse => executor.ExecuteQuery(param);
    public Task<ResultBox<TOutput>> ExecuteQuery<TOutput>(IQueryInput<TOutput> param) where TOutput : IQueryResponse =>
        executor.ExecuteQuery(param);
}
