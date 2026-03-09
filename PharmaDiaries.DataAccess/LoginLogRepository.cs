using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;
using Logicon.Kaidu.Platform.Helpers;

namespace PharmaDiaries.DataAccess
{
    public class LoginLogRepository : ILoginLogRepository
    {
        private readonly string _connectionString;

        public LoginLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("APIconnectionString")!;
        }

        public LoginLogModel? Save(LoginLogInsertRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO [mcDCR].[LoginLogs] ([CompID], [UID], [Source], [LoginDT])
                    OUTPUT INSERTED.[LogID], INSERTED.[CompID], INSERTED.[UID], INSERTED.[Source], INSERTED.[LoginDT], INSERTED.[LogoutDT]
                    VALUES (@CompID, @UID, @Source, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CompID", request.CompID);
                    cmd.Parameters.AddWithValue("@UID", request.UID);
                    cmd.Parameters.AddWithValue("@Source", request.Source ?? (object)DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoginLogModel
                            {
                                LogID = reader.GetInt32(reader.GetOrdinal("LogID")),
                                CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                                UID = reader.GetInt32(reader.GetOrdinal("UID")),
                                Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
                                LoginDT = reader.GetDateTime(reader.GetOrdinal("LoginDT")),
                                LogoutDT = reader.IsDBNull(reader.GetOrdinal("LogoutDT")) ? null : reader.GetDateTime(reader.GetOrdinal("LogoutDT"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public LoginLogModel? Update(LoginLogUpdateRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    UPDATE [mcDCR].[LoginLogs]
                    SET [LogoutDT] = SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')
                    WHERE [LogID] = @LogID;

                    SELECT [LogID], [CompID], [UID], [Source], [LoginDT], [LogoutDT]
                    FROM [mcDCR].[LoginLogs]
                    WHERE [LogID] = @LogID;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@LogID", request.LogID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoginLogModel
                            {
                                LogID = reader.GetInt32(reader.GetOrdinal("LogID")),
                                CompID = reader.GetInt32(reader.GetOrdinal("CompID")),
                                UID = reader.GetInt32(reader.GetOrdinal("UID")),
                                Source = reader.IsDBNull(reader.GetOrdinal("Source")) ? null : reader.GetString(reader.GetOrdinal("Source")),
                                LoginDT = reader.GetDateTime(reader.GetOrdinal("LoginDT")),
                                LogoutDT = reader.IsDBNull(reader.GetOrdinal("LogoutDT")) ? null : reader.GetDateTime(reader.GetOrdinal("LogoutDT"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<LoginLogModel> GetByCompUID(int compId, int uid)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByCompUID]",
                compId,
                uid
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
            }
            return new List<LoginLogModel>();
        }

        public LoginLogModel? GetByLogId(int logId)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByLogId]",
                logId
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var list = DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
                return list.Count > 0 ? list[0] : null;
            }
            return null;
        }

        public List<LoginLogModel> GetByCompUIDMonthYear(int compId, int uid, int monthOf, int yearOf)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByCompUIDMonthYear]",
                compId,
                uid,
                monthOf,
                yearOf
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
            }
            return new List<LoginLogModel>();
        }

        public List<LoginLogModel> GetByCompUIDDate(int compId, int uid, DateTime dateOf)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByCompUIDDate]",
                compId,
                uid,
                dateOf.Date
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
            }
            return new List<LoginLogModel>();
        }

        public List<LoginLogModel> GetByCompIdMonthYear(int compId, int monthOf, int yearOf)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByCompIdMonthYear]",
                compId,
                monthOf,
                yearOf
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
            }
            return new List<LoginLogModel>();
        }

        public List<LoginLogModel> GetByCompIdDate(int compId, DateTime dateOf)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                _connectionString,
                "[mcDCR].[usp_LoginLogGetByCompIdDate]",
                compId,
                dateOf.Date
            );

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableHelper.ConvertDataTable<LoginLogModel>(ds.Tables[0]);
            }
            return new List<LoginLogModel>();
        }
    }
}
