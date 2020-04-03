﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Interfaces.Delegates.Confirm;
using Interfaces.Delegates.Replace;
using Interfaces.Delegates.Format;
using Interfaces.Delegates.Convert;
using Interfaces.Delegates.Itemize;
using Interfaces.Delegates.Map;
using Interfaces.Delegates.GetData;

using GOG.Interfaces.Delegates.GetDeserialized;

using Attributes;

using Models.Dependencies;
using GOG.Models;

namespace GOG.Delegates.GetDeserialized.ProductTypes
{
    // TODO: Refactor?
    public class GetDeserializedGameDetailsAsyncDelegate : IGetDeserializedAsyncDelegate<GameDetails>
    {
        private readonly IGetDataAsyncDelegate<string> getUriDataAsyncDelegate;
        private readonly IConvertDelegate<(string, IDictionary<string,string>), string> convertUriParametersToUriDelegate;
        private readonly IConvertDelegate<string,GameDetails> convertJSONToGameDetailsDelegate;
        private readonly IConvertDelegate<string, OperatingSystemsDownloads[][]> convertJSONToOperatingSystemsDownloads2DArrayDelegate;
        readonly IConvertDelegate<string,string> convertLanguageToCodeDelegate;
        readonly IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
        readonly IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate;
        readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate;
        readonly IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate;
        readonly IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate;
        readonly IConvertDelegate<
            OperatingSystemsDownloads[][],
            OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate;
        readonly IMapDelegate<string> mapStringDelegate;

        [Dependencies(
            "Delegates.Convert.Network.ConvertUriDictionaryParametersToUriDelegate,Delegates",
            "GOG.Delegates.GetData.Network.GetUriDataRateLimitedAsyncDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToGameDetailsDelegate,GOG.Delegates",
            "GOG.Delegates.Convert.JSON.ProductTypes.ConvertJSONToOperatingSystemsDownloads2DArrayDelegate,GOG.Delegates",
            "Delegates.Convert.ConvertLanguageToCodeDelegate,Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertGameDetailsDownloadLanguagesToEmptyStringDelegate,GOG.Delegates",
            "GOG.Delegates.Confirm.ProductTypes.ConfirmGameDetailsContainsLanguageDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadLanguagesDelegate,GOG.Delegates",
            "GOG.Delegates.Itemize.ProductTypes.ItemizeGameDetailsDownloadsDelegate,GOG.Delegates",
            "Delegates.Replace.ReplaceMultipleStringsDelegate,Delegates",
            "GOG.Delegates.Convert.ProductTypes.ConvertOperatingSystemsDownloads2DArrayToArrayDelegate,GOG.Delegates",
            "Delegates.Map.System.MapStringDelegate,Delegates")]
        public GetDeserializedGameDetailsAsyncDelegate(
            IConvertDelegate<(string, IDictionary<string, string>), string> convertUriParametersToUriDelegate,            
            IGetDataAsyncDelegate<string> getUriDataAsyncDelegate,
            IConvertDelegate<string,GameDetails> convertJSONToGameDetailsDelegate,
            IConvertDelegate<string, OperatingSystemsDownloads[][]> convertJSONToOperatingSystemsDownloads2DArrayDelegate,
            IConvertDelegate<string,string> convertLanguageToCodeDelegate,
            IConvertDelegate<string, string> convertGameDetailsDownloadLanguagesToEmptyStringDelegate,
            IConfirmDelegate<string> confirmStringContainsLanguageDownloadsDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadLanguagesDelegate,
            IItemizeDelegate<string, string> itemizeGameDetailsDownloadsDelegate,
            IReplaceMultipleDelegate<string> replaceMultipleStringsDelegate,
            IConvertDelegate<
                OperatingSystemsDownloads[][], 
                OperatingSystemsDownloads[]> convert2DArrayToArrayDelegate,
            IMapDelegate<string> mapStringDelegate)
        {
            this.convertUriParametersToUriDelegate = convertUriParametersToUriDelegate;
            this.getUriDataAsyncDelegate = getUriDataAsyncDelegate;
            this.convertJSONToGameDetailsDelegate = convertJSONToGameDetailsDelegate;
            this.convertJSONToOperatingSystemsDownloads2DArrayDelegate = convertJSONToOperatingSystemsDownloads2DArrayDelegate;
            this.convertLanguageToCodeDelegate = convertLanguageToCodeDelegate;
            this.convertGameDetailsDownloadLanguagesToEmptyStringDelegate = convertGameDetailsDownloadLanguagesToEmptyStringDelegate;
            this.confirmStringContainsLanguageDownloadsDelegate = confirmStringContainsLanguageDownloadsDelegate;
            this.itemizeGameDetailsDownloadLanguagesDelegate = itemizeGameDetailsDownloadLanguagesDelegate;
            this.itemizeGameDetailsDownloadsDelegate = itemizeGameDetailsDownloadsDelegate;
            this.replaceMultipleStringsDelegate = replaceMultipleStringsDelegate;
            this.convert2DArrayToArrayDelegate = convert2DArrayToArrayDelegate;
            this.mapStringDelegate = mapStringDelegate;
        }

