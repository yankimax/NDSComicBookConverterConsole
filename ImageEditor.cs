using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.Arm;

namespace NDSComicBookConverterConsole
{
    using Dimensions = (int Width, int Height, int Rotation);
    internal class ImageEditor
    {
        private readonly string[] allowedImageTypes = { "jpg", "jpeg", "png", "bmp" };
        private string outputDirectory { get; set; }
        private int index = 0;
        private Dimensions SNDimensions = (137, 192, 0); // Маленький размер норм
        private Dimensions SRDimensions = (256, 182, 90); // Маленький размер справа налево
        private Dimensions TNDimensions = (32, 46, 0);  // Превью размер норм
        private Dimensions TRDimensions = (62, 44, 90); // Превью размер справа налево

        public ImageEditor(string tempDirectoryWithManga){
            outputDirectory = Path.Combine(Config.OutputDirectory, "_temp");
            // Заходим в каждый каталог с мангой
            foreach(var mangaDir in Directory.GetDirectories(tempDirectoryWithManga))
            {
                // Заходим в каждый каталог каждой главы
                foreach (var chapterDir in Directory.GetDirectories(mangaDir))
                {

                    // TODO: Перед началом обработки каждой главы нам необходимо перезаписывать информацию в BookINI
                    index = 1;
                    BookINI.IWidth = 0;
                    BookINI.IHeight = 0;
                    BookINI.Credits1 = Path.GetFileName(chapterDir) ?? "Unknown";
                    BookINI.Credits2 = Path.GetFileName(mangaDir) ?? "Unknown";
                    BookINI.Credits3 = Path.GetFileName(mangaDir) ?? "Unknown";
                    string pathToMangaDir = Path.Combine(Config.OutputDirectory, Path.GetFileName(mangaDir) ?? "unknown");
                    if(File.Exists(Path.Combine(pathToMangaDir, (Path.GetFileName(chapterDir) ?? "unknown") + ".cbds")))
                    {
                        Console.WriteLine($"{Path.Combine(pathToMangaDir, (Path.GetFileName(chapterDir) ?? "unknown") + ".cbds")} уже существует. Пропускаю.");
                        continue;
                    }
                    Program.CreateCBDSStructure(Config.OutputDirectory);

                    var files = Directory.GetFiles(chapterDir);
                    if (files.Length == 0) continue;
                    //var fileList = files.OrderBy(f => int.TryParse(Path.GetFileNameWithoutExtension(f), out int n) ? n : int.MaxValue).ToList();
                    var fileList = files.OrderBy(f => Path.GetFileNameWithoutExtension(f), new CustomFileComparer()).ToList();

                    // Теперь перебираем каждый файл в каталоге
                    foreach (var file in fileList)
                    {
                        Console.WriteLine($"Обрабатываю {file}");
                        var imageType = Path.GetExtension(file).ToLower().TrimStart('.');
                        // Если файл не является картинкой, пропускаем
                        if (!allowedImageTypes.Contains(imageType))
                        {
                            Console.WriteLine($"Игнорируем файл: {file}");
                            continue;
                        }
                        EditAndMoveImage(file, "IMAGE", Config.ImageQuality); // Сперва сохраняем оригинал.
                        using (var image = SixLabors.ImageSharp.Image.Load(file))
                        {
                            // Ширину и высоту оригинального изображения передаем в BookINI
                            BookINI.IWidth = image.Width;
                            BookINI.IHeight = image.Height;

                            //todo: изменять размеры
                            EditAndMoveImage(file, "SMALL_N", Config.ImageQuality, SNDimensions);
                            EditAndMoveImage(file, "SMALL_R", Config.ImageQuality, SRDimensions);
                            EditAndMoveImage(file, "THMB_N", Config.ImageQuality, TNDimensions);
                            EditAndMoveImage(file, "THMB_R", Config.ImageQuality, TRDimensions);
                        }
                        index++;
                    }
                    BookINI.PagesCount = index - 1;
                    BookINI.CreateBookINIFile();
                    BookINI.CreateNameFiles(Path.Combine(outputDirectory, "NAME"));
                    Directory.CreateDirectory(pathToMangaDir);
                    Archive.CreateArchive(outputDirectory, Path.Combine(pathToMangaDir, (Path.GetFileName(chapterDir) ?? "unknown") + ".cbds"));
                    Program.RemoveCBDSStructure(Config.OutputDirectory);
                    
                }
            }
        }

        public void EditAndMoveImage(string file, string subDirectory, int quality)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(file))
            {
                var encoder = new JpegEncoder{ Quality = quality };
                image.Metadata.HorizontalResolution = 96;
                image.Metadata.VerticalResolution = 96;
                image.Save(Path.Combine(outputDirectory, subDirectory, index.ToString() + Path.GetExtension(file)), encoder);
                Console.WriteLine($"Сохраняю файл: {Path.Combine(outputDirectory, subDirectory, index.ToString() + Path.GetExtension(file))}");
            }
        }
        public void EditAndMoveImage(string file, string subDirectory, int quality, Dimensions dimensions)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(file))
            {
                image.Mutate(x => { x.Resize(dimensions.Width, dimensions.Height); x.Rotate(dimensions.Rotation);});
                var encoder = new JpegEncoder { Quality = quality };
                image.Metadata.HorizontalResolution = 96;
                image.Metadata.VerticalResolution = 96;
                image.Save(Path.Combine(outputDirectory, subDirectory, index.ToString() + Path.GetExtension(file)), encoder);
                Console.WriteLine($"Сохраняю файл: {Path.Combine(outputDirectory, subDirectory, index.ToString() + Path.GetExtension(file))}");
            }
        }

        private class CustomFileComparer : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                string xName = Path.GetFileNameWithoutExtension(x) ?? "";
                string yName = Path.GetFileNameWithoutExtension(y) ?? "";

                bool xIsNumber = int.TryParse(xName, out int xNumber);
                bool yIsNumber = int.TryParse(yName, out int yNumber);

                if (xIsNumber && yIsNumber)
                    return xNumber.CompareTo(yNumber);
                else if (xIsNumber)
                    return -1; // x идет перед y
                else if (yIsNumber)
                    return 1; // y идет перед x
                return string.Compare(xName, yName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
