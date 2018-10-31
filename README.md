# BangazonAPI

The first group sprint of NSS c# backend program, BangazonAPI is a personal E-Commerce platform. The goal is to allow users to browse and purchase items, as well as sell items to other visiters to the site.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

What things you need to install the software and how to install them

* Visual Studio
* Microsoft SQL Server Managment Studio
* Postman (or another application that can call an API)

### Installing & Running

A step by step series of examples that tell you how to get a development env running

* Fork a copy of the Repo
* Download or Clone the Repo to your local machine
* Open SSMS and run the SQL scrip in the repo, [SQL Script](https://github.com/NSS-Therapeutic-Raccoons/BangazonAPI/blob/master/SQL/Bangazon.sql), then execute the script into a local database.
* Open the project in Visual Studio, look for SQL Server Object Explorer and navigate to your local database. Right click on the name and click on properties. Look for "Connection String" and copy the value to the right of it.
* Paste that value over "Default Connection" in appsettings.json
* Look at the top bar and find the green arrow "play button", and make sure BangazonAPI is selected and click the arrow.
* Open Postman and use "http://localhost:5000/api/" as the template for getting data.
** Navigate the controllers to find syntax for GET, POST, PUT, and DELETE for each type.

## Built With

* C#

## Authors

* **Ricky Bruner** - [ricky-bruner](https://github.com/ricky-bruner)
* **Klaus Hardt** - [KHardt](https://github.com/KHardt)
* **Jeremiah Pritchard** - [jeremiah3643](https://github.com/jeremiah3643)
* **Mike Parrish** - [thatmikeparrish](https://github.com/thatmikeparrish)

## Acknowledgments

* Special thanks to **Andy Collins** - [askingalot](https://github.com/askingalot) for putting up with us!

## Notes

* Here is the ERD for our project.
![Bangazon ERD](https://github.com/NSS-Therapeutic-Raccoons/BangazonAPI/blob/master/BangazonAPI-ERD.png?raw=true)