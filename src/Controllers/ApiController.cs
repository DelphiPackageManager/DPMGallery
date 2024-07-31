using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using DPMGallery.Extensions;
using System.Text.Json;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Identity;


namespace DPMGallery.Controllers
{
    [ApiController]
    public class ApiController : Controller
    {
        private readonly ILogger _log;

        public ApiController(ILogger logger)
        {
            _log = logger;
        }

        protected ILogger Log => _log;

        protected IActionResult GetIdentityResultAsResponse(UserManager<User> userManager, string currentAction, IdentityResult result)
        {
            var validationErrors = new List<string>();
            foreach (IdentityError error in result.Errors)
            {
                bool isValidationError = userManager.ErrorDescriber.IsValidationError(error.Code);
                if (!isValidationError)
                {
                    string message = string.IsNullOrWhiteSpace(error.Code) ? error.Description : $"({error.Code}) {error.Description}";
                    Log.Error("{CurrentAction} failed: {Message}", currentAction, message);
                    return BadRequest(Constants.ErrorMessages.ServerError);
                }
                validationErrors.Add(error.Description);
            }

            var resultModel = new
            {
                result.Succeeded,
                Errors = validationErrors
            };
            return Ok(resultModel);
        }

        protected IActionResult GetProblemResponse(string title, int statusCode = StatusCodes.Status500InternalServerError, Exception exception = null, string detail = "", string type = null, bool logExceptionDetails = true, Dictionary<string, object> additionalInformation = null, [CallerMemberName] string callerName = "")
        {
            if (string.IsNullOrWhiteSpace(type))
                type = MapStatusCodeToTypeUrl(statusCode);

            if (string.IsNullOrWhiteSpace(detail) && exception != null)
                detail = exception.Message;

            if (logExceptionDetails && exception != null)
            {
                string message = $"[{callerName}] {title}: {exception.ToDetailedString()}";
                if (additionalInformation != null)
                {
                    try
                    {
                        string additionalInformationMessage = JsonSerializer.Serialize(additionalInformation);
                        message += $"\nAdditional Information: {additionalInformationMessage}";
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while serializing additional information for problem response");
                    }
                }
                Log.Error(exception, message);
            }
            if (additionalInformation == null)
                return Problem(detail, statusCode: statusCode, title: title, type: type);


            ProblemDetails problemDetails;
            if (ProblemDetailsFactory == null)
                problemDetails = new ProblemDetails { Detail = detail, Status = statusCode, Title = title, Type = type, Extensions = additionalInformation };
            else
            {
                problemDetails = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: statusCode, title: title, type: type, detail: detail);
                problemDetails.Extensions = additionalInformation;
            }

            return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        }

        private string MapStatusCodeToTypeUrl(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                StatusCodes.Status401Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                StatusCodes.Status403Forbidden => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                StatusCodes.Status404NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                StatusCodes.Status405MethodNotAllowed => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.5",
                StatusCodes.Status406NotAcceptable => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.6",
                StatusCodes.Status408RequestTimeout => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.7",
                StatusCodes.Status409Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                StatusCodes.Status410Gone => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.9",
                _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            };
        }
    }
}
