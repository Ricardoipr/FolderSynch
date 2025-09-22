namespace FolderSynchronizer
{
    /// <summary>
    /// Represents the configuration options required for folder synchronization, where:
    /// - SourcePath -> Path of the source folder to be synchronized
    /// - ReplicaPath -> Path of the replica folder to match the content of the source folder
    /// - LogFilePath -> Path of the log file where the operations will be recorded to
    /// - IntervalSeconds ->  Synchronization interval in seconds
    /// </summary>
    public class SyncOptions
    {
        public required string SourcePath { get; set; }
        public required string ReplicaPath { get; set; }
        public required string LogFilePath { get; set; }
        public int IntervalSeconds { get; set; }
    }
}
