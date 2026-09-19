@dashboard @smoke @regression @readOnly
Feature: Dashboard calendar
  Scenario: Calendar displays the current date
    Given I am logged in with valid credentials
    When I navigate to the Dashboard
    Then the calendar should show the current date month and year
