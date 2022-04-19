using AspNetCore.Identity.Mongo.Model;
using IdentityServer.Interfaces;
using IdentityServer.Models.System;
using LionwoodSoftware.DataFilter;
using LionwoodSoftware.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class SystemService : ISystemService
    {
        private readonly IRepository _repository;
        private readonly IAttachmentService _attachmentService;
        private readonly UserManager<MongoUser> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;

        public SystemService(IAttachmentService attachmentService, IRepository repository, UserManager<MongoUser> userManager, RoleManager<MongoRole> roleManager)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _attachmentService = attachmentService;
        }

        public async Task<PrivateUserVM> GetPrivateUserByIdAsync(string id, bool withRoles = false, bool withImage = false)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var privateUser = new PrivateUserVM()
            {
                Id = user.Id.ToString(),
                Email = user.Email,
            };

            if (withRoles)
            {
                var roles = await _userManager.GetRolesAsync(user);
                privateUser.Roles.AddRange(roles);
            }

            if (withImage)
            {
                var imgUrl = await _attachmentService.GetImgURLByOwnerIdAndAttachmentTypeAsync(id, "ProfileImage");
                privateUser.ImageURL = imgUrl;
            }

            return privateUser;
        }

        /// <summary>
        /// Request will get all the users specified by params
        /// </summary>
        /// <param name="dataFilter"> KendoData Filter - company library for complex get queries </param>
        /// <param name="withRoles"> If user wants to get user with roles it must be set to true</param>
        /// <param name="withImage"> If user wants to get user with images it must be set to true</param>
        /// <returns>A list of users</returns>
        public async Task<KendoResponse<PrivateUserVM>> GetPrivateUsersAsync(KendoDataFilter filter, bool withRoles = false, bool withImage = false)
        {
            var users = await _repository.GetCollection<MongoUser>("Users")
               .Find(filter.GetFilters<MongoUser>())
               .ToListAsync();

            var data = new List<PrivateUserVM>();

            foreach (var user in users)
            {
                var res = new PrivateUserVM()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                };

                if (withRoles)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    res.Roles.AddRange(roles);
                }

                if (withImage)
                {
                    var imgUrl = await _attachmentService.GetImgURLByOwnerIdAndAttachmentTypeAsync(user.Id.ToString(), "ProfileImage");
                    res.ImageURL = imgUrl;
                }

                data.Add(res);
            }

            return new KendoResponse<PrivateUserVM>()
            {
                Data = data
            };
        }
    }
}
