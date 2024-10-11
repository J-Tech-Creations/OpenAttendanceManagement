using ResultBoxes;
using Sekiban.Core;
namespace OpenAttendanceManagement.Common.UseCases;

public static class SekibanExecutorExtensions
{
    public static Task<ResultBox<TOut>> Execute<TIn, TOut>(
        this ISekibanExecutor executor,
        ISekibanUsecaseAsync<TIn, TOut> usecaseAsync)
        where TIn : class, ISekibanUsecaseAsync<TIn, TOut>, IEquatable<TIn> where TOut : notnull
        => executor
            .GetRequiredService<ISekibanUsecaseContext>()
            .Conveyor(context => new SekibanUsecaseExecutor(context).Execute(usecaseAsync));
    public static ResultBox<TwoValues<T1, T2>> GetRequiredService<T1, T2>(this ISekibanExecutor executor)
        where T1 : class where T2 : class =>
        executor.GetRequiredService<T1>().Combine(_ => executor.GetRequiredService<T2>());

    public static ResultBox<ThreeValues<T1, T2, T3>> GetRequiredService<T1, T2, T3>(this ISekibanExecutor executor)
        where T1 : class where T2 : class where T3 : class =>
        executor.GetRequiredService<T1, T2>().Combine((_, _) => executor.GetRequiredService<T3>());

    public static ResultBox<FourValues<T1, T2, T3, T4>> GetRequiredService<T1, T2, T3, T4>(
        this ISekibanExecutor executor)
        where T1 : class where T2 : class where T3 : class where T4 : class =>
        executor.GetRequiredService<T1, T2, T3>().Combine((_, _, _) => executor.GetRequiredService<T4>());

    public static Task<ResultBox<TOut>> Execute<TIn, TOut>(
        this ISekibanUsecaseContext context,
        ISekibanUsecaseAsync<TIn, TOut> usecaseAsync)
        where TIn : class, ISekibanUsecaseAsync<TIn, TOut>, IEquatable<TIn> where TOut : notnull
        => new SekibanUsecaseExecutor(context).Execute(usecaseAsync);

    public static ResultBox<TwoValues<T1, T2>> GetRequiredService<T1, T2>(this ISekibanUsecaseContext executor)
        where T1 : class where T2 : class =>
        executor.GetRequiredService<T1>().Combine(_ => executor.GetRequiredService<T2>());

    public static ResultBox<ThreeValues<T1, T2, T3>> GetRequiredService<T1, T2, T3>(
        this ISekibanUsecaseContext executor)
        where T1 : class where T2 : class where T3 : class =>
        executor.GetRequiredService<T1, T2>().Combine((_, _) => executor.GetRequiredService<T3>());

    public static ResultBox<FourValues<T1, T2, T3, T4>> GetRequiredService<T1, T2, T3, T4>(
        this ISekibanUsecaseContext executor)
        where T1 : class where T2 : class where T3 : class where T4 : class =>
        executor.GetRequiredService<T1, T2, T3>().Combine((_, _, _) => executor.GetRequiredService<T4>());
}
