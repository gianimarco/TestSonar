/****************************************************************************************************/
//  Module name:    SecurityData
//  Date Created:   2020/06/01
//  Creator:        Marco-Pieter Kruger
//  Description:    Base return model for SecurityData
//                                  ***Change history:***
//  Log/PR/Task:    Date:       Developer:      Reason:
//
/*****************************************************************************************************/

namespace DirectTransact.WebApi.PayAT.HelperModels
{
  /// <summary>
  /// The SecurityData object contains data used for authentication and data verification.
  /// </summary>
  public class SecurityData
  {
    /// <summary>
    /// Login element to contain loginId and password
    /// </summary>
    public Login login { get; set; }
  }

  /// <summary>
  /// Login element to contain loginId and password
  /// </summary>
  public class Login
  {
    /// <summary>
    /// The login ID or username assigned to Pay@
    /// </summary>
    public string loginID { get; set; }

    /// <summary>
    /// The password assigned to Pay@
    /// </summary>
    public string password { get; set; }
  }
}