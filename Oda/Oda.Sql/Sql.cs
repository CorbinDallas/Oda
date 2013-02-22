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
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
namespace Oda {
    /// <summary>
    /// This plugin allows you to attach to a T-SQL database and preserve an open connection.
    /// It also contains CRUD utilities (Create, Read, Update, Delete) for JSON to SQL and SQL to JSON.
    /// </summary>
    public class Sql : Plugin {
        /// <summary>
        /// Gets the connection state.
        /// </summary>
        /// <seealso cref="UseStaticConnection"/>
        /// <seealso cref="ReconnectStaticSqlConnection"/>
        /// <seealso cref="Connection"/>
        /// <seealso cref="Name"/>
        public static ConnectionState State {
            get {
                return Connection.State;
            }
        }
        /// <summary>
        /// This is set by the web.config application setting UseStaticSqlConnection.
        /// When true (default) a static connection will be maintained.
        /// </summary>
        /// <seealso cref="ReconnectStaticSqlConnection"/>
        /// <seealso cref="Connection"/>
        /// <seealso cref="Name"/>
        /// <seealso cref="State"/>
        public bool UseStaticConnection { get; internal set; }
        /// <summary>
        /// This is set by the web.config application setting ReconnectStaticSqlConnection.
        /// When true (default) and static a connection is on, the connection will
        /// automatically reconnect when it is closed or broken.
        /// </summary>
        /// <seealso cref="UseStaticConnection"/>
        /// <seealso cref="Connection"/>
        /// <seealso cref="Name"/>
        /// <seealso cref="State"/>
        public bool ReconnectStaticSqlConnection { get; internal set; }
        /// <summary>
        /// Gets the name of the connection specified in the web.config of the host site.
        /// If no name is specified then the name "sql" will be used.
        /// </summary>
        /// <seealso cref="UseStaticConnection"/>
        /// <seealso cref="ReconnectStaticSqlConnection"/>
        /// <seealso cref="Connection"/>
        /// <seealso cref="State"/>
        public static string Name { get; private set; }
        /// <summary>
        /// The persistent Sql connection.
        /// </summary>
        /// <seealso cref="UseStaticConnection"/>
        /// <seealso cref="ReconnectStaticSqlConnection"/>
        /// <seealso cref="Name"/>
        /// <seealso cref="State"/>
        public static SqlConnection Connection;
        /// <summary>
        /// Gets settings from the Web.Config file.
        /// </summary>
        private void GetWebConfigSettings() {
            ReconnectStaticSqlConnection = Core.GetWebConfigSetting("ReconnectStaticSqlConnection", true);
            UseStaticConnection = Core.GetWebConfigSetting("UseStaticSqlConnection", true);
            Name = Core.GetWebConfigSetting("connectionName", "sql");
        }
        private static string _jsonReadOrDeleteQuery;
        private static string _createOrUpdateQuery;
        /// <summary>
        /// Initializes a new instance of the <see cref="Sql"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public Sql() {
            GetWebConfigSettings();
            Connection = CreateConnection();
            // make sure the connection stays open
            if (ReconnectStaticSqlConnection) { Connection.StateChange += ConnectionStateChange; }
            Open();
            // check that required objects for CRUD functions exist, if they don't, create them.
            var cmds = new[] { 
                "/Sql/CreateUIColumns.sql", 
                "/Sql/CreateRowUpdateTableType.sql", 
                "/Sql/CreateParameterTypeToDeclarationFunction.sql",
                "/Sql/CreateSplitFunction.sql", 
                "/Sql/CreateRowUpdateTableToUpdateStringFunction.sql", 
                "/Sql/CreateParameterListTableType.sql",
                "/Sql/CreateJsonReadOrDelete.sql",  
                "/Sql/CreateCreateDeleteOrUpdate.sql"
            };
            foreach(var q in cmds) {
                using (var cmd = new SqlCommand(GetResourceString(q), Connection)) {
                    cmd.ExecuteNonQuery();
                }
            }
            // assign exec SPs to static query strings
            _jsonReadOrDeleteQuery = GetResourceString("/Sql/ExecJsonReadOrDelete.sql");
            _createOrUpdateQuery = GetResourceString("/Sql/ExecCreateDeleteOrUpdate.sql");
            // if the user doesn't want to use the static connection then close the connection.
            if (!UseStaticConnection) { Close(); }
        }
        static void ConnectionStateChange(object sender, StateChangeEventArgs e)
        {
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
        /// Creates a new Sql connection using the default connection string defined in the web.config
        /// of the current web site.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection CreateConnection() {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[Name].ConnectionString);
        }
        /// <summary>
        /// Creates a new Sql connection using a named connection string defined in the web.config
        /// of the current site.
        /// </summary>
        /// <param name="connectionName">The name of the connection string.</param>
        /// <returns></returns>
        public static SqlConnection CreateConnection(string connectionName) {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
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
        /// Create and Update functions for database interaction. 
        /// A column called RowVersion of the type rowversion or timestamp must be part of the table being edited for this method to work correctly.
		/// </summary>
		/// <param name="objectName">Name of the object.</param>
		/// <param name="data">Data should be provided as a JSON list of object literals in this format 
		/// <code>
		/// [{
		///     "name" : &lt;ColumnName&gt;,
		///     "value" : &lt;ColumnValue&gt;,
		///     "primaryKey" : &lt;PrimaryKeyValue&gt;,
		///     "dataType" : &lt;type of SQL data&gt;,
		///     "dataLength" : &lt;length of data if any&gt;
        /// }, &lt;Addtional members&gt; ...]
        /// </code>
		/// </param>
		/// <param name="overwrite">if set to <c>true</c> the row will be written even if the rowversion differs from that found in the database.</param>
		/// <param name="commandType">Type of the command.  0 = Update, 1 = Insert, 2 = Delete</param>
		/// <returns>JSON Data related to status of row insert, update or delete.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static Dictionary<string, object> CreateUpdateOrDelete( string objectName, List<object> data, bool overwrite, Int64 commandType ) {
			var j = new Dictionary<string, object>();
			var rowData = new List<SqlDataRecord>();
			SqlMetaData[] rowUpdateTable = { 
				new SqlMetaData("KeyName",SqlDbType.VarChar,100),
				new SqlMetaData("KeyValue",SqlDbType.Variant),
				new SqlMetaData("Primary_key",SqlDbType.Bit),
				new SqlMetaData("DataType",SqlDbType.VarChar,50),
				new SqlMetaData("DataLength",SqlDbType.Int),
				new SqlMetaData("VarCharMaxValue",SqlDbType.VarChar,-1)
			};
			foreach( var field in data ) {
				var inner = ( Dictionary<string, object> )field;
                var rec = new SqlDataRecord(rowUpdateTable);
				var varCharMaxValue = "";
				foreach(var innerField in inner ) {
					switch (innerField.Key)
					{
					    case "dataType":
					        rec.SetString( 3, innerField.Value.ToString() );
					        break;
					    case "primaryKey":
					        rec.SetBoolean( 2, Convert.ToBoolean( innerField.Value ) );
					        break;
					    case "name":
					        rec.SetString( 0, innerField.Value.ToString() );
					        break;
					    case "value":
					        varCharMaxValue = innerField.Value.ToString();
					        rec.SetValue( 1, innerField.Value );
					        break;
					    case "dataLength":
					        rec.SetValue( 4, Convert.ToInt32( innerField.Value ) );
					        break;
					}
				}
				rec.SetValue( 5, Convert.ToString( varCharMaxValue ) );
				rowData.Add( rec );
			}
            using(var cn = CreateConnection()) {
                cn.Open();
                using(var cmd = cn.CreateCommand()) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = _createOrUpdateQuery;
                    cmd.Parameters.Add("@objectName", SqlDbType.VarChar, 50);
                    cmd.Parameters["@objectName"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@objectName"].Value = objectName;

                    cmd.Parameters.Add("@row", SqlDbType.Structured);
                    cmd.Parameters["@row"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@row"].Value = rowData;

                    cmd.Parameters.Add("@overwrite", SqlDbType.Bit);
                    cmd.Parameters["@overwrite"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@overwrite"].Value = overwrite;

                    cmd.Parameters.Add("@commandType", SqlDbType.Int);
                    cmd.Parameters["@commandType"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@commandType"].Value = Convert.ToInt32(commandType);/* 0 = Update, 1 = insert, 2 = delete*/
                    using(var u = cmd.ExecuteReader()) {
                        u.Read();
                        j.Add("error", u.GetInt32(0));
                        j.Add("description", u.GetString(1));
                        j.Add("primaryKey", u.GetValue(2).ToString());
                        if(commandType == 0) {
                            if(u.GetInt32(0) == 0) {
                                j.Add("RowVersion", u.GetValue(3).ToString());
                            }
                        } else {
                            j.Add("RowVersion", -1);
                        }
                        j.Add("commandType", commandType.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
			return j;
		}
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="query">The query to execute.  This will be turned into a vew that is removed when the HttpApplication is disposed.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="orderBy">The column order by.</param>
        /// <param name="orderByDirection">The order by direction.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        public static JsonResponse JsonReadOrDelete(string query, int rowFrom, int rowTo, string orderBy, OrderDirection orderByDirection) {
            return JsonReadOrDelete(query, rowFrom, rowTo, Guid.Empty, null , orderBy, orderByDirection, null);
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="query">The query to execute.  This will be turned into a vew that is removed when the HttpApplication is disposed.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="parameters">The parameters collection to use in the query.</param>
        /// <param name="orderBy">The column order by.</param>
        /// <param name="orderByDirection">The order by direction.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        public static JsonResponse JsonReadOrDelete(string query, int rowFrom, int rowTo, SqlParameter[] parameters, string orderBy, OrderDirection orderByDirection) {
            return JsonReadOrDelete(query, rowFrom, rowTo, Guid.Empty, parameters, orderBy, orderByDirection, null);
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="query">The query to execute.  This will be turned into a vew that is removed when the HttpApplication is disposed.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="parameters">The parameters collection to use in the query.</param>
        /// <param name="orderBy">The column order by.</param>
        /// <param name="orderByDirection">The order by direction.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        public static JsonResponse JsonReadOrDelete(string query, int rowFrom, int rowTo, Guid accountId, SqlParameter[] parameters, string orderBy, OrderDirection orderByDirection) {
            return JsonReadOrDelete(query, rowFrom, rowTo, accountId, parameters, orderBy, orderByDirection, null);
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="query">The query to execute.  This will be turned into a vew that is removed when the HttpApplication is disposed.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="orderBy">The column to order by.</param>
        /// <param name="orderByDirection">The order by direction.</param>
        /// <param name="connection">The SQL connection to use.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static JsonResponse JsonReadOrDelete(string query, int rowFrom, int rowTo, Guid accountId, SqlParameter[] parameters, string orderBy, 
            OrderDirection orderByDirection, SqlConnection connection) {
            //create view hash name
            var hashViewName = string.Format("temp_{0}",Core.Hash(query).Replace("/","").Replace("+","").Replace("=",""));
            var cn = connection ?? CreateConnection();
            if(cn.State != ConnectionState.Open) { cn.Open(); }
            var cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format(@"if not exists(select 0 from sysobjects where name = '{0}' and type = 'V') begin
	            declare @statement nvarchar(max) = 'create view {0} as {1}'
                exec sp_executesql @statement
            end", hashViewName, query.Replace("'","''"));
            var c = new SqlWhere { Parmeters = parameters };
            cmd.ExecuteNonQuery();
            Core.DisposeHttpApplication += delegate {
                using (var icn = CreateConnection()) {
                    icn.Open();
                    using (var icmd = icn.CreateCommand()) {
                        icmd.CommandType = CommandType.Text;
                        icmd.CommandText = string.Format(@"if exists(select 0 from sysobjects where name = '{0}' and type = 'V') begin
                            drop view {0}
                            end", hashViewName);
                        icmd.ExecuteNonQuery();
                    }
                }
            };
            var j = JsonReadOrDelete(hashViewName, rowFrom, rowTo, c, accountId, null, null, null, true, -1, false, orderBy, orderByDirection, cn);
            cmd.Dispose();
            cn.Dispose();
            return j;
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="objectName">Name of the table, view or object.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="where">The where clause object.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        public static JsonResponse JsonReadOrDelete(string objectName, int rowFrom, int rowTo, SqlWhere where) {
            return JsonReadOrDelete(objectName, rowFrom, rowTo, where, Guid.Empty, null, null, null, true, -1, false, string.Empty, OrderDirection.Descending, null);
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// </summary>
        /// <param name="objectName">Name of the table, view or object.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <returns>JSON schema data and row data and status or delete status.</returns>
        public static JsonResponse JsonReadOrDelete(string objectName, int rowFrom, int rowTo) {
            return JsonReadOrDelete(objectName, rowFrom, rowTo, null, Guid.Empty, null, null, null, true, -1, false, string.Empty, OrderDirection.Descending, null);
        }
                /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// Note: Delete requires a [RowVersion] rowversion column present in the table.
        /// State data is stored in the table UIColumns on a per user basis.  Users are defined in Oda.Authentication class
        /// and in the table Accounts.
        /// </summary>
        /// <param name="objectName">Name of the table, view or object.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="whereClause">The where clause object.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="searchClause">The search clause object.</param>
        /// <param name="aggregates">The aggregates to return if any.</param>
        /// <param name="selectedRows">The selected rows.  For use with the delete bit.</param>
        /// <param name="includeSchemaData">If set to <c>true</c> then schema data will be included.</param>
        /// <param name="checksum">The checksum of the table.  This is the some of the rowversion column.  For use with the delete bit.</param>
        /// <param name="deleteSelection">if set to <c>true</c> then the rows defined in the selectedRows parameter will be deleted if the table checksome matches.</param>
        /// <param name="orderBy">The column order by.</param>
        /// <param name="orderByDirection">The direction to order by.</param>
        /// <returns>
        /// JSON schema data and row data and status or delete status.
        /// </returns>
        public static JsonResponse JsonReadOrDelete(string objectName, int rowFrom, int rowTo, SqlWhere whereClause, Guid accountId, SqlWhere searchClause,
            IDictionary<string, string> aggregates, ICollection<int> selectedRows, bool includeSchemaData, Int64 checksum, bool deleteSelection,
            string orderBy, OrderDirection orderByDirection) {
            return JsonReadOrDelete(objectName, rowFrom, rowTo, whereClause, accountId, searchClause,
            aggregates, selectedRows, includeSchemaData, checksum, deleteSelection,
            orderBy, orderByDirection, null);
        }
        /// <summary>
        /// Read from SQL objects and return as JSON.  Uses the JsonReadOrDelete stored procedure to return limited
        /// record sets to the Sql client.  Great for streaming pages of data to a stateless web client.
        /// Note: Delete requires a [RowVersion] rowversion column present in the table.
        /// State data is stored in the table UIColumns on a per user basis.  Users are defined in Oda.Authentication class
        /// and in the table Accounts.
        /// </summary>
        /// <param name="objectName">Name of the table, view or object.</param>
        /// <param name="rowFrom">The row to fetch from.</param>
        /// <param name="rowTo">The row to fetch to.</param>
        /// <param name="whereClause">The where clause object.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="searchClause">The search clause object.</param>
        /// <param name="aggregates">The aggregates to return if any.</param>
        /// <param name="selectedRows">The selected rows.  For use with the delete bit.</param>
        /// <param name="includeSchemaData">If set to <c>true</c> then schema data will be included.</param>
        /// <param name="checksum">The checksum of the table.  This is the some of the rowversion column.  For use with the delete bit.</param>
        /// <param name="deleteSelection">if set to <c>true</c> then the rows defined in the selectedRows parameter will be deleted if the table checksome matches.</param>
        /// <param name="orderBy">The column order by.</param>
        /// <param name="orderByDirection">The direction to order by.</param>
        /// <param name="connection">The SQL connection to use.</param>
        /// <returns>
        /// JSON schema data and row data and status or delete status.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static JsonResponse JsonReadOrDelete(string objectName, int rowFrom, int rowTo, SqlWhere whereClause, Guid accountId, SqlWhere searchClause,
                                            IDictionary<string, string> aggregates, ICollection<int> selectedRows, bool includeSchemaData, Int64 checksum, bool deleteSelection,
                                            string orderBy, OrderDirection orderByDirection, SqlConnection connection) {
            var s = new JsonResponse();
            var aggs = new StringBuilder();
            var sRows = new StringBuilder();
            s.MethodName = "JsonReadOrDelete";
            var rows = new List<object>();
            s.Add("rows", rows);
            // convert aggregate column dictionary to a string
            aggregates = aggregates ?? new Dictionary<string, string>();
            if(aggregates.Count>0){
                foreach(var k in aggregates){
                    aggs.AppendFormat("{0}|{1},", k.Key, k.Value);
                }
                // remove trailing comma
                aggs.Remove(aggs.Length - 1, 1);
            }
            selectedRows = selectedRows ?? new Collection<int>();
            foreach(var i in selectedRows){
                sRows.AppendFormat("{0},",i);
                // remove trailing comma
                sRows.Remove(aggs.Length - 1, 1);
            }
            using (var cn = connection ?? CreateConnection()) {
                if (cn.State != ConnectionState.Open) { cn.Open(); }
                using (var cmd  = cn.CreateCommand()) {
                    cmd.CommandText = _jsonReadOrDeleteQuery;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlMetaData[] prameterList = { 
				        new SqlMetaData("Name",SqlDbType.VarChar,100),
                        new SqlMetaData("Type",SqlDbType.VarChar,100),
                        new SqlMetaData("Length",SqlDbType.VarChar,10),
                        new SqlMetaData("Value",SqlDbType.Variant)
			        };
                    var whereParameterList = new List<SqlDataRecord>();
                    var searchParameterList = new List<SqlDataRecord>();
                    whereClause = whereClause ?? new SqlWhere();
                    searchClause = searchClause ?? new SqlWhere();
                    foreach(var p in whereClause.Parmeters){
                        var whereParameter = new SqlDataRecord(prameterList);
                        whereParameter.SetString(0, p.Name);
                        whereParameter.SetString(1, p.SqlDbType.ToString());
                        whereParameter.SetValue(2, p.Length);
                        whereParameter.SetValue(3, p.Value);
                        whereParameterList.Add(whereParameter);
                    }
                    foreach (var p in searchClause.Parmeters) {
                        var searchParameter = new SqlDataRecord(prameterList);
                        searchParameter.SetString(0, p.Name);
                        searchParameter.SetString(1, p.SqlDbType.ToString());
                        searchParameter.SetValue(2, p.Length);
                        searchParameter.SetValue(3, p.Value);
                        searchParameterList.Add(searchParameter);
                    }
                    cmd.Parameters.Add("@objName", SqlDbType.VarChar).Value = objectName;
                    cmd.Parameters.Add("@record_from", SqlDbType.Int).Value = rowFrom;
                    cmd.Parameters.Add("@record_to", SqlDbType.Int).Value = rowTo;
                    cmd.Parameters.Add("@suffix", SqlDbType.VarChar).Value = whereClause.WhereClause;
                    cmd.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
                    cmd.Parameters.Add("@searchSuffix", SqlDbType.VarChar).Value = searchClause.WhereClause;
                    cmd.Parameters.Add("@aggregateColumns", SqlDbType.VarChar).Value = aggs.ToString();
                    cmd.Parameters.Add("@selectedRowsCSV", SqlDbType.VarChar).Value = sRows.ToString();
                    cmd.Parameters.Add("@includeSchema", SqlDbType.Bit).Value = includeSchemaData;
                    cmd.Parameters.Add("@checksum", SqlDbType.BigInt).Value = checksum;
                    cmd.Parameters.Add("@delete", SqlDbType.Bit).Value = deleteSelection;
                    cmd.Parameters.Add("@orderBy_override", SqlDbType.VarChar).Value = orderBy;
                    cmd.Parameters.Add("@orderDirection_override", SqlDbType.VarChar).Value = orderByDirection == OrderDirection.Ascending ? "asc" : "desc";
                    cmd.Parameters.Add("@whereParameterList", SqlDbType.Structured);
                    cmd.Parameters["@whereParameterList"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@whereParameterList"].Value = (whereParameterList.Count == 0 ? null : whereParameterList);
                    cmd.Parameters.Add("@searchParameterList", SqlDbType.Structured);
                    cmd.Parameters["@searchParameterList"].Direction = ParameterDirection.Input;
                    cmd.Parameters["@searchParameterList"].Value = (searchParameterList.Count == 0 ? null : searchParameterList);
                    using (var r = cmd.ExecuteReader()) {
                        if(searchClause.WhereClause.Length == 0){
                            // add range data
                            var range = new Dictionary<string, object> { { "from", rowFrom }, { "to", rowTo } };
                            s.Add("range", range);
                            // first row contains error data
                            r.Read();
                            var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(r.GetString(0));
                            s.Add("header", header);
                            // pull error and description info from SP result
                            s.Error = Convert.ToInt32((Int64)header["error"]);
                            s.Message = (string)header["description"];
                            // second row contains schema data
                            r.Read();
                            var schema = JsonConvert.DeserializeObject<List<object>>(r.GetString(0));
                            s.Add("columns", schema);
                            // the rest are row data
                            while (r.Read()) {
                                var row = JsonConvert.DeserializeObject<List<object>>(r.GetString(0));
                                rows.Add(row);
                            }
                        }else {
                            if(r.HasRows){
                                s["rows"] = JsonConvert.DeserializeObject<List<int>>(r.GetString(0));
                            }else {
                                s["rows"] = new List<int>();
                            }
                        }
                    }
                }
            }
            return s;
        }
    }
}