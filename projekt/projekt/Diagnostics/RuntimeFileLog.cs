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
    }
}