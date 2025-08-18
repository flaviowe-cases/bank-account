namespace Bank.Commons.Applications.Factories.Results;

public class ResultFail
{
    public required string Code { get; init; } 
    public required string Message { get; set; } 
    
    public List<ResultFail>? Failures { get; set; }
}