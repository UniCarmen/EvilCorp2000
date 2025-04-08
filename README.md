# Product-Management C# + ASP.NET Core + Razor Pages Learning Project - EvilCorp2000 (not code reviewed yet)

I was desperately in search of an idea for a project I could build. Of course a managing system would come to mind. But wouldn't it be boring to manage books or cakes or that ever ocurring stuff?
And then I was inspired by a really, really old website (http://www.eviloverlord.com/lists/overlord.html) that has several Evil Overlord Lists that I found quite entertaining in my youth.

The documentation is accessible under /docs/architecture.md

### Product Management (refactoring is planned, but it works in the current state)
Since I don't use JS, a big challenge here is that the product overview opens in a modal written in  a partial.

* Enter, view, delete and change products (name, description, categories, price, amount on stock) with UI Validation
* Add and delete product discounts
* Product + Discount Backend Validation
* Saving and Loading Product Images (path is stored in DB, images in the project as an indermediate step to storing them in a blob storage)
* Implementation of Identity - Log In, Log Out with different Roles and authorizations, ErrorPages
* Shop View

### Programming concepts included 
* Data is stored in a MS SQL Server DB
* Seperation of concerns (DAL, BL, UI): different projects (there is still a relic of which I wanted to take care and DTOs which are used in both UI and BL)
* Errorlogging with Serilog in Console (Debug), File (Warning) and Db (Error)
* Unit Tests
* First big refactor

### Planned next

* UI Improvements: Pagination, Sorting, Filtering
* Shopping Cart
* Customer Identity
* Checkout - Product OnStock-changes
* Fictional Orders
* Ratingsystem

### Planned in the future
* Add, alter, delete(?) Categories
* Customize Currency?
* Produktstate as a Property of Product: aktiv / inaktiv?
* Change Discounts??
* Restoration of deleted Discounts / Products - Event Sourcing??
* Completely Change the New/Alter-Product-Views because it's causing to many problems: These should not be partials / modals


### Known Problems
* After entering a new Discount, the Modal is refreshed automatically but the fields are still filled - could be resolved by getting rid of the modals
* Invisible Modal after "exiting" the Modal with Esc-Button - could be resolved by getting rid of the modals
