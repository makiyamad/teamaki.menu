
using System;
using Raven.Client.Documents;
using TeaMaki.Menu;

namespace TeaMaki.Persistence
{
    public interface IRavenDbContext
    {
        public IDocumentStore store { get; }
    }
}