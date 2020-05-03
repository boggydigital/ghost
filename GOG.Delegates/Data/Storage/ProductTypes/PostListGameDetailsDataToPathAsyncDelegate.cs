using System.Collections.Generic;
using Attributes;
using Delegates.Data.Storage;
using Interfaces.Delegates.Data;
using Interfaces.Delegates.Values;
using GOG.Models;
using Delegates.GetPath.ProductTypes;

namespace GOG.Delegates.Data.Storage.ProductTypes
{
    public class PostListGameDetailsDataToPathAsyncDelegate : PostJSONDataToPathAsyncDelegate<List<GameDetails>>
    {
        [Dependencies(
            typeof(PostListGameDetailsDataAsyncDelegate),
            typeof(GetGameDetailsPathDelegate))]
        public PostListGameDetailsDataToPathAsyncDelegate(
            IPostDataAsyncDelegate<List<GameDetails>> postListGameDetailsDataAsyncDelegate,
            IGetValueDelegate<string,(string Directory,string Filename)> getGameDetailsPathDelegate) :
            base(
                postListGameDetailsDataAsyncDelegate,
                getGameDetailsPathDelegate)
        {
            // ...
        }
    }
}