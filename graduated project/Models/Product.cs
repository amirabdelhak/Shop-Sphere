using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using graduated_project.metadatamodels;


namespace graduated_project.Models
    

{
    [MetadataType(typeof(ProductMetadata))]
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Pricebeforediscount { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageName {  get; set; }
        public decimal? Discount { get; set; }
        
        [NotMapped]
        public decimal Priceafterdiscount
        {
            get
            {
                if (Discount.HasValue && Discount > 0)
                {
                    return Pricebeforediscount - (Pricebeforediscount * (Discount.Value / 100));
                }
                return Pricebeforediscount;
            }
        }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }
        public virtual ICollection<AppUserProduct>? AppUserProducts { get; set; }


    }
}
