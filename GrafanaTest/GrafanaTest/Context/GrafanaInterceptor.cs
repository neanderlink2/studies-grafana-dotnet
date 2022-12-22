using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenTelemetry.Trace;
using System.Data.Common;

namespace GrafanaTest.Context
{
    public class GrafanaInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            LogQueryTime(eventData);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            LogQueryTime(eventData);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        private static void LogQueryTime(CommandEventData eventData)
        {
            var time = Math.Abs(eventData.StartTime.Subtract(DateTime.UtcNow).TotalMilliseconds);
            var list = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("elapsed-ms", time),
                new KeyValuePair<string, object>("type", eventData.Command.CommandType.ToString()),
                new KeyValuePair<string, object>("query", eventData.Command.CommandText)
            };
            Tracer.CurrentSpan.AddEvent("database.query", eventData.StartTime, new SpanAttributes(list));
        }
    }
}
