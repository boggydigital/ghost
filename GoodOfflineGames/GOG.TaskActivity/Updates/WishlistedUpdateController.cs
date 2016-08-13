﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Network;
using Interfaces.Extraction;
using Interfaces.Serialization;
using Interfaces.Storage;
using Interfaces.ProductTypes;

using Models.Uris;
using Models.QueryParameters;

using GOG.TaskActivities.Abstract;

namespace GOG.TaskActivities.Updates
{
    public class WishlistedUpdateController: TaskActivityController
    {
        private INetworkController networkController;
        private IExtractionController gogDataExtractionController;
        private ISerializationController<string> serializationController;
        private IProductTypeStorageController productStorageController;

        public WishlistedUpdateController(
            INetworkController networkController,
            IExtractionController gogDataExtractionController,
            ISerializationController<string> serializationController,
            IProductTypeStorageController productStorageController,
            ITaskReportingController taskReportingController):
            base(taskReportingController)
        {
            this.networkController = networkController;
            this.gogDataExtractionController = gogDataExtractionController;
            this.serializationController = serializationController;
            this.productStorageController = productStorageController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.AddTask("Request wishlist content");
            var wishlistedContent = await networkController.Get(Uris.Paths.Account.Wishlist);
            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Extract wishlist data");
            var wishlistedGogDataCollection = gogDataExtractionController.ExtractMultiple(wishlistedContent);
            if (wishlistedGogDataCollection == null)
            {
                taskReportingController.ReportFailure("Extracted wishlist data is null.");
                return;
            }
            if (wishlistedGogDataCollection.Count() == 0)
            {
                taskReportingController.ReportFailure("Extracted wishlist data is empty.");
                return;
            }
            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Deserialize wishlist data");

            var wishlistedGogData = wishlistedGogDataCollection.ElementAt(0);
            var wishlistedProductPageResult = serializationController.Deserialize<Models.ProductsPageResult>(wishlistedGogData);

            if (wishlistedProductPageResult == null)
            {
                taskReportingController.ReportFailure("Failed to deserialize wishlist data");
                return;
            }

            if (wishlistedProductPageResult.Products == null)
            {
                taskReportingController.ReportFailure("Deserialized wishlist data contains to products");
                return;
            }

            taskReportingController.CompleteTask();

            taskReportingController.AddTask("Save wishlist data");

            var wishlisted = new List<long>();
            foreach (var product in wishlistedProductPageResult.Products)
            {
                if (product == null) continue;
                wishlisted.Add(product.Id);
            }

            await productStorageController.Push(ProductTypes.WishlistedProduct, wishlisted);

            taskReportingController.CompleteTask();
        }
    }
}
