using ResultBoxes;
namespace OpenAttendanceManagement.Common.UseCases;

public interface ISekibanUsecase<in TIn, TOut>
    where TIn : class, ISekibanUsecase<TIn, TOut>, IEquatable<TIn>
    where TOut : notnull
{
    public static abstract ResultBox<TOut> Execute(TIn input, ISekibanUsecaseContext context);
}
