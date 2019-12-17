﻿using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Respond;

using Controllers.Instances;
using Delegates.Convert.Requests;

namespace vangogh.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var singletonInstancesController = new SingletonInstancesController();

            var applicationStatus = singletonInstancesController.GetInstance(
                typeof(Models.Status.Status))
                as Models.Status.Status;

            var convertArgsToRequestsDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertArgsToRequestsDelegate))
                as ConvertArgsToRequestsDelegate;

            var convertRequestToRespondDelegateTypeDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertRequestToRespondDelegateTypeDelegate))
                as ConvertRequestToRespondDelegateTypeDelegate;

            await foreach (var request in convertArgsToRequestsDelegate.ConvertAsync(args, applicationStatus))
            {
                var respondToRequestDelegateType = convertRequestToRespondDelegateTypeDelegate.Convert(request);

                if (respondToRequestDelegateType == null)
                    throw new System.InvalidOperationException(
                        $"No respond delegate registered for request: {request.Method} {request.Collection}");

                var respondToRequestDelegate = singletonInstancesController.GetInstance(
                    respondToRequestDelegateType)
                    as IRespondAsyncDelegate;

                await respondToRequestDelegate.RespondAsync(request.Parameters);
            }

            // TODO: Implement request handlers (will be done in the PR after implementationDependencies-prototype)

            System.Console.WriteLine("Press ENTER to exit...");
            System.Console.ReadLine();
        }
    }
}