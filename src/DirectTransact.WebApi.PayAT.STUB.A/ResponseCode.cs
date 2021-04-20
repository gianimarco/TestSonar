/****************************************************************************************************/
//  Module name:    ResponseCode
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for ResponseCode
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT
{
  /// <summary>
  /// Response Codes
  /// </summary>
  public enum ResponseCode
  {
    /// <summary>
    /// Successfully processed the transaction.
    /// </summary>
    APPROVED,

    /// <summary>
    /// The number of payments allowed on an account has exceeded
    /// </summary>
    EXCEEDS_NUMBER_OF_PAYMENTS,

    /// <summary>
    /// The account number is not a valid issuer account.
    /// </summary>
    INVALID_ACCOUNT_NUMBER,

    /// <summary>
    /// Incorrect amount for the issuer account.
    /// </summary>
    INVALID_AMOUNT,

    /// <summary>
    /// Invalid login details
    /// </summary>
    INVALID_AUTHENTICATION,

    /// <summary>
    /// The amount exceeds the maximum amount payable
    /// </summary>
    EXCEEDS_MAXIMUM_AMOUNT,

    /// <summary>
    /// The amount is less than the minimum amount payable.
    /// </summary>
    BELOW_MINIMUM_AMOUNT,

    /// <summary>
    /// Any other reason why an account number cannot be paid.
    /// </summary>
    PAYMENT_NOT_ALLOWED,

    /// <summary>
    /// Any other security related errors
    /// </summary>
    SECURITY_VIOLATION,

    /// <summary>
    /// System exception
    /// </summary>
    SYSTEM_MALFUNCTION,

    /// <summary>
    /// Processing timeout. Normally sent when the issuer connects a third party processor.
    /// </summary>
    TIMEOUT
  }
}