using Ax.Arguments;
using System.IO.Compression;

namespace Ax.Operations;
internal class Unpack
{
    public static void Execute(UnpackArguments options)
    {
        if (OperatingSystem.IsWindows())
            Console.SetBufferSize(Console.BufferWidth, Console.BufferHeight + 5);

        Console.CursorVisible = false;
        // Ensure the output directory exists
        if (!Directory.Exists(options.OutputFolder))
        {
            Directory.CreateDirectory(options.OutputFolder);
        }

        using var archive = ZipFile.OpenRead(options.InputFile);

        // Find the .index entry in the archive
        var indexEntry = archive.GetEntry(".index");
        if (indexEntry == null)
        {
            throw new InvalidOperationException("The archive does not contain a valid .index file.");
        }

        // Read and deserialize the index file
        Dictionary<ulong, List<string>>? fileHashes;
        using (var indexStream = indexEntry.Open())
        using (var reader = new StreamReader(indexStream))
        {
            var json = reader.ReadToEnd();
            fileHashes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<ulong, List<string>>>(json);
        }

        if (fileHashes == null)
        {
            throw new InvalidOperationException("Failed to deserialize the .index file.");
        }

        var position = Console.GetCursorPosition();
        var totalFiles = fileHashes.Values.Sum(x => x.Count);
        var currentFile = 0;

        // Extract files based on the index
        foreach (var entry in fileHashes)
        {
            var hash = entry.Key;
            var files = entry.Value;

            Console.SetCursorPosition(position.Left, position.Top);
            Console.Write($"Unpacking {Path.GetFileName(entry.Value[0])} from archive\u001b[K");

            // Find the corresponding archive entry for the hash
            var archiveEntry = archive.GetEntry($"{hash:X16}");
            if (archiveEntry == null)
            {
                throw new InvalidOperationException($"The archive is missing an entry for hash {hash:X16}.");
            }

            // Extract the file for each relative path
            foreach (var relativePath in files)
            {
                currentFile++;

                Console.SetCursorPosition(position.Left, position.Top + 1);
                Console.Write($"Destination path: \x1B[38;5;9m{Path.GetDirectoryName(relativePath)}\x1B[38;5;7m\x1B[K");
                Console.SetCursorPosition(position.Left, position.Top + 2);
                Console.Write($"Progress: {currentFile}/{totalFiles} ");

                var destinationPath = Path.Combine(options.OutputFolder, relativePath);
                var destinationDir = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                using var archiveStream = archiveEntry.Open();
                using var fileStream = File.Create(destinationPath);
                archiveStream.CopyTo(fileStream);
            }
        }

        Console.CursorVisible = true;
    }
}