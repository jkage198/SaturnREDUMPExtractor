using System;
using System.Diagnostics;
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
        public void generateCSV(CSVBuilderArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(Directory.GetCurrentDirectory()));
            Console.WriteLine("\nGenerating CSV with the following:\n{0}", gameEntryCollection.Statistics.ToString());
            parser.GameEntryListToCSV(gameEntryCollection.GameEntries, args.csvFilename);
        }

        [ArgActionMethod, ArgDescription("Extracts games to target directory as defined in input CSV file")]
        public void extract(ExtractArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.CSVToGameEntryList(args.sourceCSV));
            
            ExtractGames(gameEntryCollection, args.outputDirectory, args.categorize);
        }

        [ArgActionMethod, ArgDescription("Extracts all games in working directory to target directory")]
        public void extractAll(ExtractAllArguments args)
        {
            Parser parser = new Parser();
            GameEntryCollection gameEntryCollection = new GameEntryCollection(parser.DirectoryToGameEntryList(Directory.GetCurrentDirectory()));
            ExtractGames(gameEntryCollection, args.outputDirectory, args.categorize);
        }

        public static void ExtractGames(GameEntryCollection gameEntryCollection, string targetDirectory, bool splitByRegion)
        {
            int currentGameCount = 1;
            int toCopyTotalArchives = gameEntryCollection.Statistics.totalArchivesToExtract;

            foreach (GameEntry ge in gameEntryCollection.GameEntries)
            {
                string sourceZipFile;
                var outputDirectory = targetDirectory + "\\" + ((splitByRegion) ? ge.Region.ToString() : "") + "\\" + ge.Title;

                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.multidiscRefs)
                    {
                        sourceZipFile = Directory.GetCurrentDirectory() + "\\" + mde.discTitle + ".zip";
                        Console.Write("{0}/{1} Unzipping {2}...", currentGameCount++, toCopyTotalArchives, mde.discTitle + ".zip");
                        unzipGame(sourceZipFile, outputDirectory);
                    }

                }
                else
                {
                    sourceZipFile = Directory.GetCurrentDirectory() + "\\" + ge.Title + ".zip";
                    Console.Write("{0}/{1} Unzipping {2}...", currentGameCount++, toCopyTotalArchives, ge.Title + ".zip");
                    unzipGame(sourceZipFile, outputDirectory);
                }
            }

            Console.WriteLine("Finished extracting.");
        }

        public static void unzipGame(string sourceZipFilename, string targetDirectory)
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

    }

    public class ExtractArguments
    {
        [ArgRequired, ArgDescription("CSV file containing which files to extract"), ArgPosition(1), ArgDefaultValue("_Input.CSV")]
        public string sourceCSV { get; set; }
        [ArgRequired, ArgDescription("Target directory where to extract archives"), ArgPosition(2)]
        public string outputDirectory { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(3), ArgDefaultValue(false)]
        public bool categorize { get; set; }
    }

    public class ExtractAllArguments
    {
        [ArgRequired, ArgDescription("Target directory where to extract archives"), ArgPosition(2)]
        public string outputDirectory { get; set; }
        [ArgShortcut("r"), ArgDescription("Categorize extracted games by Region folders (EU/US/JP/Other)"), ArgPosition(3), ArgDefaultValue(false)]
        public bool categorize { get; set; }
    }
}