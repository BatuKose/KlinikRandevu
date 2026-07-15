using Hangfire;
using KlinikRandevu.Extensions;
using Services.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.Controllers.PatientController).Assembly);

builder.Services.ConfigureSwagger();
builder.Services.AddControllers();
builder.Services.CorsConfigure();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureRateLimiter();
builder.Services.ConfigureJWTToken(builder.Configuration);
builder.Services.ConfigureHangfire(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.AddSerilogLogging();

var app = builder.Build();
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IJobService>(
    "randevu-hatirlatma-mail",
    job => job.HatirlatmalariGonderAsync(),
    "0 6 * * *");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseGlobalExceptionMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("RateLimit");// bütün apilere rate limit uygulanacak
app.Run();
