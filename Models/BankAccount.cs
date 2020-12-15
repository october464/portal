using Finportal.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        
        [StringLength(50)]
        public string FPUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        public AccountType Type { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal StartingBalance { get; set; }



        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentBalance { get; set; }

        public FPUser FPUser { get; set; }//get rid of and add fpuserid at the top

        public Household Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
