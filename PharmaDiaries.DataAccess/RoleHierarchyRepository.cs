using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
    public class RoleHierarchyRepository : IRoleHierarchyRepository
    {
        private IConfiguration _configuration;
        private string _connectionString;

        public RoleHierarchyRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        public List<DistinctRoleModel> GetDistinctRoles()
        {
            var result = new List<DistinctRoleModel>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcMaster].[usp_GetDistinctRoles]");
                result = DataTableHelper.ConvertDataTable<DistinctRoleModel>(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<RoleHierarchyModel> GetRoleHierarchyList()
        {
            var result = new List<RoleHierarchyModel>();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcMaster].[usp_RoleHierarchyList]");
                result = DataTableHelper.ConvertDataTable<RoleHierarchyModel>(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SaveRoleHierarchy(RoleHierarchySaveRequest request)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcMaster].[usp_RoleHierarchySave]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RoleName", request.RoleName);
                        cmd.Parameters.AddWithValue("@RankLevel", request.RankLevel);
                        cmd.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveRoleHierarchyBatch(RoleHierarchyBatchSaveRequest request)
        {
            try
            {
                // Convert role rankings to JSON for the stored procedure
                string roleRankingsJson = JsonSerializer.Serialize(request.RoleRankings);

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcMaster].[usp_RoleHierarchySaveBatch]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RoleRankings", roleRankingsJson);
                        cmd.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
