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
namespace Oda {
    /// <summary>
    /// A parameter for use with <see cref="SqlWhere"/>
    /// </summary>
    /// <seealso cref="SqlWhere"/>
    /// <seealso cref="Sql.JsonReadOrDelete(string,int,int,Oda.SqlWhere)"/>
    public class SqlParameter {
        /// <summary>
        /// Gets or sets the name of the parameter. You must include the @ symbol.
        /// </summary>
        /// <value>
        /// The parameter name.  E.g.: @UserAgent
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>
        /// The parameter value.  Can be any valid SQL data type.
        /// </value>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the SQL database type.
        /// </summary>
        /// <value>
        /// The SQL database type of this parameter.
        /// </value>
        public SqlDbType SqlDbType { get; set; }
        /// <summary>
        /// Gets or sets the length.  Only required on parameter types that require length, such as VarChar and Decimal.
        /// </summary>
        /// <value>
        /// The length of the SQL database type if required by the type.
        /// </value>
        public string Length { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameter"/> class.
        /// </summary>
        /// <param name="name">The parameter name.  E.g.: @UserAgent.</param>
        /// <param name="value">The parameter value.  Can be any valid SQL data type.</param>
        /// <param name="type">The SQL database type of this parameter.</param>
        /// <param name="length">The length of the SQL database type if required by the type.  Enter empy string or null if not required.</param>
        public SqlParameter(string name, object value, SqlDbType type, string length) {
            Name = name;
            Value = value;
            Length = length;
            SqlDbType = type;
        }
    }
}
