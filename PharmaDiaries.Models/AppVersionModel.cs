using System;

namespace PharmaDiaries.Models
{
    public class AppVersionConfig
    {
        public string? MinVersion { get; set; }
        public string? LatestVersion { get; set; }
        public bool ForceUpdate { get; set; }
        public string? StoreURL { get; set; }
        public string? UpdateMessage { get; set; }
    }

    public class AppVersionCheckResponse
    {
        public string? MinVersion { get; set; }
        public string? LatestVersion { get; set; }
        public bool ForceUpdate { get; set; }
        public string? StoreURL { get; set; }
        public string? UpdateMessage { get; set; }
        public bool NeedsUpdate { get; set; }
    }
}
