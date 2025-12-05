using graduated_project.metadatamodels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace graduated_project.Models
{
    [MetadataType(typeof(Ordermetadata))]
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }
        public virtual ICollection<ProductOrder>?  ProductOrders { get; set; } = new List<ProductOrder>();

    }
    
}
