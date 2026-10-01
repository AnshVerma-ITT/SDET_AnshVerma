@login @regression @readOnly
Feature: Login
  As an HRMS user
  I want login validation
  So that valid and invalid authentication behaviour is verified

  Background:
    Given I open the HRMS login page

  @smoke
  Scenario: Valid login opens Dashboard
    Then the login form should be displayed
    When I login with valid credentials
    Then the Dashboard should be opened

  @negative @accountRisk
  Scenario: Valid username with an invalid password is rejected
    When I submit the configured username with password "InvalidPassword!12345"
    Then authentication should be rejected

  @negative @accountRisk
  Scenario: Invalid username with a valid password is rejected
    When I submit username "invalid.automation.user@intimetec.com" with the configured password
    Then authentication should be rejected

  @negative
  Scenario: Invalid username and invalid password are rejected
    When I submit username "invalid.automation.user@intimetec.com" and password "InvalidPassword!12345"
    Then authentication should be rejected

  @negative
  Scenario Outline: Missing credentials are rejected by form validation
    When I submit the login form without the "<field>"
    Then login validation should prevent authentication

    Examples:
      | field    |
      | username |
      | password |
      | both     |

  @negative @accountRisk
  Scenario Outline: Whitespace around configured credentials is rejected
    When I add leading and trailing spaces to the configured "<field>"
    Then authentication should be rejected

    Examples:
      | field    |
      | username |
      | password |
