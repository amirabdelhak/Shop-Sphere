using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using graduated_project.metadatamodels;

namespace graduated_project.viewmodels
{
    

    public class Registervm
    {

        [Required(ErrorMessage = "the username is required")]
        public string? username { get; set; }
        [Required(ErrorMessage = "the password is required")]
        [DataType(DataType.Password)]
        public string? password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string? confirmpassword { get; set; }
        [Required(ErrorMessage = "the fisrt name is required")]
        [StringLength(20)]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "the address is required")]
        [StringLength(20)]
        public string? Address { get; set; }
        public string? Mobilnum { get; set; }
       


    }
}
