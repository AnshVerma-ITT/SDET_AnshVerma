@attendance @regression @readOnly
Feature: Attendance Record
  As an HRMS employee
  I want attendance filters to return the expected dates and statuses
  So that I can trust the attendance records shown to me

  @smoke
  Scenario: Weekly Off filter shows the selected weekend dates
    Given I am logged in with valid credentials
    When I navigate to "Attendance Record"
    And I select a completed four-day period that includes Saturday and Sunday
    Then one attendance record should appear for every selected date
    And the Weekly Off filter should show only the selected Saturday and Sunday

  Scenario Outline: Attendance results stay within the selected weekday period
    Given I am logged in with valid credentials
    When I navigate to "Attendance Record"
    And I select a completed weekday period of <days> days
    Then one attendance record should appear for every selected date
    And no attendance record should fall outside the selected period

    Examples:
      | days |
      | 1    |
      | 2    |
      | 4    |
