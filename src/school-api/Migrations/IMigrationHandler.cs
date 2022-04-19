using LionwoodSoftware.Repository.Interfaces;

namespace SchoolApi.Migrations
{
    interface IMigrationHandler
    {
        void Handle(IRepository repository);
    }
}
