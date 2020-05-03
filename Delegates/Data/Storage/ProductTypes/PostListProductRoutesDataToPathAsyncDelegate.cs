using System.Collections.Generic;
using Attributes;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using Models.ProductTypes;

namespace Delegates.Data.Storage.ProductTypes
{
    public class PostListProductRoutesDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<ProductRoutes>>
    {
        [Dependencies(
            typeof(PostListProductRoutesDataAsyncDelegate),
            typeof(Delegates.GetPath.ProductTypes.GetProductRoutesPathDelegate))]
        public PostListProductRoutesDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<ProductRoutes>> postListProductRoutesDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getListProductRoutesPathDelegate) :
            base(
                postListProductRoutesDataAsyncDelegate,
                getListProductRoutesPathDelegate)
        {
            // ...
        }
    }
}