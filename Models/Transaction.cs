using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Finportal.Enum;
namespace Finportal.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Display(Name = "Purchased Item")]
        public int? CategoryItemId { get; set; }

        [Display(Name = "Bank Account")]
        public int BankAccountId { get; set; }

        public string FPUserId { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        public TransactionType Type { get; set; }

        [StringLength(450,ErrorMessage ="The{0} must be at least {2} and at max {1} characters long.", MinimumLength =2)]
        public string Memo { get; set; }

        [Column(TypeName ="decimal(10,2)")]
        public decimal Amount { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public bool IsDeleted { get; set; }

        public CategoryItem CategoryItem{ get; set; }


        public BankAccount BankAccount { get; set; }

        public FPUser FPUser{ get; set; }
    }
}
