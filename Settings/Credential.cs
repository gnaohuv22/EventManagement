using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace assignment3_b3w.Settings
{
    public class Credential
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
