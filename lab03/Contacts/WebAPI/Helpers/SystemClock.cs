using Application.OutputPorts;

namespace WebAPI.Helpers;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
