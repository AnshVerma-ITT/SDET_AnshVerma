# Playwright API Framework Test Execution Report

## Execution Summary

Project: Playwright API Framework  
API Under Test: ReqRes  
Test Framework: NUnit  
Execution Type: API Testing through Playwright request context  

The framework now contains normal passing API validation tests plus one intentional failing test.

The intentional failure proves that ReqRes does not persist newly created users.

## Command Used

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Expected Test Summary

```text
Test summary: total: 18, failed: 1, succeeded: 17, skipped: 0
```

The failed test should be:

```text
ReqResSavesData_ShouldGetCreatedUser
```

## Test Cases Executed

| No. | Test Name | Type | API Method | Endpoint | Expected Result |
| --- | --- | --- | --- | --- | --- |
| 1 | CreateUser_WithJsonDataProvider - Rahul | Regression | POST | /api/users | 201 Created |
| 2 | CreateUser_WithJsonDataProvider - Morpheus | Regression | POST | /api/users | 201 Created |
| 3 | CreateUser_WithCsvDataProvider - Rahul | Regression | POST | /api/users | 201 Created |
| 4 | CreateUser_WithCsvDataProvider - Morpheus | Regression | POST | /api/users | 201 Created |
| 5 | GetUsers_PageTwo_ShouldValidateBodyAndHeaders | Regression | GET | /api/users?page=2 | 200 OK |
| 6 | UpdateUser_WithPut_ShouldReturnUpdatedUser | Regression | PUT | /api/users/2 | 200 OK |
| 7 | PatchUser_ShouldReturnChangedAttribute | Regression | PATCH | /api/users/2 | 200 OK |
| 8 | DeleteUser_ShouldReturnNoContent | Regression | DELETE | /api/users/2 | 204 No Content |
| 9 | JsonContentType_ShouldCreateUser | Regression | POST | /api/users | 201 Created |
| 10 | XmlContentType_ShouldCreateUser | Regression | POST | /api/users | 201 Created |
| 11 | FormDataContentType_ShouldCreateUser | Regression | POST | /api/users | 201 Created |
| 12 | RawTextContentType_ShouldCreateUser | Regression | POST | /api/users | 201 Created |
| 13 | DynamicDataRequest_ShouldCreateRandomUser | Regression | POST | /api/users | 201 Created |
| 14 | ApiChaining_ShouldGetUserFromPreviousResponse | Regression | GET + GET | /api/users?page=2 and /api/users/{id} | 200 OK |
| 15 | GetMissingUser_ShouldReturnNotFound | Error Validation | GET | /api/users/23 | 404 Not Found |
| 16 | RegisterWithoutPassword_ShouldReturnBadRequest | Error Validation | POST | /api/register | 400 Bad Request |
| 17 | MissingApiKey_ShouldReturnUnauthorized | Authorization | GET | /api/users/2 | 401 Unauthorized |
| 18 | ReqResSavesData_ShouldGetCreatedUser | Negative Failure Demo | POST + GET | /api/users and /api/users/{id} | Fails because ReqRes does not persist data |

## Requirement Coverage

| Requirement | Covered By |
| --- | --- |
| Configurable authorization header | AppSettings and AuthManager |
| GET request | User list, single user, chaining, and authorization |
| POST request | Create user, content type tests, register error, persistence failure |
| PUT request | Update user test |
| PATCH request | Patch user job test |
| DELETE request | Delete user test |
| Status code validation | All tests |
| Response body validation | User, error, and persistence tests |
| Response model usage | UserRequest for request body, RegisterRequest for register, User model plus JsonHelper for nested and error responses |
| Header validation | Content-Type tests |
| JSON content type | JsonContentType_ShouldCreateUser |
| XML content type | XmlContentType_ShouldCreateUser |
| Form-data content type | FormDataContentType_ShouldCreateUser |
| Raw text request | RawTextContentType_ShouldCreateUser |
| Data-driven testing | JSON and CSV test case sources |
| Environment variables | .env, AppSettings, and ReqResSettings |
| API chaining | ApiChaining_ShouldGetUserFromPreviousResponse |
| Parallel execution | NUnit Parallelizable attribute |
| Authorization testing | MissingApiKey_ShouldReturnUnauthorized |
| Negative failing test | ReqResSavesData_ShouldGetCreatedUser |

## Final Result

The normal API validation tests should pass.

The persistence test should fail intentionally to show that ReqRes create-user data is not saved.
