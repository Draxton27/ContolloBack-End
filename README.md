# ContolloBack-end

This is the back-end application for the Contollo project, built using ASP.NET Core with MongoDB as the database. Below are instructions for setting up and running the project locally.

## Prerequisites

Before you begin, ensure you have the following installed on your machine:

- **.NET SDK** (v6.0 or higher) - [Download .NET SDK](https://dotnet.microsoft.com/download)
- **Node.js** - [Download Node.js](https://nodejs.org/)
- **Git** - [Download Git](https://git-scm.com/)

## Getting Started

Follow these steps to set up and run the project locally:

### 1. Clone the Repository

First, clone the repository to your local machine:

git clone https://github.com/Draxton27/ContolloBack-end.git

cd ContolloBack-end

### 2. Restore Dependencies

dotnet restore

### 3. Run the application

dotnet run


### Front-end

This is the link to the front-end repository: https://github.com/Draxton27/ContolloFront-end

## Design Principles

### Domain-Driven Design
Domain-Driven Design (DDD) is an approach to software development that centers the design around the domain and its logic. In this project, DDD is applied in the following ways:

#### 1. Domain Layer
The Domain Layer encapsulates the core business logic and rules. This layer includes entities, value objects, aggregates, and domain services.
The User and Note classes are examples of entities that represent the core data of the domain. They contain the essential attributes and behaviors.

#### 2. Application Layer
The Application Layer coordinates the flow of data between the UI and the domain. It does not contain business logic but acts as a mediator. For example, the NoteService and UserService classes handle operations related to notes and users, making calls to the domain logic and repositories.

#### 3. Infrastructure Layer
The Infrastructure Layer is responsible for database communication, external services, and other technical concerns. 
MongoDB is used as the database, and the repositories (NoteService, UserService) interact with MongoDB to perform CRUD operations.

#### 4. Presentation Layer
The Presentation Layer includes the controllers that expose APIs. These controllers process HTTP requests and invoke the application services to perform operations.

### SOLID Principles

#### 1. Single Responsibility Principle (SRP)
Each class in the "ContolloBack-End" project has a single responsibility. For example, the NoteService class is responsible only for handling note-related operations, while UserService manages user-related functionality.

#### 2. Open/Closed Principle (OCP)
The classes are designed to be open for extension but closed for modification. For example, adding a new validation rule would involve extending a class or adding a new service without modifying existing code.

#### 3. Liskov Substitution Principle (LSP)
This principle is followed by ensuring that derived classes or implementations can be substituted for their base classes without altering the behavior of the application. Although interfaces and inheritance are not explicitly shown, this principle would be important if interfaces or base classes were introduced.

#### 4. Interface Segregation Principle (ISP)
The project avoids creating large interfaces. Instead, if interfaces were used, they would be small and specific to the needs of the clients that implement them.

#### 5. Dependency Inversion Principle (DIP)
The project relies on abstractions rather than concrete implementations. Services like NoteService and UserService could be injected as dependencies, following the dependency inversion principle, which promotes loose coupling.


