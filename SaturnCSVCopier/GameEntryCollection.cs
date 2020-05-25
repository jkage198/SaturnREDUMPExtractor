using SaturnCSVCopier;
using System.Collections.Generic;

namespace SaturnREDUMPExtractor
{
    class GameEntryCollection
    {
        public List<GameEntry> GameEntries;
        public ParsingStatistics Statistics {  get { return GenerateStatistics(); } }

        public GameEntryCollection(List<GameEntry> gameEntryList)
        {
            GameEntries = gameEntryList;
        }

        private ParsingStatistics GenerateStatistics()
        {
            ParsingStatistics statistics = new ParsingStatistics();
            statistics.totalGamesToExctract = GameEntries.Count;

            foreach (GameEntry ge in GameEntries)
            {
                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.multidiscRefs)
                    {
                        statistics.totalArchives++;
                        statistics.totalArchivesSizeInMB += mde.discSizeInMB;
                        if (ge.ToExtract)
                        {
                            statistics.totalArchivesToExtract++;
                            statistics.totalArchivesToExtractSizeInMB += mde.discSizeInMB;
                        }
                    }
                } else
                {
                    statistics.totalArchives++;
                    statistics.totalArchivesSizeInMB += ge.Size;
                    if (ge.ToExtract)
                    {
                        statistics.totalArchivesToExtract++;
                        statistics.totalArchivesToExtractSizeInMB += ge.Size;
                    }
                }
            }

            return statistics;
        }

    }
}
