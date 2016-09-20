﻿using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Storage;
using Interfaces.ProductTypes;
using Interfaces.Collection;
using Interfaces.File;
using Interfaces.DownloadSources;
using Interfaces.GOGUri;
using Interfaces.UriRedirect;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Abstract
{
    public abstract class ScheduleDownloadsController : TaskActivityController
    {
        private ScheduledDownloadTypes downloadType;
        private string destination;

        private IDownloadSourcesController downloadSourcesController;
        private IUriRedirectController uriRedirectController;
        private IGOGUriController gogUriController;
        private IProductTypeStorageController productTypeStorageController;
        private ICollectionController collectionController;
        private IFileController fileController;

        public ScheduleDownloadsController(
            ScheduledDownloadTypes downloadType,
            string destination,
            IDownloadSourcesController downloadSourcesController,
            IUriRedirectController uriRedirectController,
            IGOGUriController gogUriController,
            IProductTypeStorageController productTypeStorageController,
            ICollectionController collectionController,
            IFileController fileController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.downloadSourcesController = downloadSourcesController;
            this.uriRedirectController = uriRedirectController;
            this.gogUriController = gogUriController;
            this.productTypeStorageController = productTypeStorageController;
            this.collectionController = collectionController;
            this.fileController = fileController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Load existing scheduled downloads");
            var scheduledDownloads = await productTypeStorageController.Pull<ScheduledDownload>(ProductTypes.ScheduledDownload);
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Get " + System.Enum.GetName(typeof(ScheduledDownloadTypes), downloadType) + " sources");
            var downloadSources = await downloadSourcesController.GetDownloadSources();
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Schedule downloads if not previously scheduled and no file exists");
            foreach (var downloadSource in downloadSources)
            {
                var productId = downloadSource.Key;

                foreach (var source in downloadSource.Value)
                {
                    var existingProductDownload = collectionController.Find(scheduledDownloads, sd =>
                        sd != null &&
                        sd.Source == source);

                    if (existingProductDownload != null) continue;

                    var redirectedSource =
                        uriRedirectController != null ?
                        await uriRedirectController.GetUriRedirect(source) :
                        source;

                    var adjustedDestination =
                        gogUriController != null ?
                        gogUriController.GetDirectory(redirectedSource) :
                        destination;

                    if (fileController != null)
                    {
                        var resolvedFilename = string.Empty;

                        switch (downloadType)
                        {
                            case ScheduledDownloadTypes.Image:
                            case ScheduledDownloadTypes.Screenshot:
                                var resolvedUri = new System.Uri(redirectedSource);
                                resolvedFilename = resolvedUri.Segments[resolvedUri.Segments.Length - 1];
                                break;

                            case ScheduledDownloadTypes.File:
                            case ScheduledDownloadTypes.Extra:
                                resolvedFilename = gogUriController.GetFilename(redirectedSource);
                                break;

                            default:
                                throw new System.NotImplementedException();
                        }

                        var localFile = System.IO.Path.Combine(adjustedDestination, resolvedFilename);
                        if (fileController.Exists(localFile)) continue;
                    }

                    var newScheduledDownload = new ScheduledDownload();
                    newScheduledDownload.Id = productId;
                    newScheduledDownload.Type = downloadType;
                    newScheduledDownload.Source = redirectedSource;
                    newScheduledDownload.Destination = adjustedDestination;

                    scheduledDownloads.Add(newScheduledDownload);
                }
            }
            taskReportingController.CompleteTask();

            taskReportingController.StartTask("Save scheduled downloads");
            await productTypeStorageController.Push(ProductTypes.ScheduledDownload, scheduledDownloads);
            taskReportingController.CompleteTask();

        }
    }
}