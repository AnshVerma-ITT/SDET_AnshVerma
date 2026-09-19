@login @regression @readOnly
Feature: Login
  As an HRMS user
  I want login validation
  So that valid and invalid authentication behaviour is verified

  @smoke
  Scenario: Valid login opens Dashboard
    Given I open the HRMS login page
    Then the login form should be displayed
    When I login with valid credentials
    Then the Dashboard should be opened

  @negative
  Scenario: Invalid login displays an error
    Given I open the HRMS login page
    When I login with invalid credentials
    Then the invalid login error should be displayed
