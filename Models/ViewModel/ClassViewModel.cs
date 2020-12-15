using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models.ViewModel
{
    public class ClassViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Greeting { get; set; }

        public DateTimeOffset Established { get; set; }

        public decimal HouseholdBalance { get; set; }

        public IFormFile FormFile { get; set; }



        public FPUser Owner { get; set; } = new FPUser();
        public List<FPUser> Members { get; set; } = new List<FPUser>();
        public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public List<CategoryItem> CategoryItems { get; set; } = new List<CategoryItem>();

        public List<Invitation> Invitations { get; set; } = new List<Invitation>();

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();






















    }
}
