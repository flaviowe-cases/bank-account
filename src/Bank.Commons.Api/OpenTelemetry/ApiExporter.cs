using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Bank.Commons.Api.OpenTelemetry;

public class ApiExporter: BaseExporter<LogRecord> 
{
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        foreach (var logRecord in batch)
        {
            var logLine = $"{logRecord.Timestamp} - " +
                          $"{logRecord.LogLevel} - " +
                          $"{logRecord.Body}";

            if (logRecord.Attributes?.Count > 0)
            {
                foreach (var attribute in logRecord.Attributes)
                    logLine = logLine.Replace("{" + attribute.Key + "}", TrimValue(attribute.Value));  
            }
            
            Console.WriteLine(logLine);
        }
        
        return ExportResult.Success;
    }

    private static string TrimValue(object? value)
    {
        var content = value?.ToString() ?? string.Empty;
        
        if (content.Length > 50)
            content = string.Concat(content
                .AsSpan(0, 50), "..."); 
        
        return content; 
    }
}