# EvilCorp2000 - DAL Unit Tests

This directory contains the unit tests for the various DAL repositories in the EvilCorp2000 application.  
Each repository has its own dedicated test class to ensure a clear structure and maintainability.


## Test Environment  
All tests use an in-memory database (`UseInMemoryDatabase`) to ensure isolated and repeatable test execution.  
Each test creates a new database instance to prevent unintended dependencies between tests.  

## Running the Tests  
Tests can be executed using the following command:  
dotnet test