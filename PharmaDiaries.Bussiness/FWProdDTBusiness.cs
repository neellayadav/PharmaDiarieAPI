using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class FWProdDTBusiness : IFWProdDTBusiness
    {
        private IFWProdDTRepository _repository;

        public FWProdDTBusiness(IFWProdDTRepository repository)
        {
            _repository = repository;
        }

        public List<FWProdDT4Report> GetAllFieldworkProd(string transId)
        {
          return  _repository.GetAllFieldworkProd(transId);
        }

        public bool DeleteProdDT(string transID, int sno, string uid)
        {
            return _repository.DeleteProdDT( transID,  sno, uid);
        }

        public bool Save(FieldworkProdDT prod)
        {
            return _repository.Save(prod);
        }
    }
}