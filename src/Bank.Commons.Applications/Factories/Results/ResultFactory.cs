using FluentValidation.Results;

namespace Bank.Commons.Applications.Factories.Results;

public class ResultFactory : IResultFactory
{
    public Result<T> CreateSuccess<T>(T content)
        => new(content) { Success = true, };

    public Result<T> CreateFailure<T>(string code, string message)
        => new(default(T))
        {
            Success = false,
            Failures =
            [
                new ResultFail() { Code = code, Message = message }
            ]
        };

    public Result<T> CreateFailure<T>(Result result)
        => new(default(T)) { Success = false, Failures = result.Failures, };

    public Result<T> CreateFailure<T>(List<ResultFail> failures)
        => new(default(T)) { Success = false, Failures = failures, };

    public Result<T> CreateFailure<T>(
        string code, string message,
        List<ValidationFailure> failures)
    {
        return new Result<T>(default(T))
        {
            Success = false,
            Failures =
            [
                new()
                {
                    Code = code,
                    Message = message,
                    Failures = failures.Select(failure => new ResultFail()
                    {
                        Code = failure.ErrorCode,
                        Message = failure.ErrorMessage
                    }).ToList()
                }
            ],
        };
    }
}