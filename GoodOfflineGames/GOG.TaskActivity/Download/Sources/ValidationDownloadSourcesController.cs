﻿using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.Destination.Filename;
using Interfaces.Data;
using Interfaces.Routing;
using Interfaces.Eligibility;
using Interfaces.TaskStatus;

using Models.ProductDownloads;

namespace GOG.TaskActivities.Download.Sources
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<long> updatedDataController;
        private IDataController<ProductDownloads> productDownloadsDataController;
        private IRoutingController routingController;
        private IEligibilityDelegate<ProductDownloadEntry> downloadEntryValidationEligibilityDelegate;
        private IEligibilityDelegate<string> fileValidationEligibilityDelegate;
        private IGetFilenameDelegate getDownloadFilenameDelegate;
        private IGetFilenameDelegate getValidationFilenameDelegate;

        public ValidationDownloadSourcesController(
            IDataController<long> updatedDataController,
            IDataController<ProductDownloads> productDownloadsDataController,
            IRoutingController routingController,
            IGetFilenameDelegate getDownloadFilenameDelegate,
            IGetFilenameDelegate getValidationFilenameDelegate,
            IEligibilityDelegate<ProductDownloadEntry> downloadEntryValidationEligibilityDelegate,
            IEligibilityDelegate<string> fileValidationEligibilityDelegate)
        {
            this.updatedDataController = updatedDataController;
            this.productDownloadsDataController = productDownloadsDataController;
            this.routingController = routingController;

            this.getDownloadFilenameDelegate = getDownloadFilenameDelegate;
            this.getValidationFilenameDelegate = getValidationFilenameDelegate;

            this.downloadEntryValidationEligibilityDelegate = downloadEntryValidationEligibilityDelegate;
            this.fileValidationEligibilityDelegate = fileValidationEligibilityDelegate;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSourcesAsync(ITaskStatus taskStatus)
        {
            var validationSources = new Dictionary<long, IList<string>>();

            var updated = updatedDataController.EnumerateIds().ToArray();

            foreach (var id in updated)
            {
                var productDownloads = await productDownloadsDataController.GetByIdAsync(id);
                if (productDownloads == null) continue;

                foreach (var downloadEntry in productDownloads.Downloads)
                {
                    // only product files are eligible for validation
                    if (!downloadEntryValidationEligibilityDelegate.IsEligible(downloadEntry))
                        continue;

                    // trace route for the product file
                    var resolvedUri = await routingController.TraceRouteAsync(id, downloadEntry.SourceUri);

                    if (string.IsNullOrEmpty(resolvedUri))
                        continue;

                    if (!fileValidationEligibilityDelegate.IsEligible(resolvedUri))
                        continue;

                    if (!validationSources.ContainsKey(id))
                        validationSources.Add(id, new List<string>());

                    // compute validation filename

                    var resolvedUriFilename = getDownloadFilenameDelegate.GetFilename(resolvedUri);
                    var validationFilename = getValidationFilenameDelegate.GetFilename(resolvedUriFilename);

                    var validationUri = resolvedUri.Replace(resolvedUriFilename, validationFilename);

                    validationSources[id].Add(validationUri);
                }
            }

            return validationSources;
        }
    }
}
