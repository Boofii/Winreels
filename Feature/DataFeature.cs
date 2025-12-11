using Winreels.Core;

namespace Winreels;

/// <summary>
/// This class is used as an extension of DatabaseFragment.
/// It provides utilities to interact with a database on the server side easily.
/// </summary>
public class DataFeature
{
    private readonly ServerFragment? server;
    private readonly string? name;
    private readonly DatabaseFragment? database;
    private readonly Dictionary<string, Func<object, int>> rules = [];
    private readonly Dictionary<string, Action<string[]>> commands = [];

    // Creates a new DataFeature.
    public DataFeature(ServerFragment server, string name, DatabaseFormat format)
    {
        this.server = server;
        server.OnReceived += ParseCommand;
        this.name = name;
        this.database = new DatabaseFragment(name)
            .WithFormat(format);
        database.Establish();
    }

    // Adds a rule (a condition) for a certain value linked to a key.
    public DataFeature AddRule(string fieldName, Func<object, int> rule)
    {
        if (this.database == null)
            return this;

        rules.TryAdd(fieldName, rule);
        return this;
    }

    // Adds a response for a certain command name.
    public DataFeature AddCommand(string cmd, Action<string[]> action)
    {
        if (this.database == null)
            return this;
        
        commands.TryAdd(cmd, action);
        return this;
    }

    // Inserts values into the database, and returns 0 if done, or a certain rule's error code.
    public int Put(string[] keys, object[] values)
    {
        if (database == null)
            return -1;
        
        string sql1 = $"INSERT INTO {name} (";
        string sql2 = $"VALUES (";

        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            object value = values[i];
            if (rules.TryGetValue(key, out Func<object, int>? rule))
            {
                int result = rule.Invoke(value);
                if (result != 0) {
                    return result;
                }
            }
            sql1 += key;
            sql2 += $"@{key}";
            if (i < keys.Length - 1)
            {
                sql1 += ", ";
                sql2 += ", ";
            }
        }
        sql1 += ") ";
        sql2 += ")";

        database.NonQuery(sql1 + sql2, keys, values);
        return 0;
    }

    // Returns true if the keys with their matching values exist in the database.
    public bool Exists(string[] keys, object[] values)
    {
        if (database == null)
            return false;

        string sql = $"SELECT * FROM {name} WHERE ";
        
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            object value = values[i];
            if (rules.TryGetValue(key, out Func<object, int>? rule))
            {
                int res = rule.Invoke(value);
                if (res != 0) {
                    return false;
                }
            }
            sql += $"{key} = @{key}";
            if (i < keys.Length - 1)
            {
                sql += " AND ";
            }
        }

        List<Dictionary<string, object>> result = database.Query(sql, keys, values);
        return result.Count > 0;
    }

    // Parses a command on the server side.
    private void ParseCommand(int id, string cmd, string[] args)
    {
        if (commands.TryGetValue(cmd, out Action<string[]>? action))
        {
            action.Invoke(args);
        }
    }
}