using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetData;
using Interfaces.Delegates.Convert;

using ProtoBuf;

namespace Delegates.GetData.Storage
{
    public abstract class GetProtoBufDataAsyncDelegate<T> : IGetDataAsyncDelegate<T>
    {
        private readonly IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate;

        public GetProtoBufDataAsyncDelegate(
            IConvertDelegate<string, Stream> convertUriToReadableStreamDelegate)
        {
            this.convertUriToReadableStreamDelegate = convertUriToReadableStreamDelegate;
        }

        public async Task<T> GetDataAsync(string uri = null)
        {
            return await Task.Run(() =>
            {
                T data = default(T);

                if (System.IO.File.Exists(uri))
                {
                    using (var readableStream = convertUriToReadableStreamDelegate.Convert(uri))
                        data = Serializer.Deserialize<T>(readableStream);
                }

                return data;
            });
        }
    }
}