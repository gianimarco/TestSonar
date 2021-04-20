﻿/****************************************************************************************************/
//  Module name:    ReverseAuthorizationResponseModel
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for Reverse Authorization
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

using DirectTransact.WebApi.PayAT.HelperModels;
using System.ComponentModel.DataAnnotations;

namespace DirectTransact.WebApi.PayAT.Models
{
  /// <summary>
  /// Base return model for Reverse Authorization
  /// </summary>
  public class ReverseAuthorizationResponseModel
  {
    /// <summary>
    /// The echo data from the corresponding transaction advice message.Mandatory if present in the transaction advice message. Compulsory
    /// </summary>
    [Required]
    public string echoData { get; set; }

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