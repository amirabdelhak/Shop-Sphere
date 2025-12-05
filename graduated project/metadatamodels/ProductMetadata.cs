using graduated_project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace graduated_project.metadatamodels
{
    public class ProductMetadata
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The product name is required.")]
        [StringLength(100, ErrorMessage = "The product name cannot exceed 100 characters.")]

        public string? Name { get; set; }
        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        [Range(0.01,100000,ErrorMessage =("The price must be between 0.01 and 100,000."))]
        public decimal Pricebeforediscount { get; set; }
        [NotMapped]
        [Required(ErrorMessage ="please enter your image")]
        public IFormFile? Image { get; set; }
        public string? ImageName { get; set; }
        [Range(0, 100, ErrorMessage = "The discount must be between 0 and 100.")]
        public decimal? Discount { get; set; }
        [Required(ErrorMessage = "the product price is required")]
        [Range(0.01, 100000, ErrorMessage = ("The price must be between 0.01 and 100,000."))]
        public decimal Priceafterdiscount { get; set; }


        [ForeignKey("Category")]
        [Required(ErrorMessage = "please enter the category if this product")]

        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }
        public virtual ICollection<AppUserProduct>? AppUserProducts { get; set; }
    }
}
