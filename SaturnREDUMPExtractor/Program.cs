using PowerArgs;
using SaturnREDUMPExtractor;

namespace SaturnCSVCopier
{
    class Program
    {
        // TODO filter out demos
        // TODO filter out protos
        // TODO overwrite warning (all?)
        // Unknown category and catching compound regions such as (USA, Brasil)
        // Input directory as optional parameter
        // split by initial letter
        // PAL specific: region split

        static void Main(string[] args)
        {
            Args.InvokeAction<REDUMPExtractor>(args);
        }

    }

}
