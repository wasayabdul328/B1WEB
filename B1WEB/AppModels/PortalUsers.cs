using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace B1WEB.AppModels
{
    public class PortalUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string CNIC { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public virtual ICollection<UserCompany>? UserCompanies { get; set; }
        public virtual ICollection<UserPermission>? UserPermissions { get; set; }
    }
    public class UserCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int DatabaseId { get; set; }
        public int? UserID { get; set; }

        [ForeignKey("PortalUsersId")]
        public int? PortalUsersId { get; set; }
        public PortalUsers? PortalUsers { get; set; }


    }


    public class UserPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? UserID { get; set; }
        public int FormId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanView { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        [ForeignKey("PortalUsersId")]
        public int? PortalUsersId { get; set; }
        public PortalUsers? PortalUsers { get; set; }
    }



}
