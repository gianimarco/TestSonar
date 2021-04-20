/****************************************************************************************************/
//  Module name:    NetwordData
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for NetwordData
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT.HelperModels
{
  /// <summary>
  /// The NetworkData object contains information about the network, where the transaction was initiated.
  /// </summary>
  public class NetworkData
  {
    /// <summary>
    /// Uniquely identifies the network.
    /// </summary>
    public int networkId { get; set; }

    /// <summary>
    /// The name of the network where the transaction was initiated.
    /// </summary>
    public string networkName { get; set; }

    /// <summary>
    /// Refer to PosData
    /// </summary>
    public PosData posData { get; set; }
  }
}