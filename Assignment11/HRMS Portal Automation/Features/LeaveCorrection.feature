@leaveCorrection @regression
Feature: Leave Correction
  As an HRMS employee
  I want to submit leave corrections with required-field validation
  So that valid corrections are recorded and incomplete corrections are prevented

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Leave Correction"

  @smoke @dataMutation
  Scenario: Two-day Work from Home correction with one half day is applied successfully
    When I apply a two day Work from Home correction with one half day
    Then the leave correction success message and created record should be displayed

  @negative @validation @readOnly
  Scenario Outline: Required Leave Correction fields prevent incomplete submission
    When I submit a Leave Correction without "<field>"
    Then the incomplete Leave Correction should be rejected

    Examples:
      | field           |
      | correction type |
      | date            |
