using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class Household
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "FirstName")]
        public string Name { get; set; }


        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Greeting")]
        public string Greeting { get; set; }

        public DateTime Established { get; set; }//CHeck leter to see if this is correct


        public virtual ICollection<FPUser> Members{ get; set; } 

        public virtual ICollection<BankAccount> BankAccounts { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<CategoryItem> CategoryItems { get; set; }


        public virtual ICollection<Invitation> Invitations { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } 

        public virtual ICollection<Attachment> Attachments { get; set; }

        public Household()
        {
            Members = new HashSet<FPUser>();
            BankAccounts = new HashSet<BankAccount>();
            Categories = new HashSet<Category>();
            CategoryItems = new HashSet<CategoryItem>();
            Invitations = new HashSet<Invitation>();
            Notifications = new HashSet<Notification>();
            Attachments = new HashSet<Attachment>();
        }
    }
}


