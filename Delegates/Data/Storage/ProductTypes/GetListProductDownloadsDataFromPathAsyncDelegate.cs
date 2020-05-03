using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class
        GetListProductDownloadsDataFromPathAsyncDelegate : GetJSONDataFromPathAsyncDelegate<List<ProductDownloads>>
    {
        [Dependencies(
            typeof(GetListProductDownloadsDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductDownloadsPathDelegate))]
        public GetListProductDownloadsDataFromPathAsyncDelegate(
            IGetDataAsyncDelegate<List<ProductDownloads>, string> getListProductDownloadsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getProductDownloadsPathDelegate) :
            base(
                getListProductDownloadsDataAsyncDelegate,
                getProductDownloadsPathDelegate)
        {
            // ...
        }
    }
}