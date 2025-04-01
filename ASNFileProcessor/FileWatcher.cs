using System;
using System.IO;

namespace ASNFileProcessor
{
    //Class that will monitor folder for the new files
    public class FileWatcher
    {
        private readonly string _folderPath = @"C:\Users\dzint\Downloads\VR_Challenge_Senior_backend_developer";
        private readonly ASNFileProcessor _fileProcessor;

        public FileWatcher(ASNFileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        public  void StartWatching()
        {
            //Creates object that will monitor the folder
            var watcher = new FileSystemWatcher(_folderPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite, //Checks for file name and last edit time
                Filter = "*.txt" // Filters only by .txt files
            };

            watcher.Created += OnNewFileCreated;
            watcher.EnableRaisingEvents = true; //Starts monitoring
        }


        //Method that will be called when new file is created
        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000);

            Console.WriteLine($"New file is in: {e.FullPath}");

            _fileProcessor.ProcessFile(e.FullPath);
        }
    }
}
