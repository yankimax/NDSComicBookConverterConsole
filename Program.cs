using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO.Compression;

/**
 * Манга хранится в виде:
 * /source/Название манги/1-1.zip
 * /source/Название манги/1-2.cbz
 * /source/Название манги/1-3.rar
 * /source/Название манги/2-4.cbr
 * /source/Название манги/2-5.zip
 **/
string sourceDirectory = @"./source"; // Откуда
string outputDirectory = @"./unpackedImages"; // Куда

if (!Directory.Exists(sourceDirectory))
{
    Console.WriteLine($"Каталога {sourceDirectory} не существует.");
    return;
}
FindAndExtract(sourceDirectory, outputDirectory);
Console.WriteLine($"Готово.\nРезультат доступен в каталоге: {Path.GetFullPath(outputDirectory)}");

/**
 * 
 * Занимаемся поиском и распаковкой манги
 * 
 **/
static void FindAndExtract(string sourceDirectory, string outputDirectory)
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

/**
 * 
 * Метод распаковки архивов
 * 
 **/
static void ExtractArchive(string archiveFile, string outputPath)
{
    string extension = Path.GetExtension(archiveFile).ToLower();
    if(extension is (".zip" or ".cbz"))
    {
        ZipFile.ExtractToDirectory(archiveFile, outputPath);
    }else if(extension is (".rar" or ".cbr"))
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