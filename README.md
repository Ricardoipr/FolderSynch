# Folder Synchronizer

A C#application that synchronizes two folders, a **replica folder** with a **source folder**.  
The replica will always be an identical copy of the source, meeting the following criteria:

- Copies new or updated files  
- Deletes files and directories not present in the source  
- Logs all operations to console and a log file  
- Runs periodically at an interval provided by the user

---

##  Features
- One way synchronization (**source to replica**)  
- File change detection using **size + MD5 hash**
- Logging of all operations with timestamp on a log file, creating it if it's missing
- Periodic execution with configurable interval

---

## Requirements
- [.NET 6 SDK or later](https://dotnet.microsoft.com/download)  
- Windows, Linux or macOS  

---

##  Usage

### 1. Build the project
```bash
dotnet build
```

### 2. Run the program
```bash
dotnet run -- <source_folder> <replica_folder> <log_file> <interval_seconds>
```

### 3. Usage example:
```bash
dotnet run -- "C:\Data\Source" "D:\Backups\Replica" "C:\Logs\sync.log" 60
```

---

##  Important notes
- The source folder is not included in the repository, you must use a local test folder to use the program
- To stop the program, press Ctrl + C
