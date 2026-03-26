using Basics;
using Grpc.Core;
using Grpc.Net.Client;

Console.WriteLine("Client thas consume GRPC Service");

var options = new GrpcChannelOptions()
{

};

using var channel = GrpcChannel.ForAddress("https://localhost:7093", options);
var client = new FirstServiceDefinition.FirstServiceDefinitionClient(channel);

//Unary(client);
//ClientStreaming(client);
ServerStreaming(client);
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
        
        using var streamingCall = client.ServerStream(new Request() { Content = "Hello" });
        await foreach (var response in streamingCall.ResponseStream.ReadAllAsync(cancellationToken.Token))
        {
            if(response.Message.Contains("2"))
                cancellationToken.Cancel();

            Console.WriteLine(response.Message);
        }
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