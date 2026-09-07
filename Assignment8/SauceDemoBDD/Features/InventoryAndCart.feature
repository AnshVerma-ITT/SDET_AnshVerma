@bdd @inventory @cart
Feature: SauceDemo inventory and cart
  As a logged-in customer
  I want to sort and manage products
  So that my cart contains the products I selected

  Background:
    Given I am logged in to SauceDemo

  Scenario: Sort products by price from low to high
    When I sort products by price from low to high
    Then product prices should be in ascending order

  Scenario: Add selected products and retain them after continuing shopping
    When I add these products to the cart
      | Product    |
      | backpack   |
      | bike light |
    Then the cart badge should show 2 items
    When I open the shopping cart
    Then the cart should contain the selected products
    When I continue shopping
    Then the inventory page should be displayed
    And the cart badge should show 2 items

  Scenario: Remove a product from the inventory
    When I add "backpack" to the cart
    Then the cart badge should show 1 item
    When I remove "backpack" from the inventory
    Then the cart badge should show 0 items
