using System.IO.Compression;
using System.IO.Hashing;

namespace Ax.Operations;

internal class Pack
{
    public static void Execute(Arguments.PackArguments options)
    {
        var basePath = options.SourceFolder;
        var outputFile = string.IsNullOrEmpty(options.OutputFile) ? GetDefaultOutputFileName() : options.OutputFile;

        if (string.IsNullOrEmpty(options.SourceFolder))
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Marel\\Axin\\Modules");

            if (!Path.Exists(basePath))
            {
                Console.WriteLine($"'{basePath}' does not exist. Try specifying the target folder directly.");
                return;
            }
        }

        if (!Path.Exists(basePath))
        {
            Console.WriteLine($"'{basePath}' does not exist.");
            return;
        }

        // Check if the basePath is absolute or relative  
        if (!Path.IsPathRooted(basePath))
        {
            basePath = Path.GetFullPath(basePath);
        }

        var fileHashes = new Dictionary<ulong, List<string>>();
        var filecount = 0;
        var duplicateFilecount = 0;
        long duplicateSize = 0;
        long totalSize = 0;

        Console.WriteLine("Step 1/2 - Find duplicate files");

        var position = Console.GetCursorPosition();
        Console.CursorVisible = false;
        foreach (var file in Directory.EnumerateFiles(basePath, "*", SearchOption.AllDirectories))
        {
            filecount++;
            var fileInfo = new FileInfo(file);
            totalSize += fileInfo.Length;
            using var fileStream = File.OpenRead(file);
            var xxHash64 = new XxHash64();
            xxHash64.Append(fileStream);
            var hashBytes = xxHash64.GetCurrentHash();
            ulong hash = BitConverter.ToUInt64(hashBytes, 0);

            if (!fileHashes.ContainsKey(hash))
            {
                fileHashes[hash] = new List<string>();
            }
            else
            {
                duplicateFilecount++;
                duplicateSize += fileInfo.Length;
            }

            var relativePath = Path.GetRelativePath(basePath, file);
            fileHashes[hash].Add(relativePath);

            Console.SetCursorPosition(position.Left, position.Top);
            Console.Write($"Processed {filecount} files - \x1B[38;5;9m{duplicateFilecount}\x1B[38;5;7m duplicate files found.");
        }

        Console.WriteLine("\r\n\r\nStep 2/2 - Compress unique files and build index");

        var zipfileName = Path.GetFullPath(outputFile);
        using (var archive = ZipFile.Open(zipfileName, ZipArchiveMode.Create))
        {
            position = Console.GetCursorPosition();
            foreach (var entry in fileHashes)
            {
                var hash = entry.Key;
                var files = entry.Value;

                Console.SetCursorPosition(position.Left, position.Top);
                Console.Write($"Adding {entry.Value[0]} to archive\u001b[K");

                var zipEntry = archive.CreateEntry($"{hash:X16}", CompressionLevel.Optimal);
                using (var fileStream = File.OpenRead(Path.Combine(basePath, files[0])))
                {
                    using (var zipStream = zipEntry.Open())
                    {
                        fileStream.CopyTo(zipStream);
                    }
                }
            }

            var json = System.Text.Json.JsonSerializer.Serialize(fileHashes);

            var jsonEntry = archive.CreateEntry(".index", CompressionLevel.Optimal);
            using (var zipStream = jsonEntry.Open())
            {
                using (var writer = new StreamWriter(zipStream))
                {
                    writer.Write(json);
                }
            }
        }

        Console.WriteLine($"\r\nArchived {filecount} files ({totalSize / 1024.0 / 1024.0:F2} Mb) into {duplicateFilecount} unique files ({new FileInfo(outputFile).Length / 1024.0 / 1024.0:F2} Mb)");
        Console.CursorVisible = true;
    }

    static string GetDefaultOutputFileName()
    {
        return $"axin-{DateTime.Now:dd-MM-yyyy-HH-mm}.axz";
    }
}