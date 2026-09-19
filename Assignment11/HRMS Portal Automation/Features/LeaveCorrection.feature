@leaveCorrection @regression @dataMutation
Feature: Apply Leave Correction
  Scenario: Work from home correction with one half day is applied successfully
    Given I am logged in with valid credentials
    When I navigate to Leave Correction
    And I apply a two day Work from Home correction with one half day
    Then the leave correction success message and created record should be displayed
