@navigation @regression @readOnly
Feature: Left navigation
  @smoke
  Scenario: Required HRMS navigation links open their pages
    Given I am logged in with valid credentials
    When I verify all required left navigation links
    Then each required navigation page should open successfully

  Scenario: Navigation remains usable after page refresh and module toggling
    Given I am logged in with valid credentials
    When I navigate to Employee Directory
    And I refresh the current page and collapse and reopen Organization navigation
    Then navigation should remain usable with correct expanded and collapsed states
