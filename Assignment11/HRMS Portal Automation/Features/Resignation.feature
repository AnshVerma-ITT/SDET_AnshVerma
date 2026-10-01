@resignation @regression @readOnly
Feature: Resignation Date Calculation
  As an HRMS employee
  I want the resignation form to calculate the required last working date
  So that the configured notice period is applied correctly

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Organization My Profile"

  Scenario: Last Working Date follows the required two-month resignation period
    When I open Employment Resignation
    Then the Last Working Date should follow the two month resignation calculation
