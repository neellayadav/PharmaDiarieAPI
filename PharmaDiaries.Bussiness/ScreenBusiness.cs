using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.Bussiness
{
    public class ScreenBusiness : IScreenBusiness

    {
        private IScreenRepository _repository;

        public ScreenBusiness(IScreenRepository repository)
        {
            _repository = repository;
        }

        bool IScreenBusiness.SyncScreens(List<ScreenModel> screens)
        {
            return _repository.SyncScreens(screens);
        }

        List<ScreenModel> IScreenBusiness.GetActiveScreens()
        {
            return _repository.GetActiveScreens();
        }

        bool IScreenBusiness.SaveUserScreenPermissions(UserScreenPermissionRequest request)
        {
            return _repository.SaveUserScreenPermissions(request);
        }

        List<UserScreenPermissionModel> IScreenBusiness.GetUserScreenPermissions(int userId)
        {
            return _repository.GetUserScreenPermissions(userId);
        }
    }

}
