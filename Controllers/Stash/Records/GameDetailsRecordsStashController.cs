using System.Collections.Generic;

using Interfaces.Delegates.GetPath;

using Interfaces.Controllers.SerializedStorage;

using Interfaces.Status;

using Attributes;

using Models.Records;
using Models.Dependencies;

namespace Controllers.Stash.Records
{
    public class GameDetailsRecordsStashController : StashController<List<ProductRecords>>
    {
        [Dependencies(
            "Delegates.GetPath.Records.GetGameDetailsRecordsPathDelegate,Delegates",
            Dependencies.DefaultSerializedStorageController,
            "Controllers.Status.StatusController,Controllers")]
        public GameDetailsRecordsStashController(
            IGetPathDelegate getGameDetailsRecordsPathDelegate,
            ISerializedStorageController serializedStorageController,
            IStatusController statusController) :
            base(
                getGameDetailsRecordsPathDelegate,
                serializedStorageController,
                statusController)
        {
            // ...
        }
    }
}