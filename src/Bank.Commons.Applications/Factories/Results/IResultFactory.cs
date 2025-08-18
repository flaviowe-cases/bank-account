using FluentValidation.Results;

namespace Bank.Commons.Applications.Factories.Results;

public interface IResultFactory
{
    Result<T> CreateSuccess<T>(T payload);
    Result<T> CreateFailure<T>(string code, string message);
    Result<T> CreateFailure<T>(Result result);
    Result<T> CreateFailure<T>(List<ResultFail> failures);
    Result<T> CreateFailure<T>(string code, string message, List<ValidationFailure> failures);
    
}