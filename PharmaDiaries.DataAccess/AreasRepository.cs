using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using System.Collections.Generic;
using PharmaDiaries.Models;
using System.Data.SqlClient;

namespace PharmaDiaries.DataAccess
{
	public class AreasRepository : IAreasRepository
	{

        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;


        public AreasRepository(IConfiguration configuration)
		{
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();

        }

        public List<AreasModel> AreaList(int compId)
        {
            var result = new List<AreasModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_AreasList]", compId);
            result = DataTableHelper.ConvertDataTable<AreasModel>(ds.Tables[0]);
            return result;
        }

        public List<HeadQuaterModel> HeadQuarterListByRegion(int compId, string region)
        {
            var result = new List<HeadQuaterModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_HeadQuaterListByRegion]", compId, region);
            result = DataTableHelper.ConvertDataTable<HeadQuaterModel>(ds.Tables[0]);
            return result;
        }

        public List<PatchModel> PatchListByHeadQuater(int compId, string hQuater)
        {
            var result = new List<PatchModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_PatchListByHeadQuater]", compId, hQuater);
            result = DataTableHelper.ConvertDataTable<PatchModel>(ds.Tables[0]);
            return result;
        }

        public List<RegionModel> RegionList(int compId)
        {
            var result = new List<RegionModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_RegionList]", compId);
            result = DataTableHelper.ConvertDataTable<RegionModel>(ds.Tables[0]);
            return result;
        }

        public List<HeadQuaterModel> HeadQuaterList(int compId)
        {
            var result = new List<HeadQuaterModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_HeadQuaterList]", compId);
            result = DataTableHelper.ConvertDataTable<HeadQuaterModel>(ds.Tables[0]);
            return result;
        }

        public bool Save(AreasModel areaModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcMaster].[usp_Areas_Insert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Region", areaModel.Region);
                        cmd.Parameters.AddWithValue("@HeadQuater", areaModel.HeadQuater);
                        cmd.Parameters.AddWithValue("@Patch", areaModel.Patch);
                        cmd.Parameters.AddWithValue("@IsActive", areaModel.IsActive ?? true);
                        cmd.Parameters.AddWithValue("@CreatedBy", areaModel.CreatedBy);
                        cmd.Parameters.AddWithValue("@CompID", areaModel.CompID);

                        // OUTPUT parameter for AreaID
                        SqlParameter outputParam = new SqlParameter("@AreaID", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Get the output AreaID
                        if (outputParam.Value != DBNull.Value)
                        {
                            areaModel.AreaID = Convert.ToInt32(outputParam.Value);
                        }

                        result = rowsAffected > 0 || areaModel.AreaID > 0;

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool Update(AreasModel areaModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcMaster].[usp_Areas_Update]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AreaId", areaModel.AreaID);
                        cmd.Parameters.AddWithValue("@Region", areaModel.Region);
                        cmd.Parameters.AddWithValue("@HeadQuater", areaModel.HeadQuater);
                        cmd.Parameters.AddWithValue("@Patch", areaModel.Patch);
                        cmd.Parameters.AddWithValue("@ModifiedBy", areaModel.ModifiedBy);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool Delete(int areaId, int compId, int modifiedBy)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcMaster].[usp_Areas_Delete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AreaID", areaId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

    }
}

