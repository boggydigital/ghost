﻿using System.Linq;
using System.Threading.Tasks;

using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Data;
using Interfaces.TaskStatus;

using Models.Uris;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Update.Wishlisted
{
    public class WishlistedUpdateController: TaskActivityController
    {
        private INetworkController networkController;
        private IExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;
        private IDataController<long> wishlistedDataController;

        public WishlistedUpdateController(
            INetworkController networkController,
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController,
            IDataController<long> wishlistedDataController,
            ITaskStatus taskStatus,
            ITaskStatusController taskStatusController):
            base(
                taskStatus,
                taskStatusController)
        {
            this.networkController = networkController;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
            this.wishlistedDataController = wishlistedDataController;
        }

        public override async Task ProcessTaskAsync()
        {
            var updateWishlistTask = taskStatusController.Create(taskStatus, "Update wishlisted products");

            var requestContentTask = taskStatusController.Create(updateWishlistTask, "Request wishlist content");
            var wishlistedContent = await networkController.Get(Uris.Paths.Account.Wishlist);
            taskStatusController.Complete(requestContentTask);

            var extractTask = taskStatusController.Create(updateWishlistTask, "Extract wishlist data");
            var wishlistedGogDataCollection = gogDataExtractionController.ExtractMultiple(wishlistedContent);
            if (wishlistedGogDataCollection == null)
            {
                taskStatusController.Fail(extractTask, "Extracted wishlist data is null.");
                return;
            }
            if (wishlistedGogDataCollection.Count() == 0)
            {
                taskStatusController.Fail(extractTask, "Extracted wishlist data is empty.");
                return;
            }
            taskStatusController.Complete(extractTask);

            var deserializeDataTask = taskStatusController.Create(updateWishlistTask, "Deserialize wishlist data");
            var wishlistedGogData = wishlistedGogDataCollection.First();
            var wishlistedProductPageResult = serializationController.Deserialize<Models.ProductsPageResult>(wishlistedGogData);

            if (wishlistedProductPageResult == null ||
                wishlistedProductPageResult.Products == null)
            {
                taskStatusController.Fail(deserializeDataTask, "Failed to deserialize wishlist data");
                return;
            }
            taskStatusController.Complete(deserializeDataTask);

            var saveDataTask = taskStatusController.Create(updateWishlistTask, "Add new wishlisted products");
            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                await wishlistedDataController.AddAsync(saveDataTask, product.Id);
            }
            taskStatusController.Complete(saveDataTask);

            taskStatusController.Complete(updateWishlistTask);
        }
    }
}
