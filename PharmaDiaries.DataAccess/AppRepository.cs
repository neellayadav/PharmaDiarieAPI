using System;
using System.Data;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccess
{
    public class AppRepository : IAppRepository
    {
        private IConfiguration configuration;
        private string PharmaDiaries_ConnectionString;

        public AppRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["connectionStrings:APIconnectionString"]!.ToString();
        }

        public AppVersionConfig? GetVersionConfig(string platform)
        {
            DataSet ds = SqlHelper.ExecuteDataset(
                PharmaDiaries_ConnectionString,
                "dbo.usp_AppVersionCheck",
                platform);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var list = DataTableHelper.ConvertDataTable<AppVersionConfig>(ds.Tables[0]);
                return list.FirstOrDefault();
            }

            return null;
        }
    }
}
