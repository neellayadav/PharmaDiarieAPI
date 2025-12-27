using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
    public interface IDCRDateRequestRepository
    {
        int CreateRequest(DCRDateRequest request);
        bool Approve(int requestId, int approvedBy);
        bool Reject(int requestId, int approvedBy, string? rejectionReason);
        List<DCRDateRequest> GetByEmployee(int compId, int employeeId);
        List<DCRDateRequest> GetPendingRequests(int compId);
        List<DCRDateRequest> GetAllRequests(int compId, string? status, DateTime? fromDate, DateTime? toDate);
        bool IsDateApproved(int compId, int employeeId, DateTime requestedDate);
    }

    public class DCRDateRequestRepository : IDCRDateRequestRepository
    {
        private readonly string _connectionString;

        public DCRDateRequestRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:APIconnectionString"]!;
        }

        public int CreateRequest(DCRDateRequest request)
        {
            int requestId = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_DCRDateRequestInsert]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CompID", request.CompID);
                        cmd.Parameters.AddWithValue("@EmployeeID", request.EmployeeID);
                        cmd.Parameters.AddWithValue("@RequestedDate", request.RequestedDate);
                        cmd.Parameters.AddWithValue("@Reason", request.Reason ?? (object)DBNull.Value);

                        con.Open();
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            requestId = Convert.ToInt32(result);
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return requestId;
        }

        public bool Approve(int requestId, int approvedBy)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_DCRDateRequestApprove]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RequestID", requestId);
                        cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);

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

        public bool Reject(int requestId, int approvedBy, string? rejectionReason)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_DCRDateRequestReject]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RequestID", requestId);
                        cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                        cmd.Parameters.AddWithValue("@RejectionReason", rejectionReason ?? (object)DBNull.Value);

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

        public List<DCRDateRequest> GetByEmployee(int compId, int employeeId)
        {
            var result = new List<DCRDateRequest>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_DCRDateRequestListByEmployee]", compId, employeeId);
            result = DataTableHelper.ConvertDataTable<DCRDateRequest>(ds.Tables[0]);
            return result;
        }

        public List<DCRDateRequest> GetPendingRequests(int compId)
        {
            var result = new List<DCRDateRequest>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_DCRDateRequestListPending]", compId);
            result = DataTableHelper.ConvertDataTable<DCRDateRequest>(ds.Tables[0]);
            return result;
        }

        public List<DCRDateRequest> GetAllRequests(int compId, string? status, DateTime? fromDate, DateTime? toDate)
        {
            var result = new List<DCRDateRequest>();
            DataSet ds = SqlHelper.ExecuteDataset(_connectionString, "[mcDCR].[usp_DCRDateRequestListAll]",
                compId,
                status ?? (object)DBNull.Value,
                fromDate ?? (object)DBNull.Value,
                toDate ?? (object)DBNull.Value);
            result = DataTableHelper.ConvertDataTable<DCRDateRequest>(ds.Tables[0]);
            return result;
        }

        public bool IsDateApproved(int compId, int employeeId, DateTime requestedDate)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_DCRDateRequestCheckApproved]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CompID", compId);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@RequestedDate", requestedDate);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt32(0) == 1;
                            }
                        }
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
