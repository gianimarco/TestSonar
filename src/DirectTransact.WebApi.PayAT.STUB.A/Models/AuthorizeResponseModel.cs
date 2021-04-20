using DirectTransact.WebApi.PayAT.HelperModels;
using System.ComponentModel.DataAnnotations;

namespace DirectTransact.WebApi.PayAT.Models
{
  /// <summary>
  /// Base return model for Pay@ Authorization Request
  /// </summary>
  public class AuthorizeResponseModel
  {
    /// <summary>
    /// Refer to CustomerData
    /// </summary>
    [Required]
    public CustomerData customerData { get; set; }

    /// <summary>
    /// The date and time by which payment must be received in the ISO 8601[i] format YYYY-MMDDThh:mm:ss
    /// </summary>
    [Required]
    public string dueDate { get; set; }

    /// <summary>
    /// The echo data from the corresponding transaction advice message.Mandatory if present in the transaction advice message. Compulsory
    /// </summary>
    [Required]
    public string echoData { get; set; }

    /// <summary>
    /// An ID that uniquely identifies the transaction on the issuer system.
    /// </summary>
    [Required]
    public string issuerTransactionID { get; set; }

    /// <summary>
    /// Unique message ID from the transaction advice. Mandatory
    /// </summary>
    [Required]
    public string messageID { get; set; }

    /// <summary>
    /// The response code of the message, indicating the result of the transaction advice.
    /// </summary>
    [Required]
    public ResponseCode responseCode { get; set; }

    /// <summary>
    /// Additional information relating to the outcome of the transaction, e.g.a decline reason.
    /// </summary>
    [Required]
    public string responseText { get; set; }

    /// <summary>
    /// This field may be used to communicate industryspecific or issuer-specific data. The contents this field is in JSON format
    /// </summary>
    [Required]
    public StructuredData structuredData { get; set; }

    /// <summary>
    /// The (local) date and time at which the request message was sent, in the ISO 8601[i] format YYYYMM-DDThh:mm:ss.
    /// </summary>
    [Required]
    public string transmissionDateTime { get; set; }
  }
}