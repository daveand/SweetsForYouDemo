using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class OrdersDataTableModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string OrderPlaced { get; set; }
        public bool OrderState { get; set; }

        public int CustomerId { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }

    }
}
