using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.BusinessContract
{
	public interface IProductBusiness
	{

		public List<ProductModel> GetProductList(int compid);

        public bool Save(ProductModel prodModel);

        public bool Update(ProductModel prodModel);

        public bool Delete(ProductModel prodModel); // (int compId, int prodCode, int modifiedBy);
    }
}

