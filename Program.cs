using System;

namespace FolderBackuper
{
    class Program
    {
        static void Main(string[] args)
        {
            var backuper = new FolderBackuper("config.txt");
            foreach (var arg in args)
            {
                try
                {
                    Console.WriteLine($"Backuping {arg}...");
                    backuper.BackupRule(arg);
                    Console.WriteLine("Success");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
    }
}