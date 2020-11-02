using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using Raven.Client.Exceptions.Database;
using TeaMaki.Menu;

namespace TeaMaki.Persistence
{
    public class RavenDbContext : IRavenDbContext
    {
        private readonly DocumentStore _localStore;
        public IDocumentStore store => _localStore;
        private readonly PersistenceSettings _persistenceSettings;

        public RavenDbContext(IOptionsMonitor<PersistenceSettings> settings)
        {
            _persistenceSettings = settings.CurrentValue;

            _localStore = new DocumentStore()
            {
                Database = _persistenceSettings.DatabaseName,
                Urls = _persistenceSettings.Urls
            };

            _localStore.Initialize();

            EnsureDatabaseIsCreated();
        }

        public void EnsureDatabaseIsCreated()
        {

            try
            {
                _localStore.Maintenance.ForDatabase(_persistenceSettings.DatabaseName).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {

                _localStore.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(_persistenceSettings.DatabaseName)));

            }

        }

    }
}