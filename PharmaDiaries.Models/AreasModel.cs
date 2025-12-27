using System;
using System.Text.Json.Serialization;

namespace PharmaDiaries.Models
{
	public class AreasModel
	{

        public int AreaID { get; set; }

        public int? CompID { get; set; }

        public string? Region { get; set; }

        public string? HeadQuater { get; set; }

        public string? Patch { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        

    }

    public class RegionModel
    {
        public string? Region { get; set; }

    }

    public class HeadQuaterModel
    { 
        public string? HeadQuater { get; set; }

    }

    public class PatchModel
    {
        public string? Patch { get; set; }

    }
}

