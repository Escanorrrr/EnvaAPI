using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class CustomerListDTO
    {
        public long Id { get; set; }

        
        public string Name { get; set; }


        public string LastName { get; set; }


        public string Email { get; set; }


        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
} 