# Microservices webshop
![alt text](https://user-images.githubusercontent.com/87232706/149635713-da1dfe8e-a553-48f2-86f8-3b4904540ed8.png)

A demo webshop project on using microservices. This project consists of:

Catalog microservice:

- ASP.NET Core Web API application
- REST API principles
- CRUD operations
- MongoDB database connection and containerization
- Repository Pattern Implementation
- Swagger Open API implementation

Basket microservice:

- ASP.NET Web API application
- REST API principles, CRUD operations
- Redis database connection and containerization
- Consumes Discount Grpc Service for inter-service sync communication to calculate product final price
- Publish BasketCheckout Queue using MassTransit and RabbitMQ

Discount microservice:

- ASP.NET Grpc Server application
- inter-service gRPC Communication with Basket Microservice
- Using Dapper for micro-orm implementation to simplify data access and ensure high performance
- PostgreSQL database connection and containerization

Microservices Communication:

- Sync inter-service gRPC Communication
- Async Microservices Communication with RabbitMQ Message-Broker Service
- Using RabbitMQ Publish/Subscribe Topic Exchange Model
- Using MassTransit for abstraction over RabbitMQ Message-Broker system
- Publishing BasketCheckout event queue from Basket microservices and subscribing this event from Ordering microservices

Ordering Microservice:

- Implementing DDD, CQRS, and Clean Architecture
- Developing CQRS with using MediatR, FluentValidation and AutoMapper
- Consuming RabbitMQ BasketCheckout event queue using MassTransit-RabbitMQ Configuration
- SqlServer database connection and containerization
- Entity Framework Core ORM and auto migrate to SqlServer on application startup

API Gateway Ocelot Microservice:

- Implements API Gateways with Ocelot
- Sample microservices/containers to reroute through the API Gateways
- Gateway aggregation pattern in Shopping.Aggregator

WebUI ShoppingApp Microservice:

- ASP.NET Core Web Application with Razor template
- Calls Ocelot APIs with HttpClientFactory
