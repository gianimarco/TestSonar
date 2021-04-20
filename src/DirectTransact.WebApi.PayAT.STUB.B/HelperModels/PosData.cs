/****************************************************************************************************/
//  Module name:    PosData
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for PosData
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT.HelperModels
{
  /// <summary>
  /// The PosData object contains information about the transaction origin
  /// </summary>
  public class PosData
  {
    /// <summary>
    /// Uniquely identifies the store/location in the network
    /// </summary>
    public string storeID { get; set; }

    /// <summary>
    /// Uniquely identifies the terminal in the store
    /// </summary>
    public string terminalID { get; set; }
  }
}