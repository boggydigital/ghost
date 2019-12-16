using System.Collections.Generic;

using Interfaces.Delegates.GetPath;
using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Controllers.Stash;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Controllers.Stash.ProductTypes
{
    public class GameProductDataStashController : StashController<List<GameProductData>>
    {
        [Dependencies(
            "Delegates.GetPath.ProductTypes.GetGameProductDataPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public GameProductDataStashController(
            IGetPathDelegate getGameProductDataPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameProductDataPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}