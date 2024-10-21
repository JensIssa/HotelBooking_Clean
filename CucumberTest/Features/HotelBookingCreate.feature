Feature: CreateBooking


  @mytag
  Scenario: Create a booking for an available room
    Given there is a hotel room available
    And the room is not booked from "2024-10-25" to "2024-10-30"
    When I book the room from "2024-10-25" to "2024-10-30"
    Then the booking should be successful