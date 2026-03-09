using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccessContract
{
    public interface IAppRepository
    {
        AppVersionConfig? GetVersionConfig(string platform);
    }
}
