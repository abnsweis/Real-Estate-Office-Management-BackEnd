# 🏠 Real Estate Office Management System – Backend (ASP.NET Core Web API)

## 📋 Project Overview
A scalable and modular **Real Estate Office Management System Backend**, developed using **ASP.NET Core Web API** following **Clean Architecture principles**.  
The system provides APIs for property listings, user authentication, favorites, ratings, testimonials, and more, designed to integrate with a modern Angular frontend.

---

## 🏗️ Project Architecture
The solution follows **Clean Architecture** and is organized into the following layers:
- **API Layer:** Exposes HTTP endpoints and handles HTTP requests/responses.
- **Application Layer:** Contains business logic, use cases, CQRS handlers, and service contracts.
- **Domain Layer:** Defines core entities and domain logic.
- **Infrastructure Layer:** Manages database access (EF Core), Identity, and external services.

---

## 🛠️ Technologies & Libraries Used
- **ASP.NET Core Web API**
- **Entity Framework Core (EF Core)**
- **ASP.NET Core Identity**
- **CQRS with MediatR**
- **SQL Server**
- **AutoMapper**
- **FluentResults** 
- **JWT Authentication** 

---

## 🔐 Features
- **Authentication & Authorization** (Login / Register with ASP.NET Identity & JWT).
- **Property Listing API** with advanced filtering by categories and types.
- **Favorites Management:** Add/Remove properties to/from user favorites.
- **Property Ratings & Reviews:** Users can rate properties and add/edit/delete comments.
- **User Testimonials Management:** Users can add testimonials if they haven't before.
- **Content Pages:** API endpoints for "About Us" and "Services" pages.
- **Unified Error Handling System:** Structured error responses across the API using FluentResults and ProblemDetails (RFC 7807).
- Designed with extensibility for a future **Admin Dashboard**.

---

## 🛡️ Unified Error Handling System
In this project, I designed and implemented a **comprehensive error handling system** for the API using **FluentResults** to manage failures in an organized, consistent manner.  
Errors are transformed into **ProblemDetails-compliant HTTP responses** following **RFC 7807** standards, ensuring clarity and consistency across all API endpoints.

### Key Highlights:
- All service and business logic layers return **FluentResults<T>** objects, encapsulating success/failure states and relevant error messages.
- API controllers are designed to automatically transform FluentResults failures into standardized **ProblemDetails** responses.
- Enhanced error traceability and client-side clarity by unifying all error formats, regardless of the failure source (validation, domain, infrastructure, etc.).
- Promotes a clean separation of concerns by keeping error construction and transformation logic centralized.

---

## 🚀 Getting Started

### Prerequisites:
- [.NET SDK 8+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
 