        public async Task<GameDetails> GetDeserializedAsync(string uri, IDictionary<string, string> parameters = null)
        {
            // GOG.com quirk
            // GameDetails as sent by GOG.com servers have an intersting data structure for downloads:
            // it's represented as an array, where first element is a string with the language,
            // followed by actual download details, something like:
            // [ "English", { // download1 }, { // download2 } ]
            // Which of course is not a problem for JavaScript, but is a problem for
            // deserializing into strongly typed C# data structures. 
            // To work around this we wrapped encapsulated usual network requests,
            // data transformation and desearialization in a sinlge package. 
            // To process downloads we do the following:
            // - if response contains language downloads, signified by [[
            // - extract actual language information and remove it from the string
            // - deserialize downloads into OperatingSystemsDownloads collection
            // - assign languages, since we know we should have as many downloads array as languages

            var uriParameters = convertUriParametersToUriDelegate.Convert((uri, parameters));
            var data = await getUriDataAsyncDelegate.GetDataAsync(uriParameters);
            var gameDetails = convertJSONToGameDetailsDelegate.Convert(data);

            if (gameDetails == null) return null;

            var gameDetailsLanguageDownloads = new List<OperatingSystemsDownloads>();

            if (!confirmStringContainsLanguageDownloadsDelegate.Confirm(data)) return gameDetails;
            var downloadStrings = itemizeGameDetailsDownloadsDelegate.Itemize(data);

            foreach (var downloadString in downloadStrings)
            {
                var downloadLanguages = itemizeGameDetailsDownloadLanguagesDelegate.Itemize(downloadString);
                if (downloadLanguages == null)
                    throw new InvalidOperationException("Cannot find any download languages or download language format changed.");

                // ... and remove download lanugage strings from downloads
                var downloadsStringSansLanguages = replaceMultipleStringsDelegate.ReplaceMultiple(
                    downloadString,
                    string.Empty,
                    downloadLanguages.ToArray());

                // now it should be safe to deserialize langugage downloads
                var downloads =
                    convertJSONToOperatingSystemsDownloads2DArrayDelegate.Convert(
                    downloadsStringSansLanguages);

                // and convert GOG two-dimensional array of downloads to single-dimensional array
                var languageDownloads = convert2DArrayToArrayDelegate.Convert(downloads);

                if (languageDownloads.Count() != downloadLanguages.Count())
                    throw new InvalidOperationException("Number of extracted language downloads doesn't match number of languages.");

                // map language downloads with the language code we extracted earlier
                var languageDownloadIndex = 0;

                mapStringDelegate.Map(downloadLanguages, language =>
                {
                    var formattedLanguage = convertGameDetailsDownloadLanguagesToEmptyStringDelegate.Convert(language);
                    var languageCode = convertLanguageToCodeDelegate.Convert(formattedLanguage);

                    languageDownloads[languageDownloadIndex++].Language = languageCode;
                });

                gameDetailsLanguageDownloads.AddRange(languageDownloads);
            }

            gameDetails.LanguageDownloads = gameDetailsLanguageDownloads.ToArray();

            return gameDetails;
        }
    }
}
