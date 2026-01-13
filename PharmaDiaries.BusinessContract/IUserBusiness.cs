using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
    public interface IUserBusiness
    {
        public List<UserModel> GetUserList();

        public List<UserModel> GetUserListBycomp(int compID);

        public bool Save(UserModel uModel);

        //public UserModel SaveBasic(UserModel uModel);

        public bool Update(UserModel uModel);

        public bool Delete(UserModel uModel);

        public bool ResetPassword(ResetPasswordModel lUModel);

        public List<UserModel> GetPotentialManagers(int compID, int? currentUID, int? currentRoleID = null);

        public bool DeleteUserByUserID(DeleteUserByUserIDRequest request);

    }
}

