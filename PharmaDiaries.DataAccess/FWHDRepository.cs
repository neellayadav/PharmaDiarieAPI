using System;
using Logicon.Kaidu.Platform.Helpers;
using Microsoft.Extensions.Configuration;
using System.Data;
using PharmaDiaries.Models;
using PharmaDiaries.DataAccessContract.Repository;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using ClosedXML.Excel;

namespace PharmaDiaries.DataAccess
{
	public class FWHdRepository : IFWHdRepository
    {
        private IConfiguration configuration;
        FWEmpDTRepository fwEmpDt;
        FWProdDTRepository fwProdDt;
        OrderRepository orderRepo;
        private string PharmaDiaries_ConnectionString;

        public FWHdRepository(IConfiguration configuration)
        {

            this.configuration = configuration;
            this.PharmaDiaries_ConnectionString = configuration["ConnectionStrings:APIconnectionString"]!.ToString();

            this.fwEmpDt = new FWEmpDTRepository(this.configuration);
            this.fwProdDt = new FWProdDTRepository(this.configuration);
            this.orderRepo = new OrderRepository(this.configuration);
        }

        public List<FieldWorkHeader> FWHeaderList()
        {
            var result = new List<FieldWorkHeader>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FieldworkHDList]");
            result = DataTableHelper.ConvertDataTable<FieldWorkHeader>(ds.Tables[0]);  
            return result;
        }

        public bool Save(FieldWorkHeader fwheader)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkSave]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        
                        cmd.Parameters.AddWithValue("@CompID", fwheader.CompID);
                        //cmd.Parameters.AddWithValue("@TransID", fwheader.TransID);
                        cmd.Parameters.AddWithValue("@UID", fwheader.UID);
                        cmd.Parameters.AddWithValue("@HQcode", fwheader.HQcode);
                        cmd.Parameters.AddWithValue("@PatchName", fwheader.PatchName);
                        cmd.Parameters.AddWithValue("@IsActive", fwheader.IsActive);
                        cmd.Parameters.AddWithValue("@CustID", fwheader.CustID);
                        cmd.Parameters.AddWithValue("@Visited", fwheader.Visited);
                        cmd.Parameters.AddWithValue("@Remarks", fwheader.Remarks);
                        cmd.Parameters.AddWithValue("@CreatedBy", fwheader.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", fwheader.CreatedOn);
                        // Location tracking parameters
                        cmd.Parameters.AddWithValue("@Latitude", fwheader.Latitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Longitude", fwheader.Longitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@LocationAccuracy", fwheader.LocationAccuracy ?? (object)DBNull.Value);

                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@TransID";
                        outPutParameter.Direction = ParameterDirection.Output;
                        outPutParameter.Size = 20;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();

                        // Begin Transaction
                        //SqlTransaction transaction = cmd.Connection.BeginTransaction();

                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        if (fwheader.EmpDTs!.Any())
                        {
                            List<FieldworkEmpDT>? empDts = fwheader!.EmpDTs;
                            foreach (var item in empDts!.ToList())
                            {
                               item.TransID = (String?) outPutParameter.Value; 
                               result = fwEmpDt.Save(item);
                            }
                        }

                        if (fwheader.ProdDTs!.Any())
                        {
                            List<FieldworkProdDT>? prodDTs = fwheader!.ProdDTs;
                            foreach (var item in prodDTs!.ToList())
                            {
                                item.TransID =  outPutParameter.Value.ToString();
                                result = fwProdDt.Save(item);
                            }
                        }

                        // Save Orders (POB - Personal Order Booking)
                        if (fwheader.Orders != null && fwheader.Orders.Any())
                        {
                            string transId = outPutParameter.Value.ToString()!;
                            foreach (var order in fwheader.Orders)
                            {
                                order.TransID = transId;
                                order.CompID = fwheader.CompID;
                                order.CreatedBy = fwheader.CreatedBy;
                                result = orderRepo.Save(order);
                            }
                        }

                        //if (result)
                        //    transaction.Commit();
                        //else
                        //    transaction.Rollback();

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                //transaction.Commit();
                throw ex;
            }

            return result;
        }

        public bool Delete(int compid, String transid, String userid )
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_FieldworkHDDelete]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@CompID", compid);
                        cmd.Parameters.AddWithValue("@TransID", transid);
                        cmd.Parameters.AddWithValue("@ModifiedBy", userid);
                        cmd.Parameters.AddWithValue("@ModifiedOn", null);

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

        public bool OtherworkSave(FieldworkOthers fwheader)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(PharmaDiaries_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[mcDCR].[usp_OtherworkSave]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@CompID", fwheader.CompID);
                        cmd.Parameters.AddWithValue("@UID", fwheader.UID);
                        cmd.Parameters.AddWithValue("@WTCode", fwheader.WTCode);
                        cmd.Parameters.AddWithValue("@Remarks", fwheader.Remarks);
                        cmd.Parameters.AddWithValue("@IsActive", fwheader.IsActive);
                        cmd.Parameters.AddWithValue("@CreatedBy", fwheader.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", fwheader.CreatedOn);

                        //Add the output parameter to the command object
                        //SqlParameter outPutParameter = new SqlParameter();
                        //outPutParameter.ParameterName = "@TransID";
                        //outPutParameter.Direction = ParameterDirection.Output;
                        //outPutParameter.Size = 20;
                        //cmd.Parameters.Add(outPutParameter);

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

        public List<FieldWorkHeader> GetEmpDateWiseFW(int compId, int uid, String empWorkDate, String periodOf = "ALL")
        {
            var result = new List<FieldWorkHeader>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWEmpDateWiseList]", compId, uid, empWorkDate, periodOf);
            result = DataTableHelper.ConvertDataTable<FieldWorkHeader>(ds.Tables[0]);
            return result;
        }

        public FWSummary GetFieldworkSummary(int compId, int uid)
        {
            var result = new FWSummary();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWSummary]", compId, uid);
            result = DataTableHelper.ConvertDataTable<FWSummary>(ds.Tables[0]).First();
            return result;
        }

        public List<FWHeader4Report> GetFWMonthlyReport(int compId, int monthOf, int yearOf)
        {
            var result = new List<FWHeader4Report>();
            DataSet ds = SqlHelper.ExecuteDataset(PharmaDiaries_ConnectionString, "[mcDCR].[usp_FWHDMonthlyList]", compId, monthOf, yearOf);
            result = DataTableHelper.ConvertDataTable<FWHeader4Report>(ds.Tables[0]);

            ////Create the data set and table
            //DataSet ds = new DataSet("New_DataSet");
            //DataTable dt = new DataTable("New_DataTable");

            ////Set the locale for each
            //ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
            //dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            ////Open a DB connection (in this example with OleDB)
            //OleDbConnection con = new OleDbConnection(dbConnectionString);
            //con.Open();

            ////Create a query and fill the data table with the data from the DB
            //string sql = "SELECT Whatever FROM MyDBTable;";
            //OleDbCommand cmd = new OleDbCommand(sql, con);
            //OleDbDataAdapter adptr = new OleDbDataAdapter();

            //adptr.SelectCommand = cmd;
            //adptr.Fill(dt);
            //con.Close();

            ////Add the table to the data set
            //ds.Tables.Add(dt);

            //Create the Excel worksheet from the data set using ClosedXML
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(ds);
                workbook.SaveAs("FWMonthlyReport.xlsx");
            }


            return result;
        }
        


    }
}

