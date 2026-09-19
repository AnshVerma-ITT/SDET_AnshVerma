@employeeDirectory @regression @readOnly
Feature: Employee Directory filtering
  Scenario: Directors of Engineering are shown in Table View
    Given I am logged in with valid credentials
    When I navigate to Employee Directory
    And I filter Job Title as Director of Engineering and select Table View
    Then Archit Jain, Kapil Paliwal, and Yatin Yogi should be present

  Scenario: Employee Directory pagination works correctly
    Given I am logged in with valid credentials
    When I navigate to Employee Directory
    Then Employee Directory pagination should show no more than 12 records and have correct navigation states
