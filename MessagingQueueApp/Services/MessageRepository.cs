using Dapper;
using MessagingQueueApp.Models;
using Npgsql;

namespace MessagingQueueApp.Services;

public class MessageRepository
{
    private readonly string _connStr;
    public MessageRepository(string connStr)
    {
        _connStr = connStr;
        Initialize().Wait();
    }

    private async Task Initialize()
    {
        using var conn = new NpgsqlConnection(_connStr);
        await conn.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS messages (
                id UUID PRIMARY KEY,
                type INT NOT NULL,
                recipient TEXT NOT NULL,
                content TEXT NOT NULL,
                status INT NOT NULL,
                retrycount INT NOT NULL,
                createdat TIMESTAMP NOT NULL,
                processedat TIMESTAMP NULL,
                priority INT NOT NULL DEFAULT 1
            );
        ");
    }

    public virtual async Task AddAsync(Message msg)
    {
        using var conn = new NpgsqlConnection(_connStr);
        await conn.ExecuteAsync(
            "INSERT INTO messages (id, type, recipient, content, status, retrycount, createdat, processedat, priority) VALUES (@Id, @Type, @Recipient, @Content, @Status, @RetryCount, @CreatedAt, @ProcessedAt, @Priority)",
            msg);
    }

    public virtual async Task<Message?> GetAsync(Guid id)
    {
        using var conn = new NpgsqlConnection(_connStr);
        return await conn.QuerySingleOrDefaultAsync<Message>("SELECT * FROM messages WHERE id=@id", new { id });
    }

    public virtual async Task<IEnumerable<Message>> GetAllAsync()
    {
        using var conn = new NpgsqlConnection(_connStr);
        return await conn.QueryAsync<Message>("SELECT * FROM messages");
    }

    public virtual async Task UpdateStatusAsync(Guid id, MessageStatus status, int retryCount = 0)
    {
        using var conn = new NpgsqlConnection(_connStr);
        await conn.ExecuteAsync(
            "UPDATE messages SET status=@status, retrycount=@retryCount, processedat=CASE WHEN @status=2 THEN NOW() ELSE processedat END WHERE id=@id",
            new { id, status, retryCount });
    }

    public virtual async Task<Dictionary<MessageStatus, int>> GetStatsByStatusAsync()
    {
        using var conn = new NpgsqlConnection(_connStr);
        var result = await conn.QueryAsync("SELECT status, COUNT(*) count FROM messages GROUP BY status");
        return result.ToDictionary(r => (MessageStatus)r.status, r => (int)r.count);
    }

    public virtual async Task<Dictionary<MessageType, int>> GetStatsByTypeAsync()
    {
        using var conn = new NpgsqlConnection(_connStr);
        var result = await conn.QueryAsync("SELECT type, COUNT(*) count FROM messages GROUP BY type");
        return result.ToDictionary(r => (MessageType)r.type, r => (int)r.count);
    }
    public virtual async Task UpdateAsync(Message msg)
    {
        using var conn = new NpgsqlConnection(_connStr);
        await conn.ExecuteAsync(
            @"UPDATE messages 
          SET type = @Type, 
              recipient = @Recipient, 
              content = @Content, 
              status = @Status, 
              retrycount = @RetryCount, 
              createdat = @CreatedAt, 
              processedat = @ProcessedAt, 
              priority = @Priority 
          WHERE id = @Id", msg);
    }

    public async Task<IEnumerable<Message>> GetByStatusAsync(MessageStatus status)
    {
        using var conn = new NpgsqlConnection(_connStr);
        return await conn.QueryAsync<Message>("SELECT * FROM messages WHERE status = @Status ORDER BY createdat ASC", new { Status = (int)status });
    }


}