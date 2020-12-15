using Finportal.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Expires { get; set; }
        public bool Accepted { get; set; }

        public bool IsValid { get; set; }

        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength =2)]
        public string EmailTo { get; set; }
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Subject { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Body { get; set; }


        public PortalRole RoleName { get; set; }

        public Guid Code { get; set; }//check this whole class

        public virtual Household Household { get; set; }
    }
}
