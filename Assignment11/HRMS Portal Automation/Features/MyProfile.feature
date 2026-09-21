@profile @smoke @regression @readOnly
Feature: My Profile
  Scenario: Personal job and work scheme information is correct
    Given I am logged in with valid credentials
    When I navigate to Organization My Profile
    Then the top personal information should match the expected employee data
    When I open Job and Skills
    Then the Job section should match the expected job data
    And the current work scheme should show Saturday and Sunday as week off
