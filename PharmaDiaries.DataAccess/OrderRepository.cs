// ============================================================================
// DEPRECATED: This file is no longer in use.
// Replaced by OrdersRepository.cs which includes orderType support
// and has complete CRUD operations (Save, Update, Delete, GetByTransID, GetByCompany).
// FWHDRepository.cs now uses OrdersRepository instead.
// ============================================================================

//using Logicon.Kaidu.Platform.Helpers;
//using Microsoft.Extensions.Configuration;
//using PharmaDiaries.DataAccessContract.Repository;
//using PharmaDiaries.Models;
//using System.Data;
//using System.Data.SqlClient;

//namespace PharmaDiaries.DataAccess
//{
//    public class OrderRepository : IOrderRepository
//    {
//        private IConfiguration configuration;
//        private string PharmaDiaries_ConnectionString;

//        public OrderRepository(IConfiguration configuration)
//        {
//            this.configuration = configuration;
//            this.PharmaDiaries_ConnectionString = configuration["ConnectionStrings:APIconnectionString"]!.ToString();
//        }

//        public bool Save(OrderModel order)
//        {
//            var result = false;
//            try
//            {
//                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
//                {
//                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_OrderInsert]"))
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Connection = con;
//                        cmd.Parameters.AddWithValue("@CompID", order.CompID);
//                        cmd.Parameters.AddWithValue("@TransID", order.TransID);
//                        cmd.Parameters.AddWithValue("@ProductID", order.ProductID);
//                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
//                        cmd.Parameters.AddWithValue("@UnitPrice", order.UnitPrice ?? 0);
//                        cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount ?? 0);
//                        cmd.Parameters.AddWithValue("@CreatedBy", order.CreatedBy);
//                        // NOTE: @orderType was MISSING here - root cause of the bug
//
//                        con.Open();
//                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
//                        con.Close();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//
//            return result;
//        }

//        public List<OrderModel> GetByTransID(string transId)
//        {
//            var result = new List<OrderModel>();
//            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_OrderListByTransID]", transId);
//            result = DataTableHelper.ConvertDataTable<OrderModel>(ds.Tables[0]);
//            return result;
//        }
//    }
//}
