using Firebase.Authentication.Helpers;
using FirebaseAdmin;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection(nameof(EmailConfig)));
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection(nameof(FirebaseConfig)));
builder.Services.AddScoped<FirebaseHelper>();

FirebaseApp.Create(new AppOptions()
{
    ProjectId = builder.Configuration.GetSection("FirebaseConfig:ProjectId").Value,
    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(),builder.Configuration.GetSection("FirebaseConfig:Credentials").Value))
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

EmailHelper.AppSettingConfig(app.Services.GetRequiredService<IOptions<EmailConfig>>());

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
