using System.Data;
using Npgsql;

namespace DbUp.Lib;

public class IntegrityController
{

    private NpgsqlConnection _connection;
    
    public IntegrityController()
    {
        _connection = new NpgsqlConnection(ConnectionString.GetString());
    }

    public void BeginUpdate()
    {
        _connection.Open();
        Console.WriteLine("-~- Starting schema update, hold onto your hats 🎩 -~-");
        
        var commandString = "begin";
        var recordCommand = new NpgsqlCommand(commandString, _connection);
        var recordResult = recordCommand.ExecuteNonQuery();
        _connection.Close();
    }

    public void AbortUpdate()
    {
        _connection.Open();
        Console.WriteLine("!!! 😱 ABORT TRIGGERED ROLLING BACK 😱 !!!");
        
        var commandString = "rollback";
        var recordCommand = new NpgsqlCommand(commandString, _connection);
        var result = recordCommand.ExecuteNonQuery();
        _connection.Close();
    }
    
    public void CompleteUpdate()
    {
        _connection.Open();
        Console.WriteLine("-~- Looks like we are good to go, cleaning up and committing 🧹 -~-");

        var test = _connection.FullState;
        
        while (_connection.FullState is ConnectionState.Executing or ConnectionState.Fetching)
        {
            Console.WriteLine("waiting for command to complete ....");
            Thread.Sleep(1000);
        }
        
        var commandString = "end transaction";
        var recordCommand = new NpgsqlCommand(commandString, _connection);
        var result = recordCommand.ExecuteNonQuery();
        _connection.Close();
    }
    
}