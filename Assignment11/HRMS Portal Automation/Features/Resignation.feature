@resignation @regression @readOnly
Feature: Resignation date calculation
  Scenario: Last Working Date follows the required two month period
    Given I am logged in with valid credentials
    When I navigate to Organization My Profile
    And I open Employment Resignation
    Then the Last Working Date should follow the two month resignation calculation

  @calculation @noBrowser
  Scenario Outline: Resignation calculation handles calendar boundaries
    Given a resignation Date of Apply "<dateOfApply>"
    When the configured resignation notice period is calculated
    Then the calculated Last Working Date should follow the configured calendar-month rule

    Examples:
      | dateOfApply |
      | 2026-01-15  |
      | 2026-01-31  |
      | 2026-11-30  |
      | 2024-02-29  |
