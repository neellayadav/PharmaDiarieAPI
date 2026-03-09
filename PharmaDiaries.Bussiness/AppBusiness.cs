using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class AppBusiness : IAppBusiness
    {
        private IAppRepository _repository;

        public AppBusiness(IAppRepository repository)
        {
            _repository = repository;
        }

        public AppVersionCheckResponse? CheckVersion(string platform, string currentVersion)
        {
            var config = _repository.GetVersionConfig(platform);
            if (config == null)
                return null;

            bool needsUpdate = CompareVersions(currentVersion, config.MinVersion!) < 0;

            return new AppVersionCheckResponse
            {
                MinVersion = config.MinVersion,
                LatestVersion = config.LatestVersion,
                ForceUpdate = config.ForceUpdate,
                StoreURL = config.StoreURL,
                UpdateMessage = config.UpdateMessage,
                NeedsUpdate = needsUpdate
            };
        }

        /// <summary>
        /// Compares two version strings (e.g. "1.0.1" vs "1.0.2").
        /// Returns negative if v1 &lt; v2, zero if equal, positive if v1 &gt; v2.
        /// </summary>
        private static int CompareVersions(string v1, string v2)
        {
            var parts1 = v1.Split('.').Select(int.Parse).ToArray();
            var parts2 = v2.Split('.').Select(int.Parse).ToArray();

            int maxLen = Math.Max(parts1.Length, parts2.Length);
            for (int i = 0; i < maxLen; i++)
            {
                int p1 = i < parts1.Length ? parts1[i] : 0;
                int p2 = i < parts2.Length ? parts2[i] : 0;
                if (p1 != p2) return p1.CompareTo(p2);
            }
            return 0;
        }
    }
}
