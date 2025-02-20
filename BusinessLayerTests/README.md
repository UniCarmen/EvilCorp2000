# EvilCorp2000 - Business Logic Unit Tests


## Test Strategy

Since the BL layer interacts with repositories and external services, we use Moq to simulate dependencies.
All repository interfaces (IProductRepository, IDiscountRepository, ICategoryRepository) are mocked to ensure that tests focus solely on the BL logic.

## Tests cover:

Mappings: Ensuring data transformations between entities and DTOs.
Business Rules: Validation of product and discount constraints.
Service Methods: Ensuring expected behavior when interacting with repositories.

## Testing Frameworks Used

xUnit for writing and executing tests.
Moq for mocking repository dependencies.

## Running the Tests  
Tests can be executed using the following command:  
dotnet test