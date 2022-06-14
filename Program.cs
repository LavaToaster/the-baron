using AspNet.Security.OAuth.Discord;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TheBaron;
using TheBaron.Bot.Extensions;
using TheBaron.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddDiscord();

builder.Services.AddSingleton<Configuration>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddDiscord(options =>
    {
        var authSection = builder.Configuration.GetSection("Discord:Authentication");

        options.ClientId = authSection["ClientId"];
        options.ClientSecret = authSection["ClientSecret"];

        options.Scope.Add("guilds.members.read");
        options.Scope.Add("connections");

        options.SaveTokens = true;
    });

builder.Services.AddScoped(x =>
{
    var client = new DiscordRestClient();
    var ctx = x.GetRequiredService<IHttpContextAccessor>();
    var token = ctx.HttpContext!
        .GetTokenAsync(DiscordAuthenticationDefaults.AuthenticationScheme, "access_token")
        .GetAwaiter()
        .GetResult();

    client.LoginAsync(TokenType.Bearer, token)
        .GetAwaiter()
        .GetResult();

    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// app.MapRazorPages();
app.MapControllers();

app.Run();