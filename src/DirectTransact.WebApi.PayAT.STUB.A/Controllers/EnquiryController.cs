/****************************************************************************************************/
//  Module name:    EnquiryController
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
  /// An EnquiryRequest is used to verify that a valid account exists at the issuer, for the customer performing the transaction.
  /// </summary>
  [Route("enquire")]
  [ApiController]
  public class EnquiryController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor for EnquiryController
    /// </summary>
    /// <param name="logger"></param>
    public EnquiryController(ILogger<EnquiryController> logger, IConfiguration configuration)
    {
      this._logger = logger;
      this._configuration = configuration;
    }

    /// <summary>
    /// An EnquiryRequest is used to verify that a valid account exists at the issuer, for the customer performing the transaction.
    /// </summary>
    /// <param name="model">Enquiry model for Pay@</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Enquiry
      (
      [FromBody, Required] EnquiryModel model
      )
    {
      _ = model ?? throw new NullReferenceException("Model passed was not valid");
      _logger.LogInformation("Enquiry received");

      try
      {
        if (model.securityData.login.loginID != _configuration["StubLogin:loginID"] && model.securityData.login.password != _configuration["StubLogin:password"]) throw new ArgumentException("login is not valid");
        if (model.accountNumber.Substring(0, 5) != _configuration["StubLogin:routePrefix"]) throw new ArgumentOutOfRangeException("account number is not falid");

        Random random = new Random();
        EnquireResponseModel enquireResponseModel = new EnquireResponseModel();

        if (random.Next(0, 100) > int.Parse(_configuration["StubLogin:ErrorRatio"]))
        {
          enquireResponseModel = new EnquireResponseModel()
          {
            echoData = model.echoData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            structuredData = model.structuredData,
            messageID = model.messageID,
            responseCode = ResponseCode.APPROVED,
            responseText = "Success",
            amount = 10,
            dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
            customerData = new CustomerData()
            {
              contactNumber = "0112223344",
              firstName = "STUB",
              idNumber = "0099887766554",
              lastName = "STUB"
            },
            exactAmount = true
          };
        }
        else
        {
          enquireResponseModel = new EnquireResponseModel()
          {
            echoData = model.echoData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            structuredData = model.structuredData,
            messageID = model.messageID,
            responseCode = ResponseCode.INVALID_ACCOUNT_NUMBER,
            responseText = "invalid account number",
            amount = 0,
            dueDate = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            customerData = new CustomerData()
            {
              contactNumber = "0112223344",
              firstName = "STUB",
              idNumber = "0099887766554",
              lastName = "STUB"
            },
            exactAmount = false
          };
        }

        _logger.LogInformation("Enquiry returned");
        return Ok(enquireResponseModel);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        _logger.LogError($"Enquiry Auth threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new EnquireResponseModel()
        {
          echoData = model.echoData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          structuredData = model.structuredData,
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_ACCOUNT_NUMBER,
          responseText = "Inner Exceptionm",
          amount = 10,
          dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
          customerData = new CustomerData()
          {
            contactNumber = "0112223344",
            firstName = "STUB",
            idNumber = "0099887766554",
            lastName = "STUB"
          },
          exactAmount = true
        });
      }
      catch (ArgumentException ex)
      {
        _logger.LogError($"Enquiry threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new EnquireResponseModel()
        {
          echoData = model.echoData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          structuredData = model.structuredData,
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_AUTHENTICATION,
          responseText = "Inner Exceptionm",
          amount = 10,
          dueDate = DateTime.UtcNow.AddDays(2).ToString(Startup.TRANSMISSIONDATEANDTIME),
          customerData = new CustomerData()
          {
            contactNumber = "0112223344",
            firstName = "STUB",
            idNumber = "0099887766554",
            lastName = "STUB"
          },
          exactAmount = true
        });
      }
      catch (Exception ex)
      {
        _logger.LogError($"Enquiry threw an excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return BadRequest();
      }
    }

    /// <summary>
    /// This class build the body object for pay@
    /// </summary>
    public class EnquiryModel
    {
      /// <summary>
      /// The account holder number for the transaction.The account number is the full Pay@ Account Number as captured on the POS.
      /// </summary>
      [Required]
      public string accountNumber { get; set; }

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