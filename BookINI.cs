namespace NDSComicBookConverterConsole
{
    internal class BookINI
    {
        // Первая строка описания книги. По умолчанию, название архива без расширения (номер главы)
        public static string Credits1 { get; set; } = "ArchiveFileNameWithOutExtension";
        // Вторая строка описания книги. По умолчанию, название каталога в котором лежал архив (название манги)
        public static string Credits2 { get; set; } = "ParrentDirectory";
        // Третья строка описания книги. По умолчанию, аналогично Credits2
        public static string Credits3 { get; set; } = "ParrentDirectory";
        // Режим чтения. Справа на лево или слева на право. 
        public static int LeftToRight { get; set; } = 0;
        // Количество страниц в этой книге
        public static int PagesCount { get; set; } = 0;
        // Версия книги
        public static int Version { get; set; } = 100;

        public static int ThSize { get; set; } = 0;
        public static int OSize { get; set; } = 0;
        public static int ISize { get; set; } = 860000;

        public static int ThQuality { get; set; } = 90;
        public static int OQuality { get; set; } = 90;
        public static int IQuality { get; set; } = 90;

        public static int ThHeight { get; set; } = 46;
        public static int OHeight { get; set; } = 192;
        public static int IHeight { get; set; } = 1400;

        public static int ThWidth { get; set; } = 100;
        public static int OWidth { get; set; } = 256;
        public static int IWidth { get; set; } = 700;

        public static void CreateBookINIFile()
        {

            var keyValues = new Dictionary<string, string>
            {
                {"CbCredits1", Credits1},
                {"CbCredits2", Credits2},
                {"CbCredits3", Credits3},
                {"LeftToRight", LeftToRight.ToString()},
                {"NbPages", PagesCount.ToString()},
                {"Version", Version.ToString()},
                {"iHeight", IHeight.ToString()},
                {"iQuality", IQuality.ToString()},
                {"iSize", ISize.ToString()},
                {"iWidth", IWidth.ToString()},
                {"oHeight", OHeight.ToString()},
                {"oQuality", OQuality.ToString()},
                {"oSize", OSize.ToString()},
                {"oWidth", OWidth.ToString()},
                {"thHeight", ThHeight.ToString()},
                {"thQuality", ThQuality.ToString()},
                {"thSize",  ThSize.ToString()},
                {"thWidth", ThWidth.ToString()}
            };

            // Записываем данные в INI-файл
            WriteConfigData(Path.Combine(Config.OutputDirectory, "_temp", "ComicBookDS_book.ini"), keyValues);
            Console.WriteLine($"Создан файл настроек книги: {Path.Combine(Config.OutputDirectory, "_temp", "ComicBookDS_book.ini")}");
        }
        static void WriteConfigData(string fileName, Dictionary<string, string> data)
        {
            File.WriteAllLines(fileName, data.Select(z => $"{z.Key}={z.Value}"));
        }
    }
}
