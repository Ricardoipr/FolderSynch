using System.Security.Cryptography;

namespace FolderSynchronizer
{
    /// <summary>
    /// Provides functionality to synchronize a source folder into a replica folder.
    /// Ensures that the replica is always an identical copy of the source:
    /// - Copies new or updated files
    /// - Creates missing directories
    /// - Deletes files or directories not present in the source
    /// </summary>
    public class FolderSynchronizer
    {
        private readonly FileLogger _logger;

        /// <summary>
        /// Initializes a new instance of the FolderSynchronizer class.
        /// </summary>
        /// <param name="logger">The logger instance used to record synchronization operations.</param>
        public FolderSynchronizer(FileLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates and prepares the source and replica directories for synchronization.
        /// Creates the replica directory if it does not exist.
        /// </summary>
        /// <param name="sourcePath">The full path of the source directory.</param>
        /// <param name="replicaPath">The full path of the replica directory.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the source directory does not exist.</exception>
        public void Initialize(string sourcePath, string replicaPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourcePath}");
            }

            if (!Directory.Exists(replicaPath))
            {
                Directory.CreateDirectory(replicaPath);
                _logger.LogFileOperation("CREATED DIRECTORY\n", replicaPath);
            }

            _logger.Log("Initialization completed successfully");
        }

        /// <summary>
        /// Performs a synchronization cycle between the source and replica directories.
        /// Copies new or updated files and deletes obsolete files or directories from the replica.
        /// </summary>
        /// <param name="sourcePath">The full path of the source directory.</param>
        /// <param name="replicaPath">The full path of the replica directory.</param>
        public void Synchronize(string sourcePath, string replicaPath)
        {
            SyncDirectory(sourcePath, replicaPath);

            CleanupReplica(sourcePath, replicaPath);
        }

        /// <summary>
        /// Recursively synchronizes the contents of a source directory into a replica directory.
        /// </summary>
        /// <param name="sourceDir">The current source directory to synchronize.</param>
        /// <param name="replicaDir">The corresponding replica directory.</param>
        private void SyncDirectory(string sourceDir, string replicaDir)
        {
            if (!Directory.Exists(replicaDir))
            {
                Directory.CreateDirectory(replicaDir);
                _logger.LogFileOperation("CREATED DIRECTORY\n", replicaDir);
            }

            foreach (var sourceFile in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(sourceFile);
                var replicaFile = Path.Combine(replicaDir, fileName);

                var (shouldCopy, reason) = ShouldCopyFile(sourceFile, replicaFile);
                if (shouldCopy)
                {
                    File.Copy(sourceFile, replicaFile, overwrite: true);
                    _logger.LogFileOperation($"COPIED ({reason})\n", replicaFile);
                }

            }

            foreach (var sourceSubDir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(sourceSubDir);
                var replicaSubDir = Path.Combine(replicaDir, dirName);
                SyncDirectory(sourceSubDir, replicaSubDir);
            }
        }

        /// <summary>
        /// Removes files and directories from the replica that no longer exist in the source.
        /// </summary>
        /// <param name="sourceDir">The current source directory.</param>
        /// <param name="replicaDir">The corresponding replica directory.</param>
        private void CleanupReplica(string sourceDir, string replicaDir)
        {
            if (!Directory.Exists(replicaDir))
                return;

            foreach (var replicaFile in Directory.GetFiles(replicaDir))
            {
                var fileName = Path.GetFileName(replicaFile);
                var sourceFile = Path.Combine(sourceDir, fileName);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    _logger.LogFileOperation("DELETED\n", replicaFile);
                }
            }

            foreach (var replicaSubDir in Directory.GetDirectories(replicaDir))
            {
                var dirName = Path.GetFileName(replicaSubDir);
                var sourceSubDir = Path.Combine(sourceDir, dirName);

                if (!Directory.Exists(sourceSubDir))
                {
                    Directory.Delete(replicaSubDir, recursive: true);
                    _logger.LogFileOperation("DELETED DIRECTORY\n", replicaSubDir);
                }
                else
                {
                    CleanupReplica(sourceSubDir, replicaSubDir);
                }
            }
        }

        /// <summary>
        /// Determines whether a file should be copied from the source to the replica
        /// by comparing its size and hash.
        /// </summary>
        /// <param name="sourceFile">The full path of the source file.</param>
        /// <param name="replicaFile">The full path of the replica file.</param>
        /// <returns>
        /// A tuple containing:
        /// - true if the file should be copied; otherwise false.
        /// - A string describing the reason.
        /// </returns>
        private (bool ShouldCopy, string Reason) ShouldCopyFile(string sourceFile, string replicaFile)
        {
            if (!File.Exists(replicaFile))
                return (true, "NEW FILE");

            var sourceInfo = new FileInfo(sourceFile);
            var replicaInfo = new FileInfo(replicaFile);

            if (sourceInfo.Length != replicaInfo.Length)
                return (true, "SIZE DIFFERENCE");

            var sourceHash = ComputeMD5(sourceFile);
            var replicaHash = ComputeMD5(replicaFile);

            if (!sourceHash.SequenceEqual(replicaHash))
                return (true, "HASH DIFFERENCE");

            return (false, "IDENTICAL");
        }

        /// <summary>
        /// Computes the MD5 hash of the contents of a file.
        /// </summary>
        /// <param name="filePath">The path of the file to hash.</param>
        /// <returns>A byte array containing the hash.</returns>
        private byte[] ComputeMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                return md5.ComputeHash(stream);
            }
        }

    }
}
