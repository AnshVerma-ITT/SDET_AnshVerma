@bdd @checkout
Feature: SauceDemo checkout
  As a SauceDemo customer
  I want checkout validation and order completion
  So that valid purchases succeed and incomplete information is rejected

  Background:
    Given I am logged in to SauceDemo

  Scenario: Complete the Week 6 purchase journey
    When I sort products by price from low to high
    Then product prices should be in ascending order
    When I add these products to the cart
      | Product    |
      | backpack   |
      | bike light |
    And I open the shopping cart
    Then the cart should contain the selected products
    When I continue shopping
    And I add "bolt t-shirt" to the cart
    And I open the shopping cart
    Then the cart should contain 3 products
    When I start checkout
    And I submit valid checkout information
    Then the checkout overview should contain 3 products
    When I finish the order
    Then the order should be completed successfully

  Scenario Outline: Reject incomplete checkout information
    When I add "backpack" to the cart
    And I open the shopping cart
    And I start checkout
    And I submit "<checkout case>" checkout information
    Then checkout validation should reject the information

    Examples:
      | checkout case       |
      | missing first name  |
      | missing last name   |
      | missing postal code |
