using HotelBooking.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Core.Services
{
    public class CustomerManager: ICustomerManager
    {
        private readonly IRepository<Customer> repository;

        public CustomerManager(IRepository<Customer> repos)
        {
            repository = repos;
        }

        public Customer Get(int id) {
            if (id < 0) {
                throw new ArgumentException("Id must be a positive integer");
            }
            return repository.Get(id);
        }

        public IEnumerable<Customer> GetAll() {
            return repository.GetAll();
        }

        public void Add(Customer customer) {
            if (customer == null) {
                throw new ArgumentNullException("Customer cannot be null");
            }
            repository.Add(customer);
        }

        public void Edit(Customer customer) {
            if (customer == null) {
                throw new ArgumentNullException("Customer cannot be null");
            }
            repository.Edit(customer);
        }

        public void Remove(int id) {
            if (id < 0) {
                throw new ArgumentException("Id must be a positive integer");
            }
            repository.Remove(id);
        }
    }
}
