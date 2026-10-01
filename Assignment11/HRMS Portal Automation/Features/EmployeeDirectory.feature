@employeeDirectory @regression @readOnly
Feature: Employee Directory
  As an HRMS employee
  I want to filter and browse the employee directory
  So that directory results and pagination can be trusted

  Background:
    Given I am logged in with valid credentials
    And I navigate to "Employee Directory"

  @smoke
  Scenario: Director of Engineering filter returns matching employees in Table View
    When I filter Job Title as Director of Engineering and select Table View
    Then every returned employee should match the selected job title

  Scenario: Employee Directory pagination maintains valid page size and navigation states
    Then Employee Directory pagination should show no more than 12 records and have correct navigation states
