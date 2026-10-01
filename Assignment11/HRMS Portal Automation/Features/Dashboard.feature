@dashboard @smoke @regression @readOnly
Feature: Dashboard Calendar
  As an authenticated HRMS employee
  I want the dashboard calendar to represent the current date correctly
  So that the dashboard shows the correct calendar period

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Dashboard"

  Scenario: Dashboard calendar displays the current date, month and year
    Then the calendar should show the current date month and year
