/****************************************************************************************************/
//  Module name:    CustomerData
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for CustomerData
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT.HelperModels
{
  /// <summary>
  /// The CustomerData object contains information about the customer performing the transaction
  /// </summary>
  public class CustomerData
  {
    /// <summary>
    /// The customer’s first name
    /// </summary>
    public string firstName { get; set; }

    /// <summary>
    /// The customer’s last name
    /// </summary>
    public string lastName { get; set; }

    /// <summary>
    /// The customer’s ID number
    /// </summary>
    public string idNumber { get; set; }

    /// <summary>
    /// The customer’s contact number
    /// </summary>
    public string contactNumber { get; set; }
  }
}