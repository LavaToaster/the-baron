using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBaron.Http.Controllers;

namespace TheBaron.Views.Discord;

public class Select : PageModel
{
    public string DiscordName { get; set; } = null!;
    public List<DiscordController.SteamAccount> SteamAccounts { get; set; } = null!;
}