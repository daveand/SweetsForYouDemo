using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int Participants { get; set; }
        public string PersonalNumber { get; set; }
        public string Receiver { get; set; }
        public string PayMethod { get; set; }
        public string PayNumber { get; set; }


        public IdentityUser User { get; set; }
        public List<OrderModel> Orders { get; set; }

    }
}
