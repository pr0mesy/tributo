using Microsoft.EntityFrameworkCore;
using TaxCalculatorBR.Application.Services;
using TaxCalculatorBR.Domain.Interfaces;
using TaxCalculatorBR.Infrastructure.Data;
using TaxCalculatorBR.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// db
builder.Services.AddDbContext<TaxDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// repository
builder.Services.AddScoped<IAliquotaRepository, AliquotaRepository>();

// calculators
builder.Services.AddScoped<IcmsCalculator>();
builder.Services.AddScoped<IssCalculator>();
builder.Services.AddScoped<IpiCalculator>();

// service
builder.Services.AddScoped<ITaxCalculatorService>(sp => new TaxCalculatorService(
    sp.GetRequiredService<IcmsCalculator>(),
    sp.GetRequiredService<IssCalculator>(),
    sp.GetRequiredService<IpiCalculator>()
));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();   // serve index.html automaticamente na raiz
app.UseStaticFiles();    // serve os arquivos da pasta wwwroot
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();