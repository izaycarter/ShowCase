using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kopis_Showcase.Data;

namespace Kopis_Showcase.Models
{
    public class Person 
    {
        [Key]
        public int PersonID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int GenderID { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public virtual Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        
        public int MaritalStatusID { get; set; }

        [Required]
        [Display(Name = "MaritalStatus")]
        public virtual MaritalStatus MaritalStatus { get; set; }


        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string StreetAddressLine1 { get; set; }

        [Required]
        [StringLength(100)]
        public string StreetAddressLine2 { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "State should only be two letters" )]
        public string State { get; set; }

        [Required]
        public string Zip { get; set; }

        
    }

    
}
