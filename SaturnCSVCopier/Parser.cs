using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using PowerArgs;
using SaturnCSVCopier;

namespace SaturnREDUMPExtractor
{
    class Parser
    {
        private const string REGEX_REGION = "\\(Japan\\)|\\(USA\\)|\\(Europe\\)|\\(Germany\\)|\\(Spain\\)|\\(France\\)|\\(Korea\\)|\\(Taiwan\\)|\\(Brazil\\)|\\(Italy\\)";
        private const string REGEX_MULTIDISC = "\\(Disc ([0-9A-Z]|I+)\\)";
        private const string CSV_HEADERS = "Title,Size,Region,ToExtract";
        private const string CATEGORY_JAPAN = "(Japan)";
        private const string CATEGORY_USA = "(USA)";
        private const string CATEGORY_EUROPE = "(Europe)";

        // Returns a list of GameEntries from a specified directory
        public List<GameEntry> DirectoryToGameEntryList(string directory)
        {
            DirectoryInfo d = new DirectoryInfo(directory);
            FileInfo[] fileInfo = d.GetFiles("*.zip");
            List<string> inputList = fileInfo.OfType<string>().ToList();
            List<GameEntry> gameEntryList = new List<GameEntry>();

            string gameTitle = "";
            GameEntry ge;
            Regex regex = new Regex(REGEX_MULTIDISC);

            foreach (FileInfo f in fileInfo)
            {
                ge = new GameEntry();
                gameTitle = f.Name[0..^4];
                
                ge.Title = gameTitle;
                ge.Size = f.Length / 1048576;
                ge.Region = ExtractRegion(gameTitle);
                ge.ToExtract = true;

                Match multidiscRegex = regex.Match(ge.Title);

                if (multidiscRegex.Success)
                {
                    int titleShortIndex = ge.Title.IndexOf(" " + multidiscRegex.Value);
                    string multiDiscTitle = ge.Title.Substring(0, titleShortIndex);

                    // check if game has already been encountered in existing list
                    GameEntry existingGameEntry = gameEntryList.Find(x => x.Title.Equals(multiDiscTitle));
                    if (existingGameEntry != null)
                    {
                        // exists
                        existingGameEntry.AddMultidiscRef(ge.Title, ge.Size);
                    }
                    else
                    {
                        // doesn't exist
                        ge.AddMultidiscRef(ge.Title, ge.Size);
                        ge.Title = multiDiscTitle;
                        ge.Size = 0;
                        gameEntryList.Add(ge);
                    }
                }
                else
                {
                    // single disc
                    gameEntryList.Add(ge);
                }
            }
            return gameEntryList;
        }

        // Generate a CSV file on disk from a Game Entry list object
        public void GameEntryListToCSV(List<GameEntry> gameEntryList, string csvFilename)
        {
            List<string> csvRows = new List<string>
            {
                CSV_HEADERS
            };

            foreach (GameEntry ge in gameEntryList)
            {
                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.MultidiscRefs)
                    {
                        csvRows.Add(GameEntryToCSVRow(ge, mde));
                    }
                } else
                {
                    csvRows.Add(GameEntryToCSVRow(ge, null));
                }
            }

            if (File.Exists(csvFilename))
            {
                Console.WriteLine("\n{0} already exists. \nPress Y to overwrite and continue or Q to Quit.", csvFilename);
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                if (consoleKeyInfo.Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                File.Delete(csvFilename);
            }

            using StreamWriter sw = File.CreateText(csvFilename);
            sw.WriteLine(csvRows[0]);
            csvRows.RemoveAt(0);

            foreach (string filename in csvRows)
            {
                sw.WriteLine(filename);
            }

            sw.Flush();
            sw.Close();

            Console.WriteLine("\n\nFinished writing CSV: {0}.csv", csvFilename);
        }   
        
