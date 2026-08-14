# Playwright API Framework Test Execution Report

## Execution Summary

Project: Playwright API Framework  
API Under Test: ReqRes  
Test Framework: NUnit  
Execution Result: Passed  

Command used:

```bash
dotnet test
```

Console output summary:

```text
NUnit3TestExecutor discovered 18 of 18 NUnit test cases
Test summary: total: 18, failed: 0, succeeded: 18, skipped: 0
Build succeeded
```

## Test Cases Executed

| No. | Test Name | Type | API Method | Endpoint | Expected Status | Main Validation |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | CreateUser_WithJsonDataProvider - Rahul | Regression | POST | /api/users | 201 | Validates response status, JSON header, name, job, id, and createdAt |
| 2 | CreateUser_WithJsonDataProvider - Morpheus | Regression | POST | /api/users | 201 | Validates response status, JSON header, name, job, id, and createdAt |
| 3 | CreateUser_WithCsvDataProvider - Rahul | Regression | POST | /api/users | 201 | Validates response status, name, and job from CSV test data |
| 4 | CreateUser_WithCsvDataProvider - Morpheus | Regression | POST | /api/users | 201 | Validates response status, name, and job from CSV test data |
| 5 | GetUsers_PageTwo_ShouldValidateBodyAndHeaders | Regression | GET | /api/users?page=2 | 200 | Validates status, content-type header, page number, user id, email, and first name |
| 6 | UpdateUser_WithPut_ShouldReturnUpdatedUser | Regression | PUT | /api/users/2 | 200 | Validates updated name, job, and updatedAt |
| 7 | PatchUser_ShouldReturnChangedAttribute | Regression | PATCH | /api/users/2 | 200 | Validates changed job and updatedAt |
| 8 | DeleteUser_ShouldReturnNoContent | Regression | DELETE | /api/users/2 | 204 | Validates no-content response body |
| 9 | JsonContentType_ShouldCreateUser | Regression | POST | /api/users | 201 | Sends application/json and validates name and job |
| 10 | XmlContentType_ShouldCreateUser | Regression | POST | /api/users | 201 | Sends application/xml and validates response content-type |
| 11 | FormDataContentType_ShouldCreateUser | Regression | POST | /api/users | 201 | Sends form-data and validates response content-type |
| 12 | RawTextContentType_ShouldCreateUser | Regression | POST | /api/users | 201 | Sends raw text and validates response content-type |
| 13 | DynamicDataRequest_ShouldCreateRandomUser | Regression | POST | /api/users | 201 | Uses Random data and validates generated name and job |
| 14 | ApiChaining_ShouldGetUserFromPreviousResponse | Regression | GET + GET | /api/users?page=2, /api/users/{id} | 200 | Takes user id from list response and uses it in another request |
| 15 | Requests_ShouldExecuteInParallel | Regression | GET | /api/users?page=1, /api/users/2, /api/users/3 | 200 | Executes three API requests in parallel using Task.WhenAll |
| 16 | GetMissingUser_ShouldReturnNotFound | Negative | GET | /api/users/23 | 404 | Validates missing user response body |
| 17 | RegisterWithoutPassword_ShouldReturnBadRequest | Negative | POST | /api/register | 400 | Validates error message when password is missing |
| 18 | MissingApiKey_ShouldReturnUnauthorized | Negative | GET | /api/users/2 | 401 | Validates unauthorized response when x-api-key header is missing |

## Requirement Coverage

| Requirement | Covered By |
| --- | --- |
| GET request | Tests 5, 14, 15, 16, 18 |
| POST request | Tests 1-4, 9-13, 17 |
| PUT request | Test 6 |
| PATCH request | Test 7 |
| DELETE request | Test 8 |
| Status code validation | All tests |
| Response body validation | Tests 1-9, 13-18 |
| Attribute validation | Tests 1-7, 9, 13, 14 |
| Header validation | Tests 1, 2, 5, 10-12 |
| JSON content type | Test 9 |
| XML content type | Test 10 |
| Form-data content type | Test 11 |
| Raw text request | Test 12 |
| Data-driven testing | Tests 1-4 |
| JSON data source | Tests 1-2 |
| CSV data source | Tests 3-4 |
| Excel test data file | ReqRes/TestData/users.xlsx |
| Environment variables | .env file |
| Random data | Test 13 |
| API chaining | Test 14 |
| Parallel execution | Test 15 |
| Negative testing | Tests 16-18 |

## Final Result

All 18 NUnit test cases passed successfully.
