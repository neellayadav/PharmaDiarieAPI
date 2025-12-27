using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PharmaDiaries.DataAccess
{
    public class ScreenRepository : IScreenRepository
    {
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;

        public ScreenRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        public bool SyncScreens(List<ScreenModel> screens)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    con.Open();
                    foreach (var screen in screens)
                    {
                        using (SqlCommand cmd = new SqlCommand("[McMaster].[usp_ScreenSync]"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;

                            cmd.Parameters.AddWithValue("@ScreenName", screen.ScreenName);
                            cmd.Parameters.AddWithValue("@ScreenRoute", screen.ScreenRoute);
                            cmd.Parameters.AddWithValue("@ScreenDescription", screen.ScreenDescription ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@IsActive", screen.IsActive ?? true);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<ScreenModel> GetActiveScreens()
        {
            var result = new List<ScreenModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[McMaster].[usp_GetActiveScreens]");
            result = DataTableHelper.ConvertDataTable<ScreenModel>(ds.Tables[0]);
            return result;
        }

        public bool SaveUserScreenPermissions(UserScreenPermissionRequest request)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[McMaster].[usp_SaveUserScreenPermissions]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@UserID", request.UserID);
                        cmd.Parameters.AddWithValue("@ScreenIDs", string.Join(",", request.ScreenIDs ?? new List<int>()));
                        cmd.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<UserScreenPermissionModel> GetUserScreenPermissions(int userId)
        {
            var result = new List<UserScreenPermissionModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[McMaster].[usp_GetUserScreenPermissions]", userId);
            result = DataTableHelper.ConvertDataTable<UserScreenPermissionModel>(ds.Tables[0]);
            return result;
        }
    }
}
