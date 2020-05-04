using System.Collections.Generic;
using Attributes;
using Delegates.Conversions.Uris;
using GOG.Models;
using Interfaces.Delegates.Conversions;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemizations;

namespace GOG.Delegates.Data.Models.ProductTypes
{
    public class GetDeserializedGOGDataAsyncDelegate :
        GetDeserializedDataAsyncDelegate<GOGData>
    {
        [Dependencies(
            typeof(ConvertUriDictionaryParametersToUriDelegate),
            typeof(Network.GetUriDataPolitelyAsyncDelegate),
            typeof(Itemize.ItemizeGOGDataDelegate),
            typeof(GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToGOGDataDelegate))]
        public GetDeserializedGOGDataAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,
            IGetDataAsyncDelegate<string,string> getUriDataAsyncDelegate,
            IItemizeDelegate<string, string> itemizeGogDataDelegate,
            IConvertDelegate<string, GOGData> convertJSONToGOGDataDelegate) :
            base(
                convertUriParametersToUriDelegate,
                getUriDataAsyncDelegate,
                itemizeGogDataDelegate,
                convertJSONToGOGDataDelegate)
        {
            // ...
        }
    }
}