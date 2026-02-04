using WorkoutTracker.Data.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin(); // restrict in production
    });
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WorkoutRepository>();
builder.Services.AddScoped<WorkoutSetRepository>();
builder.Services.AddScoped<MuscleGroupRepository>();
builder.Services.AddScoped<ExerciseRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkoutTracker.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

// IMPORTANT: enable CORS BEFORE routing/controllers so preflight (OPTIONS) is handled
app.UseCors("AllowLocalDev");

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();

