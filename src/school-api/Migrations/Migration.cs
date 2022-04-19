using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SchoolApi.Migrations
{
    [BsonIgnoreExtraElements]
    public class Migration
    {
        public Migration()
        {
            Applied = DateTime.Now;
        }

        public Migration(string name)
        {
            Name = name;
            Applied = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Applied { get; set; }
    }
}
