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
  Scenario Outline: Login rejects invalid or incomplete credential combinations
    Given I open the HRMS login page
    When I submit login using "<username>" username and "<password>" password
    Then the login result should be "<expectedResult>"

    Examples:
      | username       | password        | expectedResult |
      | valid          | invalid         | error          |
      | invalid        | valid           | error          |
      | invalid        | invalid         | error          |
      | blank          | valid           | validation     |
      | valid          | blank           | validation     |
      | blank          | blank           | validation     |
      | validWithSpaces| valid           | error          |
      | valid          | validWithSpaces | error          |
