using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace graduated_project.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class ProductOrder
    {
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        
        public int Quantity { get; set; } = 1;
    }
}
