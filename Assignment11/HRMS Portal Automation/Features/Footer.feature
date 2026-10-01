@footer @regression @readOnly
Feature: Footer Social Links
  As an authenticated HRMS employee
  I want the footer social-media links to point to their configured services
  So that external company links are valid and open as intended

  Background:
    Given I am logged in with valid credentials

  Scenario: Configured footer social-media links open their respective pages
    When I hover over and open all footer social media links
    Then all footer social media links should open valid respective pages
