using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
    public class SalesRepository : ISalesRepository
    {
        private readonly string _connectionString;

        public SalesRepository(IConfiguration configuration)
        {
            _connectionString = configuration["connectionStrings:APIconnectionString"]!;
        }

        // ===================== COMBINED CREATE =====================

        public CreateSaleResponse CreateSale(CreateSaleRequest request)
        {
            var response = new CreateSaleResponse();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (SqlTransaction txn = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Header (auto-generate SalesID)
                        using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesHeader_Insert]", con, txn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CompID", request.CompID);
                            cmd.Parameters.AddWithValue("@UID", request.UID);
                            cmd.Parameters.AddWithValue("@FromDate", (object?)request.FromDate ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ToDate", (object?)request.ToDate ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CustID", request.CustID);
                            cmd.Parameters.AddWithValue("@Type", request.Type);
                            cmd.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                            var outputParam = new SqlParameter("@SalesID", SqlDbType.VarChar, 20)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputParam);

                            cmd.ExecuteNonQuery();
                            response.SalesID = outputParam.Value?.ToString() ?? string.Empty;
                        }

                        // 2. Insert all Detail rows
                        foreach (var detail in request.Details)
                        {
                            using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesDetail_Insert]", con, txn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SalesID", response.SalesID);
                                cmd.Parameters.AddWithValue("@ProductID", detail.ProductID);
                                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                                cmd.Parameters.AddWithValue("@TotalAmount", detail.TotalAmount);
                                cmd.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                                var itemId = Convert.ToInt32(cmd.ExecuteScalar());
                                response.ItemIDs.Add(itemId);
                            }
                        }

                        txn.Commit();
                    }
                    catch
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
            return response;
        }

        // ===================== HEADER =====================

        public string CreateHeader(SalesHeaderCreateRequest request)
        {
            string salesId = string.Empty;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesHeader_Insert]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompID", request.CompID);
                    cmd.Parameters.AddWithValue("@UID", request.UID);
                    cmd.Parameters.AddWithValue("@FromDate", (object?)request.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object?)request.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustID", request.CustID);
                    cmd.Parameters.AddWithValue("@Type", request.Type);
                    cmd.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                    var outputParam = new SqlParameter("@SalesID", SqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    salesId = outputParam.Value?.ToString() ?? string.Empty;
                    con.Close();
                }
            }
            return salesId;
        }

        public SalesHeaderModel? GetHeaderById(int compId, string salesId, int uid)
        {
            SalesHeaderModel? header = null;
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_SalesHeader_GetById]", compId, salesId, uid);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var list = DataTableHelper.ConvertDataTable<SalesHeaderModel>(ds.Tables[0]);
                header = list[0];
                // Fetch nested details
                header.Details = GetDetailList(salesId);
            }
            return header;
        }

        public List<SalesHeaderModel> GetHeaderList(int compId, int? uid, int? custId, string? type, DateTime? fromDate, DateTime? toDate)
        {
            var result = new List<SalesHeaderModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesHeader_GetList]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompID", compId);
                    cmd.Parameters.AddWithValue("@UID", (object?)uid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustID", (object?)custId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", (object?)type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        result = DataTableHelper.ConvertDataTable<SalesHeaderModel>(dt);
                    }
                    con.Close();
                }
            }

            // Fetch nested details for each header
            foreach (var header in result)
            {
                header.Details = GetDetailList(header.SalesID!);
            }

            return result;
        }

        public bool UpdateHeader(SalesHeaderUpdateRequest request)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesHeader_Update]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompID", request.CompID);
                    cmd.Parameters.AddWithValue("@SalesID", request.SalesID);
                    cmd.Parameters.AddWithValue("@UID", request.UID);
                    cmd.Parameters.AddWithValue("@FromDate", (object?)request.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object?)request.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustID", request.CustID);
                    cmd.Parameters.AddWithValue("@Type", request.Type);
                    cmd.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                    con.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                }
            }
            return result;
        }

        public bool DeleteHeader(SalesHeaderDeleteRequest request)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesHeader_Delete]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompID", request.CompID);
                    cmd.Parameters.AddWithValue("@SalesID", request.SalesID);
                    cmd.Parameters.AddWithValue("@UID", request.UID);
                    cmd.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                    con.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                }
            }
            return result;
        }

        // ===================== DETAIL =====================

        public int CreateDetail(SalesDetailCreateRequest request)
        {
            int itemId = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesDetail_Insert]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SalesID", request.SalesID);
                    cmd.Parameters.AddWithValue("@ProductID", request.ProductID);
                    cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", request.UnitPrice);
                    cmd.Parameters.AddWithValue("@TotalAmount", request.TotalAmount);
                    cmd.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                    con.Open();
                    var result = cmd.ExecuteScalar();
                    itemId = Convert.ToInt32(result);
                    con.Close();
                }
            }
            return itemId;
        }

        public SalesDetailModel? GetDetailById(int itemId)
        {
            SalesDetailModel? detail = null;
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_SalesDetail_GetById]", itemId);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var list = DataTableHelper.ConvertDataTable<SalesDetailModel>(ds.Tables[0]);
                detail = list[0];
            }
            return detail;
        }

        public List<SalesDetailModel> GetDetailList(string salesId)
        {
            var result = new List<SalesDetailModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_SalesDetail_GetList]", salesId);
            result = DataTableHelper.ConvertDataTable<SalesDetailModel>(ds.Tables[0]);
            return result;
        }

        public bool UpdateDetail(SalesDetailUpdateRequest request)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesDetail_Update]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemID", request.ItemID);
                    cmd.Parameters.AddWithValue("@ProductID", request.ProductID);
                    cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", request.UnitPrice);
                    cmd.Parameters.AddWithValue("@TotalAmount", request.TotalAmount);
                    cmd.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                    con.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                }
            }
            return result;
        }

        public bool DeleteDetail(int itemId)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_SalesDetail_Delete]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ItemID", itemId);

                    con.Open();
                    result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                }
            }
            return result;
        }
    }
}
