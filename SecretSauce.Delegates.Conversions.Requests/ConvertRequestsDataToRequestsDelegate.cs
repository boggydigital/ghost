using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Models.ArgsDefinitions;
using Models.Requests;
using SecretSauce.Delegates.Collections.ArgsDefinitions;
using SecretSauce.Delegates.Collections.Interfaces;
using SecretSauce.Delegates.Conversions.Interfaces;
using SecretSauce.Delegates.Data.Interfaces;

namespace SecretSauce.Delegates.Conversions.Requests
{
    public class ConvertRequestsDataToRequestsDelegate :
        IConvertAsyncDelegate<RequestsData, IAsyncEnumerable<Request>>
    {
        private IFindDelegate<Method> findMethodDelegate;
        private IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate;
        private IIntersectDelegate<string> intersectStringDelegate;

        [Dependencies(
            typeof(Data.Storage.ArgsDefinitions.GetStoredArgsDefinitionsDataAsyncDelegate),
            typeof(FindMethodDelegate),
            typeof(Delegates.Collections.System.IntersectStringDelegate))]
        public ConvertRequestsDataToRequestsDelegate(
            IGetDataAsyncDelegate<ArgsDefinition, string> getArgsDefinitionsDataFromPathAsyncDelegate,
            IFindDelegate<Method> findMethodDelegate,
            IIntersectDelegate<string> intersectStringDelegate)
        {
            this.getArgsDefinitionsDataFromPathAsyncDelegate = getArgsDefinitionsDataFromPathAsyncDelegate;
            this.findMethodDelegate = findMethodDelegate;
            this.intersectStringDelegate = intersectStringDelegate;
        }

        public async IAsyncEnumerable<Request> ConvertAsync(RequestsData requestsData)
        {
            var requests = new List<Request>();
            var argsDefinitions =
                await getArgsDefinitionsDataFromPathAsyncDelegate.GetDataAsync(string.Empty);

            foreach (var method in requestsData.Methods)
            {
                var methodDefinition = findMethodDelegate.Find(
                    argsDefinitions.Methods,
                    m => m.Title == method);

                if (methodDefinition == null)
                    throw new ArgumentException();

                var methodCollections = intersectStringDelegate.Intersect(
                    requestsData.Collections,
                    methodDefinition.Collections);

                // parameters
                var methodParameters = new Dictionary<string, IEnumerable<string>>();
                foreach (var parameter in requestsData.Parameters.Keys)
                    if (methodDefinition.Parameters.Contains(parameter))
                        methodParameters.Add(parameter, requestsData.Parameters[parameter]);

                // add method without collections if method definition supports that
                if (methodDefinition.Collections == null ||
                    methodDefinition.Collections.Length == 0)
                {
                    var request = new Request
                    {
                        Method = method,
                        Collection = string.Empty,
                        Parameters = methodParameters
                    };
                    yield return request;
                }

                foreach (var collection in methodCollections)
                {
                    var request = new Request
                    {
                        Method = method,
                        Collection = collection,
                        Parameters = methodParameters
                    };
                    yield return request;
                }
            }
        }
    }
}