using System;
using System.Collections.Generic;
using PharmaDiaries.Models;

namespace PharmaDiaries.BusinessContract
{
    public interface IRoleHierarchyBusiness
    {
        List<DistinctRoleModel> GetDistinctRoles();
        List<RoleHierarchyModel> GetRoleHierarchyList();
        bool SaveRoleHierarchy(RoleHierarchySaveRequest request);
        bool SaveRoleHierarchyBatch(RoleHierarchyBatchSaveRequest request);
    }
}
