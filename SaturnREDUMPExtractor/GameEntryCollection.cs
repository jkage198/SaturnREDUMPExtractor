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
            ParsingStatistics statistics = new ParsingStatistics
            {
                TotalGamesToExctract = GameEntries.Count
            };

            foreach (GameEntry ge in GameEntries)
            {
                if (ge.IsMultidisc)
                {
                    foreach (MultidiscEntry mde in ge.MultidiscRefs)
                    {
                        statistics.TotalArchives++;
                        statistics.TotalArchivesSizeInMB += mde.DiscSizeInMB;
                        if (ge.ToExtract)
                        {
                            statistics.TotalArchivesToExtract++;
                            statistics.TotalArchivesToExtractSizeInMB += mde.DiscSizeInMB;
                        }
                    }
                } else
                {
                    statistics.TotalArchives++;
                    statistics.TotalArchivesSizeInMB += ge.Size;
                    if (ge.ToExtract)
                    {
                        statistics.TotalArchivesToExtract++;
                        statistics.TotalArchivesToExtractSizeInMB += ge.Size;
                    }
                }
            }

            return statistics;
        }

    }
}
