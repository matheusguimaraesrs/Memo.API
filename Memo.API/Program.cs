using Memo.Application;
using Memo.Application.Common.CQRS;
using Memo.Infra;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();