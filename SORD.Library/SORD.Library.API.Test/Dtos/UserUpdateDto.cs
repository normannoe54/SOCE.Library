using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SORD.Library.API.Test.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public string FirsName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailAdress { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
