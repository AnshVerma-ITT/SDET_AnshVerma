@navigation @regression @readOnly
Feature: Left Navigation
  As an authenticated HRMS employee
  I want the left navigation to open each supported HRMS page
  So that every required module and submenu destination remains accessible

  Background:
    Given I am logged in with valid credentials

  @smoke
  Scenario Outline: Required HRMS navigation destinations open successfully
    When I navigate to "<page>"
    Then the current navigation destination should open successfully

    Examples:
      | page                    |
      | Dashboard               |
      | Organization My Profile |
      | Employee Directory      |
      | Attendance Record       |
      | Leaves Application      |
      | Leave Entitlements      |
      | Leave Correction        |
      | My Holidays             |

  Scenario: Navigation remains usable after refresh and Organization module toggling
    When I navigate to "Employee Directory"
    And I refresh the current page and collapse and reopen Organization navigation
    Then navigation should remain usable with correct expanded and collapsed states
