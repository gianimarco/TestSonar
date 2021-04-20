/****************************************************************************************************/
//  Module name:    ReverseAuthorizationController
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for Advice Transaction
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
  /// A ReversalAdvice used to reverse a successful authorization.
  /// This advice notifies the issuer to close the open transaction.
  /// A reversal will only be sent after an authorization. A reversal typically means that the customer opted
  /// to not continue with the transaction, or that there was failure at the point-of-sale.
  /// A reversal is never sent after a transaction has been completed (via the TransactionAdvice method).
  /// </summary>
  [Route("reverseAuthorization")]
  [ApiController]
  public class ReverseAuthorizationController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor for ReverseAuthorizationController
    /// </summary>
    /// <param name="logger"></param>
    public ReverseAuthorizationController(ILogger<ReverseAuthorizationController> logger, IConfiguration configuration)
    {
      this._logger = logger;
      this._configuration = configuration;
    }

    /// <summary>
    /// A ReversalAdvice used to reverse a successful authorization.
    /// </summary>
    /// <param name="model">ReverseAuthorizationModel body for Pay@</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult ReverseAuthorization
      (
      [FromBody, Required] ReverseAuthorizationModel model
      )
    {
      _ = model ?? throw new NullReferenceException("Model passed was not valid");
      _logger.LogInformation("Reverse Auth received");

      try
      {
        if (model.securityData.login.loginID != _configuration["StubLogin:loginID"] && model.securityData.login.password != _configuration["StubLogin:password"]) throw new ArgumentException("login is not valid");
        if (model.accountNumber.Substring(0, 5) != _configuration["StubLogin:routePrefix"]) throw new ArgumentOutOfRangeException("account number is not falid");

        ReverseAuthorizationResponseModel reverseAuthorizationResponseModel = new ReverseAuthorizationResponseModel();
        Random random = new Random();

        if (random.Next(0, 100) > int.Parse(_configuration["StubLogin:ErrorRatio"]))
        {
          reverseAuthorizationResponseModel = new ReverseAuthorizationResponseModel()
          {
            echoData = model.echoData,
            messageID = model.messageID,
            responseCode = ResponseCode.APPROVED,
            responseText = "Success",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME)
          };
        }
        else
        {
          reverseAuthorizationResponseModel = new ReverseAuthorizationResponseModel()
          {
            echoData = model.echoData,
            messageID = model.messageID,
            responseCode = ResponseCode.EXCEEDS_NUMBER_OF_PAYMENTS,
            responseText = "reverse auth limit reached",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME)
          };
        }

        _logger.LogInformation("Reverse Auth returned");
        return Ok(reverseAuthorizationResponseModel);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        _logger.LogError($"Reverse Auth threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new ReverseAuthorizationResponseModel()
        {
          echoData = model.echoData,
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_ACCOUNT_NUMBER,
          responseText = "Inner Excpetion",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME)
        });
      }
      catch (ArgumentException ex)
      {
        _logger.LogError($"Reverse Auth threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new ReverseAuthorizationResponseModel()
        {
          echoData = model.echoData,
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_AUTHENTICATION,
          responseText = "Inner Excpetion",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME)
        });
      }
      catch (Exception ex)
      {
        _logger.LogError($"Reverse Auth threw an excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return BadRequest();
      }
    }

    /// <summary>
    /// ReverseAuthorizationModel body for Pay@
    /// </summary>
    public class ReverseAuthorizationModel
    {
      /// <summary>
      ///The account holder number for the transaction.The account number is the full Pay@ Account Number as captured on the POS.
      /// </summary>
      [Required]
      public string accountNumber { get; set; }

      /// <summary>
      ///This field must be echoed back in the response message.
      /// </summary>
      public string echoData { get; set; }

      /// <summary>
      ///The ID that uniquely identifies the transaction on the issuer system, this is from the authorization response.This field will only be present if the authorization response was received.
      /// </summary>
      public string issuerTransactionID { get; set; }

      /// <summary>
      ///Unique message ID for the authorization request.
      /// </summary>
      [Required]
      public string messageID { get; set; }

      /// <summary>
      ///he unique Pay@ transaction ID.
      /// </summary>
      [Required]
      public string networkTransactionID { get; set; }

      /// <summary>
      ///Indicates whether this is a repeat of a previous reversal advice.False if this is the first reversal and True if this is a repeat of an earlier reversal
      /// </summary>
      [Required]
      public bool repeaterIndicator { get; set; }

      /// <summary>
      ///Refer to NetworkData
      /// </summary>
      public NetworkData networkData { get; set; }

      /// <summary>
      ///Refer to SecurityData
      /// </summary>
      [Required]
      public SecurityData securityData { get; set; }

      /// <summary>
      ///This field may be used to communicate industry-specific or issuer-specific data.The contents of this field is in JSON[ii] format.
      /// </summary>
      public StructuredData structuredData { get; set; }

      /// <summary>
      ///The (local) date and time at which the requestmessage was sent, in the ISO 8601[i] format YYYY-MM-DDThh:mm:ss
      /// </summary>
      [Required]
      public string transmissionDateTime { get; set; }
    }
  }
}