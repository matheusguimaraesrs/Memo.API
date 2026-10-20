using Memo.API.Exceptions;
using Memo.Application;
using Memo.Application.Common.CQRS;
using Memo.Infra;

var builder = WebApplication.CreateBuilder(args);

// EXCEPTION HANDLER
builder.Services.AddExceptionHandler<ExceptionHandler>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();