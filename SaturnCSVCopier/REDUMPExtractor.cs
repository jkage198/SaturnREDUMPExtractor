using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using PowerArgs;
using SaturnCSVCopier;

namespace SaturnREDUMPExtractor
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    class REDUMPExtractor
    {
        public static Stopwatch stopwatch = new Stopwatch();

        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Generates an input-compatibe CSV file from the current working directory")]
        public void GenerateCSV(CSVBuilderArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(Directory.GetCurrentDirectory()));
            Console.WriteLine("Generating CSV with the following:\n{0}", gameEntryCollection.Statistics.ToString());
            parser.GameEntryListToCSV(gameEntryCollection.GameEntries, args.CsvFilename);
        }

        [ArgActionMethod, ArgDescription("Extracts games to target directory as defined in input CSV file")]
        public void Extract(ExtractArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.CSVToGameEntryList(args.SourceCSV));
            Console.WriteLine("Extracting the following:\n{0}", gameEntryCollection.Statistics.ToString());
            PromptQuitIfNotKey("\nProceed with extraction?\n", ConsoleKey.Y);
            ExtractGames(gameEntryCollection, args.OutputDirectory, args.Categorize);
        }

        [ArgActionMethod, ArgDescription("Extracts all games in working directory to target directory")]
        public void ExtractAll(ExtractAllArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(Directory.GetCurrentDirectory()));
            Console.WriteLine("Extracting the following:\n{0}", gameEntryCollection.Statistics.ToString());
            PromptQuitIfNotKey("\nProceed with extraction?\n", ConsoleKey.Y);
            ExtractGames(gameEntryCollection, args.OutputDirectory, args.Categorize);
        }

        public static void ExtractGames(GameEntryCollection gameEntryCollection, string targetDirectory, bool splitByRegion)
        {
            int currentGameCount = 1;
            int toCopyTotalArchives = gameEntryCollection.Statistics.TotalArchivesToExtract;

            foreach (GameEntry ge in gameEntryCollection.GameEntries)
            {
                string sourceZipFile;
                var outputDirectory = targetDirectory + "\\" + ((splitByRegion) ? ge.Region.ToString() : "") + "\\" + ge.Title;

                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.MultidiscRefs)
                    {
                        sourceZipFile = Directory.GetCurrentDirectory() + "\\" + mde.DiscTitle + ".zip";
                        Console.Write("{0}/{1} Unzipping {2}...", currentGameCount++, toCopyTotalArchives, mde.DiscTitle + ".zip");
                        UnzipGame(sourceZipFile, outputDirectory);
                    }

                }
                else
                {
                    sourceZipFile = Directory.GetCurrentDirectory() + "\\" + ge.Title + ".zip";
                    Console.Write("{0}/{1} Unzipping {2}...", currentGameCount++, toCopyTotalArchives, ge.Title + ".zip");
                    UnzipGame(sourceZipFile, outputDirectory);
                }
            }

            Console.WriteLine("Finished extracting.");
        }

        public static void UnzipGame(string sourceZipFilename, string targetDirectory)
        {

            if (File.Exists(sourceZipFilename))
            {
                stopwatch.Start();
                ZipFile.ExtractToDirectory(sourceZipFilename, targetDirectory, true);
                stopwatch.Stop();
                Console.WriteLine("[{0:mm\\:ss}] DONE", stopwatch.Elapsed);
                stopwatch.Reset();
            }
            else
            {
                Console.WriteLine("File not found! [" + sourceZipFilename + "].");
            }
        }

        public static void PromptQuitIfNotKey(String prepend, ConsoleKey proceedKey)
        {
            Console.WriteLine(prepend + "Press {0} to continue or any other key to Quit", proceedKey);
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            if (consoleKeyInfo.Key != proceedKey)
            {
                Environment.Exit(0);
            }
            Console.WriteLine("\n");
        }
    }

    public class ExtractArguments
    {
        [ArgRequired, ArgDescription("CSV file containing which files to extract"), ArgPosition(1), ArgDefaultValue("_Input.CSV")]
        public string SourceCSV { get; set; }
        [ArgRequired, ArgDescription("Target directory where to extract archives"), ArgPosition(2)]
        public string OutputDirectory { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(3), ArgDefaultValue(false)]
        public bool Categorize { get; set; }
    }

    public class ExtractAllArguments
    {
        [ArgRequired, ArgDescription("Target directory where to extract archives"), ArgPosition(2)]
        public string OutputDirectory { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(3), ArgDefaultValue(false)]
        public bool Categorize { get; set; }
    }
}