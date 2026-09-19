@logout @smoke @regression @readOnly
Feature: Logout functionality
  Scenario: Logged-in user can logout successfully
    Given I am logged in with valid credentials
    When I logout from the profile menu
    Then I should be redirected to the login page
