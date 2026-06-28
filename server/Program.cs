using AI_Assisted_URL_Shortener.Middleware;
using AI_Assisted_URL_Shortener.Repositories;
using AI_Assisted_URL_Shortener.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IUrlRepository, UrlRepository>();
builder.Services.AddSingleton<IUrlService, UrlService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowLocal");
app.UseMiddleware<ExceptionMiddleware>();
app.MapGet("/", () => Results.Ok(new { message = "AI-Assisted URL Shortener API is running." }));
app.MapControllers();

app.Run();

public partial class Program { }
