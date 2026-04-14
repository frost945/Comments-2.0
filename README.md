# SPA Comments
A single-page application for posting comments with cascading replies. Built with a focus on secure input, an object-oriented approach, and easy data sorting.

## Features
- Posting comments and replies to comments (unlimited nesting)
- Saving comments and user data in a relational database
- Tabular output of root comments sorted by: userName, email, date added (in both directions)
- Default sorting - LIFO
- Pagination - 25 posts per page
- The ability to add a picture or text file
- Viewing images is enhanced with visual effects using GLightbox
- Validation of input data on the server and client side
- The user may use the following permitted HTML tags in messages: `<a href=”” title=””> </a> <code> </code> <i> </i> <strong> </strong>`
- Check for closing tags, code is valid XHTML.
- Global error handling and logging
- Swagger documentation

## Technologies

- .NET 9
- ASP.NET Core Web API: for building RESTful services
- Entity Framework Core: ORM for database operations
- MS SQL Server: open-source relational database
- Ganss.XSS: server-side HtmlSanitizer
- DOMPurify: client-side HtmlSanitizer
- JavaScript: for frontend
- Swagger: for API documentation

## Project Structure

- Comments.Api: Web API controllers and middleware
- Comments.Application: business logic, application services, validation, sanitization, interfaces
- Comments.Model: entities, value objects, enums, filters
- Comments.Infrastructure: data access, EF Core DbContext, entity configurations and migrations
- Comments.Contracts: API contracts, DTOs, request and response models used for communication between the Web API and external clients
- Comments.wwwroot — SPA UI

## Requirements
- .NET 9 SDK
- EF Core
- SQL Server
- Docker and Docker Compose (for containerized deployment)

## Setup Instructions

### Local Development
1. Clone the repository: https://github.com/frost945/Project_Comments
2. Set up a local MS SQL instance or use Docker
3. Update connection string in appsettings.json, if needed
4. Run migrations to create the database schema:
```bash
cd src
```
5. Run the application:
```bash
dotnet run
```
6. Access the Swagger UI at https://localhost:7107/swagger

### Using Docker Compose
1. Clone the repository
2. Update connection string in appsettings.json, if needed
3. Run the application with Docker Compose:
```bash
docker-compose up -d
```
4. Access the Swagger UI at https://localhost:7107/swagger

Access to the app Frontend: http://localhost:5000

## API Endpoints
- `POST /Comment`: Create a new comment
- `GET /Comment/parent`: Get all root comments
- `GET /Comment/children/{parentId}`: Get all replies by id


## Security Notes
- XSS mitigation: HTML sanitizer, strict whitelist
- SQL Injection mitigation: EF Core parameterized queries
- server + client validation

## Future Enhancements
- Implement authentication and authorization
- Adding caching for posts
- Adding integration and unit tests
- Add CI/CD pipeline for automated testing and deployment