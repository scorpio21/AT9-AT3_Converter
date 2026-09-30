using System;
using System.IO;

namespace at3_at9_Converter.Services
{
    public class FileManager
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to delete file '{path}': {ex.Message}", ex);
                }
            }
        }

        public void MoveFile(string sourcePath, string destinationPath)
        {
            if (File.Exists(sourcePath))
            {
                try
                {
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                    File.Move(sourcePath, destinationPath);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to move file from '{sourcePath}' to '{destinationPath}': {ex.Message}", ex);
                }
            }
        }

        public void RenameFile(string oldPath, string newPath)
        {
            if (File.Exists(oldPath))
            {
                try
                {
                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                    }
                    File.Move(oldPath, newPath);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to rename file from '{oldPath}' to '{newPath}': {ex.Message}", ex);
                }
            }
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public string NormalizePath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string GetTempFilePath(string baseName, string extension)
        {
            string tempDir = Path.GetTempPath();
            string tempFileName = Guid.NewGuid().ToString() + "_" + baseName + extension;
            return Path.Combine(tempDir, tempFileName);
        }

        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string[] GetFilesInDirectory(string path, string searchPattern = "*")
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path, searchPattern);
            }
            return new string[0];
        }
    }
}
