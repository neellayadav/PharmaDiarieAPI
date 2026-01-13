using System;
using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
	public class CustomerBusiness: ICustomerBusiness
	{
        private ICustomerRepository _irepository;
		public CustomerBusiness(ICustomerRepository irepository)
		{
            this._irepository = irepository;
		}

        public List<CustomerModel> CustomerListByCompType(int compid, String custType)
        {
            return _irepository.CustomerListByCompType(compid, custType);
        }

        public bool Delete(DeleteCustomerModel delCustMod)
        {
            return _irepository.Delete(delCustMod);
        }

        public bool Save(CustomerModel custModel)
        {
            return _irepository.Save(custModel);
        }

        public bool Update(CustomerModel custModel)
        {
            return _irepository.Update(custModel);
        }

        public CustomerLocationResponse? GetCustomerLocation(int compId, int custId)
        {
            return _irepository.GetCustomerLocation(compId, custId);
        }

        public bool UpdateCustomerLocation(CustomerLocationUpdateRequest request)
        {
            return _irepository.UpdateCustomerLocation(request);
        }
    }
}

