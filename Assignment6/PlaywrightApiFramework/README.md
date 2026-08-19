# Playwright API Framework Assignment

## Project Details

| Field | Value |
| --- | --- |
| Project Name | Playwright API Framework Assignment |
| Application Type | API Automation Testing Framework |
| API Under Test | ReqRes |
| Developed By | Ansh Verma |
| Language | C# |
| Test Framework | NUnit |
| Automation Tool | Playwright |
| Platform | .NET Test Project |

## Project Description

This project is a C# Playwright API testing framework created for the ReqRes API assignment.

The framework validates REST API requests using Playwright request context. It covers GET, POST, PUT, PATCH, and DELETE methods with response status code, response body, response attributes, and response header validations.

The project also demonstrates test data management using JSON, CSV, Excel test data files, environment variables, and random data generation.

The code is separated into reusable framework classes, ReqRes-specific service classes, endpoint classes, models, test data, and NUnit test scenarios.

## Framework Summary

Reusable framework code is kept inside the Framework folder.

ReqRes-specific code is kept inside the ReqRes folder.

Test scenarios are kept inside the Tests folder.

AppSettings loads the base URL, API key, and authorization header name from the .env file.

AuthManager creates common request headers using the configured authorization header name.

ApiFixture creates and closes the Playwright API request context using NUnit OneTimeSetUp and OneTimeTearDown.

ApiClient contains reusable methods for GET, POST, PUT, PATCH, DELETE, and content-type based request handling.

UserService contains ReqRes user API actions.

UserEndpoints contains ReqRes endpoint paths.

ReportHelper prints clear terminal report lines during test execution.

## Mentor Change Summary

Authorization header name was moved to configuration so x-api-key can be changed later without code changes.

ApiClient now uses a contentType variable with if-else handling for JSON, XML, form-data, and raw text requests.

Common NUnit OneTimeSetUp and OneTimeTearDown were moved to a base test class.

Authorization testing was separated into its own test class.

Negative testing now includes one real failing ReqRes persistence test inside UserTests because created users are not saved by ReqRes.

HTTP response status values now use HttpStatusCode based constants.

Only reusable content types and Content-Type header value are kept in framework constants.

Models were simplified back to one ReqRes model file: User.cs.

UserDataProvider and UserScenarioData are kept in Tests/DataProviders because they support NUnit test data management.

Assertion failure messages are handled inside ApiAssert with endpoint and field details.

Tests now use UserEndpoints instead of hardcoded endpoint strings.

Test data provider methods were moved out of test classes.

Hardcoded test data was moved into JSON, CSV, and scenario data files.

## Features Implemented

### 1. API Request Testing

GET Users

GET Single User

POST Create User

PUT Update User

PATCH Update User Attribute

DELETE User

POST Register Negative Scenario

### 2. API Validations

HTTP status code validation

Response body validation

Response attribute validation

Response header validation

Content-Type header validation

Empty response body validation for DELETE request

Error message validation for negative tests

### 3. Content Type Testing

JSON request

XML request

Form-data request

Raw text request

ApiClient handles these content types using the contentType variable:

| Content Type Constant | Actual Value | Used For |
| --- | --- | --- |
| ApiConstants.ApplicationJson | application/json | Normal JSON request body |
| ApiConstants.ApplicationXml | application/xml | XML request body |
| ApiConstants.FormData | multipart/form-data | Form-data request body |
| ApiConstants.TextPlain | text/plain | Raw text request body |

## Model Files

| File | Purpose |
| --- | --- |
| ReqRes/Models/User.cs | Single simple model used for create, update, patch, and create-response validation. Contains Id, Name, Job, Email, CreatedAt, and UpdatedAt. |

GET list, GET single user, and error response bodies are read directly with JsonHelper because those responses have nested JSON.

This keeps the model layer simple and avoids extra wrapper model files.

## Assertion Message Usage

| ApiAssert Method | Used For |
| --- | --- |
| Status | Shows which endpoint returned the wrong HTTP status. |
| HeaderContains | Shows which response header did not contain the expected value. |
| FieldEquals | Shows which response body field had the wrong value. |
| FieldContains | Shows which response body field did not contain expected text. |
| FieldNotEmpty | Shows which response field was empty. |
| GreaterThanZero | Shows which number field was not greater than zero. |
| ArrayNotEmpty | Shows which response array was empty. |
| EmptyBody | Shows when DELETE response body was not empty. |

The intentional ReqRes persistence failure message is written in the test because it is ReqRes-specific, not framework reusable.

## HTTP Status Code Values

HTTP status values are written in Framework/Constants/HttpStatusCodes.cs.

That file gets values from the C# System.Net.HttpStatusCode enum and converts them to int.

