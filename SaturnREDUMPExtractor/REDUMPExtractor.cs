using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using PowerArgs;
using SaturnCSVCopier;
using SevenZipExtractor;

namespace SaturnREDUMPExtractor
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    class REDUMPExtractor
    {
        public static Stopwatch stopwatch = new Stopwatch();

        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Generates an input-compatibe CSV file")]
        public void GenerateCSV(CSVBuilderArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(args.InputDirectory));
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
            ExtractGames(gameEntryCollection, args.InputDirectory, args.OutputDirectory, args.Categorize);
        }

        [ArgActionMethod, ArgDescription("Extracts all games to target directory")]
        public void ExtractAll(ExtractAllArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(args.InputDirectory));
            Console.WriteLine("Extracting the following:\n{0}", gameEntryCollection.Statistics.ToString());
            PromptQuitIfNotKey("\nProceed with extraction?\n", ConsoleKey.Y);
            ExtractGames(gameEntryCollection, args.InputDirectory, args.OutputDirectory, args.Categorize);
        }

        public static void ExtractGames(GameEntryCollection gameEntryCollection, string sourceDirectory, string targetDirectory, bool splitByRegion)
        {
            int currentGameCount = 1;
            int toCopyTotalArchives = gameEntryCollection.Statistics.TotalArchivesToExtract;

            foreach (GameEntry ge in gameEntryCollection.GameEntries)
            {
                string sourceArchiveFile;
                var outputDirectory = targetDirectory + "\\" + ((splitByRegion) ? ge.Region.ToString() : "") + "\\" + ge.Title;

                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.MultidiscRefs)
                    {
                        sourceArchiveFile = sourceDirectory + "\\" + mde.DiscTitle;
                        Console.Write("{0}/{1} Extracting {2}...", currentGameCount++, toCopyTotalArchives, mde.DiscTitle + ge.ArchiveExtension);
                        ExtractGame(sourceArchiveFile, outputDirectory, ge.ArchiveExtension);
                    }
                }
                else
                {
                    sourceArchiveFile = sourceDirectory + "\\" + ge.Title;
                    Console.Write("{0}/{1} Extracting {2}...", currentGameCount++, toCopyTotalArchives, ge.Title + ge.ArchiveExtension);
                    ExtractGame(sourceArchiveFile, outputDirectory, ge.ArchiveExtension);
                }
            }

            Console.WriteLine("Finished extracting.");
        }

        public static void ExtractGame(string sourceArchiveName, string targetDirectory, string archiveExtension)
        {

            if (File.Exists(sourceArchiveName + archiveExtension))
            {
                stopwatch.Start();
                ArchiveFile archiveFile = new ArchiveFile(sourceArchiveName + archiveExtension);
                archiveFile.Extract(targetDirectory);
                stopwatch.Stop();
                Console.WriteLine("[{0:mm\\:ss}] DONE", stopwatch.Elapsed);
                stopwatch.Reset();
            }
            else
            {
                Console.WriteLine("File not found! [" + sourceArchiveName + "].");
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
        [ArgRequired, ArgDescription("Input directory containing game archives"), ArgPosition(1)]
        public string InputDirectory { get; set; }
        [ArgRequired, ArgDescription("Output directory where to extract archives"), ArgPosition(2)]
        public string OutputDirectory { get; set; }
        [ArgRequired, ArgDescription("CSV file containing which files to extract"), ArgPosition(3), ArgDefaultValue("_Input.CSV")]
        public string SourceCSV { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(4), ArgDefaultValue(false)]
        public bool Categorize { get; set; }
    }

    public class ExtractAllArguments
    {
        [ArgRequired, ArgDescription("Input directory containing game archives"), ArgPosition(1)]
        public string InputDirectory { get; set; }
        [ArgRequired, ArgDescription("Output directory where to extract archives"), ArgPosition(2)]
        public string OutputDirectory { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(3), ArgDefaultValue(false)]
        public bool Categorize { get; set; }
    }
}