using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.Bussiness
{
    public class UserBusiness : IUserBusiness

    {
        private IUserRepository _repository;

        public UserBusiness(IUserRepository repository)
        {
            _repository = repository;
        }

        List<UserModel> IUserBusiness.GetUserList()
        {
            return _repository.GetUserList();
        }

        List<UserModel> IUserBusiness.GetUserListBycomp(int compID)
        {
            return _repository.GetUserListBycomp(compID);
        }

        bool IUserBusiness.Save(UserModel uModel)
        {
            return _repository.Save(uModel);
        }

        //public UserModel SignUp(UserModel uModel)
        //{
        //    return _repository.SignUp(uModel);
        //}

        bool IUserBusiness.Update(UserModel uModel)
        {
            return _repository.Update(uModel);
        }

        bool IUserBusiness.Delete(UserModel uModel)
        {
            return _repository.Delete(uModel);
        }

        bool IUserBusiness.ResetPassword(PharmaDiaries.Models.ResetPasswordModel lUModel)
        {
            return _repository.ResetPassword(lUModel);
        }

    }

}

