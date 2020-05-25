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

        public List<MultidiscEntry> multidiscRefs { get; set; }

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
                return (multidiscRefs == null) ? false : true;
            }
        }


        public void addMultidiscRef(string multidiscRef, float size)
        {
            if (multidiscRefs == null)
            {
                multidiscRefs = new List<MultidiscEntry>();
            }            
            multidiscRefs.Add(new MultidiscEntry(multidiscRef, size));
        }

        public override string ToString()
        {
            return Title + "\n" + Size + "\n" + ToExtract + "\n" + Region + "\n";
        }
    }

    public class MultidiscEntry
    {
        public string discTitle { get; }
        public float discSizeInMB { get; }

        public MultidiscEntry(string title, float size)
        {
            discTitle = title;
            discSizeInMB = size;
        }

    }
}
