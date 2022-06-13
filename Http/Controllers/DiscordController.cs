using System.ComponentModel.DataAnnotations;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBaron.Data;
using TheBaron.Entities;
using TheBaron.Views.Discord;

namespace TheBaron.Http.Controllers;

[Authorize]
[Route("/discord")]
public class DiscordController : Controller
{
    private readonly DiscordRestClient _client;
    private readonly AppDbContext _context;
    private readonly DiscordSocketClient _socketClient;
    private readonly Configuration _configuration;

    public DiscordController(DiscordRestClient client, AppDbContext context, DiscordSocketClient socketClient,
        Configuration configuration)
    {
        _client = client;
        _context = context;
        _socketClient = socketClient;
        _configuration = configuration;
    }

    [HttpGet("select")]
    public async Task<IActionResult> Select()
    {
        var member = await _socketClient.Rest.GetGuildUserAsync(_configuration.GetGuildId(), _client.CurrentUser.Id);

        if (member == null)
        {
            return View("Error");
        }

        var canAccess = member.RoleIds.Intersect(_configuration.GetRoleIds()).Any();

        if (!canAccess)
        {
            return View("NoRole");
        }

        var user = await _context.Users.FindAsync(_client.CurrentUser.Id);

        if (user != null)
        {
            return RedirectToAction("Info");
        }

        var steamAccounts = await GetSteamAccounts();

        var viewModel = new Select
        {
            DiscordName = $"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}",
            SteamAccounts = steamAccounts,
        };

        // if (!steamAccounts.Any()) return View();
        // if (steamAccounts.Count > 1) return View();

        return View(viewModel);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> PostConfirm(SelectAccountForm input)
    {
        var user = await _context.Users.FindAsync(_client.CurrentUser.Id);

        if (user != null)
        {
            return RedirectToAction("Info");
        }

        var steamAccounts = await GetSteamAccounts();

        if (!steamAccounts.Exists(x => x.SteamId == input.AccountId))
        {
            return RedirectToAction("Select");
        }

        _context.Users.Add(new User
        {
            DiscordId = _client.CurrentUser.Id,
            SteamId = input.AccountId,
        });

        await _context.SaveChangesAsync();

        return RedirectToAction("Success");
    }

    [HttpGet("success")]
    public async Task<IActionResult> Success()
    {
        var user = await _context.Users.FindAsync(_client.CurrentUser.Id);

        if (user == null)
        {
            return RedirectToAction("Select");
        }

        var steamAccounts = await GetSteamAccounts();
        var steamAccount = steamAccounts.First(x => x.SteamId == user.SteamId)!;

        return View(new Success
        {
            DiscordName = $"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}",
            LinkedSteamAccount = steamAccount,
        });
    }

    [HttpGet("info")]
    public async Task<IActionResult> Info()
    {
        var user = await _context.Users.FindAsync(_client.CurrentUser.Id);

        if (user == null)
        {
            return RedirectToAction("Select");
        }

        var steamAccounts = await GetSteamAccounts();
        var steamAccount = steamAccounts.First(x => x.SteamId == user.SteamId)!;

        return View(new Info
        {
            DiscordName = $"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}",
            LinkedSteamAccount = steamAccount,
        });
    }

    private async Task<List<SteamAccount>> GetSteamAccounts()
    {
        var connections = await _client.GetConnectionsAsync();
        var steamIds = from connection in connections
            where connection.Type == "steam"
            where connection.Verified
            select new SteamAccount(Convert.ToUInt64(connection.Id), connection.Name);

        return steamIds.ToList();
    }

    public record SteamAccount(ulong SteamId, string Username);

    public class SelectAccountForm
    {
        [Required] public ulong AccountId { get; set; }
    }
}