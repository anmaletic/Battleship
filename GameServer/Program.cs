using GameServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR().AddHubOptions<GameHub>(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddCors(policy=>
{
    policy.AddPolicy("OpenCorsPolicy",opt=>opt
        .WithOrigins(
            "http://localhost:7200",
            "https://localhost:7201",           
            "http://localhost:7300",
            "https://localhost:7301",
            "https://battleship.anmal.dev")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseStaticFiles();

app.MapHub<GameHub>("/gamehub");

app.Run();
