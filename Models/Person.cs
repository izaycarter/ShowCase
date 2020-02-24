using System;
using System.ComponentModel.DataAnnotations;
namespace Kopis_Showcase.Models
{
    public class Person
    {
        public int PersonID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Gender")]
        public int GenderID { get; set; }

        public Gender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "MaritalStatus")]
        public int MaritalStatusID { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public string StreetAddressLine1 { get; set; }

        public string StreetAddressLine2 { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }

    
}
