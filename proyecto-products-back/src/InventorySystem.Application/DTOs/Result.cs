namespace InventorySystem.Application.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string Message { get; }

    private Result(bool isSuccess, T? data, string message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public static Result<T> Success(T data, string message = "") => new(true, data, message);
    public static Result<T> Failure(string message) => new(false, default, message);
}

public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }

    private Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success(string message = "") => new(true, message);
    public static Result Failure(string message) => new(false, message);
}
