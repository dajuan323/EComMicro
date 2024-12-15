# E-Commerce Microservices API

## Overview

This project is an E-Commerce API built using **.NET Core** and designed with a **microservices architecture**. It provides a set of services for managing various aspects of an e-commerce platform, including product management, customer accounts, orders, payments, and more. Each service operates independently, communicating with other services via HTTP and messaging queues.

## Features

- **Product Service**: Manages product catalogs, categories, and inventory.
- **Order Service**: Handles customer orders, order status, and payments.
- **Customer Service**: Manages customer accounts and profiles.
- **Authentication & Authorization**: Secure login, JWT-based authentication, and user roles.
- **API Gateway**: Centralized point of access for client applications to interact with microservices.
- **Message Queue Integration**: Facilitates asynchronous communication between microservices (e.g., RabbitMQ, Kafka).
- **Database**: Each microservice uses its own database to ensure data isolation.
  
## Tech Stack

- **Backend**: .NET 8 (C#)
- **API Communication**: RESTful APIs, gRPC (if applicable)
- **Message Broker**: RabbitMQ, Apache Kafka (depending on your setup)
- **Database**: SQL Server, MongoDB, or any other DB per service
- **Authentication**: JWT Tokens, IdentityServer (or another authentication provider)
- **Containerization**: Docker for containerizing services
- **API Gateway**: Ocelot or YARP (if used)
- **CI/CD**: GitHub Actions / Jenkins / Azure DevOps (if applicable)
  
## Architecture

The system follows a **microservices architecture**, where each service is independent and communicates via:

- **REST APIs** for synchronous communication.
- **Message Queues** for asynchronous communication (event-driven).

## Getting Started

### Prerequisites

Before getting started, make sure you have the following installed on your machine:

- **.NET SDK**: [Download .NET SDK](https://dotnet.microsoft.com/download)
- **Docker** (if you want to run the services in containers): [Download Docker](https://www.docker.com/products/docker-desktop)
- **Message Broker** (e.g., RabbitMQ, Kafka): Dockerized instances or local setup
- **SQL Server** or **MongoDB** (depending on your service setup): Local or containerized instance
  
### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/e-commerce-microservices-api.git
   cd e-commerce-microservices-api

2. Restore the NuGet packages:

   ```bash
   dotnet restore

3. Set up your database(s):  Each service has its own database (SQL Server or MongoDB). You can either run them locally or use Docker containers to simplify the setup.

4. Set up the message broker (RabbitMQ, Kafka). If using Docker, you can run a containerized version

   ```bash
   docker run -d -p 15672:15672 -p 5672:5672 --name rabbitmq rabbitmq:management

5. Configure environment variables or use an appsettings.json file for service configurations (e.g., database connection strings, JWT secret keys, etc.).

6. Run the services:

   ```bash
   dotnet run --project src/ProductService/ProductService.csproj
   dotnet run --project src/OrderService/OrderService.csproj

  Or if you are using Docker Compose, you can bring up all services:
   
   ```bash
   docker-compose up
