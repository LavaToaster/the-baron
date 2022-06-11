using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBaron.Http.Controllers;

namespace TheBaron.Views.Discord;

public class Info : PageModel
{
    public string DiscordName { get; set; } = null!;
    public DiscordController.SteamAccount LinkedSteamAccount { get; set; } = null!;
}