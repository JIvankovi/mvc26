namespace projekt.Diagnostics
{
    public static class RuntimeFileLog
    {
        private static readonly Lock SyncLock = new();

        public static void Write(string contentRootPath, string message)
        {
            var logsDirectory = Path.Combine(contentRootPath, "logs");
            Directory.CreateDirectory(logsDirectory);

            var logPath = Path.Combine(logsDirectory, "runtime-errors.log");
            var line = $"[{DateTime.UtcNow:u}] {message}{Environment.NewLine}";

            lock (SyncLock)
            {
                File.AppendAllText(logPath, line);
            }
        }

        public static void WriteException(string contentRootPath, string message, Exception exception)
        {
            Write(contentRootPath, $"{message}{Environment.NewLine}{exception}");
        }

        public static void WriteApiTransaction(
            string contentRootPath,
            DateTime timestampUtc,
            string method,
            string path,
            string queryString,
            int statusCode,
            long durationMs,
            string user)
        {
            var logsDirectory = Path.Combine(contentRootPath, "logs");
            Directory.CreateDirectory(logsDirectory);

            var logPath = Path.Combine(logsDirectory, "api-transactions.log");
            var line =
                $"[{timestampUtc:u}] {method} {path}{queryString} | status={statusCode} | durationMs={durationMs} | user={user}{Environment.NewLine}";

            lock (SyncLock)
            {
                File.AppendAllText(logPath, line);
            }
        }
    }
}