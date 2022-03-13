using Consumer;
using Consumer.AppSettings;

var builder = WebApplication.CreateBuilder(args);
var kafkaConfiguration = new KafkaConfiguration();

// Add services to the container.

builder.Configuration.GetSection(nameof(KafkaConfiguration)).Bind(kafkaConfiguration);
builder.Services.Configure<KafkaConfiguration>(builder.Configuration.GetSection("KafkaConfiguration"));

builder.Services.AddMasstransitKafkaServiceExtension(kafkaConfiguration);

var app = builder.Build();

app.Run();
