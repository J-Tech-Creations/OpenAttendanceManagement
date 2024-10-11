using ResultBoxes;
namespace OpenAttendanceManagement.Common.UseCases;

public interface ISekibanUsecaseExecutor
{
    public Task<ResultBox<TOut>> Execute<TIn, TOut>(ISekibanUsecaseAsync<TIn, TOut> usecaseAsync)
        where TIn : class, ISekibanUsecaseAsync<TIn, TOut>, IEquatable<TIn> where TOut : notnull;
    public ResultBox<TOut> Execute<TIn, TOut>(ISekibanUsecase<TIn, TOut> usecaseAsync)
        where TIn : class, ISekibanUsecase<TIn, TOut>, IEquatable<TIn> where TOut : notnull;
}
