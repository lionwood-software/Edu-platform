using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using LionwoodSoftware.DataFilter;
using LionwoodSoftware.ResponseHandler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RouterApi.Domain.Enums;
using RouterApi.Interfaces.Services;
using RouterApi.Models.Request;

namespace RouterApi.Controllers
{
    [ApiController]
    [Route("api/v1/requests")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestsController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly IMapper _mapper;

        public RequestsController(IRequestService requestService, IMapper mapper)
        {
            _requestService = requestService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> RequestCreate([FromForm] BaseCreateRequestVM model)
        {
            if (User.IsInRole("CHILD"))
            {
                return Forbid();
            }

            var userId = User.Claims.First(x => x.Type == "sub").Value;

            string requestId;
            switch (model.Type)
            {
                case RequestType.Child:
                    var requestChildModel = _mapper.Map<CreateChildRequestVM>(model);

                    if (!TryValidateModel(requestChildModel))
                    {
                        return UnprocessableEntity(ModelStateValidator.Result(ControllerContext));
                    }

                    requestId = await _requestService.CreateChildRequestAsync(userId, requestChildModel);
                    break;
                case RequestType.School:
                    var requestSchoolModel = _mapper.Map<CreateSchoolRequestVM>(model);
                    if (!TryValidateModel(requestSchoolModel))
                    {
                        return UnprocessableEntity(ModelStateValidator.Result(ControllerContext));
                    }

                    requestId = await _requestService.CreateSchoolRequestAsync(userId, requestSchoolModel);
                    break;
                case RequestType.Teacher:
                    var requestTeacherModel = _mapper.Map<CreateTeacherRequestVM>(model);
                    if (!TryValidateModel(requestTeacherModel))
                    {
                        return UnprocessableEntity(ModelStateValidator.Result(ControllerContext));
                    }

                    requestId = await _requestService.CreateTeacherRequestAsync(userId, requestTeacherModel);
                    break;
                case RequestType.Mark:
                    var requestMarkModel = _mapper.Map<CreateMarkRequestVM>(model);
                    if (!TryValidateModel(requestMarkModel))
                    {
                        return UnprocessableEntity(ModelStateValidator.Result(ControllerContext));
                    }

                    requestId = await _requestService.CreateMarkRequestAsync(userId, requestMarkModel);
                    break;
                default:
                    throw new LionwoodSoftware.ResponseHandler.Exceptions.ValidationException(new Dictionary<string, List<string>> { { "type", new List<string> { "Некоректний тип заявки" } } });
            }

            return Ok(new { Id = requestId });
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetByIdRequest(string requestId)
        {
            var request = await _requestService.GetRequestByIdAndUserIdAsync(User, requestId);

            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] KendoDataFilter dataFilter)
        {
            var requests = await _requestService.GetByUserIdAsync(User, dataFilter);

            return Ok(requests);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(string id)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");

            await _requestService.ApproveRequestAsync(userId, id, token);

            return Ok();
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectRequest(string id, RejectRequestVM model)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            var token = await HttpContext.GetTokenAsync("access_token");

            await _requestService.RejectRequestAsync(userId, id, token, model);

            return Ok();
        }

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelRequest(CancelRequestVM model)
        {
            await _requestService.CancelRequestAsync(model);

            return Ok();
        }
    }
}