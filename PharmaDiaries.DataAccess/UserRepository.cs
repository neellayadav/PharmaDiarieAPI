using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.EMMA;

namespace PharmaDiaries.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;


        public UserRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();

        }

        public List<UserModel> GetUserList()
        {
            var result = new List<UserModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_UserList]");
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]);
            return result;
        }

        public List<UserModel> GetUserListBycomp(int compID)
        {
            var result = new List<UserModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_UserListByCompany]", compID);
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]);
            return result;
        }

        public bool Save(UserModel uModel)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UserInsert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@compID", uModel.CompID);
                        cmd.Parameters.AddWithValue("@userID", uModel.UserID);
                        cmd.Parameters.AddWithValue("@password", uModel.Password);
                        cmd.Parameters.AddWithValue("@name", uModel.Name);
                        cmd.Parameters.AddWithValue("@headquater", uModel.HeadQuater);
                        cmd.Parameters.AddWithValue("@address1", uModel.Address1);
                        cmd.Parameters.AddWithValue("@locality", uModel.Locality);
                        cmd.Parameters.AddWithValue("@cityOrTown", uModel.CityOrTown);
                        cmd.Parameters.AddWithValue("@pincode", uModel.Pincode);
                        cmd.Parameters.AddWithValue("@district", uModel.District);
                        cmd.Parameters.AddWithValue("@state", uModel.State);
                        cmd.Parameters.AddWithValue("@country", uModel.Country);
                        cmd.Parameters.AddWithValue("@mobile", uModel.Mobile);
                        cmd.Parameters.AddWithValue("@telephone", uModel.Telephone);
                        cmd.Parameters.AddWithValue("@ProfileImageURL", (object?)uModel.ProfileImageURL ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@isCompAdmin", uModel.IsCompAdmin ?? false);
                        cmd.Parameters.AddWithValue("@roleID", (object?)uModel.RoleID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@reportingManagerID", (object?)uModel.ReportingManagerID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@emailid", uModel.emailid ?? string.Empty);
                        cmd.Parameters.AddWithValue("@createdBy", uModel.CreatedBy);

                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return result;
                    }
                }

            }

            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }

        }

        public UserModel SignUp(UserModel uModel)
        {
            //var result = false;

            var result = new UserModel();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_UserBasicInsert]", uModel.CompID!, uModel.UserID!, uModel.Password!, uModel.Name ?? "", uModel.Mobile!, uModel.HeadQuater!,0);
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]).First();
            return result;

            //0,'USR00020','12345678','TESTvNAME', '9090901212', null,0

            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            //try
            //{
            //    using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            //    {
            //        using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UserBasicInsert]"))
            //        {
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Connection = con;

            //            cmd.Parameters.AddWithValue("@compID", uModel.CompID);
            //            cmd.Parameters.AddWithValue("@userID", uModel.UserID);
            //            cmd.Parameters.AddWithValue("@password", uModel.Password);
            //            cmd.Parameters.AddWithValue("@name", uModel.Name);
            //            cmd.Parameters.AddWithValue("@mobile", uModel.Mobile);
            //            cmd.Parameters.AddWithValue("@headquater", uModel.HeadQuater);
            //            //cmd.Parameters.AddWithValue("@address1", uModel.Address1);
            //            //cmd.Parameters.AddWithValue("@locality", uModel.Locality);
            //            //cmd.Parameters.AddWithValue("@cityOrTown", uModel.CityOrTown);
            //            //cmd.Parameters.AddWithValue("@pincode", uModel.Pincode);
            //            //cmd.Parameters.AddWithValue("@district", uModel.District);
            //            //cmd.Parameters.AddWithValue("@state", uModel.State);
            //            //cmd.Parameters.AddWithValue("@country", uModel.Country);

            //            //cmd.Parameters.AddWithValue("@telephone", uModel.Telephone);
            //            cmd.Parameters.AddWithValue("@createdBy", uModel.CreatedBy);

            //            con.Open();

            //            result = Convert.ToBoolean(cmd.ExecuteNonQuery());


            //            return result;
            //        }
            //    }

            //}

            //catch (Exception ex)
            //{
            //    //transaction.Commit();
            //    throw ex;
            //}

        }

        public bool Update(UserModel uModel)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UserUpdate]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@compID", uModel.CompID);
                        cmd.Parameters.AddWithValue("@UID", uModel.UID);
                        cmd.Parameters.AddWithValue("@userID", uModel.UserID);
                        cmd.Parameters.AddWithValue("@password", uModel.Password);
                        cmd.Parameters.AddWithValue("@name", uModel.Name);
                        cmd.Parameters.AddWithValue("@headquater", uModel.HeadQuater);
                        cmd.Parameters.AddWithValue("@address1", uModel.Address1);
                        cmd.Parameters.AddWithValue("@locality", uModel.Locality);
                        cmd.Parameters.AddWithValue("@cityOrTown", uModel.CityOrTown);
                        cmd.Parameters.AddWithValue("@pincode", uModel.Pincode);
                        cmd.Parameters.AddWithValue("@district", uModel.District);
                        cmd.Parameters.AddWithValue("@state", uModel.State);
                        cmd.Parameters.AddWithValue("@country", uModel.Country);
                        cmd.Parameters.AddWithValue("@mobile", uModel.Mobile);
                        cmd.Parameters.AddWithValue("@telephone", uModel.Telephone);
                        cmd.Parameters.AddWithValue("@ProfileImageURL", (object?)uModel.ProfileImageURL ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@isCompAdmin", uModel.IsCompAdmin ?? false);
                        cmd.Parameters.AddWithValue("@roleID", (object?)uModel.RoleID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@reportingManagerID", (object?)uModel.ReportingManagerID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@emailid", uModel.emailid ?? string.Empty);
                        cmd.Parameters.AddWithValue("@modifiedBy", uModel.ModifiedBy);

                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return result;
                    }
                }

            }

            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }

        }

        public List<UserModel> GetPotentialManagers(int compID, int? currentUID, int? currentRoleID = null)
        {
            var result = new List<UserModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_GetPotentialManagers]", compID, currentUID ?? (object)DBNull.Value, currentRoleID ?? (object)DBNull.Value);
            result = DataTableHelper.ConvertDataTable<UserModel>(ds.Tables[0]);
            return result;
        }

        public bool Delete(UserModel uModel)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UserDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@compID", uModel.CompID);
                        cmd.Parameters.AddWithValue("@UID", uModel.UID);
                        cmd.Parameters.AddWithValue("@modifiedBy", uModel.ModifiedBy);

                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return result;
                    }
                }

            }

            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }
        }


        public bool ResetPassword(ResetPasswordModel lumodel)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UpdateUserPassword]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@compID", lumodel.CompID);
                        cmd.Parameters.AddWithValue("@uid", lumodel.UsrId);
                        cmd.Parameters.AddWithValue("@password", lumodel.Password);
                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return result;
                    }
                }

            }

            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }
        }

        public bool DeleteUserByUserID(DeleteUserByUserIDRequest request)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_UserDeleteByUserID_Pwd]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@compid", request.CompID);
                        cmd.Parameters.AddWithValue("@UserID", request.UserID);
                        cmd.Parameters.AddWithValue("@pwd", request.Password);

                        con.Open();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
