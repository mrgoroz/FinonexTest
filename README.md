# FinonexTest ETL Process Implementation

This project implements a simple ETL (Extract, Transform, Load) process using .NET Core and PostgreSQL.

## Prerequisites

1. .NET Core SDK (version 6.0 or later)
2. PostgreSQL (version 13 or later)
3. Internet connection for NuGet packages

## Setup Instructions

1. **Database Setup**
   ```bash
   # Create database
   createdb etl_db
   
   # Run the schema script
   psql -d etl_db -f db.sql
   ```

2. **Project Setup**
   ```bash
   # Install .NET dependencies
   dotnet restore
   ```

3. **Configuration**
   - Update the connection string in both `server.cs` and `data_processor.cs` if your PostgreSQL credentials are different
   - Default connection string uses:
     - Host: localhost
     - Database: etl_db
     - Username: postgres
     - Password: postgres

## Running the Application

1. **Start the Server**
   ```bash
   dotnet run --project server.cs
   ```

2. **Run the Client**
   ```bash
   # Make sure events.jsonl exists in the same directory
   dotnet run --project client.cs
   ```

3. **Process Data**
   ```bash
   dotnet run --project data_processor.cs
   ```

## Testing

1. Create a sample `events.jsonl` file with events:
   ```json
   {"userId": "user1", "name": "add_revenue", "value": 100}
   {"userId": "user1", "name": "subtract_revenue", "value": 50}
   ```

2. Run the client to send events to the server
3. Run the data processor to update the database
4. Query user data using: `http://localhost:8000/userEvents/user1`

## Architecture Notes

- The server saves events to `server_events.jsonl`
- The data processor uses UPSERT to handle concurrent updates safely
- Authentication uses a simple token ("secret") in the Authorization header

## Error Handling

- The server validates the Authorization header
- The client includes retry logic and error reporting
- The data processor uses transactions for data consistency

## Performance Considerations

- Database indexing on user_id for faster queries
- Batch processing capability in the data processor
- Connection pooling in database operations
