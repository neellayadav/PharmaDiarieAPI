using System;
namespace PharmaDiaries.Models
{
    public class ProductModel
    {

        public int? CompID { get; set; }

        public int? prodcode { get; set; }

        public string? proddesc { get; set; }

        public string? prodtype { get; set; }

        public string? prodpack { get; set; }

        /// <summary>
        /// Stockist Price (SP) - The price at which stockist purchases
        /// </summary>
        public double prodprice { get; set; }

        /// <summary>
        /// Maximum Retail Price - The price displayed to end customers
        /// </summary>
        public double? MRP { get; set; }

        /// <summary>
        /// URL of the product image stored in R2 storage
        /// </summary>
        public string? ImageURL { get; set; }

        public bool? isActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

}

