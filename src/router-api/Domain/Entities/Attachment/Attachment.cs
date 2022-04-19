using MongoDB.Bson.Serialization.Attributes;

namespace RouterApi.Domain.Entities.Attachment
{
    [BsonIgnoreExtraElements]
    public class Attachment : BaseEntity
    {
        public string OriginalFileName { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string BucketName { get; set; }

        public string FilePath { get; set; }

        public string OwnerId { get; set; }

        public string Type { get; set; }
    }
}
