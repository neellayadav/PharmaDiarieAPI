using System;
using System.Collections.Generic;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class SalesBusiness : ISalesBusiness
    {
        private readonly ISalesRepository _repository;

        public SalesBusiness(ISalesRepository repository)
        {
            _repository = repository;
        }

        public string CreateHeader(SalesHeaderCreateRequest request) => _repository.CreateHeader(request);
        public SalesHeaderModel? GetHeaderById(int compId, string salesId, int uid) => _repository.GetHeaderById(compId, salesId, uid);
        public List<SalesHeaderModel> GetHeaderList(int compId, int? uid, int? custId, string? type, DateTime? fromDate, DateTime? toDate) => _repository.GetHeaderList(compId, uid, custId, type, fromDate, toDate);
        public bool UpdateHeader(SalesHeaderUpdateRequest request) => _repository.UpdateHeader(request);
        public bool DeleteHeader(SalesHeaderDeleteRequest request) => _repository.DeleteHeader(request);

        public CreateSaleResponse CreateSale(CreateSaleRequest request) => _repository.CreateSale(request);

        public int CreateDetail(SalesDetailCreateRequest request) => _repository.CreateDetail(request);
        public SalesDetailModel? GetDetailById(int itemId) => _repository.GetDetailById(itemId);
        public List<SalesDetailModel> GetDetailList(string salesId) => _repository.GetDetailList(salesId);
        public bool UpdateDetail(SalesDetailUpdateRequest request) => _repository.UpdateDetail(request);
        public bool DeleteDetail(int itemId) => _repository.DeleteDetail(itemId);
    }
}
