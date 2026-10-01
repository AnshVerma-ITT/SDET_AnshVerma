@leave @regression
Feature: Leave Application
  As an HRMS employee
  I want to submit leave requests with required-field validation
  So that valid requests are created and incomplete requests are prevented

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Leaves Application"

  @smoke @dataMutation
  Scenario: Casual Leave request is applied successfully for the configured date range
    When I apply for Casual Leave using the configured date range
    Then the leave request success message should be displayed

  @negative @validation @readOnly
  Scenario Outline: Required Leave Application fields prevent incomplete submission
    When I submit a Leave Application without "<field>"
    Then the incomplete Leave Application should be rejected

    Examples:
      | field      |
      | leave type |
      | date       |
