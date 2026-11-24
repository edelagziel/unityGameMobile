using System.Threading;
using System.Threading.Tasks;

namespace Game.Services
{
    /// <summary>
    /// A service that must be ready before the first screen uses it.
    /// Bootstrap calls <see cref="InitializeAsync"/> once per service, in the order the services are registered.
    /// It returns a Task so a later service (cloud save, remote config, ads SDK) can wait for the network.
    /// </summary>
    public interface IInitializableService
    {
        Task InitializeAsync(CancellationToken cancellation);
    }
}
