using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class FWEmpDTBusiness : IFWEmpDTBusiness
    {
        private IFWEmpDTRepository _repository;

        public FWEmpDTBusiness(IFWEmpDTRepository repository)
        {
            _repository = repository;
        }

        public List<FWEmpDT4Report> GetAllFieldworkEmp(string transId)
        {
          return  _repository.GetAllFieldworkEmp(transId);
        }

        public bool DeleteEmpDT(string transID, int sno, string uid)
        {
            return _repository.DeleteEmpDT( transID,  sno, uid);
        }

        public bool Save(FieldworkEmpDT fwEmpDT)
        {
            return _repository.Save(fwEmpDT);
        }
    }
}