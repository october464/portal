using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class Category
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "FirstName")]
        public string Name { get; set; }

      
        [Display(Name = "Description")]
        public string Description { get; set; }


        public virtual Household Household { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }



        public virtual ICollection<CategoryItem> CategoryItems { get; set; } 
        public Category()
        {
            Transactions = new HashSet<Transaction>();
            CategoryItems = new HashSet<CategoryItem>();
        }
    }
}
