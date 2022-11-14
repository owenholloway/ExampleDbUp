using DbUp.Lib;
using Npgsql;

await using var connection = new NpgsqlConnection(ConnectionString.GetString());
await connection.OpenAsync();


var integrityController = new IntegrityController();

integrityController.BeginUpdate();

var transactionLog = new InitSchema();

transactionLog.SchemaInit();

var structurePath = "./Transforms/03_Structure/";

var files = Directory.GetFiles(structurePath);

foreach (var file in files.OrderBy(q => q))
{
    var fileName = file.Replace(structurePath, "");

    var fileSql = new SqlFile(structurePath, fileName);
    fileSql.Run();

}

integrityController.CompleteUpdate();