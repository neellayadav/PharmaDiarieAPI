using System;
using System.Data;
using System.Data.SqlClient;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
	public class LookupRepository : ILookupRepository
	{

        private IConfiguration configuration;
        private string PharmaDiaries_ConnectionString;

        public LookupRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        public List<Lookup> Lookuplist()
        {
            var result = new List<Lookup>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_LookupList]");
            result = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]);
            return result;

        }

        public List<Lookup> LookupListByType(String lookupType)
        {
            var result = new List<Lookup>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_LookupListByType]", lookupType);
            result = DataTableHelper.ConvertDataTable<Lookup>(ds.Tables[0]);
            return result;
        }

        public bool DeleteLookUp(int compID, string code)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_LookupDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", compID);
                        cmd.Parameters.AddWithValue("@code", code);


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

