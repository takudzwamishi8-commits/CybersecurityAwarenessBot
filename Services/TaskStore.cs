
using MySqlConnector;

namespace CybersecurityAwarenessBot.Services;

internal sealed record SafetyTask(
    long Id,
    string Title,
    string Description,
    DateTime? Due,
    bool Completed)
{
    public string Reminder =>
        Due?.ToLocalTime().ToString("yyyy-MM-dd HH:mm") ?? "None";

    public string Status =>
        Completed ? "Completed" :
        Due <= DateTime.UtcNow ? "Due" : "Open";
}

internal sealed class TaskStore
{
    private const string ConnectionVariable = "CYBERBOT_MYSQL";

    private static async Task<MySqlConnection> Open()
    {
        string settings =
            Environment.GetEnvironmentVariable(ConnectionVariable)
            ?? throw new InvalidOperationException(
                "Set CYBERBOT_MYSQL and restart the app.");

        var connection = new MySqlConnection(settings);

        try
        {
            await connection.OpenAsync();
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    public async Task<List<SafetyTask>> Read()
    {
        await using var connection = await Open();

        await using var command = new MySqlCommand(
            "SELECT id, title, description, due_utc, completed " +
            "FROM safety_tasks ORDER BY completed, id DESC",
            connection);

        await using var reader = await command.ExecuteReaderAsync();

        var tasks = new List<SafetyTask>();

        while (await reader.ReadAsync())
        {
            DateTime? due = reader.IsDBNull(3)
                ? null
                : DateTime.SpecifyKind(
                    reader.GetDateTime(3),
                    DateTimeKind.Utc);

            tasks.Add(new SafetyTask(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                due,
                reader.GetBoolean(4)));
        }

        return tasks;
    }

    public async Task<long> Add(
        string title,
        string description,
        DateTime? due)
    {
        await using var connection = await Open();

        await using var command = new MySqlCommand(
            "INSERT INTO safety_tasks " +
            "(title, description, due_utc) " +
            "VALUES (@title, @description, @due)",
            connection);

        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@due", due);

        await command.ExecuteNonQueryAsync();

        return command.LastInsertedId;
    }

    public Task<bool> Complete(long id) =>
        Change(
            "UPDATE safety_tasks SET completed = TRUE " +
            "WHERE id = @id AND completed = FALSE",
            id);

    public Task<bool> Delete(long id) =>
        Change(
            "DELETE FROM safety_tasks WHERE id = @id",
            id);

    public Task<bool> Remind(long id, DateTime due) =>
        Change(
            "UPDATE safety_tasks SET due_utc = @due " +
            "WHERE id = @id AND completed = FALSE",
            id,
            due);

    private static async Task<bool> Change(
        string sql,
        long id,
        DateTime? due = null)
    {
        await using var connection = await Open();

        await using var command = new MySqlCommand(
            sql,
            connection);

        command.Parameters.AddWithValue("@id", id);

        if (due.HasValue)
        {
            command.Parameters.AddWithValue(
                "@due",
                due.Value);
        }

        return await command.ExecuteNonQueryAsync() == 1;
    }
}

