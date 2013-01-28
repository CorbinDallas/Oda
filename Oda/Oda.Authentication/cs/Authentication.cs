/*
 * Copyright (c) 2012 Tony Germaneri
 * Permission is hereby granted,  free of charge, to any person 
 * obtaining a copy of this software and associated documentation files 
 * (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARSING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Data.SqlClient;
using System.Data;
namespace Oda {
    /// <summary>
    /// Events used in authentication (logon/logoff)
    /// and related procedures like password changing, 
    /// creating accounts and resetting passwords.
    /// </summary>
    public interface IAuthentication {
        /// <summary>
        /// Occurs before an account is created.
        /// </summary>
        event EventHandler BeforeCreateAccount;
        /// <summary>
        /// Occurs after an account is created.
        /// </summary>
        event EventHandler AfterCreateAccount;
        /// <summary>
        /// Occurs before logon.
        /// </summary>
        event EventHandler BeforeLogon;
        /// <summary>
        /// Occurs after logon.
        /// </summary>
        event EventHandler AfterLogon;
        /// <summary>
        /// Occurs before logoff.
        /// </summary>
        event EventHandler BeforeLogoff;
        /// <summary>
        /// Occurs after logoff.
        /// </summary>
        event EventHandler AfterLogoff;
        /// <summary>
        /// Occurs before changing passwords.
        /// </summary>
        event EventHandler BeforeChangePassword;
        /// <summary>
        /// Occurs after changing passwords.
        /// </summary>
        event EventHandler AfterChangePassword;
        /// <summary>
        /// Occurs before resetting password.
        /// </summary>
        event EventHandler BeforeResetPassword;
        /// <summary>
        /// Occurs after resetting password.
        /// </summary>
        event EventHandler AfterResetPassword;
    }
    /// <summary>
    /// Arguments for the IAuthentication event interface
    /// </summary>
    public class UpdateAccountArgs : EventArgs {
        /// <summary>
        /// Gets the session.
        /// </summary>
        public Session Session { private set; get; }
        /// <summary>
        /// Gets the account.
        /// </summary>
        public Account Account { private set; get; }
        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { private set; get; }
        /// <summary>
        /// Gets the new password.
        /// </summary>
        public string NewPassword { internal set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether the default event is aborted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [abort default]; otherwise, <c>false</c>.
        /// </value>
        public bool AbortDefault { get; set; }
        /// <summary>
        /// Gets the Json result.
        /// </summary>
        public JsonResponse JsonResponse { private set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAccountArgs" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="account">The account.</param>
        /// <param name="password">The password.</param>
        /// <param name="abortDefault">if set to <c>true</c> [abort default].</param>
        /// <param name="jsonResponse">The json response.</param>
        public UpdateAccountArgs(Session session, Account account, 
            string password, bool abortDefault, JsonResponse jsonResponse) {
            Session = session;
            Account = account;
            Password = password;
            JsonResponse = jsonResponse;
            AbortDefault = abortDefault;
        }
    }
    /// <summary>
    /// All the methods for authentication.
    /// </summary>
    public class Authentication : JsonMethods, IAuthentication {
        #region Events
        #region Event : AfterResetPassword
        internal static void RaiseOnAfterResetPassword(UpdateAccountArgs args) {
            if (AfterResetPassword != null) AfterResetPassword(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs after resetting password.
        /// </summary>
        public static event EventHandler AfterResetPassword;
        /// <summary>
        /// Occurs after resetting password.
        /// </summary>
        event EventHandler IAuthentication.AfterResetPassword {
            add {
                lock(AfterResetPassword) {
                    AfterResetPassword += value;
                }
            }
            remove {
                lock(AfterResetPassword) {
                    AfterResetPassword -= value;
                }
            }
        }
        #endregion
        #region Event : BeforeResetPassword
        internal static void RaiseOnBeforeResetPassword(UpdateAccountArgs args) {
            if (BeforeResetPassword != null) BeforeResetPassword(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs before resetting password.
        /// </summary>
        public static event EventHandler BeforeResetPassword;
        /// <summary>
        /// Occurs before reseting password.
        /// </summary>
        event EventHandler IAuthentication.BeforeResetPassword {
            add {
                lock(BeforeResetPassword) {
                    BeforeResetPassword += value;
                }
            }
            remove {
                lock(BeforeResetPassword) {
                    BeforeResetPassword -= value;
                }
            }
        }
        #endregion
        #region Event : AfterChangePassword
        internal static void RaiseOnAfterChangePassword(UpdateAccountArgs args) {
            if (AfterChangePassword != null) AfterChangePassword(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs after changing passwords.
        /// </summary>
        public static event EventHandler AfterChangePassword;
        /// <summary>
        /// Occurs after changing passwords.
        /// </summary>
        event EventHandler IAuthentication.AfterChangePassword {
            add {
                lock(AfterChangePassword) {
                    AfterChangePassword += value;
                }
            }
            remove {
                lock(AfterChangePassword) {
                    AfterChangePassword -= value;
                }
            }
        }
        #endregion
        #region Event : BeforeChangePassword
        internal static void RaiseOnBeforeChangePassword(UpdateAccountArgs args) {
            if (BeforeChangePassword != null) BeforeChangePassword(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs before changing passwords.
        /// </summary>
        public static event EventHandler BeforeChangePassword;
        /// <summary>
        /// Occurs before changing passwords.
        /// </summary>
        event EventHandler IAuthentication.BeforeChangePassword {
            add {
                lock(BeforeChangePassword) {
                    BeforeChangePassword += value;
                }
            }
            remove {
                lock(BeforeChangePassword) {
                    BeforeChangePassword -= value;
                }
            }
        }
        #endregion
        #region Event : AfterLogoff
        internal static void RaiseOnAfterLogoff(UpdateAccountArgs args) {
            if (AfterLogoff != null) AfterLogoff(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs after logoff.
        /// </summary>
        public static event EventHandler AfterLogoff;
        /// <summary>
        /// Occurs after logoff.
        /// </summary>
        event EventHandler IAuthentication.AfterLogoff {
            add {
                lock(AfterLogoff) {
                    AfterLogoff += value;
                }
            }
            remove {
                lock(AfterLogoff) {
                    AfterLogoff -= value;
                }
            }
        }
        #endregion
        #region Event : BeforeLogoff
        internal static void RaiseOnBeforeLogoff(UpdateAccountArgs args) {
            if (BeforeLogoff != null) BeforeLogoff(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs before logoff.
        /// </summary>
        public static event EventHandler BeforeLogoff;
        /// <summary>
        /// Occurs before logoff.
        /// </summary>
        event EventHandler IAuthentication.BeforeLogoff {
            add {
                lock(BeforeLogoff) {
                    BeforeLogoff += value;
                }
            }
            remove {
                lock(BeforeLogoff) {
                    BeforeLogoff -= value;
                }
            }
        }
        #endregion
        #region Event : AfterLogon
        internal static void RaiseOnAfterLogon(UpdateAccountArgs args) {
            if (AfterLogon != null) AfterLogon(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs after logon.
        /// </summary>
        public static event EventHandler AfterLogon;
        /// <summary>
        /// Occurs after logon.
        /// </summary>
        event EventHandler IAuthentication.AfterLogon {
            add {
                lock(AfterLogon) {
                    AfterLogon += value;
                }
            }
            remove {
                lock(AfterLogon) {
                    AfterLogon -= value;
                }
            }
        }
        #endregion
        #region Event : BeforeLogon
        internal static void RaiseOnBeforeLogon(UpdateAccountArgs args) {
            if(BeforeLogon != null) BeforeLogon(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs before logon.
        /// </summary>
        public static event EventHandler BeforeLogon;
        /// <summary>
        /// Occurs before logon.
        /// </summary>
        event EventHandler IAuthentication.BeforeLogon {
            add {
                lock(BeforeLogon) {
                    BeforeLogon += value;
                }
            }
            remove {
                lock(BeforeLogon) {
                    BeforeLogon -= value;
                }
            }
        }
        #endregion
        #region Event : AfterCreateAccount
        internal static void RaiseOnAfterCreateAccount(UpdateAccountArgs args) {
            if (AfterCreateAccount != null) AfterCreateAccount(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs after an account is created.
        /// </summary>
        public static event EventHandler AfterCreateAccount;
        /// <summary>
        /// Occurs after an account is created.
        /// </summary>
        event EventHandler IAuthentication.AfterCreateAccount {
            add {
                lock(AfterCreateAccount) {
                    AfterCreateAccount += value;
                }
            }
            remove {
                lock(AfterCreateAccount) {
                    AfterCreateAccount -= value;
                }
            }
        }
        #endregion
        #region Event : BeforeCreateAccount
        internal static void RaiseOnBeforeCreateAccount(UpdateAccountArgs args) {
            if (BeforeCreateAccount != null) BeforeCreateAccount(Core.HttpApplication, args);
        }
        /// <summary>
        /// Occurs before an account is created.
        /// </summary>
        public static event EventHandler BeforeCreateAccount;
        /// <summary>
        /// Occurs before an account is created.
        /// </summary>
        event EventHandler IAuthentication.BeforeCreateAccount {
            add {
                lock(BeforeCreateAccount) {
                    BeforeCreateAccount += value;
                }
            }
            remove {
                lock(BeforeCreateAccount) {
                    BeforeCreateAccount -= value;
                }
            }
        }
        #endregion
        #endregion
        #region Private Methods
        /// <summary>
        /// Gets the nonce from the database by looking up an account by logon name.
        /// </summary>
        /// <param name="logon">The logon.</param>
        /// <returns></returns>
        private static string GetNonce(string logon) {
            var nonceQuery = GetResString("/Sql/GetNonce.sql");
            using(var cmd = new SqlCommand(nonceQuery, Sql.Connection)) {
                cmd.Parameters.Add("@Logon", SqlDbType.VarChar).Value = logon;
                using(SqlDataReader r = cmd.ExecuteReader()) {
                    return r.Read() ? r.GetString(0) : BCrypt.GenerateSalt();
                }
            }
        }
        /// <summary>
        /// Gets a resource string from the static plugin reference.
        /// </summary>
        /// <param name="resourceString">The resource string.</param>
        /// <returns></returns>
        private static string GetResString(string resourceString) {
            return AuthenticationPlugin.
                AuthenticationPluginRef.GetResourceString(resourceString);
        }
        #endregion
        #region JSON Methods
        /// <summary>
        /// Creates an account.
        /// </summary>
        /// <param name="logon">The logon.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static JsonResponse CreateAccount(string logon, string password) {
            var j = new JsonResponse();
            var current = Session.Current;
            var nonce = BCrypt.GenerateSalt();
            var query = GetResString("/Sql/CreateAccount.sql");
            var digestPassword = BCrypt.HashPassword(password, nonce);
            var args = new UpdateAccountArgs(current, Session.AnonymousAccount, password, false, j);
            RaiseOnBeforeCreateAccount(args);
            if(!args.AbortDefault) {
                // @AccountId, @Logon, @DigestPassword
                using(var cmd = new SqlCommand(query, Sql.Connection)) {
                    cmd.Parameters.Add("@Logon", SqlDbType.VarChar).Value = logon;
                    cmd.Parameters.Add("@DigestPassword", SqlDbType.VarChar).Value = digestPassword;
                    cmd.Parameters.Add("@Nonce", SqlDbType.VarChar).Value = nonce;
                    using(var r = cmd.ExecuteReader()) {
                        r.Read();
                        current.AccountId = r.GetGuid(0);
                        j.Message = r.GetString(1);
                        if(current.AccountId == Guid.Empty) {
                            j.Error = 1;
                        }
                    }
                }
            }
            RaiseOnAfterCreateAccount(args);
            return j;
        }
        /// <summary>
        /// Logon the current session using logon and password.
        /// </summary>
        /// <param name="logon">The _logon.</param>
        /// <param name="password">The _password.</param>
        /// <returns></returns>
        public static JsonResponse Logon(string logon, string password) {
            var current = Session.Current;
            var j = new JsonResponse();
            var query = GetResString("/Sql/LogonSession.sql");
            var args = new UpdateAccountArgs(current, current.Account, password, false, j);
            RaiseOnBeforeLogon(args);
            if(!args.AbortDefault) {
                var nonce = GetNonce(logon);
                var digestPassword = BCrypt.HashPassword(password, nonce);
                // @Logon @DigestPassword @SessionId
                using(var cmd = new SqlCommand(query, Sql.Connection)) {
                    cmd.Parameters.Add("@Logon", SqlDbType.VarChar).Value = logon;
                    cmd.Parameters.Add("@DigestPassword", SqlDbType.VarChar).Value = digestPassword;
                    cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = current.Id;
                    using(SqlDataReader r = cmd.ExecuteReader()) {
                        r.Read();
                        // Result 1 @AccountId,'Logged On';
                        current.AccountId = r.GetGuid(0);
                        j.Message = r.GetString(1);
                        // Result 2 occurs when AccountId != Guid.Empty
                        if(current.AccountId == Guid.Empty) {
                            // logon failed
                            j.Error = 1;
                        }
                    }
                }
            }
            RaiseOnAfterLogon(args);
            return j;
        }
        /// <summary>
        /// Logoffs the current session.
        /// </summary>
        /// <returns></returns>
        public static JsonResponse Logoff() {
            var current = Session.Current;
            var j = new JsonResponse();
            var args = new UpdateAccountArgs(current, current.Account, "", false, j);
            RaiseOnBeforeLogoff(args);
            if(!args.AbortDefault) {
                var query = GetResString("/Sql/LogoffSession.sql");
                using(var cmd = new SqlCommand(query, Sql.Connection)) {
                    cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = current.Id;
                    cmd.ExecuteNonQuery();
                }
                // run another query to update the session data
                current.Refresh();
            }
            RaiseOnAfterLogoff(args);
            return j;
        }
        /// <summary>
        /// Resets the password based on logon.
        /// Warning: this method shouldn't be used without
        /// subscribing to the AfterResetPassword event.
        /// Failure to do so will create a password nobody can discover.
        /// </summary>
        /// <param name="logon">The logon.</param>
        /// <returns></returns>
        public static JsonResponse ResetPassword(string logon) {
            var salt = BCrypt.GenerateSalt();
            var newPassword = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToLower().Substring(1, 10);
            var newDigestPassword = BCrypt.HashPassword(newPassword, salt);
            var current = Session.Current;
            var j = new JsonResponse();
            var args = new UpdateAccountArgs(current, current.Account, "", false, j);
            RaiseOnBeforeResetPassword(args);
            if(!args.AbortDefault) {
                // @NewDigestPassword @Logon
                var query = GetResString("/Sql/ResetPassword.sql");
                using(var cmd = new SqlCommand(query, Sql.Connection)) {
                    cmd.Parameters.Add("@Logon", SqlDbType.UniqueIdentifier).Value = logon;
                    cmd.Parameters.Add("@NewDigestPassword", SqlDbType.VarChar).Value = newDigestPassword;
                    using(var r = cmd.ExecuteReader()) {
                        r.Read();
                        j.Error = r.GetInt32(0);
                        j.Message = r.GetString(1);
                    }
                }
            }
            RaiseOnAfterResetPassword(args);
            return j;
        }
        /// <summary>
        /// Changes the password by providing a logon, the old password and a new password.
        /// </summary>
        /// <param name="logon">The logon.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public static JsonResponse ChangePassword(string logon, string oldPassword, string newPassword) {
            var j = new JsonResponse();
            var oldNonce = GetNonce(logon);
            var newNonce = BCrypt.GenerateSalt();
            var current = Session.Current;
            var oldDigestPassword = BCrypt.HashPassword(oldPassword, oldNonce);
            var newDigestPassword = BCrypt.HashPassword(newPassword, newNonce);
            var args = new UpdateAccountArgs(current, current.Account, oldPassword, false, j) {NewPassword = newPassword};
            RaiseOnBeforeChangePassword(args);
            if(!args.AbortDefault) {
                var query = GetResString("/Sql/ChangePassword.sql");
                // @NewDigestPassword @AccountId @DigestPassword
                using(var cmd = new SqlCommand(query, Sql.Connection)) {
                    cmd.Parameters.Add("@Logon", SqlDbType.UniqueIdentifier).Value = logon;
                    cmd.Parameters.Add("@DigestPassword", SqlDbType.VarChar).Value = oldDigestPassword;
                    cmd.Parameters.Add("@NewDigestPassword", SqlDbType.VarChar).Value = newDigestPassword;
                    cmd.Parameters.Add("@NewNonce", SqlDbType.VarChar).Value = newNonce;
                    cmd.Parameters.Add("@OldNonce", SqlDbType.VarChar).Value = oldNonce;
                    using(var r = cmd.ExecuteReader()) {
                        r.Read();
                        j.Error = r.GetInt32(0);
                        j.Message = r.GetString(1);
                    }
                }
            }
            RaiseOnAfterChangePassword(args);
            return j;
        }
        #endregion
    }
}