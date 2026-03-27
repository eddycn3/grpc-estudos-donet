using Basics;
using ClientConsoleApp.Interceptors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static Basics.FirstServiceDefinition;



Console.WriteLine("Client thas consume GRPC Service");


// Configuração de retry policy
var retryPolicy  = new MethodConfig
{
    Names = { MethodName.Default },
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,
        InitialBackoff = TimeSpan.FromSeconds(0.5),
        MaxBackoff = TimeSpan.FromSeconds(0.5),
        BackoffMultiplier = 1,
        RetryableStatusCodes = { StatusCode.Internal }
    }
};

// Hedging policy
var headingPolicy = new MethodConfig
{
    Names = { MethodName.Default },
    HedgingPolicy = new HedgingPolicy
    {
        MaxAttempts = 5,
        HedgingDelay = TimeSpan.FromSeconds(0.5),
        NonFatalStatusCodes = { StatusCode.Internal }
    }
};

var options = new GrpcChannelOptions()
{
    ServiceConfig = new ServiceConfig()
    {
        MethodConfigs = { retryPolicy}
    }
};

//using var channel = GrpcChannel.ForAddress("https://localhost:7093", options);
//var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);

var services = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddTransient<ClientLoggerInterceptor>()
    .BuildServiceProvider();

var interceptor = services.GetRequiredService<ClientLoggerInterceptor>();

using var channel = GrpcChannel.ForAddress("https://localhost:7093", options);

// Load balcing configuration
//using var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions()
//{
//    Credentials = ChannelCredentials.Insecure,
//    ServiceConfig = new ServiceConfig()
//    {
//        LoadBalancingConfigs = { new RoundRobinConfig() }
//    },
//     ServiceProvider = services
//});

var client = new FirstServiceDefinition.FirstServiceDefinitionClient(
    channel.Intercept(interceptor)
);

Unary(client);
//ClientStreaming(client);
//ServerStreaming(client);
//BiDirectionalStreaming(client); 

Console.ReadLine();

// Unary
void Unary(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    Console.WriteLine("Unary Call");
    var request = new Request() { Content = "Hello you !" };
    var response = client.Unary(request);

    // Com Call Dead line
    // Grpc.Core.RpcException: 'Status(StatusCode="DeadlineExceeded", Detail="")'
    //var response = client.Unary(request, deadline: DateTime.UtcNow.AddMilliseconds(3));
    Console.WriteLine("Unary : Response " + response);
}

// Client Streaming
async void ClientStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    Console.WriteLine("Client Streaming");
    using var call = client.ClientStream();
    for(var i = 0; i < 100; i++)
    {
        var request = new Request() { Content = $"Hello you ! {i}" };
        await call.RequestStream.WriteAsync(request);
    }
    await call.RequestStream.CompleteAsync();
    Response response = await call;
    Console.WriteLine("ClientStreaming : Response " + response);
}

// Server Streaming
async Task ServerStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    Console.WriteLine("Server Streaming");
    try
    {
        var cancellationToken = new CancellationTokenSource();
        var metadata = new Metadata();
        metadata.Add("my-first-key", "my-first-value");
        metadata.Add("my-second-key", "my-second-value");

        
        using var streamingCall = client.ServerStream(
            new Request() { Content = "Hello" },
            headers: metadata
            );

        await foreach (var response in streamingCall.ResponseStream.ReadAllAsync(cancellationToken.Token))
        {
            if(response.Message.Contains("2"))
                cancellationToken.Cancel();

            Console.WriteLine(response.Message);
        }


        // Getting Trailers from server
        var myTrailers = streamingCall.GetTrailers();
        var myValue = myTrailers.GetValue("trailer-from-server");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {

    }

   
}

//Bidirectional Streaming
async void BiDirectionalStreaming(FirstServiceDefinition.FirstServiceDefinitionClient client)
{
    Console.WriteLine("Bidirectional Streaming");
    using (var call = client.BiDirecitiopnalStream())
    {
        var request = new Request();
        // Senting Requests
        for(var i=0; i < 10; i++)
        {
            request.Content = i.ToString();
            Console.WriteLine("Sending " + request.Content);
            await call.RequestStream.WriteAsync(request);
        }

        // await the responses
        while (await call.ResponseStream.MoveNext())
        {
            var message = call.ResponseStream.Current;
            Console.WriteLine("Received " + message);
        }
        
        await call.RequestStream.CompleteAsync();
    }
}