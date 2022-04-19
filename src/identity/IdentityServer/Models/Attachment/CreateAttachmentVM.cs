namespace IdentityServer.Models.Attachment
{
    public class CreateAttachmentVM
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string OwnerId { get; set; }

        public string Type { get; set; }
    }
}
