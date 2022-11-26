using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimelineService.Application.Commands;
using TimelineService.Application.Queries;
using TimelineService.Domain.Entities;
using TimelineService.Domain.ViewModels;

namespace TimelineService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TimelineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimelineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}")]
        [ProducesResponseType(typeof(TimelineViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Get(string username)
        {
            try
            {
                var query = new GetTimelineQuery(username);

                var timeline = await _mediator.Send(query);

                return Ok(timeline);
            }
            catch(Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Post(HonkEntity entity)
        {
            try
            {
                var command = new CreateTimelineCommand(entity);

                var addedHonk = await _mediator.Send(command);

                return Ok(addedHonk);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
