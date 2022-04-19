using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using LionwoodSoftware.DataFilter;
using LionwoodSoftware.Repository.Interfaces;
using LionwoodSoftware.ResponseHandler.Exceptions;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using RouterApi.ApiClients.Identity;
using RouterApi.ApiClients.Identity.Enums;
using RouterApi.Domain.Entities.Request;
using RouterApi.Domain.Enums;
using RouterApi.Interfaces.Repositories;
using RouterApi.Interfaces.Services;
using RouterApi.Models.Request;
using RouterApi.Observers.Request;

namespace RouterApi.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRepository _repository;
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly IdentityApiClient _identityApiClient;
        private readonly RequestEventHandler _requestEventHandler;

        public RequestService(IMapper mapper,
            IRepository repository,
            IRequestRepository requestRepository,
            RequestEventHandler requestEventHandler,
            IdentityApiClient identityApiClient)
        {
            _mapper = mapper;
            _repository = repository;
            _requestRepository = requestRepository;
            _identityApiClient = identityApiClient;
            _requestEventHandler = requestEventHandler;
        }

        /// <summary>
        /// A method that will create request that will holds inside router API as unapproved until someone approved it.
        /// In our case the request will be ChildRequest Type
        /// </summary>
        /// <param name="userId">User that send that request</param>
        /// <param name="requestChildModel">Model that will passed to DB</param>
        /// <returns>Returns an Id of created request</returns>
        /// <exception cref="ValidationException">Exceptions depeneds on what validation it does not pass</exception>
        public async Task<string> CreateChildRequestAsync(string userId, CreateChildRequestVM requestChildModel)
        {
            var sender = await _identityApiClient.GetUserAsync(userId);
            if (sender == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            var child = await _identityApiClient.GetUserByCodeAsync(requestChildModel.Code);
            if (child == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            if (child.Type != UserType.Child)
            {
                throw new ValidationException("lorem ipsum");
            }

            if (sender.Relations.Any(x => x.Id == child.Id))
            {
                throw new ValidationException("lorem ipsum");
            }

            var receivers = child.Relations.Select(x => x.Id).ToList();

            if (!receivers.Any())
            {
                throw new ValidationException("lorem ipsum");
            }

            Request request = new Request()
            {
                Receivers = receivers,
                CreatedAt = DateTime.Now,
                Child = new RequestChild
                {
                    Id = child.Id,
                    Code = child.Code,
                    FirstName = child.FirstName,
                    MiddleName = child.MiddleName,
                    LastName = child.LastName
                },
                Sender = new RequestSender
                {
                    Id = sender.Id,
                    Email = sender.Email,
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    FullName = $"{sender.LastName} {sender.FirstName} {sender.MiddleName}"
                },
                School = null,
                Type = RequestType.Child,
                Status = RequestStatus.Pending
            };

            var newId = await _requestRepository.AddAsync(request);

            _requestEventHandler.CreateInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                ChangedByUserId = userId,
            });

            return newId;
        }

        /// <summary>
        /// Same as previous function but except type of request.
        /// </summary>
        public async Task<string> CreateSchoolRequestAsync(string userId, CreateSchoolRequestVM model)
        {
            var sender = await _identityApiClient.GetUserAsync(userId);
            if (sender == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            var child = await _identityApiClient.GetUserAsync(model.ChildId);
            if (child == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            if (!string.IsNullOrEmpty(child.SchoolUId))
            {
                throw new ValidationException($"lorem ipsum");
            }

            Request request = new Request
            {
                Child = new RequestChild
                {
                    Id = child.Id,
                    Code = child.Code,
                    FirstName = child.FirstName,
                    MiddleName = child.MiddleName,
                    LastName = child.LastName
                },
                Sender = new RequestSender
                {
                    Id = sender.Id,
                    Email = sender.Email,
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    FullName = $"{sender.LastName} {sender.FirstName} {sender.MiddleName}"
                },
                CreatedAt = DateTime.Now,
                Class = model.Class,
                Type = RequestType.School,
                Status = RequestStatus.Pending
            };

            var newId = await _requestRepository.AddAsync(request);

            _requestEventHandler.CreateInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                Attachments = model.Attachments,
                ChangedByUserId = userId,
            });

            return newId;
        }

        /// <summary>
        /// Same as previous function but except type of request.
        /// </summary>
        public async Task<string> CreateTeacherRequestAsync(string userId, CreateTeacherRequestVM model)
        {
            var sender = await _identityApiClient.GetUserAsync(userId);
            if (sender == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            if (!string.IsNullOrEmpty(sender.SchoolUId))
            {
                throw new ValidationException($"lorem ipsum");
            }

            Request request = new Request
            {
                Child = null,
                Sender = new RequestSender
                {
                    Id = sender.Id,
                    Email = sender.Email,
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    FullName = $"{sender.LastName} {sender.FirstName} {sender.MiddleName}"
                },
                CreatedAt = DateTime.Now,
                Type = RequestType.Teacher,
                Status = RequestStatus.Pending,
            };

            var newId = await _requestRepository.AddAsync(request);

            _requestEventHandler.CreateInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                Attachments = model.Attachments,
                ChangedByUserId = userId,
            });

            return newId;
        }

        /// <summary>
        /// Same as previous function but except type of request.
        /// </summary>
        public async Task<string> CreateMarkRequestAsync(string userId, CreateMarkRequestVM model)
        {
            var sender = await _identityApiClient.GetUserAsync(userId);
            if (sender == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            var child = await _identityApiClient.GetUserAsync(model.ChildId);
            if (child == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            Request request = new Request
            {
                Child = new RequestChild
                {
                    Id = child.Id,
                    Code = child.Code,
                    FirstName = child.FirstName,
                    MiddleName = child.MiddleName,
                    LastName = child.LastName
                },
                Sender = new RequestSender
                {
                    Id = sender.Id,
                    Email = sender.Email,
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    FullName = $"{sender.LastName} {sender.FirstName} {sender.MiddleName}"
                },
                Mark = new RequestMark()
                {
                    Id = model.MarkId,
                    NewMarkRating = model.NewMarkRating,
                    NewMarkPresence = model.NewMarkPresence,
                    ReasonOfChanging = model.ReasonOfChanging,
                    SubType = GetMarkRequestSubType(new CreateMarkRequestSubTypeVM
                    {
                        OldMarkRating = model.OldMarkRating,
                        OldMarkPresence = model.OldMarkPresence,
                        NewMarkRating = model.NewMarkRating,
                        NewMarkPresence = model.NewMarkPresence
                    }),
                    Class = model.Class,
                    Subject = model.Subject,
                    Day = model.MarkDay,
                    OldMarkRating = model.OldMarkRating,
                    OldMarkPresence = model.OldMarkPresence,
                    ColumnId = model.ColumnId,
                    Description = model.Description,
                    GroupId = model.GroupId,
                    ClassId = model.ClassId,
                    IsDeleting = model.IsDeleting,
                },
                CreatedAt = DateTime.Now,
                Type = RequestType.Mark,
                Status = RequestStatus.Pending,
                ChangedByUserId = null
            };

            var newId = await _requestRepository.AddAsync(request);

            _requestEventHandler.CreateInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                Attachments = new List<IFormFile>(),
                ChangedByUserId = userId,
            });

            return newId;
        }

        /// <summary>
        /// Get specific request
        /// </summary>
        /// <param name="user">User that requested request</param>
        /// <param name="requestId">Id of specfic request</param>
        /// <returns>Returns specified request</returns>
        public async Task<RequestVM> GetRequestByIdAndUserIdAsync(ClaimsPrincipal user, string requestId)
        {
            var userId = user.Claims.First(x => x.Type == "sub").Value;

            var request = await _requestRepository.GetWhereAsync(x => x.Id == requestId);
            if (request == null)
            {
                return null;
            }

            var mappedRequest = _mapper.Map<RequestVM>(request);

            return mappedRequest;
        }

        public async Task<KendoResponse<GridRequestVM>> GetByUserIdAsync(ClaimsPrincipal user, KendoDataFilter dataFilter)
        {
            var total = await _repository.GetCollection<Request>()
                .CountDocumentsAsync(dataFilter.GetFilters<Request>());

            var requests = await _repository.GetCollection<Request>()
                .Find(dataFilter.GetFilters<Request>())
                .Sort(dataFilter.GetSort<Request>())
                .Skip(dataFilter.Skip)
                .Limit(dataFilter.Take)
                .ToListAsync();

            var mappedRequests = _mapper.Map<List<GridRequestVM>>(requests);

            return new KendoResponse<GridRequestVM>
            {
                Data = mappedRequests,
                Total = total
            };
        }

        public async Task RejectRequestAsync(string userId, string requestId, string token, RejectRequestVM model)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                throw new ValidationException("Запит не знайдено");
            }

            request.UpdatedAt = DateTime.Now;
            request.Status = RequestStatus.Reject;
            request.RejectDecision = model.Decision;
            request.ChangedByUserId = userId;

            await _requestRepository.ReplaceAsync(request);

            _requestEventHandler.RejectInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                ChangedByUserId = userId,
            });
        }

        public async Task ApproveRequestAsync(string userId, string id, string token)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
            {
                throw new ValidationException("Запит не знайдено");
            }

            request.UpdatedAt = DateTime.Now;
            request.Status = RequestStatus.Approval;
            request.ChangedByUserId = userId;

            await _requestRepository.ReplaceAsync(request);

            _requestEventHandler.ApproveInvoke(this, new RequestEventArgs
            {
                RequestId = request.Id,
                ChangedByUserId = userId,
            });
        }

        public async Task CancelRequestAsync(CancelRequestVM model)
        {
            var request = await _requestRepository.GetWhereAsync(x => x.ClassMaterial.Id == model.ClassMaterialId
                                          && x.ClassMaterial.AttachmentId == model.AttachmentId && x.ClassMaterial.GroupId == model.GroupId);
            if (request == null)
            {
                throw new ValidationException("lorem ipsum");
            }

            request.UpdatedAt = DateTime.Now;
            request.Status = RequestStatus.Cancel;

            await _requestRepository.ReplaceAsync(request);
        }

        private MarkSubType GetMarkRequestSubType(CreateMarkRequestSubTypeVM model)
        {
            if (!model.OldMarkRating.HasValue && model.NewMarkRating.HasValue)
            {
                return MarkSubType.AddRating;
            }

            if (model.OldMarkRating.HasValue && model.NewMarkRating.HasValue)
            {
                return MarkSubType.EditRating;
            }

            if (model.NewMarkPresence)
            {
                return MarkSubType.AddPresence;
            }

            if (model.OldMarkRating.HasValue && !model.NewMarkRating.HasValue)
            {
                return MarkSubType.RemoveRating;
            }

            if (model.OldMarkPresence && !model.NewMarkPresence)
            {
                return MarkSubType.RemovePresence;
            }

            return default;
        }
    }
}
