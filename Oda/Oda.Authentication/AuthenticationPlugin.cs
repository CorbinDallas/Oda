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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        void CoreInitialize(object sender, EventArgs e) {
            // check that the session table exists
            using (var cmd = new SqlCommand(GetResourceString("/Sql/CreateSessionTable.sql"), Sql.Connection)) {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
