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
    public class FWProdDTRepository : IFWProdDTRepository
    {
        private IConfiguration configuration;
        private string PharmaDiaries_ConnectionString;
        public FWProdDTRepository(IConfiguration configuration)
        {

            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["ConnectionStrings:APIconnectionString"]!.ToString();
        }

        public List<FWProdDT4Report> GetAllFieldworkProd(string transId)
        {
            var result = new List<FWProdDT4Report>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FieldworkProdDTList]", transId);
            result = DataTableHelper.ConvertDataTable<FWProdDT4Report>(ds.Tables[0]);
            return result;
        }

        public bool DeleteProdDT(string transID, int sno, string uid)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkProdDTDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TransID", transID);
                        cmd.Parameters.AddWithValue("@SNo", sno);
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

        public bool Save(FieldworkProdDT fwProdDT)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkProdDTSave]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TransId", fwProdDT.TransID);
                        cmd.Parameters.AddWithValue("@SNo", fwProdDT.SNo);
                        cmd.Parameters.AddWithValue("@prodcode", fwProdDT.Prodcode);
                        cmd.Parameters.AddWithValue("@IsActive", fwProdDT.IsActive);


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
