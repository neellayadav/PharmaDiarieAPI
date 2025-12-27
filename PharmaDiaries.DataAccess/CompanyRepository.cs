using System;
using DocumentFormat.OpenXml.ExtendedProperties;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Logicon.Kaidu.Platform.Helpers;

namespace PharmaDiaries.DataAccess
{
    public class CompanyRepository : ICompanyRepository
    {
        private IConfiguration _configuration;
        private string _PharmaDiaries_ConnectionString;


        public CompanyRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();

        }
//    }
//}

////using CompanyAPI.Data;
////using CompanyAPI.Models;
////using Microsoft.Data.SqlClient;
////using System.Data;

//namespace CompanyAPI.Repositories
//{
//    public class CompanyRepository : ICompanyRepository
//    {
//        private readonly DatabaseHelper _dbHelper;

//        public CompanyRepository(DatabaseHelper dbHelper)
//        {
//            _dbHelper = dbHelper;
//        }

        public async Task<int> CreateCompanyAsync(CompanyCreateRequest request)
        {
            //using var connection = await .CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyCreate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Name", request.Name);
                command.Parameters.AddWithValue("@Address1", (object?)request.Address1 ?? DBNull.Value);
                command.Parameters.AddWithValue("@Locality", (object?)request.Locality ?? DBNull.Value);
                command.Parameters.AddWithValue("@CityOrTown", (object?)request.CityOrTown ?? DBNull.Value);
                command.Parameters.AddWithValue("@Pincode", (object?)request.Pincode ?? DBNull.Value);
                command.Parameters.AddWithValue("@District", (object?)request.District ?? DBNull.Value);
                command.Parameters.AddWithValue("@State", (object?)request.State ?? DBNull.Value);
                command.Parameters.AddWithValue("@Country", (object?)request.Country ?? DBNull.Value);
                command.Parameters.AddWithValue("@Mobile", (object?)request.Mobile ?? DBNull.Value);
                command.Parameters.AddWithValue("@Telephone", (object?)request.Telephone ?? DBNull.Value);
                command.Parameters.AddWithValue("@Fax", (object?)request.Fax ?? DBNull.Value);
                //command.Parameters.AddWithValue("@IsActive", request.IsActive);
                command.Parameters.AddWithValue("@CreatedBy", request.CreatedBy);

                var compIdParam = new SqlParameter("@CompID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(compIdParam);

                con.Open();
                await command.ExecuteNonQueryAsync();
                con.Close();
                return (int)compIdParam.Value;
            }
        }

        public async Task<CompanyModel?> GetCompanyByIdAsync(int compId)
        {
            //var result = new CompanyModel();
            //DataSet ds = SqlHelper.ExecuteDataset(_PharmaDiaries_ConnectionString, "[mcDCR].[usp_CompanyGetById]", compId);
            //result = DataTableHelper.ConvertDataTable<CompanyModel>(ds.Tables[0]).First();
            //return await Task.FromResult<CompanyModel?>(result);

            ////using var connection = await _dbHelper.CreateConnectionAsync();
            var result = new CompanyModel();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyGetById]",con)
                {
                    CommandType = CommandType.StoredProcedure,
                    //Connection = con // This also works instead of pasing as argument in the SqlCommand()
                };

