using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.DataAccessContract.Repository
{
    public interface IRoleHierarchyRepository
    {
        List<DistinctRoleModel> GetDistinctRoles();
        List<RoleHierarchyModel> GetRoleHierarchyList();
        bool SaveRoleHierarchy(RoleHierarchySaveRequest request);
        bool SaveRoleHierarchyBatch(RoleHierarchyBatchSaveRequest request);
    }
}
