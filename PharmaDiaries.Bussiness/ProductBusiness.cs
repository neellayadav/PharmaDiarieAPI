using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
	public class ProductBusiness : IProductBusiness
	{
        private IProductRepository _repository;

        public ProductBusiness(IProductRepository repository)
        {
            _repository = repository;
        }

        public List<ProductModel> GetProductList(int compid)
        {
            return _repository.GetProductList(compid);
        }

        public bool Save(ProductModel prodModel)
        {
            return _repository.Save(prodModel);
        }

        public bool Update(ProductModel prodModel)
        {
            return _repository.Update(prodModel);
        }

        public bool Delete(ProductModel prodModel) // (int compId, int prodCode, int modifiedBy);
        {
            return _repository.Delete(prodModel);
        }

    }
}

