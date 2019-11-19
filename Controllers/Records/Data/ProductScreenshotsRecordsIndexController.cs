using Interfaces.Controllers.Data;
using Interfaces.Status;

using Models.Records;

namespace Controllers.Records.Data
{
    public class ProductScreenshotsRecordsIndexController : IndexRecordsController
    {
        public ProductScreenshotsRecordsIndexController(
            IDataController<ProductRecords> productScreenshotsRecordsController,
            IStatusController statusController) :
            base(
                productScreenshotsRecordsController,
                statusController)
        {
            // ...
        }
    }
}