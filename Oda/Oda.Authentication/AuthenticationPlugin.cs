using System;
using System.Data.SqlClient;
namespace Oda {
    /// <summary>
    /// The main plugin class for Sessions and Authentication
    /// </summary>
    [PluginDependencies(typeof(Sql))]
    public class AuthenticationPlugin : Plugin {
        internal static AuthenticationPlugin AuthenticationPluginRef;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPlugin"/> class.
        /// Bind events to Initialize and BeginHttpRequest.
        /// </summary>
        public AuthenticationPlugin() {
            Core.Initialize += CoreInitialize;
            Core.BeginHttpRequest += Core_BeginHttpRequest;
            AuthenticationPluginRef = this;
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
            // check that the session table exists
            using (var cmd = new SqlCommand(GetResourceString("/Sql/CreateSessionTable.sql"), Sql.Connection)) {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
