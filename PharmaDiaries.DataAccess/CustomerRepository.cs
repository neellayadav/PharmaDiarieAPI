using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Xml.Linq;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
	public class CustomerRepository : ICustomerRepository
	{
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;


        public CustomerRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();

        }

        public List<CustomerModel> CustomerListByCompType(int compid, String custType)
        {
            var result = new List<CustomerModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_CustomerListByComp_Type]", compid, custType);
            result = DataTableHelper.ConvertDataTable<CustomerModel>(ds.Tables[0]);
            return result;
        }

        public bool Delete(DeleteCustomerModel delCustMod)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_CustomerDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", delCustMod.CompID);
                        cmd.Parameters.AddWithValue("@custid", delCustMod.CustID);
                        cmd.Parameters.AddWithValue("@ModifiedBy", delCustMod.ModifiedBy);


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

        public bool Save(CustomerModel custModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_CustomerInsert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", custModel.CompID);
                        cmd.Parameters.AddWithValue("@Name", custModel.Name);
                        cmd.Parameters.AddWithValue("@type", custModel.Type);
                        cmd.Parameters.AddWithValue("@IsListed", custModel.IsListed);
                        cmd.Parameters.AddWithValue("@QUALIFICATION", custModel.QUALIFICATION);
	                    cmd.Parameters.AddWithValue("@Speciality", custModel.Speciality);
	                    cmd.Parameters.AddWithValue("@HeadQuater", custModel.HeadQuater);
	                    cmd.Parameters.AddWithValue("@PATCH", custModel.Patch);
	                    cmd.Parameters.AddWithValue("@Address1", custModel.Address1);
	                    cmd.Parameters.AddWithValue("@Locality", custModel.Locality);
	                    cmd.Parameters.AddWithValue("@CityOrTown", custModel.CityOrTown);
	                    cmd.Parameters.AddWithValue("@Pincode", custModel.Pincode);
	                    cmd.Parameters.AddWithValue("@District", custModel.District);
	                    cmd.Parameters.AddWithValue("@State", custModel.State);
	                    cmd.Parameters.AddWithValue("@Country", custModel.Country);
	                    cmd.Parameters.AddWithValue("@Mobile", custModel.Mobile);
	                    cmd.Parameters.AddWithValue("@Telephone", custModel.Telephone);
	                    cmd.Parameters.AddWithValue("@CreatedBy", custModel.CreatedBy);


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

        public bool Update(CustomerModel custModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_CustomerUpdate]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", custModel.CompID);
                        cmd.Parameters.AddWithValue("@custid", custModel.CustID);
                        cmd.Parameters.AddWithValue("@Name", custModel.Name);
                        cmd.Parameters.AddWithValue("@type", custModel.Type);
                        cmd.Parameters.AddWithValue("@IsListed", custModel.IsListed);
                        cmd.Parameters.AddWithValue("@QUALIFICATION", custModel.QUALIFICATION);
                        cmd.Parameters.AddWithValue("@Speciality", custModel.Speciality);
                        cmd.Parameters.AddWithValue("@HeadQuater", custModel.HeadQuater);
                        cmd.Parameters.AddWithValue("@PATCH", custModel.Patch);
                        cmd.Parameters.AddWithValue("@Address1", custModel.Address1);
                        cmd.Parameters.AddWithValue("@Locality", custModel.Locality);
                        cmd.Parameters.AddWithValue("@CityOrTown", custModel.CityOrTown);
                        cmd.Parameters.AddWithValue("@Pincode", custModel.Pincode);
                        cmd.Parameters.AddWithValue("@District", custModel.District);
                        cmd.Parameters.AddWithValue("@State", custModel.State);
                        cmd.Parameters.AddWithValue("@Country", custModel.Country);
                        cmd.Parameters.AddWithValue("@Mobile", custModel.Mobile);
                        cmd.Parameters.AddWithValue("@Telephone", custModel.Telephone);
                        cmd.Parameters.AddWithValue("@ModifiedBy", custModel.ModifiedBy);


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

