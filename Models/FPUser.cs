using Finportal.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finportal.Models
{
    public class FPUser : IdentityUser
    {


        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [Display(Name = "Avatar")]
        [NotMapped]
        [AllowedExtenstions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public int? HouseholdId { get; set; }

        public Household Household { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
        //public virtual ICollection<CategoryItem> CategoryItems { get; set; } = new HashSet<CategoryItem>();

        //public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

    }
}
