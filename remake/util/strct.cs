using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;
using Newtonsoft.Json;
namespace TestMod.remake.util
{   
    public struct avatar_data
    {
        public string asseturl;
        public int polys;
    }
    public partial class avi
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonProperty("assetUrl")]
        public string AssetUrl { get; set; }

        [JsonProperty("assetUrlObject")]
        public UrlObject AssetUrlObject { get; set; }

        [JsonProperty("tags")]
        public object[] Tags { get; set; }

        [JsonProperty("releaseStatus")]
        public string ReleaseStatus { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("unityPackageUrl")]
        public string UnityPackageUrl { get; set; }

        [JsonProperty("unityPackageUrlObject")]
        public UrlObject UnityPackageUrlObject { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }

        [JsonProperty("assetVersion")]
        public long AssetVersion { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("featured")]
        public bool Featured { get; set; }

        [JsonProperty("imported")]
        public bool Imported { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("unityPackages")]
        public UnityPackage[] UnityPackages { get; set; }
    }

    public partial class UrlObject
    {
    }

    public partial class UnityPackage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("assetUrl")]
        public string AssetUrl { get; set; }

        [JsonProperty("assetUrlObject")]
        public UrlObject AssetUrlObject { get; set; }

        [JsonProperty("unityVersion")]
        public string UnityVersion { get; set; }

        [JsonProperty("unitySortNumber")]
        public long UnitySortNumber { get; set; }

        [JsonProperty("assetVersion")]
        public long AssetVersion { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
