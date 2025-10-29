namespace apiRESTCine.Data;   // <-- EXACTO

using Microsoft.Extensions.Configuration;

public class CineDbContext
{
    public string ConnectionString { get; }

    public CineDbContext(IConfiguration config)
    {
        ConnectionString = config.GetConnectionString("CineDb")
            ?? throw new InvalidOperationException("Falta 'CineDb' en appsettings.json");
    }
}
