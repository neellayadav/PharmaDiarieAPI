using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
    public interface IOrdersRepository
    {
        bool Save(OrderModel order);
        bool Update(OrderModel order);
        bool Delete(int orderId, int modifiedBy);
        List<OrderModel> GetByTransID(string transId);
        List<OrderModel> GetByCompany(int compId, DateTime? fromDate, DateTime? toDate);
    }

    public class OrdersRepository : IOrdersRepository
    {
        private readonly string _connectionString;

        public OrdersRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:APIconnectionString"]!;
        }

        public bool Save(OrderModel order)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_OrderInsert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CompID", order.CompID);
                        cmd.Parameters.AddWithValue("@TransID", order.TransID);
                        cmd.Parameters.AddWithValue("@ProductID", order.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);
                        cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        cmd.Parameters.AddWithValue("@orderType", order.orderType ?? "POB");
                        cmd.Parameters.AddWithValue("@CreatedBy", order.CreatedBy);

                        con.Open();
                        result = cmd.ExecuteNonQuery() > 0;
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

        public bool Update(OrderModel order)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_OrderUpdate]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);
                        cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        cmd.Parameters.AddWithValue("@orderType", order.orderType ?? "POB");
                        cmd.Parameters.AddWithValue("@ModifiedBy", order.ModifiedBy);

                        con.Open();
                        result = cmd.ExecuteNonQuery() > 0;
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

        public bool Delete(int orderId, int modifiedBy)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_OrderDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                        con.Open();
                        result = cmd.ExecuteNonQuery() > 0;
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

        public List<OrderModel> GetByTransID(string transId)
        {
            var result = new List<OrderModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_OrderListByTransID]", transId);
            result = DataTableHelper.ConvertDataTable<OrderModel>(ds.Tables[0]);
            return result;
        }

        public List<OrderModel> GetByCompany(int compId, DateTime? fromDate, DateTime? toDate)
        {
            var result = new List<OrderModel>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_OrderListByCompany]",
                compId,
                fromDate ?? (object)DBNull.Value,
                toDate ?? (object)DBNull.Value);
            result = DataTableHelper.ConvertDataTable<OrderModel>(ds.Tables[0]);
            return result;
        }
    }
}
