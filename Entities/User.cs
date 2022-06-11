using System.ComponentModel.DataAnnotations;

namespace TheBaron.Entities;

public class User
{
    [Key]
    public ulong DiscordId { get; set; }
    public ulong SteamId { get; set; }
}