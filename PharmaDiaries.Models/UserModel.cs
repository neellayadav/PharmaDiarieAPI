using System;
namespace PharmaDiaries.Models
{
    public class UserModel
    {
        public int? CompID { get; set; }

        public int? UID { get; set; }

        public string? UserID { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? HeadQuater { get; set; }

        public string? Address1 { get; set; }

        public string? Locality { get; set; }

        public string? CityOrTown { get; set; }

        public int? Pincode { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? Mobile { get; set; }

        public string? Telephone { get; set; }

        public string? ProfileImageURL { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsCompAdmin { get; set; }

        public int? RoleID { get; set; }

        public string? RoleName { get; set; }

        public int? ReportingManagerID { get; set; }

        public string? ReportingManagerName { get; set; }

        public string emailid { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    public class SignUpModel
    {
        public string? UserID { get; set; }

        public string? Password { get; set; }

        public string? Mobile { get; set; }
    }

    public class LoginUserModel
    {
        public int? CompID { get; set; }

        public string? UserID { get; set; }

        public string? Password { get; set; }
    }

    public class ResetPasswordModel
    {
        public int? CompID { get; set; }

        public int? UsrId { get; set; }

        public string? Password { get; set; }
    }

    public class ScreenModel
    {
        public int? ScreenID { get; set; }

        public string? ScreenName { get; set; }

        public string? ScreenRoute { get; set; }

        public string? ScreenDescription { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

    public class UserScreenPermissionModel
    {
        public int? PermissionID { get; set; }

        public int? UserID { get; set; }

        public int? ScreenID { get; set; }

        public bool? HasAccess { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    public class UserScreenPermissionRequest
    {
        public int? UserID { get; set; }

        public List<int>? ScreenIDs { get; set; }

        public int? CreatedBy { get; set; }
    }

    public class UserWithPermissionsModel
    {
        public UserModel? User { get; set; }

        public List<int>? ScreenIDs { get; set; }
    }

    public class DeleteUserByUserIDRequest
    {
        public int CompID { get; set; }

        public string UserID { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

