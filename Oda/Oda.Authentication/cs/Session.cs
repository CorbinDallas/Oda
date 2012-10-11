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
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
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
            Core.Initialize += new EventHandler(Core_Initialize);
            Core.BeginHttpRequest +=new EventHandler(Core_BeginHttpRequest);
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
        void Core_Initialize(object sender, EventArgs e) {
            // wait until the connection is opened
            while(Sql.State != System.Data.ConnectionState.Open) {
                Thread.Sleep(10);
            }
            // check that the session table exists
            using(SqlCommand cmd = new SqlCommand(this.GetResrouceString("/Sql/CreateSessionTable.sql"), Sql.Connection)) {
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
        /// <summary>
        /// Private field for AccountId.
        /// </summary>
        private Guid _AccountId;
        /// <summary>
        /// Private field for SessionId.
        /// </summary>
        private Guid _SessionId;
        /// <summary>
        /// Private field for Id.
        /// </summary>
        private Guid _Id;
        /// <summary>
        /// Private field for Name.
        /// </summary>
        private string _Name;
        /// <summary>
        /// Private field for Value.
        /// </summary>
        private string _Value;
        /// <summary>
        /// Private field for DataType.
        /// </summary>
        private string _DataType;
        /// <summary>
        /// The unique id of this property
        /// </summary>
        public Guid Id { 
            set {
                _Id = value;
                Update();
            }
            get {
                if(_Id == null) {
                    _Id = Guid.NewGuid();
                }
                return _Id;
            }
        }
        /// <summary>
        /// The account associated this property.
        /// </summary>
        public Guid AccountId {
            set {
                _AccountId = value;
                Update();
            }
            get {
                if(_AccountId == null) {
                    _AccountId = Guid.Empty;
                }
                return _AccountId;
            }
        }
        /// <summary>
        /// The session associated this property.
        /// </summary>
        public Guid SessionId {
            set {
                _SessionId = value;
                Update();
            }
            get {
                if(_SessionId == null) {
                    _SessionId = Guid.Empty;
                }
                return _SessionId;
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
                _Name = value;
                Update();
            }
            get {
                if(_Name == null) {
                    _Name = "";
                }
                return _Name;
            }
        }
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value {
            set {
                _Value = value;
                Update();
            }
            get {
                if(_Value == null) {
                    _Value = "";
                }
                return _Value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the data of of the property.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string DataType {
            set {
                _DataType = value;
                Update();
            }
            get {
                if(_DataType == null) {
                    _DataType = "string";
                }
                return _DataType;
            }
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
            string query = SessionInit.SessionInitRef.GetResrouceString("/Sql/UpdateSessionProperties.sql");
            //@AccountId @SessionId @SessionPropertyId @Name @Value @DataType
            using(SqlCommand cmd = new SqlCommand(query,cn, trans)){
                cmd.Parameters.Add("@SessionPropertyId", SqlDbType.UniqueIdentifier).Value = this.Id;
                cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = this.AccountId;
                cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = this.SessionId;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = this.Name;
                cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = this.Value;
                cmd.Parameters.Add("@DataType", SqlDbType.VarChar).Value = this.DataType;
                cmd.ExecuteNonQuery();
            }
            return this;
        }
    }
    public class SessionProperties {
        /// <summary>
        /// The accountId associated with these properties.
        /// </summary>
        public Guid AccountId;
        /// <summary>
        /// The account associated with these properties.
        /// </summary>
        public Guid SessionId;
        List<SessionProperty> Properties;
        public void Add(SessionProperty _sessionProperty) {
            _sessionProperty.SuspendUpdates = true;
            _sessionProperty.AccountId = AccountId;
            _sessionProperty.SessionId = SessionId;
            _sessionProperty.Update();
            _sessionProperty.SuspendUpdates = false;
            Properties.Add(_sessionProperty);
        }
        public SessionProperty Find(Predicate<SessionProperty> match) {
            return Properties.Find(match);
        }
        public bool Remove(SessionProperty _sessionProperty) {
            _sessionProperty.SuspendUpdates = true;
            _sessionProperty.AccountId = AccountId;
            _sessionProperty.SessionId = SessionId;
            _sessionProperty.Update();
            _sessionProperty.SuspendUpdates = false;
            return Properties.Remove(_sessionProperty);
        }
        public void AddRange(SessionProperty[] _sessionProperties) {
            foreach(SessionProperty _sessionProperty in _sessionProperties) {
                _sessionProperty.SuspendUpdates = true;
                _sessionProperty.AccountId = AccountId;
                _sessionProperty.SessionId = SessionId;
                _sessionProperty.Update();
                _sessionProperty.SuspendUpdates = false;
                Properties.Add(_sessionProperty);
            }
        }
        public SessionProperties(Guid _sessionId) {
            SessionId = _sessionId;
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
        /// <param name="_id">The _id.</param>
        /// <returns></returns>
        public SessionProperty GetProperty(Guid _id) {
            return Properties.Find(delegate(SessionProperty p) {
                return p.Id == _id;
            });
        }
        /// <summary>
        /// Gets the property by the properties name.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <returns></returns>
        public SessionProperty GetProperty(string _name) {
            return Properties.Find(delegate(SessionProperty p) {
                return p.Name == _name;
            });
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
            HttpCookie sessionCookie = context.Request.Cookies[Session.SessionKey];
            string strSessionId = "";
            if(sessionCookie == null) {
                // no cookie found, so assign one
                strSessionId = Guid.NewGuid().ToString();
                sessionCookie = new HttpCookie(Session.SessionKey);
                sessionCookie.Value = strSessionId;
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
                Session session = ((Session)HttpContext.Current.Items[SessionKey]);
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
            string refreshCommand = SessionInit.SessionInitRef.GetResrouceString("/Sql/CreateUpdateSession.sql");
            using(SqlCommand cmd = new SqlCommand(refreshCommand, cn)) {
                cmd.Parameters.Add("@SessionId", SqlDbType.UniqueIdentifier).Value = this.Id;
                System.Collections.Specialized.NameValueCollection s = HttpContext.Current.Request.ServerVariables;
                string Referer = s["HTTP_REFERER"];
                string UserAgent = s["HTTP_USER_AGENT"];
                string IpAddress = s["REMOTE_ADDR"];
                if(Referer == null) { Referer = ""; }
                if(UserAgent == null) { UserAgent = ""; }
                if(IpAddress == null) { IpAddress = ""; }
                cmd.Parameters.Add("@Referer", SqlDbType.VarChar).Value = Referer;
                cmd.Parameters.Add("@UserAgent", SqlDbType.VarChar).Value = UserAgent;
                cmd.Parameters.Add("@IpAddress", SqlDbType.VarChar).Value = IpAddress;
                using(SqlDataReader r = cmd.ExecuteReader()) {
                    r.Read();
                    /* get AccountId */
                    this.AccountId = r.GetGuid(0);
                    /* get properties */
                    r.NextResult();
                    while(r.Read()) {
                        SessionProperty p = new SessionProperty();
                        p.SuspendUpdates = true;
                        p.SessionId = this.Id;
                        p.AccountId = this.AccountId;
                        p.Id = r.GetGuid(0);
                        p.Name = r.GetString(1);
                        p.Value = r.GetString(2);
                        p.DataType = r.GetString(3);
                        this.Properties.Add(p);
                        p.SuspendUpdates = false;
                    }
                    /* get contacts */
                    r.NextResult();
                    while(r.Read()) {
                        Contact c = new Contact();
                        c.Id = r.GetGuid(0);
                        c.AccountId = r.GetGuid(1);
                        c.First = r.GetString(2);
                        c.Middle = r.GetString(3);
                        c.Last = r.GetString(4);
                        c.Address = r.GetString(5);
                        c.Address2 = r.GetString(6);
                        c.City = r.GetString(7);
                        c.State = r.GetString(8);
                        c.Zip = r.GetString(9);
                        c.Email = r.GetString(10);
                        c.Company = r.GetString(11);
                        c.Title = r.GetString(12);
                        c.WebAddress = r.GetString(13);
                        c.IMAddress = r.GetString(14);
                        c.Fax = r.GetString(15);
                        c.Home = r.GetString(16);
                        c.Work = r.GetString(17);
                        c.Mobile = r.GetString(18);
                        c.Notes = r.GetString(19);
                        c.Type = (ContactType)r.GetInt32(20);
                        if(c.Id == this.Id){
                            // if this contact id matches the
                            // account id, then this is the account's
                            // primary contact
                            this.Account.Contact = c;
                        }else{
                            this.Account.Contacts.Add(c);
                        }
                    }
                }
            }
            Properties.AccountId = this.AccountId;
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
        /// Private field for Id.
        /// </summary>
        private Guid _Id;
        /// <summary>
        /// Gets or sets the Id of this session.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id {
            get {
                return _Id;
            }
            set {
                _Id = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="_id">The _id.</param>
        public Session(Guid _id) {
            Id = _id;
            Account = AnonymousAccount;
            this.Properties = new SessionProperties(_id);
        }
    }
}
