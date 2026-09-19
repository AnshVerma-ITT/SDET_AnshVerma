@resignation @regression @readOnly
Feature: Resignation date calculation
  Scenario: Last Working Date follows the required two month period
    Given I am logged in with valid credentials
    When I navigate to Organization My Profile
    And I open Employment Resignation
    Then the Last Working Date should follow the two month resignation calculation
