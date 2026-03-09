using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Data;
using System.Data.SqlClient;

namespace PharmaDiaries.DataAccess
{
    public class ExpensePolicyRepository : IExpensePolicyRepository
    {
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;

        public ExpensePolicyRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        // ===========================================
        // TRAVEL MODES
        // ===========================================

        public async Task<int> CreateTravelModeAsync(TravelModeCreateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_TravelModeInsert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ModeName", request.ModeName);
                command.Parameters.AddWithValue("@ModeType", request.ModeType);
                command.Parameters.AddWithValue("@RequiresReceipt", request.RequiresReceipt);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("TravelModeID"));
                con.Close();
            }
            return 0;
        }

        public async Task<bool> UpdateTravelModeAsync(TravelModeUpdateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_TravelModeUpdate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@TravelModeID", request.TravelModeID);
                command.Parameters.AddWithValue("@ModeName", request.ModeName);
                command.Parameters.AddWithValue("@ModeType", request.ModeType);
                command.Parameters.AddWithValue("@RequiresReceipt", request.RequiresReceipt);
                command.Parameters.AddWithValue("@IsActive", request.IsActive);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<TravelModeModel>> GetTravelModesAsync()
        {
            var list = new List<TravelModeModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_TravelModeGetAll]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new TravelModeModel
                    {
                        TravelModeID = reader.GetInt32(reader.GetOrdinal("TravelModeID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        ModeName = reader.GetString(reader.GetOrdinal("ModeName")),
                        ModeType = reader.GetString(reader.GetOrdinal("ModeType")),
                        RequiresReceipt = reader.GetBoolean(reader.GetOrdinal("RequiresReceipt")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // ALLOWANCE POLICY
        // ===========================================

        public async Task<int> CreatePolicyAsync(AllowancePolicyCreateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_AllowancePolicyInsert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyName", request.PolicyName);
                command.Parameters.AddWithValue("@EffectiveFrom", request.EffectiveFrom);
                command.Parameters.AddWithValue("@EffectiveTo", (object?)request.EffectiveTo ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return Convert.ToInt32(reader["PolicyID"]);
                con.Close();
            }
            return 0;
        }

        public async Task<bool> UpdatePolicyAsync(AllowancePolicyUpdateRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_AllowancePolicyUpdate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@PolicyID", request.PolicyID);
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyName", request.PolicyName);
                command.Parameters.AddWithValue("@EffectiveFrom", request.EffectiveFrom);
                command.Parameters.AddWithValue("@EffectiveTo", (object?)request.EffectiveTo ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", request.IsActive);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<AllowancePolicyModel>> GetPoliciesAsync(int compID)
        {
            var list = new List<AllowancePolicyModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_AllowancePolicyGetAll]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new AllowancePolicyModel
                    {
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyName = reader.GetString(reader.GetOrdinal("PolicyName")),
                        EffectiveFrom = reader.GetDateTime(reader.GetOrdinal("EffectiveFrom")),
                        EffectiveTo = reader.IsDBNull(reader.GetOrdinal("EffectiveTo")) ? null : reader.GetDateTime(reader.GetOrdinal("EffectiveTo")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }

        public async Task<AllowancePolicyModel?> GetActivePolicyAsync(int compID, DateTime asOfDate)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_AllowancePolicyGetActive]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@AsOfDate", asOfDate);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new AllowancePolicyModel
                    {
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyName = reader.GetString(reader.GetOrdinal("PolicyName")),
                        EffectiveFrom = reader.GetDateTime(reader.GetOrdinal("EffectiveFrom")),
                        EffectiveTo = reader.IsDBNull(reader.GetOrdinal("EffectiveTo")) ? null : reader.GetDateTime(reader.GetOrdinal("EffectiveTo")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    };
                }
                con.Close();
            }
            return null;
        }

        // ===========================================
        // DA RATES
        // ===========================================

        public async Task<bool> SaveDARateAsync(DARateSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_DARateBulkSave]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyID", request.PolicyID);
                command.Parameters.AddWithValue("@RoleID", request.RoleID);
                command.Parameters.AddWithValue("@LocationType", request.LocationType);
                command.Parameters.AddWithValue("@FullDayRate", request.FullDayRate);
                command.Parameters.AddWithValue("@HalfDayRate", request.HalfDayRate);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<DARateModel>> GetDARatesByPolicyAsync(int compID, int policyID)
        {
            var list = new List<DARateModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_DARateGetByPolicy]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@PolicyID", policyID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new DARateModel
                    {
                        DARateID = reader.GetInt32(reader.GetOrdinal("DARateID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                        RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                        LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                        FullDayRate = reader.GetDecimal(reader.GetOrdinal("FullDayRate")),
                        HalfDayRate = reader.GetDecimal(reader.GetOrdinal("HalfDayRate")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // TA RATES
        // ===========================================

        public async Task<bool> SaveTARateAsync(TARateSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_TARateBulkSave]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyID", request.PolicyID);
                command.Parameters.AddWithValue("@RoleID", request.RoleID);
                command.Parameters.AddWithValue("@TravelModeID", request.TravelModeID);
                command.Parameters.AddWithValue("@RatePerUnit", request.RatePerUnit);
                command.Parameters.AddWithValue("@MaxDailyLimit", (object?)request.MaxDailyLimit ?? DBNull.Value);
                command.Parameters.AddWithValue("@MaxMonthlyLimit", (object?)request.MaxMonthlyLimit ?? DBNull.Value);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<TARateModel>> GetTARatesByPolicyAsync(int compID, int policyID)
        {
            var list = new List<TARateModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_TARateGetByPolicy]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@PolicyID", policyID);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new TARateModel
                    {
                        TARateID = reader.GetInt32(reader.GetOrdinal("TARateID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                        RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                        TravelModeID = reader.GetInt32(reader.GetOrdinal("TravelModeID")),
                        ModeName = reader.IsDBNull(reader.GetOrdinal("ModeName")) ? null : reader.GetString(reader.GetOrdinal("ModeName")),
                        ModeType = reader.IsDBNull(reader.GetOrdinal("ModeType")) ? null : reader.GetString(reader.GetOrdinal("ModeType")),
                        RatePerUnit = reader.GetDecimal(reader.GetOrdinal("RatePerUnit")),
                        MaxDailyLimit = reader.IsDBNull(reader.GetOrdinal("MaxDailyLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxDailyLimit")),
                        MaxMonthlyLimit = reader.IsDBNull(reader.GetOrdinal("MaxMonthlyLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxMonthlyLimit")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // EMPLOYEE DA RATES
        // ===========================================

        public async Task<bool> SaveEmployeeDARateAsync(EmployeeDARateSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_EmployeeDARateSave]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyID", request.PolicyID);
                command.Parameters.AddWithValue("@UID", request.UID);
                command.Parameters.AddWithValue("@LocationType", request.LocationType);
                command.Parameters.AddWithValue("@FullDayRate", request.FullDayRate);
                command.Parameters.AddWithValue("@HalfDayRate", request.HalfDayRate);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<EmployeeDARateModel>> GetEmployeeDARatesAsync(int compID, int policyID, int? uid = null)
        {
            var list = new List<EmployeeDARateModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_EmployeeDARateGet]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@PolicyID", policyID);
                if (uid.HasValue)
                    command.Parameters.AddWithValue("@UID", uid.Value);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new EmployeeDARateModel
                    {
                        EmployeeDARateID = reader.GetInt32(reader.GetOrdinal("EmployeeDARateID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        UID = reader.GetInt32(reader.GetOrdinal("UID")),
                        EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader.GetString(reader.GetOrdinal("EmployeeName")),
                        LocationType = reader.GetString(reader.GetOrdinal("LocationType")),
                        FullDayRate = reader.GetDecimal(reader.GetOrdinal("FullDayRate")),
                        HalfDayRate = reader.GetDecimal(reader.GetOrdinal("HalfDayRate")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }

        // ===========================================
        // EMPLOYEE TA RATES
        // ===========================================

        public async Task<bool> SaveEmployeeTARateAsync(EmployeeTARateSaveRequest request)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_EmployeeTARateSave]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@PolicyID", request.PolicyID);
                command.Parameters.AddWithValue("@UID", request.UID);
                command.Parameters.AddWithValue("@TravelModeID", request.TravelModeID);
                command.Parameters.AddWithValue("@RatePerUnit", request.RatePerUnit);
                command.Parameters.AddWithValue("@MaxDailyLimit", (object?)request.MaxDailyLimit ?? DBNull.Value);
                command.Parameters.AddWithValue("@MaxMonthlyLimit", (object?)request.MaxMonthlyLimit ?? DBNull.Value);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
                con.Close();
            }
            return false;
        }

        public async Task<List<EmployeeTARateModel>> GetEmployeeTARatesAsync(int compID, int policyID, int? uid = null)
        {
            var list = new List<EmployeeTARateModel>();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcExpense].[usp_EmployeeTARateGet]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@CompID", compID);
                command.Parameters.AddWithValue("@PolicyID", policyID);
                if (uid.HasValue)
                    command.Parameters.AddWithValue("@UID", uid.Value);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new EmployeeTARateModel
                    {
                        EmployeeTARateID = reader.GetInt32(reader.GetOrdinal("EmployeeTARateID")),
                        CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                        PolicyID = reader.GetInt32(reader.GetOrdinal("PolicyID")),
                        UID = reader.GetInt32(reader.GetOrdinal("UID")),
                        EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? null : reader.GetString(reader.GetOrdinal("EmployeeName")),
                        TravelModeID = reader.GetInt32(reader.GetOrdinal("TravelModeID")),
                        ModeName = reader.IsDBNull(reader.GetOrdinal("ModeName")) ? null : reader.GetString(reader.GetOrdinal("ModeName")),
                        ModeType = reader.IsDBNull(reader.GetOrdinal("ModeType")) ? null : reader.GetString(reader.GetOrdinal("ModeType")),
                        RatePerUnit = reader.GetDecimal(reader.GetOrdinal("RatePerUnit")),
                        MaxDailyLimit = reader.IsDBNull(reader.GetOrdinal("MaxDailyLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxDailyLimit")),
                        MaxMonthlyLimit = reader.IsDBNull(reader.GetOrdinal("MaxMonthlyLimit")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxMonthlyLimit")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
                con.Close();
            }
            return list;
        }
    }
}
