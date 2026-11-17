GrandStay – Hotel Management System


About the Project:

GrandStay is an ASP.NET Core MVC web application designed to manage the main operations of a hotel.
It provides a centralized interface for managing rooms, room types, guests, bookings, and room amenities.

The project was developed as part of the SoftUni ASP.NET Fundamentals course.


Main Features

Room Management – Create, view, edit, and delete hotel rooms.
Room Types – Manage room categories, descriptions, prices, and maximum occupancy.
Guest Management – Store and manage guest information.
Booking Management – Create, view, edit, and delete bookings.
Booking Validation – Validate booking dates, check room availability, and prevent overlapping reservations.
Automatic Price Calculation – Calculate booking prices based on the room's base price and the number of nights.
Amenity Management – Manage available hotel amenities.
Room Amenities – Assign and remove amenities for individual rooms.


Project Architecture

The application follows the Model-View-Controller (MVC) architectural pattern.
Controllers – Handle application requests and business logic.
Models – Define the application's data entities.

Views – Provide the user interface using Razor.
Data – Contains the Entity Framework Core database context.
ViewModels – Transfer data between controllers and views.
Migrations – Manage database schema changes.

Database

The application uses Microsoft SQL Server with Entity Framework Core.
The database is named GrandStayDb and contains the following main entities:

Room
RoomType
Guest
Booking
Amenity
RoomAmenity

The application uses the DefaultConnection setting from appsettings.json.
