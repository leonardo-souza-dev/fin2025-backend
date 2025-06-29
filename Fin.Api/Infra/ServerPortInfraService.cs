using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Fin.Api.Infra;

public class ServerPortInfraService(IServer server)
{
    public int GetPort()
    {
        var addresses = server.Features.Get<IServerAddressesFeature>();
        if (addresses?.Addresses?.FirstOrDefault() is string address)
        {
            var uri = new Uri(address);
            return uri.Port;
        }
        return 0; // ou lance uma exceção se preferir
    }
}