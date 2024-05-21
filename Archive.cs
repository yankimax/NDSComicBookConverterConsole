using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO.Compression;

namespace NDSComicBookConverterConsole
{
    internal class Archive
    {
        // Поиск и обработка архивов
        public static void FindAndExtract(string sourceDirectory, string outputDirectory)
        {
            // Обходим корневой каталог.
            foreach (var mangaDir in Directory.GetDirectories(sourceDirectory))
            {
                // В корневом каталоге расположены каталоги по названию манги.
                string mangaName = Path.GetFileName(mangaDir);

                // Обходим каждый каталог с мангой, ищем файлы.
                foreach (var archiveFile in Directory.GetFiles(mangaDir, "*.*", SearchOption.TopDirectoryOnly))
                {
                    string extension = Path.GetExtension(archiveFile).ToLower();
                    // Если это не архив, который мы можем распаковать - игнорируем.
                    if (extension is not (".zip" or ".cbz" or ".rar" or ".cbr")) continue;

                    string archiveName = Path.GetFileNameWithoutExtension(archiveFile);

                    // Теперь нам нужно проверить, не распаковали ли мы его ранее.
                    string outputPath = Path.Combine(outputDirectory, mangaName, archiveName);
                    if (Directory.Exists(outputPath))
                    {
                        Console.WriteLine($"{mangaName}/{archiveName} уже распакован.");
                        continue;
                    }
                    Console.WriteLine($"Распаковка {mangaName}/{archiveName}.");
                    Directory.CreateDirectory(outputPath);
                    ExtractArchive(archiveFile, outputPath);
                }
            }
        }

        // Распаковка архивов
        static void ExtractArchive(string archiveFile, string outputPath)
        {
            string extension = Path.GetExtension(archiveFile).ToLower();
            if (extension is (".zip" or ".cbz"))
                ZipFile.ExtractToDirectory(archiveFile, outputPath);
            else if (extension is (".rar" or ".cbr"))
            {
                using (var archive = ArchiveFactory.Open(archiveFile))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                            entry.WriteToDirectory(outputPath, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
        }
        public static void CreateArchive(string inputPath, string output, bool force = false)
        {
            if(File.Exists(output) && force) {
                Console.WriteLine($"Файл {output} уже существует. Удаляю.");
                File.Delete(output);
            }
            else if (File.Exists(output) && !force)
            {
                Console.WriteLine($"Файл {output} уже существует. Не трогаю.");
                return;
            }
            ZipFile.CreateFromDirectory(inputPath, output);
        }

    }
}
