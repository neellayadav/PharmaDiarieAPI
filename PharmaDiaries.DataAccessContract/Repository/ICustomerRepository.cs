using System;
using PharmaDiaries.Models;
using System.Collections.Generic;

namespace PharmaDiaries.DataAccessContract.Repository
{
	public interface ICustomerRepository
	{
        public List<CustomerModel> CustomerListByCompType(int compid, String custType);

        public bool Save(CustomerModel custModel);

        public bool Update(CustomerModel custModel);

        public bool Delete(DeleteCustomerModel delCustMod);
    }
}

