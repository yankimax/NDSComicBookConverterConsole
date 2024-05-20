namespace NDSComicBookConverterConsole
{
    class Program
    {
        public static void Main(string[] args)
        {
            Archive.FindAndExtract(Config.InputDirectory, Config.UnpackedDirectory);
            Console.WriteLine($"Готово.\nРезультат доступен в каталоге: {Path.GetFullPath(Config.UnpackedDirectory)}");
            CreateCBDSStructure(Config.OutputDirectory);
            BookINI.CreateBookINIFile();
            //RemoveCBDSStructure(Config.OutputDirectory);
        }

        static void CreateCBDSStructure(string cbdsDirectory)
        {
            var directories = new[] { "IMAGE", "NAME", "SMALL_N", "SMALL_R", "THMB_N", "THMB_R" };
            foreach (var directory in directories)
            {
                Directory.CreateDirectory(Path.Combine(cbdsDirectory, "_temp", directory));
                Console.WriteLine($"Создал каталог: {Path.Combine(cbdsDirectory, "_temp", directory)}");
            }
        }
        static void RemoveCBDSStructure(string cbdsDirectory)
        {
            Directory.Delete(Path.Combine(cbdsDirectory, "_temp"), true);
            Console.WriteLine($"Удалил каталог: {Path.Combine(cbdsDirectory, "_temp")}");
        }
    }
}