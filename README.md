# Product-Management C# + ASP.NET Core + Razor Pages Learning Project - EvilCorp2000 (not code reviewed yet)

I was desperately in search of an idea for a project I could build. Of course a managing system would come to mind. But wouldn't it be boring to manage books or cakes or that ever ocurring stuff?
And then I was inspired by a really, really old website (http://www.eviloverlord.com/lists/overlord.html) that has several Evil Overlord Lists that I found quite entertaining in my youth.


### Product Management (still in work)
Since I don't use JS, a big challenge here is that the product overview opens in a modal written in  a partial.

* Enter, view and change products (name, description, categories, price, amount on stock) with UI Validation
* Add and delete product discounts
* Product + Discount Backend Validation


### Programming concepts included 
* Data is stored in a MS SQL Server DB
* Seperation of concerns (DAL, BL, UI)


### Planned

* Delete Products
* Add, alter, delete(?) Categories
* Add Imagesupport - save in Datebase
* Sorting Product List
* Unit Tests
* Logging in DB
* Use of Identity - Log In, Log Out, Error Pages

* Add Shop: MainPage, Product Pages
* Add Paging in ProductManagement and ShoppingSite

* Make UI more appealing
* Customize Currency?
* Produktstate as a Property of Product: aktiv / inaktiv?
* Change Discounts??
* Restoration of deleted Discounts / Products - Event Sourcing??


### Known Problems
* After entering a new Discount, the Modal is refreshed automatically but the fields are still filled
* Error when deleting two discounts after another



