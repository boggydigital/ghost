using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.GetPath;
using GOG.Models;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListProductDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<Product>>
    {
        [Dependencies(
            "GOG.Delegates.Data.Storage.ProductTypes.PostListProductDataAsyncDelegate,GOG.Delegates",
            "Delegates.GetPath.ProductTypes.GetProductsPathDelegate,Delegates")]
        public PostListProductDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<Product>> postListProductDataAsyncDelegate,
            IGetPathDelegate getProductPathDelegate) :
            base(
                postListProductDataAsyncDelegate,
                getProductPathDelegate)
        {
            // ...
        }
    }
}