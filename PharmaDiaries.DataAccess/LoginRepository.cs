using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;
using Microsoft.Extensions.Configuration;

namespace PharmaDiaries.DataAccess
{
    public class LoginRepository : ILoginRepository
    {
        private IConfiguration configuration;
        private string PharmaDiaries_ConnectionString;
        public LoginRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        public UserModel Validate(LoginUserModel lumodel)
        {
            var result = new UserModel();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_UserLogin]", lumodel.CompID!, lumodel.UserID!, lumodel.Password!);
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]).First();
            return result;
        }


        public UserModel SignUp(UserModel uModel)
        {
            //var result = false;

            var result = new UserModel();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_UserSignUp]", uModel.CompID!, uModel.UserID!, uModel.Password!, uModel.Name ?? "", uModel.Mobile!, uModel.HeadQuater!, 0, true);
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]).First();
            return result;

            //0,'USR00020','12345678','TESTvNAME', '9090901212', null,0,1
        }

    }
}
