namespace FolderSynchronizer
{
    /// <summary>
    /// Entry point of the folder synchronization application.
    /// Handles initialization, logging, and the periodic execution loop.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry method of the program.
        /// Expects four command-line arguments:
        /// - Source folder path
        /// - Replica folder path
        /// - Log file path
        /// - Synchronization interval in second
        /// </summary>
        /// <param>
        /// Command-line arguments:
        /// args[0]= Source folder path,  
        /// args[1] = Replica folder path,  
        /// args[2] = Log file path,  
        /// args[3] = Interval in seconds.
        /// </param>
        /// <remarks>
        /// The program runs indefinitely until manually stopped (Ctrl+C).  
        /// Each cycle synchronizes the replica folder with the source folder,  
        /// logs operations to both console and file, then waits for the given interval to start the next cycle.
        /// </remarks>
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: program.exe <source_folder> <replica_folder> <log_file> <interval_seconds>");
                return;
            }

            var options = new SyncOptions
            {
                SourcePath = args[0],
                ReplicaPath = args[1],
                LogFilePath = args[2],
                IntervalSeconds = int.Parse(args[3])
            };

            Console.WriteLine($"Source Path: {options.SourcePath}");
            Console.WriteLine($"Replica Path: {options.ReplicaPath}");
            Console.WriteLine($"Log File Path: {options.LogFilePath}");
            Console.WriteLine($"Interval (seconds): {options.IntervalSeconds}");

            var logger = new FileLogger(options.LogFilePath);
            var synchronizer = new FolderSynchronizer(logger);
            try
            {
                synchronizer.Initialize(options.SourcePath, options.ReplicaPath);
                logger.Log("Folder synch started");
                Console.WriteLine("Press Ctrl+C to stop synchronization");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization failed: {ex.Message}");
                return;
            }

            while (true)
            {
                try
                {
                    logger.Log("Starting synchronization cycle");
                    synchronizer.Synchronize(options.SourcePath, options.ReplicaPath);
                    logger.Log("Synchronization cycle completed\n");

                    Console.WriteLine($"Sync complete. Next sync in {options.IntervalSeconds} seconds");
                    Thread.Sleep(options.IntervalSeconds * 1000);
                }
                catch (Exception ex)
                {
                    logger.Log($"Error during synchronization: {ex.Message}\n");
                    Console.WriteLine($"Error: {ex.Message}");
                    Thread.Sleep(5000);
                }
            }
        }
    }
}