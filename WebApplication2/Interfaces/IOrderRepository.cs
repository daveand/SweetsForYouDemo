using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Interfaces
{
    public interface IOrderRepository
    {
        void CreateOrder(OrderModel order);
    }
}
