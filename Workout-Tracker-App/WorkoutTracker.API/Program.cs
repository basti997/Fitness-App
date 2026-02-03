// We need to tell our app about the new folder
using WorkoutTracker.Data.Repositories; 

var builder = WebApplication.CreateBuilder(args);
// --- This is the ONLY line we need to register our module ---
// This "registers" our new Repository so the controller can use it.
// --- End of change ---

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WorkoutRepository>();
builder.Services.AddScoped<WorkoutSetRepository>();
builder.Services.AddScoped<MuscleGroupRepository>();
builder.Services.AddScoped<ExerciseRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.UseRouting();

// MOVE CORS HERE (before Authorization and MapControllers)
app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

app.UseAuthorization();

// Map controllers after CORS and Authorization
app.MapControllers();

app.Run();