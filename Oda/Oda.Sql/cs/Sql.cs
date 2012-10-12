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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Oda {
    /// <summary>
    /// This plugin allows you to attach to a T-SQL database and preserve an open connection.
    /// </summary>
    public class Sql : Plugin {
        /// <summary>
        /// Gets the connection state.
        /// </summary>
        public static ConnectionState State {
            get {
                return Connection.State;
            }
        }
        /// <summary>
        /// The private field for the Name property
        /// </summary>
        private static string _name;
        /// <summary>
        /// Gets the name of the connection specified in the web.config of the host site.
        /// If no name is specified then the name "sql" will be used.
        /// </summary>
        public static string Name {
            get {
                return _name;
            }
        }
        /// <summary>
        /// The persistent Sql connection.
        /// </summary>
        public static SqlConnection Connection;
        /// <summary>
        /// Private connection string drawn from the web.config
        /// </summary>
        private static string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="Sql"/> class.
        /// </summary>
        public Sql() {
            _name = ConfigurationManager.AppSettings["connectionName"] ?? "sql";
            _connectionString = ConfigurationManager.ConnectionStrings[_name].ConnectionString;
            Connection = new SqlConnection(_connectionString);
            Open();
        }
        /// <summary>
        /// Creates a new Sql connection using the connection string defined in the web.config
        /// of the current web site.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection CreateConnection() {
            return new SqlConnection(_connectionString);
        }
        /// <summary>
        /// Opens the persistent connection.
        /// </summary>
        public static void Open(){
            Connection.Open();
        }
        /// <summary>
        /// Closes the persistent connection.
        /// </summary>
        public static void Close() {
            Connection.Close();
        }
    }
}