﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GOG.Model
{
    [DataContract]
    public class GameDetails
    {
        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage { get; set; }
        [DataMember(Name = "cdKey")]
        public string CDKey { get; set; }
        [DataMember(Name = "changelog")]
        public string Changelog { get; set; }
        [DataMember(Name = "combinedExtrasDownloaderUrl")]
        public string CombinedExtrasDownloaderUrl { get; set; }
        [DataMember(Name = "dlcs")]
        public List<GameDetails> DLCs { get; set; }
        [DataMember(Name = "downloads")]
        public LanguageDownloads Downloads { get; set; }
        [DataMember(Name = "extras")]
        public List<DownloadEntry> Extras { get; set; }
        [DataMember(Name = "forumLink")]
        public string ForumLink { get; set; }
        [DataMember(Name = "isPreOrder")]
        public bool IsPreOrder { get; set; }
        [DataMember(Name = "messages")]
        public string Messages { get; set; }
        [DataMember(Name = "releaseTimestamp")]
        public long ReleaseTimestamp { get; set; }
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }
        [DataMember(Name = "textInformation")]
        public string TextInformation { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; } 
    }
}
