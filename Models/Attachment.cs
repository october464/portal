using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finportal.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        public string Description { get; set; }

        public string ContentType { get; set; }

        public byte[] FileData { get; set; }
    }
}
