using EchoServerOptimization;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//1. ��� �����������
/*app.MapGet("/", async context =>
{
    var stopwatch = Stopwatch.StartNew();
    var query = context.Request.QueryString.Value;
    await context.Response.WriteAsync(query);
    stopwatch.Stop();
    Console.Out.WriteLine($"������ ��������� �� {stopwatch.ElapsedMilliseconds} ��.");
});*/

//2. � ������������
var bufferPool = new BufferPool(4096, 100);
app.MapGet("/", async context =>
{
    var stopwatch = Stopwatch.StartNew();
    var query = context.Request.QueryString.Value;
    var queryBytes = Encoding.UTF8.GetBytes(query);

    if (queryBytes.Length < 4096)
    {
        Console.Out.WriteLine($"������ ������ 4 ��.");
        await context.Response.WriteAsync(query);
    }
    else
    {
        Console.Out.WriteLine($"������ ������ 4 ��.");
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
    Console.WriteLine($"������ ��������� �� {stopwatch.ElapsedMilliseconds} ��.");

    Console.WriteLine("������ ������");
    GC.Collect();
    GC.WaitForPendingFinalizers();
});

app.Run();
