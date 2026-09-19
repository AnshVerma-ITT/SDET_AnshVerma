@navigation @smoke @regression @readOnly
Feature: Left navigation
  Scenario: Required HRMS navigation links open their pages
    Given I am logged in with valid credentials
    When I verify all required left navigation links
    Then each required navigation page should open successfully
