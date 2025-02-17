```markdown
# Architectur-Dokumentation

## 1. Overview
This project is an ASP.NET Core project using the **Layer-Architecture** with seperate **UI**, **Business-Layer**, **DataAccess-Layer** and a **Database**

## 2. Technology-Stack
- **Frontend:** ASP.NET Core Razor Pages
- **Backend:** ASP.NET Core
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentification:** ASP.NET Core Identity

---

## 3. Layer-Struktur
Das Projekt folgt einer klassischen **Layered Architecture**:


/MyApp
  ├── EvilCorp2000.EvilCorp2000/      # Frontend (ASP.NET Core Razor Pages)
  ├── EvilCorp2000.BusinessLayer/     # Service Layer (Business Layer)
  ├── EvilCorp2000.DataAccess/        # DAL (EF Core, Repository Pattern)
  ├── EvilCorp2000.DatabaseTests/     # Unit Tests Database
  ├── EvilCorp2000.UI Tests/          # Unit Tests UI
  ├── docs/                           # Documentation


### **Beschreibung der Layer**
1. **Web (Frontend)**
   - Presentation-Layer mit ASP.NET Core Razor Pages.
   - Communicaties with the Business Layer.

2. **Service Layer (Business Layer)**
   - Contains BusinessObjects, Businesslogic and Validations.
   - Uses DataAccess-Layer for DataAccess.

3. **DataAccess**
   - Contains Entities and Repositories.
   - Manages the communication with the database using EF Core.


---

## 4. Datenbank-Design
- **Tabellen:**
  - `Category (CategoryId, CategoryName)`
  - `CategoryProduct (CategoriesCategoryId, ProductsProductId)`
  - `Discounts (DiscountId, ProductId, StartDate, EndDate, DiscountPercentages)`
  - `Products (ProductId, ProductName, ProductDescription, ProductPicture, AmountOnStock, Rating, ProductPrice)`

- **Entity-Relationship Diagram:**

  [Products] 1 --- * [Discounts]
  [Products] * --- * [Categories]

- **Additional Tables:**
  - `Tables scaffolded by ASP.NET Core Identity`
  - `Table for Logs`

---

## 5. Deployment
The application can just be run locally at the moment:
- **Local:** Über `dotnet run`


---

## 6. Installation & Setup

### **1: Clone Repository**
git clone https://github.com/UniCarmen/EvilCorp2000.git
cd EvilCorp2000

### **2: install dependencies**
dotnet restore

### **3: Migrate Database**
dotnet ef database update

### **4: Start Application**
dotnet run --project EvilCorp2000

Die Application ist accessible under `https://localhost:7135`

```
