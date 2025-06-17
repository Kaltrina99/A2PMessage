# Messaging Queue Processor (.NET 9)

## Overview

This project is a .NET 9 application that simulates processing messages in an A2P (Application-to-Person) platform. It demonstrates a robust message queue system with REST API endpoints, asynchronous processing, persistence, monitoring, and several advanced features.

---

## Features

- **In-memory message queue** with producer/consumer pattern
- **Supports multiple message types:** SMS, Email, Push Notification
- **REST API** for submitting messages, checking status, and viewing statistics
- **Asynchronous processing** with configurable throttling and simulated external API calls
- **Persistence** using Dapper and PostgreSQL to survive restarts
- **Structured logging** with Serilog
- **Basic metrics:** throughput, error rates, queue depth
- **Unit tests** with >50% coverage
- **Dependency injection** throughout
- **Exception handling** and proper status codes

---

## Bonus Features Implemented

### 1. Web UI for Monitoring

A simple dashboard page is included to visualize system status, queue depth, and message statistics in real time.  
**Reason:**  
A web UI provides immediate, user-friendly insight into the health and activity of the system, making it easier for operators and developers to monitor and debug message processing without digging through logs or API responses.

### 2. Message Prioritization

The queue supports prioritizing messages, ensuring that high-priority messages are processed before lower-priority ones.  
**Reason:**  
In real-world messaging systems, certain messages (such as alerts or critical notifications) must be delivered faster than others. Implementing prioritization demonstrates an understanding of practical requirements and adds significant value to the system’s flexibility and reliability.

---

## Setup Instructions

Follow these steps to get the project running locally or in your preferred environment:

### 1. Clone the Repository

```sh
git clone https://github.com/yourusername/messaging-queue-processor.git
cd messaging-queue-processor
```

### 2. Configure the Database

- Ensure you have a running PostgreSQL instance.
- Create a database (e.g., `appdb`) and a user with appropriate permissions.
- Update the connection string in `appsettings.json`:

  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=appdb;Username=appuser;Password=apppassword"
  }
  ```

  Or, set the connection string via environment variable:
  ```
  ConnectionStrings__DefaultConnection=Host=localhost;Database=appdb;Username=appuser;Password=apppassword
  ```

### 3. Build the Application

```sh
dotnet build
```

### 4. Run the Application

```sh
dotnet run --project MessagingQueueApp
```

- The API will be available at `https://localhost:5001/api/messages`
- The dashboard UI will be available at `https://localhost:5001/dashboard`

---

## Running with Docker

To build and run the application in a Docker container using the default configuration from `appsettings.json`:

1. **Build the Docker image:**
   ```sh
   docker build -t messaging-queue-processor .
   ```

2. **Run the Docker container:**
   ```sh
   docker run -p 5000:8080 messaging-queue-processor
   ```

- The API will be available at [http://localhost:5000/api/messages](http://localhost:5000/api/messages)
- The dashboard UI will be available at [http://localhost:5000/dashboard](http://localhost:5000/dashboard)

> **Note:**  
> The container will use the database connection string specified in `appsettings.json`.  
> Make sure your PostgreSQL instance is accessible from inside the container using those settings.

---

## Running Unit Tests

```sh
dotnet test
```

---

## API Usage

- **Submit a message:**  
  `POST /api/messages` with JSON body:
  ```json
  {
    "type": "SMS",
    "content": "Hello, world!",
    "priority": 1
  }
  ```
- **Check message status:**  
  `GET /api/messages/{id}`

- **View statistics:**  
  `GET /api/messages/stats`  
  `GET /api/messages/type-stats`

---

## Configuration

- Throttling, retry policies, and other settings can be adjusted in `appsettings.json`.

---

## Troubleshooting

- Ensure PostgreSQL is running and accessible.
- Check logs for errors (Serilog outputs to console and file).
- For Docker, verify environment variables and port mappings.

---

## Architectural Decisions

- **In-memory queue** for simplicity, with easy extension to distributed queues (e.g., Redis)
- **Dapper** for lightweight, performant data access
- **Serilog** for structured, extensible logging
- **Separation of concerns** via services, repositories, and controllers
- **Extensible message model** for future message types

---

## Assumptions

- Message processing is simulated; no real external APIs are called.
- Throttling and retry policies are configurable via `appsettings.json`.
- The dashboard is intentionally simple for demonstration.

---

## Optional Components Implemented

- Recovery mechanism for failed messages (retry and dead letter queue)
- Web UI dashboard for monitoring
- Retry policies with exponential backoff
- Message