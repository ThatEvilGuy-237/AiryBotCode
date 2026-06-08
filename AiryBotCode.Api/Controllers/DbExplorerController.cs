using AiryBotCode.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Read-only explorer over the live Postgres database: schemas -> tables -> rows.
    /// Identifiers are validated against the catalog and quoted, so only existing
    /// schema/table names can be queried (no SQL injection via names).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/db")]
    public class DbExplorerController : ControllerBase
    {
        private readonly string _connectionString;

        // Columns whose values must never leave the DB explorer in the clear.
        // Keyed by table name (case-insensitive); BotSettings.Token is the only
        // secret in this database.
        private static readonly Dictionary<string, HashSet<string>> RedactedColumns =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["BotSettings"] = new(StringComparer.OrdinalIgnoreCase) { "Token" },
            };

        private const string RedactionMask = "••••••••";

        public DbExplorerController(IConfiguration configuration)
        {
            _connectionString = new ConfigurationReader(configuration).GetDatabaseConnectionString();
        }

        // GET /api/db/schemas -> ["public", ...]
        [HttpGet("schemas")]
        public async Task<IActionResult> GetSchemas()
        {
            const string sql = @"
                SELECT schema_name FROM information_schema.schemata
                WHERE schema_name NOT IN ('information_schema')
                  AND schema_name NOT LIKE 'pg_%'
                ORDER BY schema_name";
            return Ok(await ReadColumnAsync(sql));
        }

        // GET /api/db/tables?schema=public -> [{ name, rowCount }]
        [HttpGet("tables")]
        public async Task<IActionResult> GetTables([FromQuery] string schema)
        {
            if (string.IsNullOrWhiteSpace(schema)) return BadRequest("schema is required.");

            await using var conn = await OpenAsync();

            // Table names come from the catalog (safe to quote afterwards).
            var names = new List<string>();
            await using (var cmd = new NpgsqlCommand(@"
                SELECT table_name FROM information_schema.tables
                WHERE table_schema = @schema AND table_type = 'BASE TABLE'
                ORDER BY table_name", conn))
            {
                cmd.Parameters.AddWithValue("schema", schema);
                await using var r = await cmd.ExecuteReaderAsync();
                while (await r.ReadAsync()) names.Add(r.GetString(0));
            }

            // Exact counts (small DB; one cheap COUNT per table).
            var tables = new List<object>();
            foreach (var name in names)
            {
                await using var c = new NpgsqlCommand($"SELECT COUNT(*) FROM {Quote(schema)}.{Quote(name)}", conn);
                var count = Convert.ToInt64(await c.ExecuteScalarAsync());
                tables.Add(new { name, rowCount = count });
            }
            return Ok(tables);
        }

        // GET /api/db/data?schema=public&table=ChatUsers&limit=100 -> { columns, rows }
        [HttpGet("data")]
        public async Task<IActionResult> GetData([FromQuery] string schema, [FromQuery] string table, [FromQuery] int limit = 100)
        {
            if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(table))
                return BadRequest("schema and table are required.");
            limit = Math.Clamp(limit, 1, 1000);

            await using var conn = await OpenAsync();

            // Validate identifiers exist before interpolating them into the query.
            await using (var check = new NpgsqlCommand(
                "SELECT 1 FROM information_schema.tables WHERE table_schema=@s AND table_name=@t", conn))
            {
                check.Parameters.AddWithValue("s", schema);
                check.Parameters.AddWithValue("t", table);
                if (await check.ExecuteScalarAsync() is null)
                    return NotFound($"Table {schema}.{table} was not found.");
            }

            await using var cmd = new NpgsqlCommand($"SELECT * FROM {Quote(schema)}.{Quote(table)} LIMIT {limit}", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var columns = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++) columns.Add(reader.GetName(i));

            // Resolve which columns of this table must be masked (e.g. tokens).
            RedactedColumns.TryGetValue(table, out var redacted);

            var rows = new List<Dictionary<string, object?>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>(reader.FieldCount);
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    // Mask secret columns: keep null as null (so "no token" is still
                    // visible), but never return a stored value in the clear.
                    if (redacted != null && redacted.Contains(columns[i]))
                    {
                        row[columns[i]] = reader.IsDBNull(i) ? null : RedactionMask;
                        continue;
                    }

                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    // Stringify non-primitive types so the JSON stays simple for the table UI.
                    row[columns[i]] = value switch
                    {
                        null => null,
                        string or bool or short or int or long or float or double or decimal => value,
                        _ => value.ToString()
                    };
                }
                rows.Add(row);
            }

            return Ok(new { columns, rows });
        }

        private async Task<NpgsqlConnection> OpenAsync()
        {
            var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }

        private async Task<List<string>> ReadColumnAsync(string sql)
        {
            await using var conn = await OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            var list = new List<string>();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(r.GetString(0));
            return list;
        }

        private static string Quote(string ident) => "\"" + ident.Replace("\"", "\"\"") + "\"";
    }
}
