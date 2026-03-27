using EstudoServiceGrpc.Interceptors;
using EstudoServiceGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(option =>
{
    option.Interceptors.Add<ServerLoggingInterceptor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<FirstService>();


app.Run();
