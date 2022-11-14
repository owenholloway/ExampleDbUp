using Npgsql;

namespace DbUp.Lib;

public class SqlFile
{
    private readonly NpgsqlConnection _connection;
    private readonly string _path;
    private readonly string _filename;

    public SqlFile(string path, string fileName)
    {
        _connection = new NpgsqlConnection(ConnectionString.GetString());
        _path = path;
        _filename = fileName;

    }
    
    public void Run()
    {
        Console.WriteLine($"🟦 Checking SQL File -- {_filename} -- ");
        
        if (HasBeenRun())
        {
            Console.WriteLine($"🟧 Already Run -- {_filename} -- ");
            return;
        }

        if (Execute())
        {
            Console.WriteLine($"🟩 Applied to Schema -- {_filename} -- ");
        }

    }
    
    private bool HasBeenRun()
    {
        _connection.Open();
        var commandString = $"SELECT EXISTS ( SELECT FROM schema_versions WHERE name = '{_filename}' )";

        var command = new NpgsqlCommand(commandString, _connection);
        var reader = command.ExecuteReader();

        bool exist = false;

        while (reader.Read())
        {
            if (reader[0].ToString()!.Contains("True"))
            {
                exist = true;
            }
        }

        while (!reader.IsClosed)
        {
            reader.Close();
        }

        _connection.Close();
        
        return exist;
    }

    private bool Execute(bool recordRun = true)
    {

        _connection.Open();
        
        var fullPath = $"{_path}/{_filename}";
        var fileCommands = File.ReadAllText(fullPath);
        var command = new NpgsqlCommand(fileCommands, _connection);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            var integrityController = new IntegrityController();
            integrityController.AbortUpdate();
            Console.WriteLine(e);
            throw;
        }
        
        var recordCommandString = $"INSERT INTO schema_versions VALUES ('{_filename}',now())";
        var recordCommand = new NpgsqlCommand(recordCommandString, _connection);
        recordCommand.ExecuteNonQuery();
        
        _connection.Close();
        
        return true;
    }
    
}