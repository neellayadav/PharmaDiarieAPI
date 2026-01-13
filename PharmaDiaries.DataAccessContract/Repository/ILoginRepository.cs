using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract.Repository
{
	public interface ILoginRepository
	{
        public UserModel Validate(LoginUserModel lumodel);

        public UserModel SignUp(SignUpModel uModel);

    }
}

