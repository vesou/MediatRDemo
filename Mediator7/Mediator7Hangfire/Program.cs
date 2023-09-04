using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Mediator7Hangfire.Behaviours;
using Mediator7Hangfire.Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(TimingPipelineBehaviour<,>));
    cfg.AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>));
    cfg.AddOpenBehavior(typeof(RetryPipelineBehaviour<,>));
});

builder.Services.AddHangfireServ(builder.Configuration);

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
var filter = new BasicAuthAuthorizationFilter(
    new BasicAuthAuthorizationFilterOptions
    {
        RequireSsl = false,
        SslRedirect = false,
        LoginCaseSensitive = true,
        Users = new[]
        {
            new BasicAuthAuthorizationUser
            {
                Login = app.Configuration.GetSection("Hangfire:Dashboard:UserName").Value,
                // Password as plain text, SHA1 will be used
                PasswordClear = app.Configuration.GetSection("Hangfire:Dashboard:Password").Value
            }
        }
    });
app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { filter } });

app.Run();