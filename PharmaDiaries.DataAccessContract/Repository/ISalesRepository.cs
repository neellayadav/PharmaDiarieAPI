using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccessContract.Repository
{
    public interface ISalesRepository
    {
        // Header
        string CreateHeader(SalesHeaderCreateRequest request);
        SalesHeaderModel? GetHeaderById(int compId, string salesId, int uid);
        List<SalesHeaderModel> GetHeaderList(int compId, int? uid, int? custId, string? type, DateTime? fromDate, DateTime? toDate);
        bool UpdateHeader(SalesHeaderUpdateRequest request);
        bool DeleteHeader(SalesHeaderDeleteRequest request);

        // Combined
        CreateSaleResponse CreateSale(CreateSaleRequest request);

        // Detail
        int CreateDetail(SalesDetailCreateRequest request);
        SalesDetailModel? GetDetailById(int itemId);
        List<SalesDetailModel> GetDetailList(string salesId);
        bool UpdateDetail(SalesDetailUpdateRequest request);
        bool DeleteDetail(int itemId);
    }
}
