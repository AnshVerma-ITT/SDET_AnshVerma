@leave @regression
Feature: Apply Leave
  @smoke @dataMutation
  Scenario: Casual Leave request is applied successfully
    Given I am logged in with valid credentials
    When I navigate to Leaves Application
    And I apply for Casual Leave using the configured date range
    Then the leave request success message should be displayed

  @negative @validation @readOnly
  Scenario Outline: Required Leave Application fields prevent incomplete submission
    Given I am logged in with valid credentials
    When I navigate to Leaves Application
    And I submit a Leave Application without "<field>"
    Then the incomplete Leave Application should be rejected

    Examples:
      | field      |
      | leave type |
      | date       |
