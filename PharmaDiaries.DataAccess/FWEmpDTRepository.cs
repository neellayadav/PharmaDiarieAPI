using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;

namespace PharmaDiaries.DataAccess
{
    public class FWEmpDTRepository : IFWEmpDTRepository
    {
        private IConfiguration configuration;
        private string PharmaDiaries_ConnectionString;
        public FWEmpDTRepository(IConfiguration configuration)
        {

            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["ConnectionStrings:APIconnectionString"]!.ToString();
        }

        public List<FWEmpDT4Report> GetAllFieldworkEmp(string transId)
        {
            var result= new List<FWEmpDT4Report>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FieldworkEmpDTList]", transId);
            result =DataTableHelper.ConvertDataTable<FWEmpDT4Report>(ds.Tables[0]);
            return result;
        }

        public bool DeleteEmpDT(string transID, int callno, string uid)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkEmpDTDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TransID", transID);
                        cmd.Parameters.AddWithValue("@SNo", callno);
                        cmd.Parameters.AddWithValue("@UID", uid);

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

        public bool Save(FieldworkEmpDT fwEmpDT)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkEmpDTSave]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TransId", fwEmpDT.TransID);
                        cmd.Parameters.AddWithValue("@SNo", fwEmpDT.SNo);
                        cmd.Parameters.AddWithValue("@UID", fwEmpDT.UID);
                        cmd.Parameters.AddWithValue("@IsActive", fwEmpDT.IsActive);


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
