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
        /// If the given path is a folder or missing a filename, 
        /// a default "sync.log" file is created inside it.
        /// </summary>
        /// <param name="logFilePath">The path to the log file or folder.</param>
        public FileLogger(string logFilePath)
        {
            if (Directory.Exists(logFilePath) || string.IsNullOrWhiteSpace(Path.GetFileName(logFilePath)))
            {
                logFilePath = Path.Combine(logFilePath, "sync.log");
            }

            var fullPath = Path.GetFullPath(logFilePath);
            var dir = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _logFilePath = fullPath;

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
