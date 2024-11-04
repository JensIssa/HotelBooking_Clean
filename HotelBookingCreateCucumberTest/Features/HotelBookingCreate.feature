Feature: HotelBookingCreate

Simple feature for creating a hotel booking reservation. 

@tag1
Scenario: Create a booking for an available room
    Given there is a hotel room available
    And the room is not booked from "2024-11-25" to "2024-11-30"
    When I book the room from "2024-11-25" to "2024-11-30"
    Then the booking should be successful

@tag2
Scenario: Create a booking for a room that is already booked
    Given there is a hotel room available
    And the room is booked from "2024-11-25" to "2024-11-30"
    When I book the room from "2024-11-27" to "2024-11-29"
    Then the booking should be unsuccessful

#@tag3
#Scenario: Create a booking with overlapping dates
#    Given there is a hotel room available
#    And the room is booked from "2024-10-26" to "2024-10-28"
#    When I book the room from "2024-10-25" to "2024-10-27"
#    Then the booking should be unsuccessful
#

#@tag5
#Scenario: Book a room with invalid date range, where start date is after end date
#    Given there is a hotel room available
#    When I try to book the room from "2024-11-10" to "2024-11-05"
#    Then the booking should be unsuccessful

#
#
#@tag6
#Scenario: Book a room without specifying dates
#    Given there is a hotel room available
#    When I attempt to book the room without providing check-in and check-out dates
#    Then the booking should be unsuccessful
#
#@tag7
#Scenario: Book a room with exact date conflict
#    Given there is a hotel room available
#    And the room is booked from "2024-10-25" to "2024-10-30"
#    When I try to book the room from "2024-10-25" to "2024-10-30"
#    Then the booking should be unsuccessful
#

#@tag8 
#    Scenario: Book a room with check-in date in the past
#	Given there is a hotel room available
#	When I try to book the room with a check-in date in the past
#	Then the booking should be unsuccessful
