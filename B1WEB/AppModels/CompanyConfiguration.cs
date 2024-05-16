using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace B1WEB.AppModels
{
    public class CompanyConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string DatabaseName { get; set; }

        public string CompanyLogo { get; set; }

        public string ServiceLayerURL { get; set; }

        public string ServiceLayerUsername { get; set; }

        public string ServiceLayerPassword { get; set; }

        public bool DefaultCompany { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int CreatedByUserID { get; set; }

        public int UpdatedByUserID { get; set; }

    }
}
