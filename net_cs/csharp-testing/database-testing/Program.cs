using System.Diagnostics;
using database_testing;
using mocking;
using MySqlConnector;

Console.WriteLine("Connection");

// await Warmup();


await InsertDataTable();
await InsertStringBuilder();
await InsertBulkCopy();
// await InsertTransaction();
// await InsertPrimitive();

Console.WriteLine("Done");
return;

async Task InsertPrimitive()
{
    Console.WriteLine("Primitive");
    var persons = CreatePersons();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncPrimitive(persons, mySqlConnection);
    stopwatch.Stop();

    await DatabaseHandling.Truncate(mySqlConnection);

    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
}

async Task InsertTransaction()
{
    Console.WriteLine("Transaction");
    var persons = CreatePersons();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncTransaction(persons, mySqlConnection);
    stopwatch.Stop();

    await DatabaseHandling.Truncate(mySqlConnection);

    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
}

async Task InsertStringBuilder()
{
    Console.WriteLine("StringBuilder");
    var persons = CreatePersons();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();

    stopwatch.Start();
    await DatabaseHandling.InsertPersonsAsyncBulkStringBuilder(persons, mySqlConnection);
    stopwatch.Stop();

    await DatabaseHandling.Truncate(mySqlConnection);

    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
}

async Task InsertBulkCopy()
{
    Console.WriteLine("BulkCopy");
    var persons = CreatePersons();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();

    stopwatch.Start();
    await DatabaseHandling.InsertPersonAsyncBulkCopy(persons, mySqlConnection);
    stopwatch.Stop();

    await DatabaseHandling.Truncate(mySqlConnection);

    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
}

async Task InsertDataTable()
{
    Console.WriteLine("DataSet");
    var persons = CreatePersons();
    var stopwatch = new Stopwatch();

    await using var mySqlConnection = CreateConnection();
    await mySqlConnection.OpenAsync();

    stopwatch.Start();
    await DatabaseHandling.InsertPersonAsyncBulkDataTable(persons, mySqlConnection);
    stopwatch.Stop();

    await DatabaseHandling.Truncate(mySqlConnection);

    var time = stopwatch.Elapsed;
    Console.WriteLine(time);
}

IEnumerable<Person> CreatePersons()
{
    // Console.WriteLine("Creating Data");
    return Person.GeneratePersons(50_000).ToArray();
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
        await InsertDataTable();
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