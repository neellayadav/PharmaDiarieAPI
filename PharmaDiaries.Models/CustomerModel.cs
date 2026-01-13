using System;
namespace PharmaDiaries.Models
{
	public class CustomerModel
	{

            public int? CompID { get; set; }

            public int? CustID { get; set; }

            public string? Name { get; set; }

            public string? Type { get; set; }

            public bool? IsListed { get; set; }

            public string? QUALIFICATION { get; set; }

            public string? Speciality { get; set; }  

            public string? HeadQuater { get; set; }

            public string? Patch { get; set; }

            public string? Address1 { get; set; }

            public string? Locality { get; set; }

            public string? CityOrTown { get; set; }

            public string? Pincode { get; set; }

            public string? District { get; set; }

            public string? State { get; set; }

            public string? Country { get; set; }

            public string? Mobile { get; set; }

            public string? Telephone { get; set; }

            public bool? IsActive { get; set; }

            public decimal? Latitude { get; set; }

            public decimal? Longitude { get; set; }

            public int? CreatedBy { get; set; }

            public DateTime? CreatedOn { get; set; }

            public int? ModifiedBy { get; set; }

            public DateTime? ModifiedOn { get; set; }

    }

    public class CustomerLocationUpdateRequest
    {
        public int CompID { get; set; }
        public int CustID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ModifiedBy { get; set; }
    }

    public class CustomerLocationResponse
    {
        public int CustID { get; set; }
        public string? Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class DeleteCustomerModel
    {

        public int? CompID { get; set; }

        public int? CustID { get; set; }

        public string? ModifiedBy { get; set; }

    }

}

