@leave @regression @dataMutation
Feature: Apply Leave
  Scenario: Casual Leave request is applied successfully
    Given I am logged in with valid credentials
    When I navigate to Leaves Application
    And I apply for Casual Leave using the configured date range
    Then the leave request success message should be displayed
