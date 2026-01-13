using PharmaDiaries.BusinessContract;
using PharmaDiaries.DataAccessContract.Repository;
using PharmaDiaries.Models;

namespace PharmaDiaries.Bussiness
{
    public class RoleHierarchyBusiness : IRoleHierarchyBusiness
    {
        private IRoleHierarchyRepository _repository;

        public RoleHierarchyBusiness(IRoleHierarchyRepository repository)
        {
            _repository = repository;
        }

        public List<DistinctRoleModel> GetDistinctRoles()
        {
            return _repository.GetDistinctRoles();
        }

        public List<RoleHierarchyModel> GetRoleHierarchyList()
        {
            return _repository.GetRoleHierarchyList();
        }

        public bool SaveRoleHierarchy(RoleHierarchySaveRequest request)
        {
            return _repository.SaveRoleHierarchy(request);
        }

        public bool SaveRoleHierarchyBatch(RoleHierarchyBatchSaveRequest request)
        {
            return _repository.SaveRoleHierarchyBatch(request);
        }
    }
}
