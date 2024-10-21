using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CucumberTest.StepDefinitions
{
    [Binding]
    public sealed class HotelBookingCreateStepDefinitions
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _isRoomAvailable;

        [Given(@"there is a hotel room available")]
        public void GivenThereIsAHotelRoomAvailable()
        {
            _isRoomAvailable = true;
        }

        [Given(@"the room is not booked from ""(.*)"" to ""(.*)""")]
        public void GivenTheRoomIsNotBookedFromTo(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;

            _isRoomAvailable = true;
        }

        [When(@"I book the room from ""(.*)"" to ""(.*)""")]
        public void WhenIBookTheRoomFromTo(DateTime startDate, DateTime endDate)
        {
            if (_isRoomAvailable && startDate >= DateTime.Now && endDate > startDate)
            {
                _isRoomAvailable = false;
            }
            else
            {
                _isRoomAvailable = false;
            }
        }

        [Then(@"the booking should be successful")]
        public void ThenTheBookingShouldBeSuccessful()
        {
            if (!_isRoomAvailable)
            {
                throw new Exception("Booking failed");
            }

        }
    }
}
