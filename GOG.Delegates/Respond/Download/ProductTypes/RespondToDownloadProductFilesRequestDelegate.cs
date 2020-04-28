﻿using Interfaces.Delegates.Data;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Activities;
using Models.ProductTypes;
using GOG.Interfaces.Delegates.DownloadProductFile;
using Attributes;
using GOG.Models;

namespace GOG.Delegates.Respond.Download.ProductTypes
{
    [RespondsToRequests(Method = "download", Collection = "productfiles")]
    public class RespondToDownloadProductFilesRequestDelegate : RespondToDownloadRequestDelegate<ProductFile>
    {
        [Dependencies(
            "Delegates.Itemize.ProductTypes.ItemizeAllProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.UpdateProductDownloadsAsyncDelegate,Delegates",
            "Delegates.Data.Models.ProductTypes.DeleteProductDownloadsAsyncDelegate,Delegates",
            "GOG.Delegates.DownloadProductFile.DownloadManualUrlFileAsyncDelegate,GOG.Delegates",
            "Delegates.Activities.StartDelegate,Delegates",
            "Delegates.Activities.SetProgressDelegate,Delegates",
            "Delegates.Activities.CompleteDelegate,Delegates")]
        public RespondToDownloadProductFilesRequestDelegate(
            IItemizeAllAsyncDelegate<ProductDownloads> itemizeAllProductDownloadsAsyncDelegate,
            IUpdateAsyncDelegate<ProductDownloads> updateProductDownloadsAsyncDelegate,
            IDeleteAsyncDelegate<ProductDownloads> deleteProductDownloadsAsyncDelegate,
            IDownloadProductFileAsyncDelegate downloadProductFileAsyncDelegate,
            IStartDelegate startDelegate,
            ISetProgressDelegate setProgressDelegate,
            ICompleteDelegate completeDelegate) :
            base(
                itemizeAllProductDownloadsAsyncDelegate,
                updateProductDownloadsAsyncDelegate,
                deleteProductDownloadsAsyncDelegate,
                downloadProductFileAsyncDelegate,
                startDelegate,
                setProgressDelegate,
                completeDelegate)
        {
            // ...
        }
    }
}