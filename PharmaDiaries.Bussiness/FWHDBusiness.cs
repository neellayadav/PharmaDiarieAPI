using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class FWHDBusiness : IFWHDBusiness
    {
        private IFWHdRepository _repository;

        public FWHDBusiness(IFWHdRepository repository)
        {
            _repository = repository;
        }

        public List<FieldWorkHeader> FWHeaderList()
        {
          return  _repository.FWHeaderList();
        }

        public bool Save(FieldWorkHeader fwHeader)
        {
            return _repository.Save(fwHeader);
        }

        public bool Delete(int compid, String transid, String userid)
        {
            return _repository.Delete(compid, transid,  userid);
        }

        public bool OtherworkSave(FieldworkOthers owSave)
        {
            return _repository.OtherworkSave(owSave);
        }

        public List<FieldWorkHeader> GetEmpDateWiseFW(int compId, int uid, String empWorkDate, String periodOf)
        {
            return _repository.GetEmpDateWiseFW(compId,uid, empWorkDate, periodOf);
        }

        public FWSummary GetFieldworkSummary(int compId, int uid)
        {
            return _repository.GetFieldworkSummary(compId, uid);
        }

        public List<FWHeader4Report> GetFWMonthlyReport(int compId, int monthOf, int yearOf)
        {
            return _repository.GetFWMonthlyReport(compId, monthOf, yearOf);
        }
    }
}