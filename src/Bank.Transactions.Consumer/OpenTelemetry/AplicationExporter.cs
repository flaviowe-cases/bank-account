using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Bank.Transactions.Consumer.OpenTelemetry;


public class AplicationExporter: BaseExporter<LogRecord> 
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
                    logLine = logLine.Replace("{" + attribute.Key + "}", $"{attribute.Value}");  
            }
            
            Console.WriteLine(logLine);
        }
        
        return ExportResult.Success;
    }
}