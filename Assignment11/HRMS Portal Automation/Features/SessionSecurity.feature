@session @regression @readOnly
Feature: Session authorization
  Scenario: Authenticated pages remain protected after logout
    Given I am logged in with valid credentials
    When I logout from the profile menu
    Then I should be redirected to the login page
    When I attempt direct access to every protected route then refresh and navigate back
    Then every post-logout attempt should still require authentication
