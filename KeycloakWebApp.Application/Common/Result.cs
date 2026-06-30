namespace KeycloakWebApp.Application.Common;

// If you want to return a result with data, you can use the generic Result<T> class
public class Result<T>
{
    public T Data { get; set; }
    public bool Success { get; }
    public string ErrorMessage { get; }

    public Result(bool success, T data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Ok(T data) =>
         new Result<T>(true, data, null);

    public static Result<T> Fail(string errorMessage) =>
        new Result<T>(false, default, errorMessage);

}

// If no data is needed, you can use the non-generic Result class
public class Result
{
    public bool Success { get; }
    public string ErrorMessage { get; }
    public Result(bool success, string errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
    public static Result Ok() =>
         new Result(true, null);
    public static Result Fail(string errorMessage) =>
        new Result(false, errorMessage);
}
