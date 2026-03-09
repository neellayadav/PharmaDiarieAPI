using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Data;
using System.Data.SqlClient;

namespace PharmaDiaries.DataAccess
{
    public class ExpenseRepository : IExpenseRepository
    {
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;

        public ExpenseRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        // ===========================================
        // CALCULATION
        // ===========================================

        public async Task<ExpenseCalculateResponse> CalculateExpenseAsync(ExpenseCalculateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_CalculateExpense]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@UID", request.UID);
                command.Parameters.AddWithValue("@DutyDate", request.DutyDate);
                command.Parameters.AddWithValue("@PatchName", request.PatchName);
                command.Parameters.AddWithValue("@IsHalfDay", request.IsHalfDay);
                command.Parameters.AddWithValue("@TravelModeID", (object?)request.TravelModeID ?? DBNull.Value);
                command.Parameters.AddWithValue("@TravelKM", request.TravelKM);
                command.Parameters.AddWithValue("@ActualFare", request.ActualFare);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    if (status == "ERROR")
                    {
                        return new ExpenseCalculateResponse
                        {
                            Status = "ERROR",
                            Message = reader.GetString(reader.GetOrdinal("Message"))
                        };
                    }
                    return new ExpenseCalculateResponse
                    {
                        Status = status,
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        HQCode = reader.GetString(reader.GetOrdinal("HQCode")),
                        LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                        DARateApplied = reader.GetDecimal(reader.GetOrdinal("DARateApplied")),
                        DAAmount = reader.GetDecimal(reader.GetOrdinal("DAAmount")),
                        TARateApplied = reader.GetDecimal(reader.GetOrdinal("TARateApplied")),
                        TAAmount = reader.GetDecimal(reader.GetOrdinal("TAAmount")),
                        LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                    };
                }
                con.Close();
            }
            return new ExpenseCalculateResponse { Status = "ERROR", Message = "Calculation failed" };
        }

        // ===========================================
        // CLAIMS
        // ===========================================

        public async Task<ExpenseClaimCreateResponse> CreateClaimAsync(ExpenseClaimCreateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ExpenseClaimInsert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@UID", request.UID);
                command.Parameters.AddWithValue("@ClaimMonth", request.ClaimMonth);
                command.Parameters.AddWithValue("@ClaimYear", request.ClaimYear);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ExpenseClaimCreateResponse
                    {
                        ClaimID = Convert.ToInt32(reader["ClaimID"]),
                        ClaimNumber = reader["ClaimNumber"].ToString()!
                    };
                }
                con.Close();
            }
            return new ExpenseClaimCreateResponse();
        }

        public async Task<bool> UpdateClaimAsync(ExpenseClaimUpdateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ExpenseClaimUpdate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@ClaimMonth", request.ClaimMonth);
                command.Parameters.AddWithValue("@ClaimYear", request.ClaimYear);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<ExpenseClaimModel>> GetClaimsByUIDAsync(int compID, int uid, int? month, int? year, string? status)
        {
            var list = new List<ExpenseClaimModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ExpenseClaimGetByUID]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@UID", uid);
                command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
                command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(MapClaimFromReader(reader));
                }
                con.Close();
            }
            return list;
        }

        public async Task<List<ExpenseClaimModel>> GetAllClaimsAsync(int compID, int? month, int? year, int? uid, string? status)
        {
            var list = new List<ExpenseClaimModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ExpenseClaimGetAll]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
                command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@UID", (object?)uid ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(MapClaimDetailFromReader(reader));
                }
                con.Close();
            }
            return list;
        }

        public async Task<ExpenseClaimModel?> GetClaimByIDAsync(int compID, int claimID)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ExpenseClaimGetByID]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                // First result set: claim header
                if (await reader.ReadAsync())
                {
                    return MapClaimDetailFromReader(reader);
                }
                con.Close();
            }
            return null;
        }

        public async Task<List<ExpenseClaimLineModel>> GetClaimLinesByClaimIDAsync(int compID, int claimID)
        {
            var list = new List<ExpenseClaimLineModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ClaimLineGetByClaimID]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(MapClaimLineFromReader(reader));
                }
                con.Close();
            }
            return list;
        }

        public async Task<List<ApprovalActionModel>> GetApprovalHistoryByClaimIDAsync(int compID, int claimID)
        {
            return await GetApprovalHistoryAsync(compID, claimID);
        }

        public async Task<SpStatusResponse> SubmitClaimAsync(ClaimSubmitRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ExpenseClaimSubmit]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@UID", request.UID);
            });
        }

        public async Task<SpStatusResponse> ResubmitClaimAsync(ClaimSubmitRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ExpenseClaimResubmit]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@UID", request.UID);
            });
        }

        // ===========================================
        // CLAIM LINES
        // ===========================================

        public async Task<ClaimLineInsertResponse> AddClaimLineAsync(ClaimLineAddRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ClaimLineInsert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@UID", request.UID);
                command.Parameters.AddWithValue("@DutyDate", request.DutyDate);
                command.Parameters.AddWithValue("@PatchName", request.PatchName);
                command.Parameters.AddWithValue("@IsHalfDay", request.IsHalfDay);
                command.Parameters.AddWithValue("@TravelModeID", (object?)request.TravelModeID ?? DBNull.Value);
                command.Parameters.AddWithValue("@TravelKM", request.TravelKM);
                command.Parameters.AddWithValue("@ActualFare", request.ActualFare);
                command.Parameters.AddWithValue("@AutoCalculatedKM", (object?)request.AutoCalculatedKM ?? DBNull.Value);
                command.Parameters.AddWithValue("@KMSource", request.KMSource);
                command.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    if (status == "ERROR")
                    {
                        return new ClaimLineInsertResponse
                        {
                            Status = "ERROR",
                            Message = reader.GetString(reader.GetOrdinal("Message"))
                        };
                    }
                    return new ClaimLineInsertResponse
                    {
                        Status = status,
                        LineID = reader.GetInt32(reader.GetOrdinal("LineID")),
                        LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                        DARateApplied = reader.GetDecimal(reader.GetOrdinal("DARateApplied")),
                        DAAmount = reader.GetDecimal(reader.GetOrdinal("DAAmount")),
                        TARateApplied = reader.GetDecimal(reader.GetOrdinal("TARateApplied")),
                        TAAmount = reader.GetDecimal(reader.GetOrdinal("TAAmount")),
                        LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                    };
                }
                con.Close();
            }
            return new ClaimLineInsertResponse { Status = "ERROR", Message = "Failed to add line" };
        }

        public async Task<ClaimLineInsertResponse> UpdateClaimLineAsync(ClaimLineUpdateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ClaimLineUpdate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@LineID", request.LineID);
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PatchName", request.PatchName);
                command.Parameters.AddWithValue("@IsHalfDay", request.IsHalfDay);
                command.Parameters.AddWithValue("@TravelModeID", (object?)request.TravelModeID ?? DBNull.Value);
                command.Parameters.AddWithValue("@TravelKM", request.TravelKM);
                command.Parameters.AddWithValue("@ActualFare", request.ActualFare);
                command.Parameters.AddWithValue("@AutoCalculatedKM", (object?)request.AutoCalculatedKM ?? DBNull.Value);
                command.Parameters.AddWithValue("@KMSource", request.KMSource);
                command.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var status = reader.GetString(reader.GetOrdinal("Status"));
                    if (status == "ERROR")
                    {
                        return new ClaimLineInsertResponse
                        {
                            Status = "ERROR",
                            Message = reader.GetString(reader.GetOrdinal("Message"))
                        };
                    }
                    return new ClaimLineInsertResponse
                    {
                        Status = status,
                        LineID = reader.GetInt32(reader.GetOrdinal("LineID")),
                        LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                        DARateApplied = reader.GetDecimal(reader.GetOrdinal("DARateApplied")),
                        DAAmount = reader.GetDecimal(reader.GetOrdinal("DAAmount")),
                        TARateApplied = reader.GetDecimal(reader.GetOrdinal("TARateApplied")),
                        TAAmount = reader.GetDecimal(reader.GetOrdinal("TAAmount")),
                        LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                    };
                }
                con.Close();
            }
            return new ClaimLineInsertResponse { Status = "ERROR", Message = "Failed to update line" };
        }

        public async Task<SpStatusResponse> DeleteClaimLineAsync(int compID, int lineID, int modifiedBy)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ClaimLineDelete]", cmd =>
            {
                cmd.Parameters.AddWithValue("@LineID", lineID);
                cmd.Parameters.AddWithValue("@CompID", compID);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
            });
        }

        // ===========================================
        // ATTACHMENTS
        // ===========================================

        public async Task<int> InsertAttachmentAsync(ClaimAttachmentInsertRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ClaimAttachmentInsert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@LineID", request.LineID);
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@FileName", request.FileName);
                command.Parameters.AddWithValue("@FileURL", request.FileURL);
                command.Parameters.AddWithValue("@FileSize", (object?)request.FileSize ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return Convert.ToInt32(reader["AttachmentID"]);
                con.Close();
            }
            return 0;
        }

        public async Task<List<ClaimAttachmentModel>> GetAttachmentsByLineAsync(int compID, int lineID)
        {
            var list = new List<ClaimAttachmentModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ClaimAttachmentGetByLine]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@LineID", lineID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new ClaimAttachmentModel
                    {
                        AttachmentID = reader.GetInt32(reader.GetOrdinal("AttachmentID")),
                        LineID = reader.GetInt32(reader.GetOrdinal("LineID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        FileName = reader.GetString(reader.GetOrdinal("FileName")),
                        FileURL = reader.GetString(reader.GetOrdinal("FileURL")),
                        FileSize = reader.IsDBNull(reader.GetOrdinal("FileSize")) ? null : reader.GetInt32(reader.GetOrdinal("FileSize")),
                        CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
                        CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // APPROVALS
        // ===========================================

        public async Task<List<ExpenseClaimModel>> GetPendingApprovalsAsync(int compID, int approverUID)
        {
            var list = new List<ExpenseClaimModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ApprovalPendingGet]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@ApproverUID", approverUID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(MapClaimFromReader(reader));
                }
                con.Close();
            }
            return list;
        }

        public async Task<SpStatusResponse> ApproveClaimAsync(ClaimApproveRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ApprovalApprove]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@ApproverUID", request.ApproverUID);
                cmd.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);
            });
        }

        public async Task<SpStatusResponse> RejectClaimAsync(ClaimRejectRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ApprovalReject]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@ApproverUID", request.ApproverUID);
                cmd.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);
            });
        }

        public async Task<SpStatusResponse> ReturnClaimAsync(ClaimReturnRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ApprovalReturn]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@ApproverUID", request.ApproverUID);
                cmd.Parameters.AddWithValue("@Remarks", request.Remarks);
            });
        }

        public async Task<SpStatusResponse> BulkApproveAsync(BulkApproveRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_ApprovalBulkApprove]", cmd =>
            {
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@ApproverUID", request.ApproverUID);
                cmd.Parameters.AddWithValue("@ClaimIDs", request.ClaimIDs);
                cmd.Parameters.AddWithValue("@Remarks", (object?)request.Remarks ?? DBNull.Value);
            });
        }

        public async Task<List<ApprovalActionModel>> GetApprovalHistoryAsync(int compID, int claimID)
        {
            var list = new List<ApprovalActionModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_ApprovalHistoryGet]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new ApprovalActionModel
                    {
                        ActionID = reader.GetInt32(reader.GetOrdinal("ActionID")),
                        ClaimID = reader.GetInt32(reader.GetOrdinal("ClaimID")),
                        ApproverUID = reader.GetInt32(reader.GetOrdinal("ApproverUID")),
                        ApproverName = reader.GetString(reader.GetOrdinal("ApproverName")),
                        Action = reader.GetString(reader.GetOrdinal("Action")),
                        Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                        ActionOn = reader.GetDateTime(reader.GetOrdinal("ActionOn"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // SETTLEMENT
        // ===========================================

        public async Task<SpStatusResponse> SettleClaimAsync(ClaimSettleRequest request)
        {
            return await ExecuteStatusSpAsync("[mcExpense].[usp_SettlementCreate]", cmd =>
            {
                cmd.Parameters.AddWithValue("@ClaimID", request.ClaimID);
                cmd.Parameters.AddWithValue("@CompID", request.CompID);
                cmd.Parameters.AddWithValue("@SettledByUID", request.SettledByUID);
                cmd.Parameters.AddWithValue("@PaymentRef", (object?)request.PaymentRef ?? DBNull.Value);
            });
        }

        public async Task<List<SettlementModel>> GetSettlementsByMonthAsync(int compID, int month, int year)
        {
            var list = new List<SettlementModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_SettlementGetByMonth]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new SettlementModel
                    {
                        SettlementID = reader.GetInt32(reader.GetOrdinal("SettlementID")),
                        ClaimID = reader.GetInt32(reader.GetOrdinal("ClaimID")),
                        ClaimNumber = reader.GetString(reader.GetOrdinal("ClaimNumber")),
                        UID = reader.GetInt32(reader.GetOrdinal("UID")),
                        EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                        SettledAmount = reader.GetDecimal(reader.GetOrdinal("SettledAmount")),
                        PaymentRef = reader.IsDBNull(reader.GetOrdinal("PaymentRef")) ? null : reader.GetString(reader.GetOrdinal("PaymentRef")),
                        SettledOn = reader.GetDateTime(reader.GetOrdinal("SettledOn")),
                        SettledByName = reader.GetString(reader.GetOrdinal("SettledByName"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // PRIVATE HELPERS
        // ===========================================

        private async Task<SpStatusResponse> ExecuteStatusSpAsync(string spName, Action<SqlCommand> addParams)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand(spName, con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                addParams(command);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SpStatusResponse
                    {
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        Message = reader.GetString(reader.GetOrdinal("Message"))
                    };
                }
                con.Close();
            }
            return new SpStatusResponse { Status = "ERROR", Message = "No response from stored procedure" };
        }

        private ExpenseClaimModel MapClaimFromReader(SqlDataReader reader)
        {
            return new ExpenseClaimModel
            {
                ClaimID = reader.GetInt32(reader.GetOrdinal("ClaimID")),
                ClaimNumber = reader.GetString(reader.GetOrdinal("ClaimNumber")),
                UID = reader.GetInt32(reader.GetOrdinal("UID")),
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader.GetString(reader.GetOrdinal("EmployeeName")),
                ClaimMonth = reader.GetInt32(reader.GetOrdinal("ClaimMonth")),
                ClaimYear = reader.GetInt32(reader.GetOrdinal("ClaimYear")),
                TotalDAAmount = reader.GetDecimal(reader.GetOrdinal("TotalDAAmount")),
                TotalTAAmount = reader.GetDecimal(reader.GetOrdinal("TotalTAAmount")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SubmittedOn = reader.IsDBNull(reader.GetOrdinal("SubmittedOn")) ? null : reader.GetDateTime(reader.GetOrdinal("SubmittedOn")),
                CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? null : reader.GetDateTime(reader.GetOrdinal("CreatedOn"))
            };
        }

        private ExpenseClaimModel MapClaimDetailFromReader(SqlDataReader reader)
        {
            var claim = MapClaimFromReader(reader);
            claim.HeadQuater = reader.IsDBNull(reader.GetOrdinal("HeadQuater")) ? null : reader.GetString(reader.GetOrdinal("HeadQuater"));
            claim.RoleID = reader.IsDBNull(reader.GetOrdinal("RoleID")) ? null : reader.GetInt32(reader.GetOrdinal("RoleID"));
            claim.RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"));
            claim.ApprovedOn = reader.IsDBNull(reader.GetOrdinal("ApprovedOn")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedOn"));
            claim.SettledOn = reader.IsDBNull(reader.GetOrdinal("SettledOn")) ? null : reader.GetDateTime(reader.GetOrdinal("SettledOn"));
            return claim;
        }

        private ExpenseClaimLineModel MapClaimLineFromReader(SqlDataReader reader)
        {
            return new ExpenseClaimLineModel
            {
                LineID = reader.GetInt32(reader.GetOrdinal("LineID")),
                ClaimID = reader.GetInt32(reader.GetOrdinal("ClaimID")),
                DutyDate = reader.GetDateTime(reader.GetOrdinal("DutyDate")),
                HQCode = reader.GetString(reader.GetOrdinal("HQCode")),
                PatchName = reader.GetString(reader.GetOrdinal("PatchName")),
                LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                IsHalfDay = reader.GetBoolean(reader.GetOrdinal("IsHalfDay")),
                PolicyIDUsed = reader.GetInt32(reader.GetOrdinal("PolicyIDUsed")),
                DARateApplied = reader.GetDecimal(reader.GetOrdinal("DARateApplied")),
                DAAmount = reader.GetDecimal(reader.GetOrdinal("DAAmount")),
                TravelModeID = reader.IsDBNull(reader.GetOrdinal("TravelModeID")) ? null : reader.GetInt32(reader.GetOrdinal("TravelModeID")),
                TravelModeName = reader.IsDBNull(reader.GetOrdinal("TravelModeName")) ? null : reader.GetString(reader.GetOrdinal("TravelModeName")),
                ModeType = reader.IsDBNull(reader.GetOrdinal("ModeType")) ? null : reader.GetString(reader.GetOrdinal("ModeType")),
                TravelKM = reader.GetDecimal(reader.GetOrdinal("TravelKM")),
                ActualFare = reader.GetDecimal(reader.GetOrdinal("ActualFare")),
                TARateApplied = reader.GetDecimal(reader.GetOrdinal("TARateApplied")),
                TAAmount = reader.GetDecimal(reader.GetOrdinal("TAAmount")),
                AutoCalculatedKM = reader.IsDBNull(reader.GetOrdinal("AutoCalculatedKM")) ? null : reader.GetDecimal(reader.GetOrdinal("AutoCalculatedKM")),
                KMSource = reader.IsDBNull(reader.GetOrdinal("KMSource")) ? "MANUAL" : reader.GetString(reader.GetOrdinal("KMSource")),
                LineTotal = reader.GetDecimal(reader.GetOrdinal("LineTotal")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? null : reader.GetDateTime(reader.GetOrdinal("CreatedOn"))
            };
        }
    }
}
