using System.Collections.Generic;
using LionwoodSoftware.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SchoolApi.Migrations.Handlers;

namespace SchoolApi.Migrations
{
    public static class MigrationExtension
    {
        private static Dictionary<string, IMigrationHandler> migrations;

        static MigrationExtension()
        {
            // order is matter
            migrations = new Dictionary<string, IMigrationHandler>
            {
                { "test_migration", new TestHandler() },
            };
        }

        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            var repository = app.ApplicationServices.GetService<IRepository>();

            var collection = repository.GetCollection<Migration>();
            foreach (var migration in migrations)
            {
                if (collection.CountDocuments(x => x.Name == migration.Key) <= 0)
                {
                    migration.Value.Handle(repository);
                    collection.InsertOne(new Migration(migration.Key));
                }
            }
        }
    }
}
