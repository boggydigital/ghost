﻿using System.IO;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Values;
using Attributes;

namespace Delegates.Format.Uri
{
    public class FormatValidationFileDelegate : IFormatDelegate<string, string>
    {
        private readonly IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate;

        [Dependencies(
            typeof(GetPath.Json.GetValidationPathDelegate))]
        public FormatValidationFileDelegate(IGetValueDelegate<string,(string Directory,string Filename)> getPathDelegate)
        {
            this.getPathDelegate = getPathDelegate;
        }

        public string Format(string uri)
        {
            return getPathDelegate.GetValue((
                string.Empty,
                Path.GetFileName(uri)));
        }
    }
}