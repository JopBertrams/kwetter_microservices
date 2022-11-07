using Microsoft.AspNetCore.Authorization;
using HonkService.Application.Commands;
using HonkService.Application.Queries;
using HonkService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using HonkService.Api.DTO;
using System.Net;
using MediatR;
using Mapster;

namespace HonkService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HonkController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HonkController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{honkId}")]
        [ProducesResponseType(typeof(HonkResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Get(Guid honkId)
        {
            try
            {
                var query = new GetHonkQuery(honkId);

                HonkResult honkResult = await _mediator.Send(query);

                HonkResponseDTO responseDTO = honkResult.Adapt<HonkResponseDTO>();

                return Ok(responseDTO);
            }
            catch
            {
                return NotFound($"Honk with id '{honkId}' not found");
            }
        }       

        [HttpPost]
        [ProducesResponseType(typeof(HonkResponseDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> Post(PostHonkRequestDTO request)
        {
            try
            {
                var command = request.Adapt<PostHonkCommand>();

                HonkResult honkResult = await _mediator.Send(command);

                HonkResponseDTO responseDTO = honkResult.Adapt<HonkResponseDTO>();

                return Ok(responseDTO);               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{honkId}")]
        [ProducesResponseType(typeof(DeletedHonkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Delete(Guid honkId)
        {
            try
            {
                var command = new DeleteHonkCommand(honkId);
            
                DeletedHonkResult honkResult = await _mediator.Send(command);

                return Ok(honkResult);             
            }
            catch
            {
                return NotFound($"Honk with id '{honkId}' not found");
            }

        }
    }
}
