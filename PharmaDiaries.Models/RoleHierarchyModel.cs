using System;

namespace PharmaDiaries.Models
{
    /// <summary>
    /// Model for Role Hierarchy - stores role rankings
    /// </summary>
    public class RoleHierarchyModel
    {
        public int? Id { get; set; }
        public string? RoleName { get; set; }
        public int? RankLevel { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    /// <summary>
    /// Model for distinct roles from Lookup table with optional ranking
    /// </summary>
    public class DistinctRoleModel
    {
        public string? RoleName { get; set; }
        public int? RankLevel { get; set; }
        public int? HierarchyId { get; set; }
    }

    /// <summary>
    /// Request model for saving role hierarchy
    /// </summary>
    public class RoleHierarchySaveRequest
    {
        public string? RoleName { get; set; }
        public int RankLevel { get; set; }
        public int ModifiedBy { get; set; }
    }

    /// <summary>
    /// Request model for batch saving role hierarchies
    /// </summary>
    public class RoleHierarchyBatchSaveRequest
    {
        public List<RoleRankingItem>? RoleRankings { get; set; }
        public int ModifiedBy { get; set; }
    }

    /// <summary>
    /// Individual role ranking item for batch save
    /// </summary>
    public class RoleRankingItem
    {
        public string? RoleName { get; set; }
        public int RankLevel { get; set; }
    }
}
