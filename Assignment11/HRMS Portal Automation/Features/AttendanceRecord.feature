@attendance @regression @readOnly
Feature: Attendance Record
  @smoke
  Scenario: Four day range and Weekly Off filtering are correct
    Given I am logged in with valid credentials
    When I navigate to Attendance Record
    And I select a four day attendance range containing Saturday and Sunday
    Then the attendance records and Weekly Off filter should be correct

  Scenario Outline: Attendance ranges contain no records outside the selected dates
    Given I am logged in with valid credentials
    When I navigate to Attendance Record
    And I select a recent weekday attendance range of <days> days
    Then only records within the selected attendance range should appear

    Examples:
      | days |
      | 1    |
      | 2    |
      | 4    |
