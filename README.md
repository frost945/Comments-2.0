# Overview
A full-stack comments system built with Angular and ASP.NET Core.
Supports nested replies, file uploads, and secure HTML rendering.

## Features
- Posting comments and replies to comments
- Saving comments and user data in a relational database
- Tabular output of root comments sorted by: username, email, createdAt (in both directions)
- Default sorting - LIFO
- Pagination - 25 posts per page
- For replies page - infinite scrolling
- The ability to add a picture or text file
- Viewing images is enhanced with visual effects using GLightbox
- Validation of input data on the server and client side
- The user may use the following permitted HTML tags in messages: `<a href="" title=""> </a> <code> </code> <i> </i> <strong> </strong>`
- Check for closing tags, code is valid XHTML.
- Global error handling and logging
- Swagger documentation

## Technologies

### Backend
- .NET 9
- ASP.NET Core Web API: for building RESTful services
- Entity Framework Core: ORM for database operations
- MS SQL Server: relational database
- Ganss.XSS: server-side HtmlSanitizer
- Swagger: for API documentation

### Frontend
- Angular 21
- TypeScript, HTMl, CSS
- DOMPurify: client-side HtmlSanitizer

## Architecture
- Clean Architecture (Backend)
- Layer separation: API - Application - Domain - Infrastructure
- REST API communication
- SPA frontend (Angular)

## Project Structure

### Backend
/Backend/src/
- Api/ Web API controllers and middleware
- Application/ business logic, application services, validation, sanitization, interfaces
- Model/ entities, value objects, enums, filters
- Infrastructure/ data access, EF Core DbContext, entity configurations and migrations
- Contracts/ API contracts, DTOs, request and response models used for communication between the Web API and external clients

### Frontend
/Frontend/src/
- app/ components and app logic
- environments/ environment configurations
- types/ global declarations

## Requirements
- .NET 9 SDK
- EF Core
- SQL Server
- Docker and Docker Compose (for containerized deployment)

## Setup Instructions

### Local Development
#### Backend:
1. Clone the repository: https://github.com/frost945/Comments-2.0
2. Set up a local MS SQL instance or use Docker
3. Run app:
```bash
cd Backend/src
dotnet run
```
- Access Backend API: https://localhost:7107
- Access Swagger UI: https://localhost:7107/swagger

#### Frontend:
1. Install dependencies:
```bash
npm install
```
2. Run app:
```bash
ng serve
```
- Access Frontend app: http://localhost:4200

### Using Docker Compose
1. Clone the repository
2. Run app with Docker Compose:
```bash
docker-compose up -d
```
- Access Backend API: http://localhost:5000
- Access Swagger UI: http://localhost:5000/swagger
- Access Frontend app: http://localhost:4200

## API Endpoints
- `POST /api/comments`: Create a new comment
- `GET /api/comments/parent`: Get all parent comments
- `GET /api/comments/children/{parentId}`: Get all replies by id
- `GET /api/comments?id={id}`: Get comment by id
- `GET api/files/text/{id}`: Get text file by id

## Security Notes
- Server-side and client-side validation
- Input sanitization (DOMPurify + Ganss.XSS)
- Strict HTML whitelist

## Future Enhancements
- Implement authentication and authorization
- Adding caching for posts
- Adding integration and unit tests
- Add CI/CD pipeline for automated testing and deployment