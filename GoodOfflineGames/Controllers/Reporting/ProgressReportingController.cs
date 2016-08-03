﻿using System;

using Interfaces.Console;
using Interfaces.Network;
using Interfaces.Formatting;

namespace Controllers
{
    public class DownloadProgressReportingController : IDownloadProgressReportingController
    {
        private IConsoleController consoleController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;
        private DateTime startTimestamp;
        private DateTime lastReportedTimestamp = DateTime.MinValue;
        private const string reportFormat = "\r{0:P1} at {1:D}/s. Est. time: {2:D}     ";
        private const string downloadComplete = "\rFile was successfully downloaded.                              ";

        public int ThrottleMilliseconds { get; set; }
        private const int throttleMillisecondsDefault = 1000;

        public DownloadProgressReportingController(
            IFormattingController bytesFormattingController, 
            IFormattingController secondsFormattingController,
            IConsoleController consoleController)
        {
            this.consoleController = consoleController;

            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;

            ThrottleMilliseconds = throttleMillisecondsDefault;
        }

        public void Initialize()
        {
            startTimestamp = DateTime.Now;
            lastReportedTimestamp = DateTime.Now;
        }

        public void Report(long currentValue, long maxValue)
        {
            if (currentValue == maxValue)
            {
                consoleController.Write(downloadComplete, MessageType.Success);
                return;
            }

            // throttle updates to once in throttleMillisecond
            if ((DateTime.Now - lastReportedTimestamp).TotalMilliseconds < ThrottleMilliseconds) return;

            // calculate percent completion
            var percent = (double)currentValue / maxValue;
            var elapsed = DateTime.Now - startTimestamp;
            var bytesPerSecond = currentValue / elapsed.TotalSeconds;
            var remainingSeconds = (maxValue - currentValue) / bytesPerSecond;

            var formattedBytesPerSecond = bytesFormattingController.Format((long)bytesPerSecond);
            var formattedRemainingSeconds = secondsFormattingController.Format((long)remainingSeconds);

            consoleController.Write(reportFormat, MessageType.Progress, percent, formattedBytesPerSecond, formattedRemainingSeconds);

            lastReportedTimestamp = DateTime.Now;
        }
    }

}