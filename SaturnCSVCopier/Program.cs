using PowerArgs;
using SaturnREDUMPExtractor;

namespace SaturnCSVCopier
{
    class Program
    {
        // TODO filter out demos
        // TODO filter out protos
        // TODO overwrite warning?
        // Unknown category and catching compound regions such as (USA, Brasil)

        static void Main(string[] args)
        {
            Args.InvokeAction<REDUMPExtractor>(args);
        }

    }

}
