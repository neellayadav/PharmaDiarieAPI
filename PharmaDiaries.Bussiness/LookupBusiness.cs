using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class LookupBusiness : ILookupBusiness
    {
        private ILookupRepository _repository;

        public LookupBusiness(ILookupRepository repository)
        {
            _repository = repository;
        }

        public List<Lookup> Lookuplist()
        {
            return _repository.Lookuplist();
        }

        public List<Lookup> LookupListByType(string type)
        {
            return _repository.LookupListByType(type);
        }

        public bool DeleteLookUp(int compID, string code)
        {
            return _repository.DeleteLookUp(compID, code);
        }
    }
}