                command.Parameters.AddWithValue("@CompID", compId);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    result = MapCompanyFromReader(reader);
                }
                con.Close();
                return result;
            }
        }

        public async Task<List<CompanyModel>> GetAllCompaniesAsync()
        {
            var companies = new List<CompanyModel>();
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyGetAll]")
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = con
                };

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    companies.Add(MapCompanyFromReader(reader));
                }
                con.Close();
                return companies;
            }
        }

        public async Task<List<CompanyModel>> GetActiveCompaniesAsync()
        {
            var companies = new List<CompanyModel>();
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyGetActive]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    companies.Add(MapCompanyFromReader(reader));
                }
                con.Close();
                return companies;
            }
        }

        public async Task<List<CompanyModel>> SearchCompaniesByNameAsync(string searchTerm)
        {
            var companies = new List<CompanyModel>();
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanySearchByName]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    companies.Add(MapCompanyFromReader(reader));
                }
                con.Close();
                return companies;
            }
        }

        public async Task<bool> UpdateCompanyAsync(CompanyUpdateRequest request)
        {
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyUpdate]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@Name", request.Name);
                command.Parameters.AddWithValue("@Address1", (object?)request.Address1 ?? DBNull.Value);
                command.Parameters.AddWithValue("@Locality", (object?)request.Locality ?? DBNull.Value);
                command.Parameters.AddWithValue("@CityOrTown", (object?)request.CityOrTown ?? DBNull.Value);
                command.Parameters.AddWithValue("@Pincode", (object?)request.Pincode ?? DBNull.Value);
                command.Parameters.AddWithValue("@District", (object?)request.District ?? DBNull.Value);
                command.Parameters.AddWithValue("@State", (object?)request.State ?? DBNull.Value);
                command.Parameters.AddWithValue("@Country", (object?)request.Country ?? DBNull.Value);
                command.Parameters.AddWithValue("@Mobile", (object?)request.Mobile ?? DBNull.Value);
                command.Parameters.AddWithValue("@Telephone", (object?)request.Telephone ?? DBNull.Value);
                command.Parameters.AddWithValue("@Fax", (object?)request.Fax ?? DBNull.Value);
                command.Parameters.AddWithValue("@LogoURL", (object?)request.LogoURL ?? DBNull.Value);
                //command.Parameters.AddWithValue("@IsActive", (object?)request.IsActive ?? DBNull.Value);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                var rowsAffected = await command.ExecuteNonQueryAsync();
                con.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateCompanyStatusAsync(CompanyStatusUpdateRequest request)
        {
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyUpdateStatus]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CompID", request.CompID);
                command.Parameters.AddWithValue("@IsActive", request.IsActive);
                command.Parameters.AddWithValue("@ModifiedBy", request.ModifiedBy);

                con.Open();
                var rowsAffected = await command.ExecuteNonQueryAsync();
                con.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteCompanyAsync(int compId, int modifiedBy)
        {
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyDelete]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@CompID", compId);
                command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                con.Open();
                var rowsAffected = await command.ExecuteNonQueryAsync();
                con.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> CheckCompanyNameExistsAsync(string name, int? excludingCompId = null)
        {
            //using var connection = await _dbHelper.CreateConnectionAsync()
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyCheckNameExists]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ExcludingCompID", (object?)excludingCompId ?? DBNull.Value);

                var existsParam = new SqlParameter("@Exists", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(existsParam);
                con.Open();
                await command.ExecuteNonQueryAsync();
                con.Close();
                return (bool)existsParam.Value;
            }
        }

        public async Task<CompanyCountResponse> GetCompanyCountAsync()
        {
            //using var connection = await _dbHelper.CreateConnectionAsync();
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand("[mcDCR].[usp_CompanyGetCount]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                con.Open();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new CompanyCountResponse
                    {
                        TotalCount = reader.GetInt32("TotalCount"),
                        ActiveCount = reader.GetInt32("ActiveCount"),
                        InactiveCount = reader.GetInt32("InactiveCount")
                    };
                }
                con.Close();
                return new CompanyCountResponse();
            }
        }

        private CompanyModel MapCompanyFromReader(SqlDataReader reader)
        {
            return new CompanyModel
            {
                CompID = reader.GetInt32("CompID"),
                Name = reader.GetString("Name"),
                Address1 = reader.IsDBNull("Address1") ? null : reader.GetString("Address1"),
                Locality = reader.IsDBNull("Locality") ? null : reader.GetString("Locality"),
                CityOrTown = reader.IsDBNull("CityOrTown") ? null : reader.GetString("CityOrTown"),
                Pincode = reader.IsDBNull("Pincode") ? null : reader.GetInt32("Pincode"),
                District = reader.IsDBNull("District") ? null : reader.GetString("District"),
                State = reader.IsDBNull("State") ? null : reader.GetString("State"),
                Country = reader.IsDBNull("Country") ? null : reader.GetString("Country"),
                Mobile = reader.IsDBNull("Mobile") ? null : reader.GetString("Mobile"),
                Telephone = reader.IsDBNull("Telephone") ? null : reader.GetString("Telephone"),
                Fax = reader.IsDBNull("Fax") ? null : reader.GetString("Fax"),
                LogoURL = reader.IsDBNull("LogoURL") ? null : reader.GetString("LogoURL"),
                IsActive = reader.GetBoolean("IsActive"),
                IsLocationTrackerEnabled = reader.IsDBNull("IsLocationTrackerEnabled") ? false : reader.GetBoolean("IsLocationTrackerEnabled"),
                CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetInt32("CreatedBy"),
                CreatedOn = reader.IsDBNull("CreatedOn") ? null : reader.GetDateTime("CreatedOn"),
                ModifiedBy = reader.IsDBNull("ModifiedBy") ? null : reader.GetInt32("ModifiedBy"),
                ModifiedOn = reader.IsDBNull("ModifiedOn") ? null : reader.GetDateTime("ModifiedOn")
            };
        }

        public async Task<bool> UpdateLocationTrackerSettingAsync(int compId, bool isEnabled, int modifiedBy)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand(
                    "UPDATE mcMaster.Company SET IsLocationTrackerEnabled = @IsEnabled, ModifiedBy = @ModifiedBy, ModifiedOn = GETDATE() WHERE CompID = @CompID", con);

                command.Parameters.AddWithValue("@CompID", compId);
                command.Parameters.AddWithValue("@IsEnabled", isEnabled);
                command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                con.Open();
                var rowsAffected = await command.ExecuteNonQueryAsync();
                con.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> GetLocationTrackerSettingAsync(int compId)
        {
            using (SqlConnection con = new SqlConnection(_PharmaDiaries_ConnectionString))
            {
                using var command = new SqlCommand(
                    "SELECT IsLocationTrackerEnabled FROM mcMaster.Company WHERE CompID = @CompID", con);

                command.Parameters.AddWithValue("@CompID", compId);

                con.Open();
                var result = await command.ExecuteScalarAsync();
                con.Close();
                return result != null && result != DBNull.Value && (bool)result;
            }
        }
    }
}