using System;
using System.Collections.Generic;
using System.IO;

namespace FolderBackuper
{
    public class FolderBackuper
    {
        public FolderBackuper(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public void BackupRule(string ruleName)
        {
            if (_masterSlavesMap == null)
            {
                ReadConfig(_configFilePath);
            }
            if (!_masterSlavesMap.ContainsKey(ruleName))
            {
                throw new ArgumentException("Rule not found");
            }
            (string master, string slave) = _masterSlavesMap[ruleName];
            string destPath = $"{slave}.{DateTime.Now.ToString(_dateFormat)}";
            CopyFolder(master, destPath);
        }

        public static void CopyFolder(string sourceDirPath, string destPath)
        {
            var dir = new DirectoryInfo(sourceDirPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirPath);
            }

            CopyFolder(dir, destPath);
        }

        private static void CopyFolder(DirectoryInfo sourceDirectory, string destPath)
        {
            DirectoryInfo[] subDirectoriesirs = sourceDirectory.GetDirectories();
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (var file in files)
            {
                string fileDestPath = Path.Combine(destPath, file.Name);
                file.CopyTo(fileDestPath, false);
            }

            foreach (var subdir in subDirectoriesirs)
            {
                string subdirDestPath = Path.Combine(destPath, subdir.Name);
                CopyFolder(subdir, subdirDestPath);
            }
        }

        private void ReadConfig(string configPath)
        {
            _masterSlavesMap = new Dictionary<string, (string, string)>();
            foreach (var line in File.ReadAllLines(configPath))
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }
                (string ruleName, string masterFolder, string slaveFolder) = ExtractRule(line);
                _masterSlavesMap[ruleName] = (masterFolder, slaveFolder);
            }
        }

        private static (string ruleName, string masterFolder, string slaveFolder) ExtractRule(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            string[] tokens = line.Split(":");
            if (tokens.Length != 2)
            {
                throw new FormatException("Wrong format of a config file");
            }
            var ruleName = tokens[0].Trim();
            var rest = tokens[1];
            
            tokens = rest.Split("=>");
            if (tokens.Length != 2)
            {
                throw new FormatException("Wrong format of a config file");
            }
            var masterFolder = tokens[0].Trim();
            var slaveFolder = tokens[1].Trim();

            return (ruleName, masterFolder, slaveFolder);
        }

        private string _configFilePath;
        private Dictionary<string, (string, string)> _masterSlavesMap;
        private string _dateFormat = "yyyy_dd_MM_HH_mm_ss";
    }
}