using Interfaces.Delegates.Values;
using Attributes;
using Delegates.Values.Directories.ProductTypes;
using Delegates.Values.Filenames.ProductTypes;

namespace Delegates.GetPath.Records
{
    public class GetGameDetailsRecordsPathDelegate : GetPathDelegate
    {
        [Dependencies(
            typeof(GetRecordsDirectoryDelegate),
            typeof(GetGameDetailsFilenameDelegate))]
        public GetGameDetailsRecordsPathDelegate(
            IGetValueDelegate<string,string> getRecordsDirectoryDelegate,
            IGetValueDelegate<string, string> getGameDetailsFilenameDelegate) :
            base(
                getRecordsDirectoryDelegate,
                getGameDetailsFilenameDelegate)
        {
            // ...
        }
    }
}