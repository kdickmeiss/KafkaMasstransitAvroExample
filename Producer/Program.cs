using Producer;
using Producer.AppSettings;

var builder = WebApplication.CreateBuilder(args);
var kafkaConfiguration = new KafkaConfiguration();

// Add services to the container.
builder.Services.AddControllers();

builder.Configuration.GetSection(nameof(KafkaConfiguration)).Bind(kafkaConfiguration);
builder.Services.Configure<KafkaConfiguration>(builder.Configuration.GetSection("KafkaConfiguration"));

builder.Services.AddMasstransitKafkaServiceExtension(kafkaConfiguration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
