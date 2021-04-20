/****************************************************************************************************/
//  Module name:    AuthorizeController
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    This controller is used to handle PAY@ Advice Transaction api reqeusts
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
  /// Pay@ Authorize Transaction
  /// </summary>
  [Route("authorize")]
  [ApiController]
  public class AuthorizeController : ControllerBase
  {
    /// <summary>
    /// Inject the logger injection
    /// </summary>
    private readonly ILogger _logger;

    private readonly IConfiguration _configuration;

    /// <summary>
    /// AuthorizeController default constructor
    /// </summary>
    /// <param name="logger">Logger injaction for AuthorizeController</param>
    public AuthorizeController(ILogger<AuthorizeController> logger, IConfiguration configuration)
    {
      this._logger = logger;
      this._configuration = configuration;
    }

    /// <summary>
    /// An AuthorizationRequest is used to authorize a payment.
    /// </summary>
    /// <param name="model">Body model for Authorize Pay@</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Authorize
      (
      [FromBody, Required] AuthorizeModel model
      )
    {
      _ = model ?? throw new NullReferenceException("No valid model was passed");
      _logger.LogInformation("Authorize request received");

      try
      {
        if (model.securityData.login.loginID != _configuration["StubLogin:loginID"] && model.securityData.login.password != _configuration["StubLogin:password"]) throw new ArgumentException("login is not valid");
        if (model.accountNumber.Substring(0, 5) != _configuration["StubLogin:routePrefix"]) throw new ArgumentOutOfRangeException("account number is not falid");

        Random random = new Random();
        AuthorizeResponseModel authorizeResponseModel = new AuthorizeResponseModel();

        if (random.Next(0, 100) > int.Parse(_configuration["StubLogin:ErrorRatio"]))
        {
          authorizeResponseModel = new AuthorizeResponseModel()
          {
            messageID = model.messageID,
            responseCode = ResponseCode.APPROVED,
            responseText = "Success",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            customerData = new CustomerData()
            {
              contactNumber = "0112223344",
              firstName = "STUB",
              idNumber = "0099887766554",
              lastName = "STUB"
            },
            dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
            echoData = model.echoData,
            issuerTransactionID = Guid.NewGuid().ToString()
          };
        }
        else
        {
          authorizeResponseModel = new AuthorizeResponseModel()
          {
            messageID = model.messageID,
            responseCode = ResponseCode.PAYMENT_NOT_ALLOWED,
            responseText = "payment not allowed",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            customerData = new CustomerData()
            {
              contactNumber = "0112223344",
              firstName = "STUBB",
              idNumber = "009988",
              lastName = "STUBB"
            },
            dueDate = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            echoData = model.echoData,
            issuerTransactionID = Guid.NewGuid().ToString()
          };
        }

        _logger.LogInformation("Authorize request returned");
        return Ok(authorizeResponseModel);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        _logger.LogError($"Authorize threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new AuthorizeResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_ACCOUNT_NUMBER,
          responseText = "invalid account number",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          customerData = new CustomerData()
          {
            contactNumber = "0112223344",
            firstName = "STUB",
            idNumber = "0099887766554",
            lastName = "STUB"
          },
          dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData,
          issuerTransactionID = Guid.NewGuid().ToString()
        });
      }
      catch (ArgumentException ex)
      {
        _logger.LogError($"Authorize threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new AuthorizeResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_AUTHENTICATION,
          responseText = "invalid authentication",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          customerData = new CustomerData()
          {
            contactNumber = "0112223344",
            firstName = "STUB",
            idNumber = "0099887766554",
            lastName = "STUB"
          },
          dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData,
          issuerTransactionID = Guid.NewGuid().ToString()
        });
      }
      catch (Exception ex)
      {
        _logger.LogError($"Authorize threw an excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new AuthorizeResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.SYSTEM_MALFUNCTION,
          responseText = "inner Excpetion",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          customerData = new CustomerData()
          {
            contactNumber = "0112223344",
            firstName = "STUB",
            idNumber = "0099887766554",
            lastName = "STUB"
          },
          dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData,
          issuerTransactionID = Guid.NewGuid().ToString()
        });
      }
    }

    /// <summary>
    /// Body model for Authorize Pay@
    /// </summary>
    public class AuthorizeModel
    {
      /// <summary>
      /// The account holder number for the transaction.The account number is the full Pay@ Account Number as captured on the POS.
      /// </summary>
      [Required]
      public string accountNumber { get; set; }

      /// <summary>
      /// The exact amount to be paid.
      /// </summary>
      [Required]
      public double amount { get; set; }

      /// <summary>
      /// This field must be echoed back in the response message.
      /// </summary>
      public string echoData { get; set; }

      /// <summary>
      /// Unique message ID for the authorization request.
      /// </summary>
      [Required]
      public string messageID { get; set; }

      /// <summary>
      /// The unique Pay@ transaction ID.
      /// </summary>
      [Required]
      public string networkTransactionID { get; set; }

      /// <summary>
      /// Refer to NetworkData
      /// </summary>
      public NetworkData networkData { get; set; }

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
      /// The (local) date and time at which the requestmessage was sent, in the ISO 8601[i] format YYYY-MM-DDThh:mm:ss
      /// </summary>
      [Required]
      public string transmissionDateTime { get; set; }
    }
  }
}