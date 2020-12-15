using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ActualAmount { get; set; }
        public virtual Category Category { get; set; }

        public Guid Code { get; set; }


        public virtual ICollection<CategoryItem> CategoryItems { get; set; }  = new HashSet<CategoryItem>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
