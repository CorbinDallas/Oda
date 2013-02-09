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
