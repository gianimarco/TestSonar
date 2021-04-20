/****************************************************************************************************/
//  Module name:    AuthorizeController
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    This controller is used to handle PAY@ echo api requests
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

using DirectTransact.WebApi.PayAT.HelperModels;
using DirectTransact.WebApi.PayAT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace DirectTransact.WebApi.PayAT.Controllers
{
  /// <summary>
  /// Pay@ will periodically send an echo test request to the issuer. The issuer approves all echo test requests
  /// </summary>
  [Route("echoTest")]
  [ApiController]
  public class EchoTestController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor for EchoTestController
    /// </summary>
    /// <param name="logger"></param>
    public EchoTestController(ILogger<EchoTestController> logger, IConfiguration configuration)
    {
      this._logger = logger;
      this._configuration = configuration;
    }

    /// <summary>
    /// Pay@ will periodically send an echo test request to the issuer. The issuer approves all echo test requests
    /// </summary>
    /// <param name="model">EchoTest body model for Pay@</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult EchoTest
      (
      [FromBody, Required]EchoTestModel model
      )
    {
      _ = model ?? throw new NullReferenceException("Model passed was not valid");
      _logger.LogInformation("Echo received");

      try
      {
        if (model.securityData.login.loginID != _configuration["StubLogin:loginID"] && model.securityData.login.password != _configuration["StubLogin:password"]) throw new ArgumentException("login is not valid");

        Random random = new Random();
        EchoTestResponseModel echoTestResponseModel = new EchoTestResponseModel();

        if (random.Next(0, 100) > int.Parse(_configuration["StubLogin:ErrorRatio"]))
        {
          echoTestResponseModel = new EchoTestResponseModel()
          {
            messageID = model.messageID,
            responseCode = ResponseCode.APPROVED,
            responseText = "Success",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            echoData = model.echoData
          };
        }
        else
        {
          echoTestResponseModel = new EchoTestResponseModel()
          {
            messageID = model.messageID,
            responseCode = ResponseCode.TIMEOUT,
            responseText = "EchoTest timeout",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            echoData = model.echoData
          };
        }

        _logger.LogInformation("Echo returned");
        return Ok(echoTestResponseModel);
      }
      catch (ArgumentException ex)
      {
        _logger.LogError($"Echo threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new EchoTestResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_AUTHENTICATION,
          responseText = "Invalid login details",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData
        });
      }
      catch (Exception ex)
      {
        _logger.LogError($"Echo threw an excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return BadRequest(new EchoTestResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.SYSTEM_MALFUNCTION,
          responseText = "Inner Exception",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData
        });
      }
    }

    /// <summary>
    /// EchoTest body model for Pay@
    /// </summary>
    public class EchoTestModel
    {
      /// <summary>
      /// This field must be echoed back in the echo test response message.
      /// </summary>
      [Required]
      public string echoData { get; set; }

      /// <summary>
      /// Unique message ID
      /// </summary>
      [Required]
      public string messageID { get; set; }

      /// <summary>
      /// Refer to SecurityData
      /// </summary>
      [Required]
      public SecurityData securityData { get; set; }

      /// <summary>
      /// This field may be used to communicate industry-specific or issuer-specific data.The contents of this field is in JSON[ii] format.
      /// </summary>
      public StructuredData structuredData { get; set; }

      /// <summary>
      /// The (local) date and time at which the request message was sent, in the ISO 8601[i] YYYY-MM-DDThh:mm:ss
      /// </summary>
      [Required]
      public string transmissionDateTime { get; set; }
    }
  }
}