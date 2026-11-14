namespace GestionCafeteriaEscolar.Application.Core;

public enum ResultType
{
    Ok,
    Created,
    NoContent,
    Invalid,
    NotFound
}

public class ServiceResult<T>
{
    public ResultType Type { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }

    public bool IsSuccess => Type is ResultType.Ok or ResultType.Created or ResultType.NoContent;

    public static ServiceResult<T> Ok(T data)
    {
        return new ServiceResult<T> { Type = ResultType.Ok, Data = data };
    }

    public static ServiceResult<T> Created(T data)
    {
        return new ServiceResult<T> { Type = ResultType.Created, Data = data };
    }

    public static ServiceResult<T> NoContent()
    {
        return new ServiceResult<T> { Type = ResultType.NoContent };
    }

    public static ServiceResult<T> Invalid(string error)
    {
        return new ServiceResult<T> { Type = ResultType.Invalid, Error = error };
    }

    public static ServiceResult<T> NotFound()
    {
        return new ServiceResult<T> { Type = ResultType.NotFound };
    }
}