| Constant Used In Tests | Actual HTTP Value | Meaning |
| --- | --- | --- |
| HttpStatusCodes.Ok | 200 | OK |
| HttpStatusCodes.Created | 201 | Created |
| HttpStatusCodes.NoContent | 204 | No Content |
| HttpStatusCodes.BadRequest | 400 | Bad Request |
| HttpStatusCodes.Unauthorized | 401 | Unauthorized |
| HttpStatusCodes.NotFound | 404 | Not Found |

### 4. Test Data Management

JSON test data source

CSV test data source

Excel test data file

Environment variables using .env file

Random test data using C# Random class

### 5. API Chaining

The framework first calls GET /api/users?page=2.

It takes a user id from the response.

It then uses the same user id in GET /api/users/{id}.

This proves that data from one API response can be reused in another API request.

### 6. Parallel Execution

The framework executes multiple API requests together using Task.WhenAll.

The parallel test sends:

GET /api/users?page=1

GET /api/users/2

GET /api/users/3

All three responses are validated after the parallel execution completes.

### 7. Terminal Reporting

Each test prints useful execution details in the terminal.

The report output includes test name, request endpoint, response status code, response status text, chained user id, and parallel request results.

## Test Cases Implemented

| No. | Test Scenario | Method | Endpoint | Expected Status |
| --- | --- | --- | --- | --- |
| 1 | Create user using JSON data | POST | /api/users | 201 |
| 2 | Create user using CSV data | POST | /api/users | 201 |
| 3 | Get users page 2 | GET | /api/users?page=2 | 200 |
| 4 | Update user | PUT | /api/users/2 | 200 |
| 5 | Patch user job | PATCH | /api/users/2 | 200 |
| 6 | Delete user | DELETE | /api/users/2 | 204 |
| 7 | JSON content type request | POST | /api/users | 201 |
| 8 | XML content type request | POST | /api/users | 201 |
| 9 | Form-data content type request | POST | /api/users | 201 |
| 10 | Raw text content type request | POST | /api/users | 201 |
| 11 | Dynamic random data request | POST | /api/users | 201 |
| 12 | API chaining | GET + GET | /api/users?page=2 and /api/users/{id} | 200 |
| 13 | Parallel execution | GET | Multiple user endpoints | 200 |
| 14 | Missing user negative test | GET | /api/users/23 | 404 |
| 15 | Register without password | POST | /api/register | 400 |
| 16 | Missing API key negative test | GET | /api/users/2 | 401 |
| 17 | Created user persistence failure | POST + GET | /api/users and /api/users/{id} | Expected failure |

## C# and Testing Concepts Used

Classes and Objects

Properties

Methods

Static Classes

Collections

List<T>

Dictionary<TKey, TValue>

Async and Await

Task.WhenAll

Conditional Statements

Loops

File Handling

JSON Deserialization

Environment Variables

Random Class

Namespaces

Separation of Responsibilities

DRY Principle

NUnit Framework

TestFixture

Test

TestCaseSource

OneTimeSetUp

OneTimeTearDown

API Assertions

Playwright APIRequestContext

## How To Run

Open the project in Visual Studio or Visual Studio Code.

Open the .env file.

Add your ReqRes API key:

```text
BASE_URL=https://reqres.in
API_KEY=your_actual_api_key_here
AUTH_HEADER_NAME=x-api-key
REQRES_EMAIL_DOMAIN=@reqres.in
```

Open Terminal in the project folder:

Restore packages:

```bash
dotnet restore
```

Run all tests:

```bash
dotnet test
```

Run tests with full terminal report:

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Expected Terminal Output

NUnit should discover and execute all test cases.

All normal API tests should pass when the API key is correct.

The created user persistence test is expected to fail because ReqRes does not save newly created users.

The terminal report should show request details such as:

```text
========== API chaining ==========
STEP: Call list users first
GET /api/users?page=2 -> 200 OK
Chained user id: 7
STEP: Use chained user id in second request
GET /api/users/7 -> 200 OK
```

The parallel execution report should show:

```text
========== Parallel execution ==========
STEP: Create three API requests before waiting for result
STEP: Await all requests together using Task.WhenAll
Parallel request 1 - GET /api/users?page=1 -> 200 OK
Parallel request 2 - GET /api/users/2 -> 200 OK
Parallel request 3 - GET /api/users/3 -> 200 OK
```

Final summary with the intentional persistence failure should show:

```text
Test summary: total: 19, failed: 1, succeeded: 18, skipped: 0
```

## Expected Output

Users can be fetched from ReqRes.

Users can be created using JSON and CSV test data.

Users can be updated using PUT.

User attributes can be updated using PATCH.

Users can be deleted using DELETE.

Different content types can be sent in API requests.

Negative scenarios return proper error status codes.

The persistence failure test shows that ReqRes created users are not actually stored.

API chaining reuses data from one response in another request.

Parallel execution sends multiple API requests together.

The terminal displays a readable test execution report.

## Conclusion

This project demonstrates API automation testing using Playwright with C# and NUnit.

It covers API request creation, request headers, request body, response handling, assertions, test data management, environment variables, API chaining, and parallel execution in a simple framework structure.
