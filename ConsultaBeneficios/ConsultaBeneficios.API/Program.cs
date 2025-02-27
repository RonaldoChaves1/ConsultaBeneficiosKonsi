using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.KonsiClient;
using ConsultaBeneficios.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var KonsiApiBaseUrl = builder.Configuration.GetValue<string>("KonsiClientConfig:BaseUrl");
var KonsiApiUsuario = builder.Configuration.GetValue<string>("KonsiClientConfig:Usuario");
var KonsiApiSenha = builder.Configuration.GetValue<string>("KonsiClientConfig:Senha");

// Add services to the container.
builder.Services.AddSingleton(new KonsiApiClient(KonsiApiBaseUrl, KonsiApiUsuario, KonsiApiSenha));
builder.Services.AddTransient<IBeneficiarioServices, BeneficiarioServices>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("V1", new OpenApiInfo { Title="ConsultaBeneficiosAPI", Version="v1" });
    c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "ConsultaBeneficiosAPI.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
