using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class LoginBusiness : ILoginBusiness

    {
        private ILoginRepository _repository;

        public LoginBusiness(ILoginRepository repository)
        {
            _repository = repository;
        }

        UserModel ILoginBusiness.Validate(LoginUserModel lumodel)
        {
            return _repository.Validate(lumodel);
        }

        UserModel ILoginBusiness.SignUp(UserModel uModel)
        {
            return _repository.SignUp(uModel);
        }
    }

}