//@TransID varchar(20),
//@CustID int,
//@CallNo int,
//@Visited VARCHAR(15),
//@Remarks VARCHAR(210),
//@IsActive bit,
//@CreatedBy int = NULL,
//@CreatedOn datetime = NULL

//using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEINVOICEHEADER))
//{
//    cmd.CommandType = CommandType.StoredProcedure;
//    cmd.Connection = con;

//    cmd.Parameters.AddWithValue("@InvoiceID", item.InvoiceID);
//    cmd.Parameters.AddWithValue("@InvoiceNo", item.InvoiceNo);
//    cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
//    cmd.Parameters.AddWithValue("@MemberID", item.MemberID);
//    cmd.Parameters.AddWithValue("@SellerID", item.SellerID);
//    cmd.Parameters.AddWithValue("@InvoiceAmount", item.InvoiceAmount);
//    cmd.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
//    cmd.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);



//    //Add the output parameter to the command object
//    SqlParameter outPutParameter = new SqlParameter();
//    outPutParameter.ParameterName = "@NewInvoiceID";
//    outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
//    outPutParameter.Direction = System.Data.ParameterDirection.Output;
//    cmd.Parameters.Add(outPutParameter);

//    con.Open();
//    result = Convert.ToBoolean(cmd.ExecuteNonQuery());

//    item.InvoiceID = Convert.ToInt64(outPutParameter.Value);

//    con.Close();
//}
