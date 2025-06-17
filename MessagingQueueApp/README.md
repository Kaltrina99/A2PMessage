# MessagingQueueApp (.NET 9)

A scalable, full-featured messaging queue processor built with **.NET 9**, featuring a REST API, Bootstrap-based dashboard UI, and PostgreSQL persistence using Dapper. Designed for local testing with an in-memory queue and production readiness with PostgreSQL-backed persistence.

---

## Features

- In-memory queue for local/testing environments  
- Support for SMS, Email, and Push Notifications (simulated)  
- PostgreSQL persistence using Dapper for efficient data access  
- REST API with Swagger documentation  
- Bootstrap-based dashboard UI for real-time metrics and message status  
- Retry logic with exponential backoff and dead-letter queue handling  
- Circuit breaker pattern simulation  
- Serilog logging integration  
- Comprehensive unit and integration tests  

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)  
- [PostgreSQL](https://www.postgresql.org/download/)  
- (Optional) API testing tools like [Postman](https://www.postman.com/)  

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/MessagingQueueApp.git
cd MessagingQueueApp


2. Configure PostgreSQL
Make sure PostgreSQL is installed and running on your machine.

Create the database:

bash
Copy
Edit
psql -U postgres -c "CREATE DATABASE msgqueue;"
Run the schema script (you can run this inside psql or via any DB tool):

sql
Copy
Edit
CREATE TABLE messages (
    id UUID PRIMARY KEY,
    recipient TEXT NOT NULL,
    content TEXT NOT NULL,
    type TEXT,
    status TEXT,
    priority TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
3. Configure the Application
Edit the appsettings.json file in the project root to configure database and queue provider settings:

json
Copy
Edit
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Username=postgres;Password=yourpassword;Database=msgqueue"
  },
  "QueueProvider": "InMemory",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
Important Configuration Options:
"DefaultConnection": Update with your PostgreSQL server details and credentials.

"QueueProvider":

Use "InMemory" for local development/testing with an in-memory queue.

Use "PostgreSQL" to enable persistent queue backed by PostgreSQL database.

4. Environment Variables (Optional)
For security, you may want to override the connection string via environment variables instead of hardcoding passwords in appsettings.json.

Set environment variables in your OS or Docker container:

bash
Copy
Edit
# Linux/macOS
export ConnectionStrings__DefaultConnection="Host=localhost;Username=postgres;Password=yourpassword;Database=msgqueue"

# Windows PowerShell
setx ConnectionStrings__DefaultConnection "Host=localhost;Username=postgres;Password=yourpassword;Database=msgqueue"
The application will automatically read environment variables prefixed with ConnectionStrings__.

5. Build and Run the Application
Restore dependencies, build, and start the app:

bash
Copy
Edit
dotnet restore
dotnet run --project MessagingQueueApp
The app will listen on default ports (usually https://localhost:57192).

Access the Application
Swagger API Docs:
https://localhost:57192/swagger

Dashboard UI:
https://localhost:57192/dashboard

Using Dapper in MessagingQueueApp
Dapper provides fast, simple data access to your PostgreSQL database.

Example: Query message by ID
csharp
Copy
Edit
using var connection = new NpgsqlConnection(_connectionString);
await connection.OpenAsync();

const string sql = @"
    SELECT id, recipient, content, type, status, priority, created_at
    FROM messages
    WHERE id = @Id";

var message = await connection.QuerySingleOrDefaultAsync<Message>(sql, new { Id = messageId });
Example: Insert a message
csharp
Copy
Edit
const string insertSql = @"
    INSERT INTO messages (id, recipient, content, type, status, priority, created_at)
    VALUES (@Id, @Recipient, @Content, @Type, @Status, @Priority, @CreatedAt)";

await connection.ExecuteAsync(insertSql, message);
API Endpoints
Method	Endpoint	Description
POST	/api/messages	Enqueue a new message
GET	/api/messages/{id}	Retrieve message by ID
GET	/api/messages/all	Retrieve all messages
GET	/api/messages/stats	Get message statistics by status
GET	/api/messages/type-stats	Get message statistics by type
GET	/api/messages/pending	Retrieve all pending messages
POST	/api/messages/{id}/status	Update message status manually

Logging Configuration
The app uses Serilog for logging. You can customize logging via appsettings.json:

json
Copy
Edit
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    {
      "Name": "Console"
    },
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
Logs will be written to console and rolling log files inside Logs/ folder.

Running Tests
Run all unit and integration tests with:

bash
Copy
Edit
dotnet test ./MessagingQueueApp.Tests