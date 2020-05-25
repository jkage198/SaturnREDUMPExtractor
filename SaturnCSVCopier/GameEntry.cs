using System.Collections.Generic;

namespace SaturnCSVCopier
{
    class GameEntry
    {
        public string Title { get; set; }
        public float Size { get; set; }
        public bool ToExtract { get; set; }
        public GameRegion Region { get; set; }
        public GameType Type { get; set; }

        public List<MultidiscEntry> MultidiscRefs { get; set; }

        public enum GameRegion
        {
            JP,
            USA,
            EU,
            Other,
            Unknown
        };

        public enum GameType
        {
            Game,
            Demo,
            Proto,
            Beta,
            Other,
            Unknown
        };

        public bool IsMultidisc
        {
            get
            {
                return MultidiscRefs != null;
            }
        }


        public void AddMultidiscRef(string multidiscRef, float size)
        {
            if (MultidiscRefs == null)
            {
                MultidiscRefs = new List<MultidiscEntry>();
            }            
            MultidiscRefs.Add(new MultidiscEntry(multidiscRef, size));
        }

        public override string ToString()
        {
            return Title + "\n" + Size + "\n" + ToExtract + "\n" + Region + "\n";
        }
    }

    public class MultidiscEntry
    {
        public string DiscTitle { get; }
        public float DiscSizeInMB { get; }

        public MultidiscEntry(string title, float size)
        {
            DiscTitle = title;
            DiscSizeInMB = size;
        }

    }
}
