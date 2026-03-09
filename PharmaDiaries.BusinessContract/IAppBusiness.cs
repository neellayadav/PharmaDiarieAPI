using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
    public interface IAppBusiness
    {
        AppVersionCheckResponse? CheckVersion(string platform, string currentVersion);
    }
}
