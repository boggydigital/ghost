using Interfaces.Delegates.GetFilename;

using Attributes;

using Models.Filenames;

namespace Delegates.GetFilename.ArgsDefinitions
{
    public class GetArgsDefinitionsFilenameDelegate: GetFixedFilenameDelegate
    {
        [Dependencies("Delegates.GetFilename.GetJsonFilenameDelegate,Delegates")]
        public GetArgsDefinitionsFilenameDelegate(IGetFilenameDelegate getFilenameDelegate):
            base(Filenames.ArgsDefinitions, getFilenameDelegate)
            {
                // ...
            }
    }
}