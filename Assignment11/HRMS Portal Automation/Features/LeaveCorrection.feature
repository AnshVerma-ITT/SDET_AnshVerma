@leaveCorrection @regression
Feature: Apply Leave Correction
  @smoke @dataMutation
  Scenario: Work from home correction with one half day is applied successfully
    Given I am logged in with valid credentials
    When I navigate to Leave Correction
    And I apply a two day Work from Home correction with one half day
    Then the leave correction success message and created record should be displayed

  @negative @validation @readOnly
  Scenario Outline: Required Leave Correction fields prevent incomplete submission
    Given I am logged in with valid credentials
    When I navigate to Leave Correction
    And I submit a Leave Correction without "<field>"
    Then the incomplete Leave Correction should be rejected

    Examples:
      | field           |
      | correction type |
      | date            |
