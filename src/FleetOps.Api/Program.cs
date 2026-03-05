using FleetOps.Application.Assignments.CreateAssignment;
using FleetOps.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CreateAssignmentHandler>();
builder.Services.AddScoped<IValidator<CreateAssignmentCommand>, CreateAssignmentCommandValidator>();

// Healthchecks
string? connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddNpgSql(connectionString!, name: "postgres", tags: new[] { "ready" });

// Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
   Predicate = r => r.Tags.Contains("live") 
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
   Predicate = r => r.Tags.Contains("ready") 
});

app.Run();
