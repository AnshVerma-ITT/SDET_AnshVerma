@bdd @authentication
Feature: SauceDemo authentication
  As a SauceDemo user
  I want login validation
  So that only supported users enter the store

  Scenario: Login with the standard user
    Given I am on the SauceDemo login page
    When I login with valid credentials
    Then the inventory page should be displayed

  Scenario Outline: Reject invalid or incomplete credentials
    Given I am on the SauceDemo login page
    When I login using "<credential case>" credentials
    Then login should be rejected

    Examples:
      | credential case  |
      | invalid username |
      | invalid password |
      | missing username |
      | missing password |
      | locked user      |
