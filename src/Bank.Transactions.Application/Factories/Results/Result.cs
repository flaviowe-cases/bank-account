namespace Bank.Transactions.Application.Factories.Results;

public abstract class Result
{
    public required bool Success { get; init; }
    public IEnumerable<ResultFail>? Failures { get; set; }
    
    public bool ContainsFailure (string code)
        => Failures?.Any(failure => failure.Code == code) ?? false; 
        
}

public class Result<T>(
    T? content) : Result
{
    private readonly T? _content = content;

    public T GetContent() 
        => _content ?? throw new NullReferenceException("Content is null");
}