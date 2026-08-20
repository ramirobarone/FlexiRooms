
var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("hotelisPassword");

var postgres = builder
    .AddPostgres("postgresServer", password: password, port: 5432)
    .WithDataVolume("hotelisVolumen")
    .AddDatabase("hotelis");

var rabbit = builder.AddRabbitMQ("serverRabbit");


builder.AddProject<Projects.ClientApp>("clientapp")
    .WithReference(postgres)
    .WithReference(rabbit);

builder.Build().Run();
