﻿using System.Security.Cryptography;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Status;

using Attributes;

namespace Delegates.Convert.Hashes
{
    public class ConvertBytesToMd5HashDelegate: IConvertAsyncDelegate<byte[], Task<string>>
    {
        readonly MD5 md5;
        readonly IConvertDelegate<byte[], string> byteToStringConversionController;

        [Dependencies("Delegates.Convert.Bytes.ConvertBytesToStringDelegate,Delegates")]
        public ConvertBytesToMd5HashDelegate(
            IConvertDelegate<byte[], string> byteToStringConversionController)
        {
            md5 = MD5.Create();
            this.byteToStringConversionController = byteToStringConversionController;
        }

        public async Task<string> ConvertAsync(byte[] data, IStatus status)
        {
            return await Task.Run(() => {
                if (data == null)
                    return string.Empty;

                var hashData = md5.ComputeHash(data);
                return byteToStringConversionController.Convert(hashData);    
            });
        }
    }
}