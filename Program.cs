using EchoServerOptimization;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//1. Без оптимиазций
/*app.MapGet("/", async context =>
{
    var stopwatch = Stopwatch.StartNew();
    var query = context.Request.QueryString.Value;
    await context.Response.WriteAsync(query);
    stopwatch.Stop();
    Console.Out.WriteLine($"Запрос обработан за {stopwatch.ElapsedMilliseconds} мс.");
});*/

//2. С оптимизацией
var bufferPool = new BufferPool(4096, 100);
app.MapGet("/", async context =>
{
    var stopwatch = Stopwatch.StartNew();
    var query = context.Request.QueryString.Value;
    var queryBytes = Encoding.UTF8.GetBytes(query);

    if (queryBytes.Length < 4096)
    {
        Console.Out.WriteLine($"Запрос меньше 4 Кб.");
        await context.Response.WriteAsync(query);
    }
    else
    {
        Console.Out.WriteLine($"Запрос больше 4 Кб.");
        var buffer = bufferPool.Get();
        try
        {
            queryBytes.AsSpan().CopyTo(buffer);
            await context.Response.BodyWriter.WriteAsync(buffer.AsMemory(0, queryBytes.Length));

        }
        finally
        {
            bufferPool.Return(buffer);
        }
    }

    stopwatch.Stop();
    Console.WriteLine($"Запрос обработан за {stopwatch.ElapsedMilliseconds} мс.");

    Console.WriteLine("Сборка мусора");
    GC.Collect();
    GC.WaitForPendingFinalizers();
});

app.Run();
