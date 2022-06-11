using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBaron.Http.Controllers;

namespace TheBaron.Views.Discord;

public class Success : PageModel
{
    public string DiscordName { get; set; } = null!;
    public DiscordController.SteamAccount LinkedSteamAccount { get; set; } = null!;
}