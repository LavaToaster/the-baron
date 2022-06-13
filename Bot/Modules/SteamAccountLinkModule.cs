using Discord;
using Discord.Interactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using TheBaron.Data;

namespace TheBaron.Bot.Modules;

public class SteamAccountLinkModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly AppDbContext _context;
    private readonly LinkGenerator _linkGenerator;

    public SteamAccountLinkModule(AppDbContext context, LinkGenerator linkGenerator)
    {
        _context = context;
        _linkGenerator = linkGenerator;
    }

    public InteractionService Commands { get; set; } = null!;

    [SlashCommand("echo", "Repeat the input")]
    public async Task Echo(string echo)
    {
        await Context.Channel.SendMessageAsync(echo);

        await DeferAsync();
        await DeleteOriginalResponseAsync();
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task Ping()
    {
        await RespondAsync($":ping_pong: It took me {Context.Client.Latency}ms to respond to you!",
            ephemeral: true);
    }

    [SlashCommand("setup-linker", "Put the button in")]
    public async Task SetupLinker()
    {
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");

        if (urls == null || urls.Length == 0)
        {
            await RespondAsync("Cannot read urls", ephemeral: true);
            return;
        }

        var cb = new ComponentBuilder()
            .WithButton("Link Account!", url: $"{urls[0]}{_linkGenerator.GetPathByAction("select", "Discord")}", style: ButtonStyle.Link);

        // Send a message with content 'pong', including a button.
        // This button needs to be build by calling .Build() before being passed into the call.
        await Context.Channel.SendMessageAsync(" ", components: cb.Build());

        await DeferAsync();
        await DeleteOriginalResponseAsync();
    }

    [SlashCommand("unlink-account", "Unlink a persons steam account")]
    public async Task Unlink(IUser discordUser)
    {
        var user = await _context.Users.FindAsync(discordUser.Id);

        if (user == null)
        {
            await RespondAsync("This user has no steam account linked", ephemeral: true);
            return;
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        await RespondAsync("Steam account has been unlinked from user", ephemeral: true);
    }
}