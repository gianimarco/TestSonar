/****************************************************************************************************/
//  Module name:    StructuredData
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for StructuredData
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT.HelperModels
{
  /// <summary>
  /// The StructuredData is a JSON formatted field that contains client-specific or industry-specific data
  /// </summary>
  public class StructuredData
  {
    /// <summary>
    /// Issuer Data class
    /// </summary>
    public IssuerData issuerData { get; set; }

    /// <summary>
    /// Refer to BusData
    /// </summary>
    public string busData { get; set; }

    /// <summary>
    /// Refer to FlightData
    /// </summary>
    public string flightData { get; set; }

    /// <summary>
    /// Refer to TrafficFineData
    /// </summary>
    public string trafficFineData { get; set; }
  }

  /// <summary>
  /// Issuer Data class
  /// </summary>
  public class IssuerData
  {
    /// <summary>
    /// The name of the issuer
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// The contact number of the issuer
    /// </summary>
    public string contactNumber { get; set; }
  }
}