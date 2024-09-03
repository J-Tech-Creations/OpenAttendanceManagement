using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public static class HttpResponseMessageResultExtensions
{
    public static Task<ResultBox<T>> GetResultFromJsonAsync<T>(
        this HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) where T : notnull
        => ResultBox.FromValue(httpResponseMessage.IsSuccessStatusCode)
            .Conveyor(
                isSuccess => isSuccess switch
                {
                    true => ResultBox.CheckNullWrapTry(
                        async () =>
                            await httpResponseMessage.Content.ReadFromJsonAsync<T>(
                                cancellationToken)),
                    false => ResultBox.CheckNullWrapTry(
                            async () =>
                                await httpResponseMessage.Content.ReadAsStringAsync(
                                    cancellationToken))
                        .Scan(error => Console.Error.WriteLine(error))
                        .Conveyor(error => ResultBox.Error<T>(new ApplicationException(error)))
                });
}