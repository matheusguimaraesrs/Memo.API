using Memo.API.Exceptions;
using Memo.Application;
using Memo.Application.Common.CQRS;
using Memo.Infra;

var builder = WebApplication.CreateBuilder(args);

// EXCEPTION HANDLER
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// DI - INFRASTRUCTURE
builder.Services.AddInfrastructure(builder.Configuration);

// DI - APPLICATION
builder.Services.AddApplication();

// CONTROLLERS & SWAGGER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.ConfigObject.AdditionalItems["operationsSorter"] = "method";
        options.ConfigObject.AdditionalItems["tagsSorter"] = "alpha";
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();