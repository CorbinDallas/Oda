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
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
namespace Oda {
    /// <summary>
    /// The main plugin class for Sessions and Authentication
    /// </summary>
    public class SessionInit : Plugin {
        internal static SessionInit SessionInitRef;
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionInit"/> class.
        /// Bind events to Initialize and BeginHttpRequest.
        /// </summary>
        public SessionInit() {
            Core.Initialize += CoreInitialize;
            Core.BeginHttpRequest += Core_BeginHttpRequest;
            SessionInitRef = this;
        }
        /// <summary>
        /// Handles the BeginHttpRequest event of the Core control.
        /// Create or refresh session from Sql database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Core_BeginHttpRequest(object sender, EventArgs e) {
            // fetch all the most current information
            // and create a reference that will last
            // the duration of the request.
            Session.Current.Refresh();
        }
        /// <summary>
        /// Handles the Initialize event of the Core control.
        /// This will make sure all tables used by Oda.Session are created
        /// in the current database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CoreInitialize(object sender, EventArgs e) {
            // wait until the connection is opened
            while(Sql.State != ConnectionState.Open) {
                Thread.Sleep(10);
            }
            // check that the session table exists
            using(var cmd = new SqlCommand(GetResourceString("/Sql/CreateSessionTable.sql"), Sql.Connection)) {
                cmd.ExecuteNonQuery();
            }
        }
    }
    /// <summary>
    /// A property drawn from an Oda.Session class and automatically committed
    /// to the database
    /// </summary>
    public class SessionProperty {
        /// <summary>
        /// Don't update on property changes when false.
        /// </summary>
        public bool SuspendUpdates {get; set;}
        private Guid _accountId;
        private Guid _sessionId;
        private Guid _id;
        private string _name;
        private string _value;
        private string _dataType;
        /// <summary>
        /// The unique id of this property
        /// </summary>
        public Guid Id { 
            set {
                _id = value;
                Update();
            }
            get {
                return _id;
            }
        }
        /// <summary>
        /// The account associated this property.
        /// </summary>
        public Guid AccountId {
            set {
                _accountId = value;
                Update();
            }
            get {
                return _accountId;
            }
        }
        /// <summary>
        /// The session associated this property.
        /// </summary>
        public Guid SessionId {
            set {
                _sessionId = value;
                Update();
            }
            get {
                return _sessionId;
            }
        }
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name {
            set {
                _name = value;
                Update();
            }
            get { return _name ?? (_name = ""); }
        }
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value {
            set {
                _value = value;
                Update();
            }
            get { return _value ?? (_value = ""); }
        }
        /// <summary>
        /// Gets or sets the type of the data of of the property.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string DataType {
            set {
                _dataType = value;
                Update();
            }
            get { return _dataType ?? (_dataType = "string"); }
        }
        /// <summary>
        /// Updates this property by writing it to the database.
        /// </summary>
        /// <returns></returns>
        public SessionProperty Update() {
            Update(Sql.Connection, null);
            return this;
        }
        /// <summary>
        /// Updates this property by writing it to the database.
        /// </summary>
        /// <returns></returns>
        public SessionProperty Update(SqlConnection cn, SqlTransaction trans) {
            if(SuspendUpdates) {
                return this;
            }
            string query = SessionInit.SessionInitRef.GetResourceString("/Sql/UpdateSessionProperties.sql");
            //@AccountId @SessionId @SessionPropertyId @Name @Value @DataType
            using(var cmd = new SqlCommand(query,cn, trans)){
                cmd.Parameters.Add("@SessionPropertyId", SqlDbType.UniqueIdentifier).Value = Id;
                cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = AccountId;
                cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = SessionId;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = Name;
                cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = Value;
                cmd.Parameters.Add("@DataType", SqlDbType.VarChar).Value = DataType;
                cmd.ExecuteNonQuery();
            }
            return this;
        }
    }
    /// <summary>
    /// Generic properties collection for sessions
    /// </summary>
    public class SessionProperties {
        /// <summary>
        /// The accountId associated with these properties.
        /// </summary>
        public Guid AccountId;
        /// <summary>
        /// The account associated with these properties.
        /// </summary>
        public Guid SessionId;
        private List<SessionProperty> Properties { get; set; }
        /// <summary>
        /// Adds the specified session property.
        /// </summary>
        /// <param name="sessionProperty">The _session property.</param>
        public void Add(SessionProperty sessionProperty) {
            sessionProperty.SuspendUpdates = true;
            sessionProperty.AccountId = AccountId;
            sessionProperty.SessionId = SessionId;
            sessionProperty.Update();
            sessionProperty.SuspendUpdates = false;
            Properties.Add(sessionProperty);
        }
        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public SessionProperty Find(Predicate<SessionProperty> match) {
            return Properties.Find(match);
        }
        /// <summary>
        /// Removes the specified _session property.
        /// </summary>
        /// <param name="sessionProperty">The _session property.</param>
        /// <returns></returns>
        public bool Remove(SessionProperty sessionProperty) {
            sessionProperty.SuspendUpdates = true;
            sessionProperty.AccountId = AccountId;
            sessionProperty.SessionId = SessionId;
            sessionProperty.Update();
            sessionProperty.SuspendUpdates = false;
            return Properties.Remove(sessionProperty);
        }
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="sessionProperties">The session properties.</param>
        public void AddRange(SessionProperty[] sessionProperties) {
            foreach(var sessionProperty in sessionProperties) {
                sessionProperty.SuspendUpdates = true;
                sessionProperty.AccountId = AccountId;
                sessionProperty.SessionId = SessionId;
                sessionProperty.Update();
                sessionProperty.SuspendUpdates = false;
                Properties.Add(sessionProperty);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionProperties" /> class.
        /// </summary>
        /// <param name="sessionId">The _session id.</param>
        public SessionProperties(Guid sessionId) {
            SessionId = sessionId;
            Properties = new List<SessionProperty>();
        }
    }
    /// <summary>
    /// A session for Oda.
    /// </summary>
    public class Session {
        /// <summary>
        /// The session cookie name.
        /// </summary>
        internal static string SessionKey = "Session";
        /// <summary>
        /// The account Id that this session belongs to.
        /// </summary>
        public Guid AccountId;
        /// <summary>
        /// The SessionProperties associated with this session.
        /// </summary>
        public SessionProperties Properties;
        /// <summary>
        /// Gets a property by the properties id.
        /// </summary>
        /// <param name="id">The _id.</param>
        /// <returns></returns>
        public SessionProperty GetProperty(Guid id) {
            return Properties.Find(p => p.Id == id);
        }
        /// <summary>
        /// Gets the property by the properties name.
        /// </summary>
        /// <param name="name">The _name.</param>
        /// <returns></returns>
        public SessionProperty GetProperty(string name) {
            return Properties.Find(p => p.Name == name);
        }
        /// <summary>
        /// An anonymous Account.
        /// </summary>
        public static Account AnonymousAccount = new Account();
        /// <summary>
        /// Gets the session id from cookie.
        /// </summary>
        /// <returns></returns>
        private static Guid GetSessionIdFromCookie() {
            HttpContext context = HttpContext.Current;
            // check if this request has a cookie with a sessionId
            HttpCookie sessionCookie = context.Request.Cookies[SessionKey];
            string strSessionId;
            if(sessionCookie == null) {
                // no cookie found, so assign one
                strSessionId = Guid.NewGuid().ToString();
                sessionCookie = new HttpCookie(SessionKey) {Value = strSessionId};
                context.Response.Cookies.Add(sessionCookie);
            } else {
                strSessionId = sessionCookie.Value;
            }
            return new Guid(strSessionId);
        }
        /// <summary>
        /// Gets the current session making the Http request.
        /// </summary>
        public static Session Current {
            get {
                var session = ((Session)HttpContext.Current.Items[SessionKey]);
                if(session == null){
                    session = new Session(GetSessionIdFromCookie());
                    HttpContext.Current.Items[SessionKey] = session;
                }
                return session;
            }
        }
        /// <summary>
        /// Refreshes this session by reading data from the database.
        /// </summary>
        /// <returns>
        /// Returns a reference to the session being refreshed.
        /// </returns>
        public Session Refresh() {
            Refresh(Sql.Connection, null);
            return this;
        }
        /// <summary>
        /// Refreshes this session by reading data from the database.
        /// </summary>
        /// <param name="cn">The Sql connection.</param>
        /// <param name="trans">The Sql transaction.</param>
        /// <returns>
        /// Returns a reference to the session being refreshed.
        /// </returns>
        public Session Refresh(SqlConnection cn, SqlTransaction trans) {
            // fetch the most up to date info from the Sql server
            // Result 1
            // -> AccountId
            // Result 2
            // --> SessionPropertyId, Name, Value, DataType
            // @SessionId, @Referer, @UserAgent, @IpAddress, @AccountId
            var refreshCommand = SessionInit.SessionInitRef.GetResourceString("/Sql/CreateUpdateSession.sql");
            using(var cmd = new SqlCommand(refreshCommand, cn)) {
                cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = Id;
                var s = HttpContext.Current.Request.ServerVariables;
                var referer = s["HTTP_REFERER"];
                var userAgent = s["HTTP_USER_AGENT"];
                var ipAddress = s["REMOTE_ADDR"];
                if(referer == null) { referer = ""; }
                if(userAgent == null) { userAgent = ""; }
                if(ipAddress == null) { ipAddress = ""; }
                cmd.Parameters.Add("@Referer", SqlDbType.VarChar).Value = referer;
                cmd.Parameters.Add("@UserAgent", SqlDbType.VarChar).Value = userAgent;
                cmd.Parameters.Add("@IpAddress", SqlDbType.VarChar).Value = ipAddress;
                using(SqlDataReader r = cmd.ExecuteReader()) {
                    r.Read();
                    /* get AccountId */
                    AccountId = r.GetGuid(0);
                    /* get properties */
                    r.NextResult();
                    while(r.Read()) {
                        var p = new SessionProperty
                                    {
                                        SuspendUpdates = true,
                                        SessionId = Id,
                                        AccountId = AccountId,
                                        Id = r.GetGuid(0),
                                        Name = r.GetString(1),
                                        Value = r.GetString(2),
                                        DataType = r.GetString(3)
                                    };
                        Properties.Add(p);
                        p.SuspendUpdates = false;
                    }
                    /* get contacts */
                    r.NextResult();
                    while(r.Read()) {
                        var c = new Contact
                                    {
                                        Id = r.GetGuid(0),
                                        AccountId = r.GetGuid(1),
                                        First = r.GetString(2),
                                        Middle = r.GetString(3),
                                        Last = r.GetString(4),
                                        Address = r.GetString(5),
                                        Address2 = r.GetString(6),
                                        City = r.GetString(7),
                                        State = r.GetString(8),
                                        Zip = r.GetString(9),
                                        Email = r.GetString(10),
                                        Company = r.GetString(11),
                                        Title = r.GetString(12),
                                        WebAddress = r.GetString(13),
                                        IMAddress = r.GetString(14),
                                        Fax = r.GetString(15),
                                        Home = r.GetString(16),
                                        Work = r.GetString(17),
                                        Mobile = r.GetString(18),
                                        Notes = r.GetString(19),
                                        Type = (ContactType) r.GetInt32(20)
                                    };
                        if(c.Id == Id){
                            // if this contact id matches the
                            // account id, then this is the account's
                            // primary contact
                            Account.Contact = c;
                        }else{
                            if (Account.Contacts==null) Account.Contacts = new List<Contact>();
                            Account.Contacts.Add(c);
                        }
                    }
                }
            }
            Properties.AccountId = AccountId;
            return this;
        }
        /// <summary>
        /// When the session is logged on then <c>true</c> otherwise <b>false</b>.
        /// </summary>
        public bool LoggedOn;
        /// <summary>
        /// The Account of this session.
        /// </summary>
        public Account Account;

        /// <summary>
        /// Gets or sets the Id of this session.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="id">The _id.</param>
        public Session(Guid id) {
            Id = id;
            Account = AnonymousAccount;
            Properties = new SessionProperties(id);
        }
    }
}
