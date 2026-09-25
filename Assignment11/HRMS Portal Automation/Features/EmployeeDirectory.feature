@employeeDirectory @regression @readOnly
Feature: Employee Directory filtering
  @smoke
  Scenario: Directors of Engineering are shown in Table View
    Given I am logged in with valid credentials
    When I navigate to Employee Directory
    And I filter Job Title as Director of Engineering and select Table View
    Then every returned employee should match the selected job title

  Scenario: Employee Directory pagination works correctly
    Given I am logged in with valid credentials
    When I navigate to Employee Directory
    Then Employee Directory pagination should show no more than 12 records and have correct navigation states
