using System;

namespace PharmaDiaries.Models
{
    public class AllModels
    {

    }


    public class Lookup
    {
        public int? CompID { get; set; }

        public int? code { get; set; }

        public string? description { get; set; }

        public string? type { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }


}

