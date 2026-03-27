using Basics;
using Grpc.Core;

namespace EstudoServiceGrpc.Services
{
    public class FirstService : FirstServiceDefinition.FirstServiceDefinitionBase, IFirstService
    {
        public override Task<Response> Unary(Request request, ServerCallContext context)
        {
            if (!context.RequestHeaders.Where(x => x.Key == "grpc-previous-rpc-attempts").Any())
            {
                throw new RpcException(new Status(StatusCode.Internal, "Not here: try again"));
            }

            var response = new Response
            {
                Message = $"Hello {request.Content} from server {context.Host}!"
            };

            return Task.FromResult(response);
        }

        public override async Task<Response> ClientStream(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
        {
            Response response = new Response() { Message = "I got" };

            while (await requestStream.MoveNext())
            {
                var requestPayload = requestStream.Current;
                Console.WriteLine(requestPayload);
                response.Message = requestPayload.ToString();
            }

            return response;
        }

        public override async Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            var metadataFirstKey = context.RequestHeaders.GetValue("my-first-key");

            var myTrailer = new Metadata.Entry("trailer-from-server", "trailer-value");
            context.ResponseTrailers.Add(myTrailer);

            for (var i = 0; i < 100; i++)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var response = new Response() { Message = $"Hello {request.Content} from server! {i}" };
                await responseStream.WriteAsync(response);
            }
        }

        public override async Task BiDirecitiopnalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {
            Response response = new Response() { Message = "" };
            while (await requestStream.MoveNext())
            {
                var requestPayload = requestStream.Current;
                response.Message = requestPayload.ToString();
                await responseStream.WriteAsync(response);
            }
        }
    }
}
