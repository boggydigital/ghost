﻿using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.DownloadSources;
using Interfaces.UriRedirection;
using Interfaces.Data;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Download.Dependencies.Validation
{
    public class ValidationDownloadSourcesController : IDownloadSourcesController
    {
        private IDataController<ScheduledDownload> scheduledDownloadsDataController;
        private IUriRedirectController uriRedirectController;

        private readonly List<string> extensionsWhitelist = new List<string>(4) {
            ".exe", // Windows
            ".bin", // Windows
            ".dmg", // Mac
            ".sh" // Linux
        };

        public ValidationDownloadSourcesController(
            IDataController<ScheduledDownload> scheduledDownloadsDataController,
            IUriRedirectController uriRedirectController)
        {
            this.scheduledDownloadsDataController = scheduledDownloadsDataController;
            this.uriRedirectController = uriRedirectController;
        }

        public async Task<IDictionary<long, IList<string>>> GetDownloadSources()
        {
            var validationSources = new Dictionary<long, IList<string>>();

            foreach (var id in scheduledDownloadsDataController.EnumerateIds())
            {
                var scheduledDownload = await scheduledDownloadsDataController.GetById(id);

                foreach (var downloadEntry in scheduledDownload.Downloads)
                { 
                    // only product files are eligible for validation
                    if (downloadEntry.Type != ScheduledDownloadTypes.File)
                        continue;

                    // and among product files only executables are eligible for validation
                    if (!extensionsWhitelist.Contains(Path.GetExtension(downloadEntry.Source)))
                        continue;

                    if (!validationSources.ContainsKey(id))
                        validationSources.Add(id, new List<string>());

                    validationSources[id].Add(await uriRedirectController.GetUriRedirect(downloadEntry.Source));
                }
            }

            return validationSources;
        }
    }
}
