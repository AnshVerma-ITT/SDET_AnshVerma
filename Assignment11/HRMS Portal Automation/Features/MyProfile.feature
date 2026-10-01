@profile @regression @readOnly
Feature: My Profile
  As an HRMS employee
  I want my profile information and work scheme to be displayed correctly
  So that I can verify the employment information recorded for my account

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Organization My Profile"

  @smoke
  Scenario: Personal information matches the expected employee data
    Then the top personal information should match the expected employee data

  Scenario: Job information matches the expected employee data
    When I open Job and Skills
    Then the Job section should match the expected job data

  Scenario: Current work scheme marks the weekend as week off
    When I open Job and Skills
    Then the current work scheme should show Saturday and Sunday as week off

  Scenario: Scheme Details can be closed and reopened
    When I open Job and Skills
    And I open close and reopen the current Scheme Details
    Then the Scheme Details dialog should be visible
