using IdentityServer.Entities;
using IdentityServer.Interfaces;
using IdentityServer.Models.Attachment;
using LionwoodSoftware.MediaStorage.Interfaces;
using LionwoodSoftware.Repository.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string _bucketName;
        private readonly IStorageService _storage;
        private readonly IRepository _repository;

        public AttachmentService(IStorageService storage, IRepository repository)
        {
            _bucketName = Environment.GetEnvironmentVariable("STORAGE_BACKET_NAME") ?? throw new ArgumentNullException("STORAGE_BACKET_NAME");
            _storage = storage;
            _repository = repository;
        }

        public static string ConvertAttachmentToBase64Image(AttachmentVM attachment)
        {
            if (attachment == null)
            {
                return null;
            }

            return attachment.ContentType switch
            {
                "image/jpeg" => "data:image/jpeg;base64, " + Convert.ToBase64String(attachment.Content),
                "image/png" => "data:image/png;base64, " + Convert.ToBase64String(attachment.Content),
                _ => null
            };
        }

        public async Task<string> CreateAsync(CreateAttachmentVM model)
        {
            var id = _repository.NewId();
            if (await _storage.CreateBucketIfNotExists(_bucketName))
            {
                await _storage.UploadAsync($"{model.OwnerId}/{model.FileName}", _bucketName, new MemoryStream(model.Content), model.ContentType);

                await _repository.AddAsync(new Attachment
                {
                    Id = id,
                    ContentType = model.ContentType,
                    BucketName = _bucketName,
                    OriginalFileName = model.FileName,
                    OwnerId = model.OwnerId,
                    FilePath = $"{Environment.GetEnvironmentVariable("PROFILE_IMAGE_URL")}/{id}",
                    FileName = $"{model.OwnerId}/{model.FileName}",
                    Type = model.Type
                });
            }
            else
            {
                throw new Exception("Can't create bucket");
            }

            return id;
        }

        public async Task DeleteAsync(string id)
        {
            var attachment = _repository.FindOneAndDelete<Attachment>(x => x.Id == id);

            await _storage.RemoveFileAsync(_bucketName, attachment.FileName);
        }

        public async Task<AttachmentVM> GetByOwnerIdAndAttachmentTypeAsync(string ownerId, string type)
        {
            var attachment = _repository.FindOne<Attachment>(x => x.OwnerId == ownerId && x.Type == type);

            AttachmentVM attachmentModel = null;
            if (attachment != null)
            {
                attachmentModel = new AttachmentVM
                {
                    Id = attachment.Id,
                    OwnerId = attachment.OwnerId,
                    ContentType = attachment.ContentType,
                    FileName = attachment.OriginalFileName,
                    Type = attachment.Type,
                    FilePath = attachment.FilePath
                };

                try
                {
                    using var stream = await _storage.GetObjectAsync(_bucketName, attachment.FileName);
                    attachmentModel.Content = stream.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return attachmentModel;
        }

        public async Task<string> GetImgURLByOwnerIdAndAttachmentTypeAsync(string ownerId, string type)
        {
            var attachment = await _repository.FindOneAsync<Attachment>(x => x.OwnerId == ownerId && x.Type == type);

            return attachment?.FilePath;
        }

        public async Task<string> GetImgURLByIdAsync(string id)
        {
            var attachment = await _repository.FindOneAsync<Attachment>(x => x.Id == id);

            return attachment?.FilePath;
        }

        public async Task<AttachmentVM> GetByIdAsync(string id)
        {
            var attachment = _repository.FindOne<Attachment>(x => x.Id == id);

            AttachmentVM attachmentModel = null;
            if (attachment != null)
            {
                attachmentModel = new AttachmentVM
                {
                    Id = attachment.Id,
                    OwnerId = attachment.OwnerId,
                    ContentType = attachment.ContentType,
                    FileName = attachment.OriginalFileName,
                    Type = attachment.Type,
                    FilePath = attachment.FilePath
                };

                using var stream = await _storage.GetObjectAsync(_bucketName, attachment.FileName);
                attachmentModel.Content = stream.ToArray();
            }

            return attachmentModel;
        }

        public async Task<List<AttachmentVM>> GetByOwnerIdAndAttachmentTypeAsync(IEnumerable<string> ownerIds, string type)
        {
            var attachments = await _repository.FindAsync<Attachment>(x => ownerIds.Contains(x.OwnerId) && x.Type == type);

            List<AttachmentVM> attachmentModels = new List<AttachmentVM>();
            foreach (var attachment in attachments)
            {
                var attachmentModel = new AttachmentVM
                {
                    Id = attachment.Id,
                    OwnerId = attachment.OwnerId,
                    ContentType = attachment.ContentType,
                    FileName = attachment.OriginalFileName,
                    Type = attachment.Type
                };

                using var stream = await _storage.GetObjectAsync(_bucketName, attachment.FileName);
                attachmentModel.Content = stream.ToArray();

                attachmentModels.Add(attachmentModel);
            }

            return attachmentModels;
        }

        public async Task<bool> UpdateAsync(CreateAttachmentVM model)
        {
            var attachment = _repository.FindOne<Attachment>(x => x.OwnerId == model.OwnerId);

            await _storage.RemoveFileAsync(_bucketName, attachment.FileName);

            await _storage.UploadAsync($"{model.OwnerId}/{model.FileName}", _bucketName, new MemoryStream(model.Content), model.ContentType);

            var result = await _repository.GetCollection<Attachment>().UpdateOneAsync(x => x.Id == attachment.Id,
               Builders<Attachment>.Update.Set(x => x.ContentType, model.ContentType)
               .Set(x => x.BucketName, _bucketName)
               .Set(x => x.OriginalFileName, model.FileName)
               .Set(x => x.OwnerId, model.OwnerId)
               .Set(x => x.FileName, $"{model.OwnerId}/{model.FileName}"));

            return result.ModifiedCount > 0;
        }
    }
}