        // Helper: convert GameEntry to a CSV row (string)
        private string GameEntryToCSVRow(GameEntry ge, MultidiscEntry mde)
        {
            StringBuilder sb = new StringBuilder();
            string gameTitle = (mde == null) ? ge.Title : mde.DiscTitle;
            float gameSize = (mde == null) ? ge.Size : mde.DiscSizeInMB;

            if (gameTitle.Contains(","))
            {
                gameTitle = "\"" + gameTitle + "\"";
            }

            sb.Append(gameTitle).Append(",");
            sb.Append(gameSize).Append(",");
            sb.Append(ExtractRegion(gameTitle).ToString()).Append(",");
            sb.Append("TRUE");
            return sb.ToString();
        }

        // Generate a list of GameEntries from CSV
        public List<GameEntry> CSVToGameEntryList(string CSVfilename)
        {
            var reader = new StreamReader(CSVfilename);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ",";
            csv.Configuration.HeaderValidated = null;
            csv.Configuration.MissingFieldFound = null;
            List<GameEntry> gameEntryList = new List<GameEntry>();

            Regex regex = new Regex("\\(Disc ([0-9A-Z]|I+)\\)");

            while (csv.Read())
            {
                GameEntry ge = csv.GetRecord<GameEntry>();

                if (ge.ToExtract)
                {
                    // check if multidisc entry
                    Match multidiscRegex = regex.Match(ge.Title);

                    if (multidiscRegex.Success)
                    {
                        int titleShortIndex = ge.Title.IndexOf(" " + multidiscRegex.Value);

                        string multiDiscTitle = ge.Title.Substring(0, titleShortIndex);

                        // check if game has already been encountered in existing copy list
                        GameEntry existingGameEntry = gameEntryList.Find(x => x.Title.Equals(multiDiscTitle));
                        if (existingGameEntry != null)
                        {
                            // exists
                            existingGameEntry.AddMultidiscRef(ge.Title, ge.Size);
                        }
                        else
                        {
                            // doesn't exist
                            ge.AddMultidiscRef(ge.Title, ge.Size);
                            ge.Title = multiDiscTitle;
                            gameEntryList.Add(ge);
                        }
                    }
                    else
                    {
                        // single disc
                        gameEntryList.Add(ge);
                    }
                }
            }

            return gameEntryList;
        }

        // Extract game region from a game title/filename
        public static GameEntry.GameRegion ExtractRegion(string filename)
        {
            Regex rg = new Regex(REGEX_REGION, RegexOptions.IgnoreCase);
            Match match = rg.Match(filename);

            while (match.Success)
            {
                return match.Value switch
                {
                    CATEGORY_JAPAN => GameEntry.GameRegion.JP,
                    CATEGORY_USA => GameEntry.GameRegion.USA,
                    CATEGORY_EUROPE => GameEntry.GameRegion.EU,
                    _ => GameEntry.GameRegion.Other,
                };
            }

            return GameEntry.GameRegion.Unknown;
        }
    }

    public class ParsingStatistics
    {
        public int TotalArchives { get; set; }
        public float TotalArchivesSizeInMB { get; set; }
        public int TotalArchivesToExtract { get; set; }
        public float TotalArchivesToExtractSizeInMB { get; set; }
        public int TotalGamesToExctract { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Total Archives: ").Append(TotalArchives);
            sb.Append("\nTotal Archives Size (in MB): ").Append(TotalArchivesSizeInMB);
            sb.Append("\nTotal Archives set to Extract: ").Append(TotalArchivesToExtract);
            sb.Append("\nTotal Extract compressed size (in MB): ").Append(TotalArchivesToExtractSizeInMB);
            sb.Append("\nTotal Games set to extract: ").Append(TotalGamesToExctract);
            return sb.ToString();
        }
    }

    public class CSVBuilderArguments
    {
        [ArgRequired, ArgDescription("CSV filename to assign"), ArgPosition(1), ArgDefaultValue("_Input.CSV")]
        public string CsvFilename { get; set; }
    }
}
