using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LikePostController : ControllerBase
    {
        private readonly ILogger<LikePostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher dispatcher)
        {
            _logger = logger;
            _commandDispatcher = dispatcher;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> LikePostAsync(Guid id)
        {
            try
            {
                await _commandDispatcher.SendAsync(new LikePostCommand {Id = id});

                return Ok(new BaseResponse
                {
                    Message = "Like Post request completed successfully"
                });
            }
            catch(InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request!");

                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch(AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed an incorrect post ID targetting teh aggregate!");

                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch(Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to LIke a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}