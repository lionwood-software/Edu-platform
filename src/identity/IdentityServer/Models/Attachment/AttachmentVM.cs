namespace IdentityServer.Models.Attachment
{
    public class AttachmentVM
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public string OwnerId { get; set; }

        public string Type { get; set; }
    }
}
