namespace TheBaron;

public class Configuration
{
    private readonly IConfiguration _configuration;

    public Configuration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetToken()
    {
        return _configuration.GetValue<string>("Discord:Token");
    }

    public ulong GetGuildId()
    {
        return _configuration.GetValue<ulong>("Discord:GuildId");
    }

    public List<ulong> GetRoleIds()
    {
        return _configuration.GetSection("Discord:RoleIds").Get<List<ulong>>();
    }
}