﻿using System;
using System.Linq;
using System.Collections.Generic;

using Interfaces.ViewModel;
using Interfaces.TaskStatus;
using Interfaces.Formatting;

using Models.Units;

namespace Controllers.ViewModel
{
    public class TaskStatusAppViewModelDelegate : IGetViewModelDelegate<ITaskStatus>
    {
        private static class TaskStatusAppViewModelSchema
        {
            public const string Title = "title";
            public const string ContainsProgress = "containsProgress";
            public const string ProgressTarget = "progressTarget";
            public const string ProgressPercent = "progressPercent";
            public const string ProgressCurrent = "progressCurrent";
            public const string ProgressTotal = "progressTotal";
            public const string ContainsETA = "containsEta";
            public const string RemainingTime = "remainingTime";
            public const string AverageUnitsPerSecond = "averageUnitsPerSecond";
            public const string ContainsFailures = "containsFailures";
            public const string FailuresCount = "failuresCount";
            public const string ContainsWarnings = "containsWarnings";
            public const string WarningsCount = "warningsCount";
        }

        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;

        public TaskStatusAppViewModelDelegate(
            IFormattingController bytesFormattingController,
            IFormattingController secondsFormattingController)
        {
            this.bytesFormattingController = bytesFormattingController;
            this.secondsFormattingController = secondsFormattingController;
        }

        public IDictionary<string, string> GetViewModel(ITaskStatus taskStatus)
        {
            if (taskStatus.Complete) return null;

            // viewmodel schemas
            var viewModel = new Dictionary<string, string>()
            {
                { TaskStatusAppViewModelSchema.Title, "" },
                { TaskStatusAppViewModelSchema.ContainsProgress, "" },
                { TaskStatusAppViewModelSchema.ProgressTarget, "" },
                { TaskStatusAppViewModelSchema.ProgressPercent, "" },
                { TaskStatusAppViewModelSchema.ProgressCurrent, "" },
                { TaskStatusAppViewModelSchema.ProgressTotal, "" },
                { TaskStatusAppViewModelSchema.ContainsETA, "" },
                { TaskStatusAppViewModelSchema.RemainingTime, "" },
                { TaskStatusAppViewModelSchema.AverageUnitsPerSecond, "" },
                { TaskStatusAppViewModelSchema.ContainsFailures, ""},
                { TaskStatusAppViewModelSchema.FailuresCount, ""},
                { TaskStatusAppViewModelSchema.ContainsWarnings, ""},
                { TaskStatusAppViewModelSchema.WarningsCount, ""}
            };

            viewModel[TaskStatusAppViewModelSchema.Title] = taskStatus.Title;

            if (taskStatus.Progress != null)
            {
                var current = taskStatus.Progress.Current;
                var total = taskStatus.Progress.Total;

                viewModel[TaskStatusAppViewModelSchema.ContainsProgress] = "true";
                viewModel[TaskStatusAppViewModelSchema.ProgressTarget] = taskStatus.Progress.Target;
                viewModel[TaskStatusAppViewModelSchema.ProgressPercent] = string.Format("{0:P1}", (double)current / total);

                var currentFormatted = current.ToString();
                var totalFormatted = total.ToString();

                if (taskStatus.Progress.Unit == DataUnits.Bytes)
                {
                    viewModel[TaskStatusAppViewModelSchema.ContainsETA] = "true";

                    currentFormatted = bytesFormattingController.Format(current);
                    totalFormatted = bytesFormattingController.Format(total);

                    var elapsed = DateTime.UtcNow - taskStatus.Started;
                    var unitsPerSecond = current / elapsed.TotalSeconds;
                    var speed = bytesFormattingController.Format((long)unitsPerSecond);
                    var remainingTime = secondsFormattingController.Format((long)((total - current) / unitsPerSecond));

                    viewModel[TaskStatusAppViewModelSchema.RemainingTime] = remainingTime;
                    viewModel[TaskStatusAppViewModelSchema.AverageUnitsPerSecond] = speed;
                }

                viewModel[TaskStatusAppViewModelSchema.ProgressCurrent] = currentFormatted;
                viewModel[TaskStatusAppViewModelSchema.ProgressTotal] = totalFormatted;
            }

            if (taskStatus.Failures != null && taskStatus.Failures.Any())
            {
                viewModel[TaskStatusAppViewModelSchema.ContainsFailures] = "true";
                viewModel[TaskStatusAppViewModelSchema.FailuresCount] = taskStatus.Failures.Count.ToString();
            }

            if (taskStatus.Warnings != null && taskStatus.Warnings.Any())
            {
                viewModel[TaskStatusAppViewModelSchema.ContainsWarnings] = "true";
                viewModel[TaskStatusAppViewModelSchema.WarningsCount] = taskStatus.Warnings.Count.ToString();
            }

            return viewModel;
        }
    }
}