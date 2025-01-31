# Product-Management C# + ASP.NET Core + Razor Pages Learning Project - EvilCorp2000 (not code reviewed yet)

I was desperately in search of an idea for a project I could build. Of course a managing system would come to mind. But wouldn't it be boring to manage books or cakes or that ever ocurring stuff?
And then I was inspired by a really, really old website (http://www.eviloverlord.com/lists/overlord.html) that has several Evil Overlord Lists that I found quite entertaining in my youth.


### Product Management (still in work)
Since I don't use JS, a big challenge here is that the product overview opens in a modal written in  a partial.

* Enter, view, delete and change products (name, description, categories, price, amount on stock) with UI Validation
* Add and delete product discounts
* Product + Discount Backend Validation
* Saving and Loading Product Images (path is stored in DB, images in the project as an indermediate step to storing them in a blob storage)

### Programming concepts included 
* Data is stored in a MS SQL Server DB
* Seperation of concerns (DAL, BL, UI): different projects (there is still a relic of which I wanted to take care and DTOs which are used in both UI and BL)
* Errorlogging with Serilog in Console (Debug), File (Warning) and Db (Error)

### Planned next
* Implementation of Identity - Log In, Log Out
* ErrorPages - 403 Unauthorized, if not logged in, Link to Mainpage / 404, Link to Mainpage
* Unit Tests

### Planned in the future
* Add, alter, delete(?) Categories
* Sorting Product List
* Add Shop: MainPage, Product Pages
* Add Paging in ProductManagement and ShoppingSite
* Make UI more appealing
* Customize Currency?
* Produktstate as a Property of Product: aktiv / inaktiv?
* Change Discounts??
* Restoration of deleted Discounts / Products - Event Sourcing??
* Completely Change the New/Alter-Product-Views because it's causing to many problems: These should not be partials


### Known Problems
* After entering a new Discount, the Modal is refreshed automatically but the fields are still filled
* Invisible Modal after "exiting" the Modal with Esc-Button
