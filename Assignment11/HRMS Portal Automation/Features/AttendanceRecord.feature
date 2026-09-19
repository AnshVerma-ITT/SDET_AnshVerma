@attendance @regression @readOnly
Feature: Attendance Record
  Scenario: Four day range and Weekly Off filtering are correct
    Given I am logged in with valid credentials
    When I navigate to Attendance Record
    And I select a four day attendance range containing Saturday and Sunday
    Then the attendance records and Weekly Off filter should be correct
