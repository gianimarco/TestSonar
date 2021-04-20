/*****************************************************************************************************/
//  Module name:    AdviceTransactionController
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
using System.Diagnostics;

namespace DirectTransact.WebApi.PayAT.Controllers
{
  /// <summary>
  /// Pay@ Transaction Advice
  /// </summary>
  [Route("adviseTransaction")]
  [ApiController]
  public class AdviceTransactionController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Default constructor for AdviceTransactionController
    /// </summary>
    /// <param name="logger"></param>
    public AdviceTransactionController(ILogger<AdviceTransactionController> logger, IConfiguration configuration)
    {
      this._logger = logger;
      this._configuration = configuration;
      
      //intentional break to code to test SonarCube
      int Pow(int num, int exponent)
      {
        num = num * Pow(num, exponent - 1);
        return num;
      }
      void InternalRecursion(int i)
      {
      start:
        goto end;
      end:
        goto start;
      }

      string username = "admin";
      string password = "Admin123"; // Sensitive
      string usernamePassword = "user=admin&password=Admin123"; // Sensitive
      string url = "scheme://user:Admin123@domain.com"; // Sensitive

      var p = new Process();
      p.StartInfo.FileName = "exportLegacy.exe";
      p.StartInfo.Arguments = " -user" + username + " -role user";
      p.Start();


      _ = Pow(1, 1);
      InternalRecursion(Pow(2, 2));

      //end intentional break to code to test SonarCube

    }

    /// <summary>
    /// The tender present
    /// </summary>
    public enum TenderType
    {
      /// <summary>
      /// Cash
      /// </summary>
      CASH,

      /// <summary>
      /// Debit Card
      /// </summary>
      DEBIT_CARD,

      /// <summary>
      /// Credit Card
      /// </summary>
      CREDIT_CARD
    }

    /// <summary>
    /// A TransactionAdvice is used to complete a transaction. This advice notifies the issuer that the payment concluded successfully.
    /// </summary>
    /// <param name="model">This class is for Advice Transaction Pay@ body</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult AdviceTransaction
      (
      [FromBody, Required] AdviceModel model
      )
    {
      _ = model ?? throw new NullReferenceException("No valid Model is passed");
      _logger.LogInformation("Advice transaction received");

      try
      {
        if (model.securityData.login.loginID != _configuration["StubLogin:loginID"] && model.securityData.login.password != _configuration["StubLogin:password"]) throw new ArgumentException("login is not valid");
        if (model.accountNumber.Substring(0, 5) != _configuration["StubLogin:routePrefix"]) throw new ArgumentOutOfRangeException("account number is not falid");

        Random random = new Random();
        
        AdviceTransactionResponseModel adviceTransactionResponseModel = new AdviceTransactionResponseModel();
        if (random.Next(0, 100) > int.Parse(_configuration["StubLogin:ErrorRatio"]))
        {
          adviceTransactionResponseModel = new AdviceTransactionResponseModel()
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
          adviceTransactionResponseModel = new AdviceTransactionResponseModel()
          {
            messageID = model.messageID,
            responseCode = ResponseCode.SECURITY_VIOLATION,
            responseText = "Security Violation",
            structuredData = model.structuredData,
            transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
            echoData = model.echoData
          };
        }

        _logger.LogInformation("Advice transaction returned");
        return Ok(adviceTransactionResponseModel);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        _logger.LogError($"Advice threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new AdviceTransactionResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_ACCOUNT_NUMBER,
          responseText = "Inner Excpetion",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData
        });
      }
      catch (ArgumentException ex)
      {
        _logger.LogError($"Advice threw an argument excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return Ok(new AdviceTransactionResponseModel()
        {
          messageID = model.messageID,
          responseCode = ResponseCode.INVALID_AUTHENTICATION,
          responseText = "Inner Excpetion",
          structuredData = model.structuredData,
          transmissionDateTime = DateTime.UtcNow.ToString(Startup.TRANSMISSIONDATEANDTIME),
          echoData = model.echoData
        });
      }
      catch (Exception ex)
      {
        _logger.LogError($"Advice threw an excpetion. Message {ex.Message}. Stacktrace {ex.StackTrace}. InnerException {ex.InnerException}");
        return BadRequest();
      }
    }

    /// <summary>
    /// This class is for Advice Transaction Pay@ body
    /// </summary>
    public class AdviceModel
    {
      /// <summary>
      /// The account holder number for the transaction.
      /// </summary>
      [Required]
      public string accountNumber { get; set; }

      /// <summary>
      /// The amount received, this amount cannot differ from the authorization request amount.
      /// </summary>
      public double amount { get; set; }

      /// <summary>
      /// This field must be echoed back in the response message.
      /// </summary>
      public string echoData { get; set; }

      /// <summary>
      /// The ID that uniquely identifies the transaction on the issuer system, this is from the authorization response.
      /// </summary>
      [Required]
      public string issuerTransactionID { get; set; }

      /// <summary>
      /// Unique message ID
      /// </summary>
      [Required]
      public string messageID { get; set; }

      /// <summary>
      /// The unique Pay@ transaction ID from the authorization request.
      /// </summary>
      [Required]
      public string networkTransactionID { get; set; }

      /// <summary>
      /// Indicates whether this is a repeat of a previous transaction advice.False if this is the first advice and True if this is a repeat of an advice.
      /// </summary>
      [Required]
      public bool repeatIndicator { get; set; }

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
      /// This field may be used to communicate industryspecific or issuer-specific data. The contents this field is in JSON[ii] format.
      /// </summary>
      public StructuredData structuredData { get; set; }

      /// <summary>
      /// The tender present. CASH, DEBIT_CARD or CREDIT_CARD
      /// </summary>
      [Required]
      public TenderType tenderType { get; set; }

      /// <summary>
      /// The (local) date and time at which the request message was sent, in the ISO 8601[i] format YYYYMM-DDThh:mm:ss
      /// </summary>
      [Required]
      public string transmissionDateTime { get; set; }
    }
  }
}