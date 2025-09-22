namespace FolderSynchronizer
{
    /// <summary>
    /// Provides functionality to log messages and file operations
    /// to both console output and a log file.
    /// </summary>
    public class FileLogger
    {
        private readonly string _logFilePath;

        /// <summary>
        /// Initializes a new instance of the FileLogger class.
        /// Ensures that the directory for the log file exists.
        /// If it doesn't exist, it'll create the log file.
        /// </summary>
        /// <param name="logFilePath">The full path to the log file.</param>
        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;

            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Dispose();
            }
        }
        /// <summary>
        /// Logs a message with a timestamp to both console and the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}";

            Console.WriteLine(logEntry);

            try
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a specific file operation (e.g. copied, deleted) with its target file path.
        /// </summary>
        /// <param name="operation">The type of file operation (e.g. "COPIED", "DELETED").</param>
        /// <param name="filePath">The path of the file involved in the operation.</param>
        public void LogFileOperation(string operation, string filePath)
        {
            Log($"{operation}: {filePath}");
        }
    }
}
