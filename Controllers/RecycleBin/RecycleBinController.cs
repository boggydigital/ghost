﻿using System.IO;

using Interfaces.Delegates.GetDirectory;

using Interfaces.Controllers.Directory;
using Interfaces.Controllers.File;

using Interfaces.RecycleBin;

namespace Controllers.RecycleBin
{
    public class RecycleBinController : IRecycleBinController
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IFileController fileController;
        private IDirectoryController directoryController;

        public RecycleBinController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IFileController fileController,
            IDirectoryController directoryController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.fileController = fileController;
            this.directoryController = directoryController;
        }

        public void MoveToRecycleBin(string uri)
        {
            var recycleBinUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                uri);
            fileController.Move(uri, recycleBinUri);
        }

        //public void MoveDirectoryToRecycleBin(string uri)
        //{
        //    directoryController.Move(uri, getDirectoryDelegate.GetDirectory());
        //}
    }
}
