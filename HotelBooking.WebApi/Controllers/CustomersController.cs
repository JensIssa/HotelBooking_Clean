using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace HotelBooking.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomerManager _customerManager;

        public CustomersController(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        // GET: customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _customerManager.GetAll();
        }

    }
}
