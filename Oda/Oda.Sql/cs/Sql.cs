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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
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
        /// Gets the name of the connection specified in the web.config of the host site.
        /// If no name is specified then the name "sql" will be used.
        /// </summary>
        public static string Name { get; private set; }
        /// <summary>
        /// The persistent Sql connection.
        /// </summary>
        public static SqlConnection Connection;
        /// <summary>
        /// Initializes a new instance of the <see cref="Sql"/> class.
        /// </summary>
        public Sql() {
            // create a new static connection on startup
            Name = ConfigurationManager.AppSettings["connectionName"] ?? "sql";
            Connection = CreateConnection();
            // make sure the connection stays open
            Connection.StateChange += ConnectionStateChange;
            Open();
            // check that UIColumns table exists
            using (var cmd = new SqlCommand(GetResourceString("/Sql/CreateUIColumns.sql"), Connection)) {
                cmd.ExecuteNonQuery();
            }
            // check that JsonCrud SP exists
            using (var cmd = new SqlCommand(GetResourceString("/Sql/CreateJsonCrud.sql"), Connection)) {
                cmd.ExecuteNonQuery();
            }
        }
        static void ConnectionStateChange(object sender, StateChangeEventArgs e){
            if(!e.CurrentState.Equals(ConnectionState.Broken | ConnectionState.Closed)){
                return;
            }
            Close();
            while (Connection.State.Equals(ConnectionState.Broken | ConnectionState.Closed)) {
                // wait five seconds and try to connect again
                System.Threading.Thread.Sleep(5000);
                Open();
            }
        }
        /// <summary>
        /// Creates a new Sql connection using the connection string defined in the web.config
        /// of the current web site.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection CreateConnection() {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[Name].ConnectionString);
        }
        /// <summary>
        /// Opens the persistent connection.
        /// </summary>
        public static void Open(){
            if(Connection.State.Equals(ConnectionState.Open)){
                return;
            }
            Connection.Open();
        }
        /// <summary>
        /// Closes the persistent connection.
        /// </summary>
        public static void Close() {
            if(Connection.State.Equals(ConnectionState.Closed)){
                return;
            }
            Connection.Close();
        }
        /// <summary>
        /// JSON Read and Delete for SQL objects.  Note: Delete requires a VerCol timestamp column present in the table.
        /// State data is stored in the table UIColumns on a per user basis.  Users are defined in Oda.Authentication class
        /// and in the table Accounts.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="rowFrom">The from row.</param>
        /// <param name="rowTo">The to row.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="searchClause">The search clause.</param>
        /// <param name="aggregates">The aggregates.</param>
        /// <param name="selectedRows">The selected rows.</param>
        /// <param name="includeSchemaData">if set to <c>true</c> include schema data.</param>
        /// <param name="checksum">The checksum.</param>
        /// <param name="deleteSelection">if set to <c>true</c> delete selection defined in selectedRows as ordered based on this checksum.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderByDirection">The order by direction.</param>
        /// <returns>JSON Row data and schema or aggregates or delete result.</returns>
        public static JsonResponse JsonCrud(string objectName, int rowFrom, int rowTo, string whereClause, Guid accountId, string searchClause,
                                            IDictionary<string, string> aggregates, ICollection<int> selectedRows, bool includeSchemaData, Int64 checksum, bool deleteSelection,
                                            string orderBy, OrderDirection orderByDirection) {
            const string query = @"JsonCrud @objName, @record_from, @record_to, @suffix, @accountId, @searchSuffix,
@aggregateColumns, @selectedRowsCSV, @includeSchema, @checksum, @delete, @orderBy_override, @orderDirection_override";
            var s = new JsonResponse();
            var aggs = new StringBuilder();
            var sRows = new StringBuilder();
            s.MethodName = "JsonCrud";
            var rows = new List<object>();
            s.Add("rows", rows);
            // convert aggregate column dictionary to a string
            if(aggregates.Count>0){
                foreach(var k in aggregates){
                    aggs.AppendFormat("{0}|{1},", k.Key, k.Value);
                }
                // remove trailing comma
                aggs.Remove(aggs.Length - 1, 1);
            }
            foreach(var i in selectedRows){
                sRows.AppendFormat("{0},",i);
                // remove trailing comma
                sRows.Remove(aggs.Length - 1, 1);
            }
            using (var cmd = new SqlCommand(query, Connection)) {
                cmd.Parameters.Add("@objName", SqlDbType.VarChar).Value = objectName;
                cmd.Parameters.Add("@record_from", SqlDbType.Int).Value = rowFrom;
                cmd.Parameters.Add("@record_to", SqlDbType.Int).Value = rowTo;
                cmd.Parameters.Add("@suffix", SqlDbType.VarChar).Value = whereClause;
                cmd.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
                cmd.Parameters.Add("@searchSuffix", SqlDbType.VarChar).Value = searchClause;
                cmd.Parameters.Add("@aggregateColumns", SqlDbType.VarChar).Value = aggs.ToString();
                cmd.Parameters.Add("@selectedRowsCSV", SqlDbType.VarChar).Value = sRows.ToString();
                cmd.Parameters.Add("@includeSchema", SqlDbType.Bit).Value = includeSchemaData;
                cmd.Parameters.Add("@checksum", SqlDbType.BigInt).Value = checksum;
                cmd.Parameters.Add("@deleteSelection", SqlDbType.Bit).Value = deleteSelection;
                cmd.Parameters.Add("@orderBy", SqlDbType.VarChar).Value = orderBy;
                cmd.Parameters.Add("@orderDirection_override", SqlDbType.VarChar).Value = orderByDirection == OrderDirection.Ascending ? "asc" : "desc";
                using (var r = cmd.ExecuteReader()) {
                    // add range data
                    var range = new Dictionary<string, object> { { "from", rowFrom }, { "to", rowTo } };
                    s.Add("range", range);
                    // first row contains error data
                    r.Read();
                    var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(r.GetString(0));
                    s.Add("header", header);
                    // pull error and description info from SP result
                    s.Error = int.Parse((string)header["error"]);
                    s.Message = (string)header["description"];
                    // second row contains schema data
                    r.Read();
                    var schema = JsonConvert.DeserializeObject<Dictionary<string, object>>(r.GetString(0));
                    s.Add("columns", schema);
                    // the rest are row data
                    while (r.Read()) {
                        var row = JsonConvert.DeserializeObject<List<object>>(r.GetString(0));
                        rows.Add(row);
                    }
                }
            }
            return s;
        }
    }
    /// <summary>
    /// Direction that a SQL result column can be ordered in.  Ascending or Descending.
    /// </summary>
    public enum OrderDirection {
        /// <summary>
        /// Ascending order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Descending order.
        /// </summary>
        Descending
    }
}