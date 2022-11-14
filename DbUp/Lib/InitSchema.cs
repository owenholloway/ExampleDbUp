using System.Text;
using Npgsql;

namespace DbUp.Lib;

public class InitSchema
{
    
    private readonly NpgsqlConnection _connection;
    private readonly bool _dropDb = false;
    
    public InitSchema()
    {
        _connection = new NpgsqlConnection(ConnectionString.GetString());
        var dropDb 
            = Environment.GetEnvironmentVariable("DB_CLEAN");
        if (dropDb != null) _dropDb = bool.Parse(dropDb);
    }

    public void SchemaInit()
    {

        if (_dropDb)
        {
            Console.WriteLine("-~- New database has been requested, will destroy everything 💣 -~-");
            DropAll();
            CreatePublic();
        }
       
        if (DbNeedsInit())
        {
            Console.WriteLine("-~- This is a fresh database, lets track what we do with it -~-");
            CreateDb();
        }
        else
        {
            Console.WriteLine("-~- This database has existing data, checking what needs to be added as we go -~-");
        }
        
        

    }

    private void DropAll()
    {
        _connection.Open();
        
        var test = _connection.FullState;
        
        var commandString = "drop schema public cascade;";
        var recordCommand = new NpgsqlCommand(commandString, _connection);
        var result = recordCommand.ExecuteNonQuery();
        _connection.Close();
        
    }

    private void CreatePublic()
    {
        _connection.Open();

        var test = _connection.FullState;
        
        var commandString = "create schema public;";
        var recordCommand = new NpgsqlCommand(commandString, _connection);
        var result = recordCommand.ExecuteNonQuery();
        _connection.Close();
    }
    
    private bool DbNeedsInit()
    {
        _connection.Open();
        
        var fileSql = File.ReadAllText("./Transforms/01_CheckInit.sql", Encoding.UTF8);
        var command = new NpgsqlCommand(fileSql, _connection);
        var reader = command.ExecuteReader();

        var needsInit = false;

        var result = reader;
        
        while (reader.Read())
        {
            if (!reader[0].ToString()!.Contains("True"))
            {
                needsInit = true;
            }
        }
        
        _connection.Close();
        return needsInit;

    }

    private void CreateDb()
    {
        _connection.Open();
        
        var fileSql = File.ReadAllText("./Transforms/02_Init.sql", Encoding.UTF8);
        var command = new NpgsqlCommand(fileSql, _connection);
        var reader = command.ExecuteReader();

        var needsInit = false;

        var result = reader;
        
        while (reader.Read())
        {
            if (!reader[0].ToString()!.Contains("True"))
            {
                needsInit = true;
            }
        }
        
        _connection.Close();
    }
    
    
}