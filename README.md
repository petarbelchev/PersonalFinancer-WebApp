# Personal Financer

## Link to the App: <a href='https://financer.azurewebsites.net/'><b>financer.azurewebsites.net</b></a>
You can use user with test data - email: "test.user@mail.com", password: "TestUser1!".

The initial loading of the page may take a little bit more time because of the Azure hosting plan.

## Overview
Personal Financer is a web application that provides its users with the ability to record their incomes and expenses structured into accounts and transactions.

![User Dashboard Page](./Screenshots/user-dashboard-page.jpeg)

## Motivation
I created this project to practice my skills, and it will also serve as my defense project for the [**ASP.NET Advanced**](https://softuni.bg/trainings/4107/asp-net-advanced-june-2023) course at [SoftUni](https://softuni.bg/ "SoftUni") (June 2023).

## Used Tech/Frameworks
- ASP.NET Core 6
    - MVC
    - Web API
    - SignalR (in the messages functionality)
    - SendGrid (for email confirmation)
    - AutoMapper
    - Distributed Caching Using Redis (on the create account and transaction pages, loading users dropdown data)
    - Logger
    - Custom Action Filter and Attribute (for HTML sanitizing)
    - Custom Model Binders (for decimal and datetime binding)
    - TempData messages
    - Admin Area
    - Partial views
    - Two-factor authentication
    - NUnit & Moq (365 unit tests with 100% coverage of the services and 99% coverage of the controllers and helpers)
    - Bootstrap
    - Modal components (for transaction details)
    - HTML5 Canvas
- JavaScript for AJAX requests and DOM manipulations
- Entity Framework Core
- Microsoft SQL Server
- MongoDB (for Messages)
- Azure

## How to Run the Project

The Project can be easy tested locally. All you need to do:
1. Populate the appsettings.json file with the required configurations;
2. That's it! Run the App & Enjoy! :)

![Appsettings File](./Screenshots/appsettings-file.png)

When the App is fired up, its will seed the database with: 
- Admin - email: "alfa.admin@admin.com", password: "Admin1!";
- User with accounts and transactions for easy and fast tests - email: "test.user@mail.com", password: "TestUser1!";
- 13 more users without transactions so that admin pages are not empty.

## Databases

The application uses MS SQL database with 6 entities and MongoDB with 2 entities (Message and Reply).

MS SQL Database Diagram:
![Database Diagram](./Screenshots/database-diagram.png)

Mongo DB example of saved documents:
![MongoDB Documents](./Screenshots/mongodb-documents.png)

## Features

- [Create, edit and delete accounts, account types and currencies](#accounts-with-type-and-currency)
- [Create, edit and delete transactions and categories](#transactions-with-category)
- [User Dashboard](#user-dashboard)
- [Transactions page](#all-transactions-page)
- [Messages](#messages)
- [User Roles](#user-roles)
- [User Profile Management](#user-profile-management)
- [Responsive design](#responsive-design)


Personal Financer is a website for recording your money flow and analyzing it.

![Welcome Guest Page](./Screenshots/welcome-page.jpeg)

![No Accounts Page](./Screenshots/home-page-no-accounts.jpeg)

[Back to Features <<](#features)

### Accounts with Type and Currency

You can create your own accounts with name, balance, account type and currency.

When you click on 'Create new account type' or 'Create new currency', you can create your own account types like 'Bank account' or 'Savings account', for example, and currencies. If the 'Delete' buttons are clicked, the selected account type or currency will be deleted after confirmation. This feature utilizes AJAX and Web APIs.

![Create Account Page](./Screenshots/create-account-page.jpeg)

When an account is created, you will be redirected to the new account details page. The Personal Financer app automatically creates an initial balance transaction with the amount provided as the account balance by you.

![New Account Details Page](./Screenshots/new-account-page.jpeg)

The details page provides options to edit and delete the account (from the three dots dropdown menu), filter transactions for a given period, and display them on separate pages, with 10 transactions per page. The loading of transactions and pages navigation is handled through AJAX requests and Web APIs on the backend.

![Account Details Page](./Screenshots/account-details-page.jpeg)

When you click on some transaction a modal window with transaction details shows up.

![Transaction Details Modal](./Screenshots/transaction-details-modal.jpeg)

On the edit account page, you can change the name, balance, account type, and currency of the account. When you change the balance of the account, the app will automatically update the initial balance transaction or create it if the account was initially created with a zero balance. This ensures that all incomes and expenses are adjusted to match the new balance.

![Edit Account Page](./Screenshots/edit-account-page.jpeg)

On the delete account page, you will need to confirm your decision. You also have the option to delete all transactions related to the account or leave them to remain in your records.

![Delete Account Confirm Page](./Screenshots/confirm-delete-account-page.jpeg)

[Back to Features <<](#features)

### Transactions with Category

On the create transaction page, you can create a transaction with the following details: amount, date, category, account, transaction type, and payment reference. The app provides an option to add and remove categories, utilizing AJAX and Web API.

![Create Transaction Page](./Screenshots/create-transaction-page.jpeg)

When a transaction is created, you will be redirected to the transaction details page. There, the app provides options to edit and delete the transaction.

![Transaction Details Page](./Screenshots/transaction-details-page.jpeg)

You can change any data on transactions, including the account. This will automatically update the balance on both accounts.

![Edit Transaction Page](./Screenshots/edit-transaction-page.jpeg)

![Edited Transaction Message](./Screenshots/edited-transaction-message.jpeg)

[Back to Features <<](#features)

### User Dashboard

The Dashboard page provides the user with the following information:
- A section displaying the user's accounts;
- The last five transactions made from all accounts for the selected period;
- Cash flow for the selected period;
- Expenses structure for the selected period (uses HTML5 Canvas).

![User Dashboard Page](./Screenshots/user-dashboard-page.jpeg)

[Back to Features <<](#features)

### Transactions page

On the Transactions page, you can manage all of your transactions and filter them by a specific period, account, currency, account type or category, for example. The loading of transactions and pages navigation is handled through AJAX requests and Web APIs on the backend.

![All Transactions Page](./Screenshots/all-transactions-page.jpeg)

When you click on some transaction a modal window with transaction details shows up.

![Transaction Details Modal](./Screenshots/transactions-page-modal.jpeg)

[Back to Features <<](#features)

### Messages

Users can write messages to the support team. The users also can attach an image to the message. Every admin can see users messages and reply to them. 

The messages functionality uses a SignalR, and all new messages and replies are received in real time by the recipients. They will also receive notifications on the navigation panel and the messages page.

Messages can be archived (only for the user when he archived it or only for the admin when admin archived it) and deleted by both users and admins (will be deleted permanently for everybody). 

The messages are stored in a separate database (MongoDB).

![Create Message Page](./Screenshots/create-message-page.jpeg)

![User Messages Page](./Screenshots/user-messages.jpeg)

![User Messages Page](./Screenshots/user-archived-messages.jpeg)

![Message Details Page](./Screenshots/message-details-page.jpeg)

![Message Details Page](./Screenshots/message-details-page-with-attached-image.jpeg)

[Back to Features <<](#features)

### User Roles

Personal Financer has two roles - User and Admin. Here's what an Admin can do.

![Admin Home Page](./Screenshots/admin-homepage.jpeg)

The administrators can manage all users and their accounts. They can edit and delete accounts and transactions, and have access to information about the number of users and accounts. 

When the 'More statistics' button on the Home page is pressed, an AJAX request is made to the server to retrieve up-to-date data on the total amount of transactions made by users, grouped by currencies.

Every administrator has access to all users' messages and can reply.

![More statistics](./Screenshots/admin-more-statistics.jpeg)

![All Users Page](./Screenshots/all-users-page.jpeg)

![All Users Page](./Screenshots/all-accounts-page.jpeg)

![All Users Page](./Screenshots/user-details-page.jpeg)

[Back to Features <<](#features)

### User Profile Management

The application requires a unique and confirmed email from users during registration. Users can change their username, first name, last name, phone number, email, and password. If the user desires, they can activate Two-factor authentication.

![All Users Page](./Screenshots/user-profile.jpeg)

### Responsive design

Personal Financer is responsive and can be comfortably used on different devices.

![Mobile View Dashboard](./Screenshots/mobile-view-dashboard.jpeg)
![Mobile View Account Details](./Screenshots/mobile-view-account-details.jpeg)
![Mobile View Create Transaction](./Screenshots/mobile-view-create-transaction.jpeg)

[Back to Features <<](#features)

[Back in the beginning <<](#personal-financer)