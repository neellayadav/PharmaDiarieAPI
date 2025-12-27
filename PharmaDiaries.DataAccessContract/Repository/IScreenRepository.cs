using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract.Repository
{
    public interface IScreenRepository
    {
        public bool SyncScreens(List<ScreenModel> screens);

        public List<ScreenModel> GetActiveScreens();

        public bool SaveUserScreenPermissions(UserScreenPermissionRequest request);

        public List<UserScreenPermissionModel> GetUserScreenPermissions(int userId);
    }
}
