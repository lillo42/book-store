using System;
using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Admin.Web.Contracts.Request
{
    public class AddUserRequest
    {
        /// <summary>
        /// E-mail
        /// </summary>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// First Name
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last Names
        /// </summary>
        [Required]
        public string LastNames { get; set; }
        
        /// <summary>
        /// Birth date
        /// </summary>
        [Required]
        public DateTime BirthDate { get; set; }
    }
}
