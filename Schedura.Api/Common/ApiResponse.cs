namespace Schedura.Api.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data) =>
        new() { Success = true, Data = data };

    public static ApiResponse<T> Empty() =>
        new() { Success = true, Data = default };

    public static ApiResponse<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Data = default, Errors = errors.ToList().AsReadOnly() };

    public static ApiResponse<T> Fail(string error) => Fail([error]);
}

public sealed class ApiResponse : ApiResponse<object?>
{
    public new static ApiResponse Empty() =>
        new() { Success = true, Data = null };

    public new static ApiResponse Fail(IEnumerable<string> errors) =>
        new() { Success = false, Data = null, Errors = errors.ToList().AsReadOnly() };

    public new static ApiResponse Fail(string error) => Fail([error]);
}
