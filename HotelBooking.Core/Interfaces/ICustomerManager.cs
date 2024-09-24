using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Core.Interfaces
{
    public interface ICustomerManager
    {
        Customer Get(int id);
        IEnumerable<Customer> GetAll();
        void Add(Customer customer);
        void Edit(Customer customer);
        void Remove(int id);
    }
}
