using System;
using Logicon.Kaidu.Platform.Helpers;
using System.Data;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PharmaDiaries.DataAccess
{
	public class ProductRepository : IProductRepository
	{
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;


        public ProductRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();

        }

        public List<ProductModel> GetProductList(int compid)
        {
            var result = new List<ProductModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_ProductList]", compid);
            result = DataTableHelper.ConvertDataTable<ProductModel>(ds.Tables[0]);
            return result;
        }

        public bool Save(ProductModel prodModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_ProductInsert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", prodModel.CompID);
                        cmd.Parameters.AddWithValue("@desc", prodModel.proddesc);
                        cmd.Parameters.AddWithValue("@type", prodModel.prodtype);
                        cmd.Parameters.AddWithValue("@pack", prodModel.prodpack);
                        cmd.Parameters.AddWithValue("@price", prodModel.prodprice);
                        cmd.Parameters.AddWithValue("@mrp", prodModel.MRP ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@imageurl", prodModel.ImageURL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", prodModel.CreatedBy);

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

        public bool Update(ProductModel prodModel)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_ProductUpdate]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", prodModel.CompID);
                        cmd.Parameters.AddWithValue("@code", prodModel.prodcode);
                        cmd.Parameters.AddWithValue("@desc", prodModel.proddesc);
                        cmd.Parameters.AddWithValue("@type", prodModel.prodtype);
                        cmd.Parameters.AddWithValue("@pack", prodModel.prodpack);
                        cmd.Parameters.AddWithValue("@price", prodModel.prodprice);
                        cmd.Parameters.AddWithValue("@mrp", prodModel.MRP ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@imageurl", prodModel.ImageURL ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", prodModel.ModifiedBy);

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

        public bool Delete(ProductModel prodModel) // (int compId, int prodCode, int modifiedBy);
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_ProductDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@compid", prodModel.CompID);
                        cmd.Parameters.AddWithValue("@code", prodModel.prodcode);
                        cmd.Parameters.AddWithValue("@ModifiedBy", prodModel.ModifiedBy);

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

