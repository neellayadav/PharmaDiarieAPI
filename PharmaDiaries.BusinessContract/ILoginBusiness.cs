using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
	public interface ILoginBusiness
	{
		public UserModel Validate(LoginUserModel lumodel);

        public UserModel SignUp(SignUpModel uModel);

    }
}

