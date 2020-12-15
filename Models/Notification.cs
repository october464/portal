using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsRead { get; set; }

        public Household Household { get; set; }

    }
    
}
