using Memo.Application.Common.CQRS;
using Memo.Infra;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// DI - INFRASTRUCTURE
builder.Services.AddInfrastructure(builder.Configuration);

// CQRS
builder.Services.AddTransient<IMediator, CustomMediator>();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IRequest<object>>()
    .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
    .AsImplementedInterfaces()
    .WithTransientLifetime());

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