@footer @regression @readOnly
Feature: Footer social links
  Scenario: Footer social media links open their respective pages
    Given I am logged in with valid credentials
    When I hover over and open all footer social media links
    Then all footer social media links should open valid respective pages
