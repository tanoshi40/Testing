using System.Diagnostics;
using database_testing;
using mocking;
using MySqlConnector;

Console.WriteLine("Start");

var sqlConnection = CreateConnection();
await sqlConnection.OpenAsync();
await DatabaseHandling.Truncate(sqlConnection);
await sqlConnection.CloseAsync();


// await Warmup();
//
// await InsertDataTable();
// await InsertStringBuilder();
await InsertBulkCopy();
// await InsertTransaction();
// await InsertPrimitive();

Console.WriteLine("Done");
return;

async Task InsertPrimitive()
{
    Console.WriteLine("Primitive");
    var persons = CreatePersons().ToArray();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();
    await DatabaseHandling.Truncate(mySqlConnection);

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncPrimitive(persons, mySqlConnection);
    stopwatch.Stop();


    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
    await mySqlConnection.CloseAsync();
}

async Task InsertTransaction()
{
    Console.WriteLine("Transaction");
    var persons = CreatePersons().ToArray();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();
    await DatabaseHandling.Truncate(mySqlConnection);

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncTransaction(persons, mySqlConnection);
    stopwatch.Stop();


    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
    await mySqlConnection.CloseAsync();
}

async Task InsertStringBuilder()
{
    Console.WriteLine("StringBuilder");
    var persons = CreatePersons().ToArray();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();
    await DatabaseHandling.Truncate(mySqlConnection);

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncBulkStringBuilder(persons, mySqlConnection);
    stopwatch.Stop();


    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
    await mySqlConnection.CloseAsync();
}

async Task InsertBulkCopy()
{
    Console.WriteLine("BulkCopy");
    var persons = CreatePersons().ToArray();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();
    await DatabaseHandling.Truncate(mySqlConnection);

    stopwatch.Start();
    await DatabaseHandling.InsertPersonAsyncBulkCopy(persons, mySqlConnection);
    stopwatch.Stop();


    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
    await mySqlConnection.CloseAsync();
}

async Task InsertDataTable()
{
    Console.WriteLine("DataSet");
    var persons = CreatePersons().ToArray();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();
    await DatabaseHandling.Truncate(mySqlConnection);

    stopwatch.Start();
    await DatabaseHandling.InsertPersonAsyncBulkDataTable(persons, mySqlConnection);
    stopwatch.Stop();


    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
    await mySqlConnection.CloseAsync();
}

IEnumerable<Person> CreatePersons(int amount = 50_000)
{
    // Console.WriteLine("Creating Data");
    return Person.GeneratePersons(amount).ToArray();
}

MySqlConnection CreateConnection()
{
    // Console.WriteLine("Creating Connection");
    return DatabaseHandling.GetConnection();
}

async Task Warmup()
{
    Console.WriteLine("WARMUP");
    Console.WriteLine("WARMUP");
    Console.WriteLine("WARMUP");
    Console.WriteLine("");
    Console.WriteLine("");
    Console.WriteLine("");

    for (var i = 0; i < 10; i++)
    {
        await InsertBulkCopy();
    }

    Console.WriteLine("");
    Console.WriteLine("");
    Console.WriteLine("");
    Console.WriteLine("WARMUP DONE");
    Console.WriteLine("WARMUP DONE");
    Console.WriteLine("WARMUP DONE");
    Console.WriteLine("");
    Console.WriteLine("");
    Console.WriteLine("");
}