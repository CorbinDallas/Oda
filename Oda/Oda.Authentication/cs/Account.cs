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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
namespace Oda {
    #region Account Classes and Enums
    /// <summary>
    /// Type of contact, work, billing, shipping, primary, secondary etc.
    /// </summary>
    public enum ContactType {
        /// <summary>
        /// Billing contact.
        /// </summary>
        [DescriptionAttribute("Billing contact.")]
        Billing = -4,
        /// <summary>
        /// Shipping contact.
        /// </summary>
        [DescriptionAttribute("Shipping contact.")]
        Shipping = -3,
        /// <summary>
        /// Home contact.
        /// </summary>
        [DescriptionAttribute("Home contact.")]
        Home = -2,
        /// <summary>
        /// Work contact.
        /// </summary>
        [DescriptionAttribute("Shipping contact.")]
        Work = -1,
        /// <summary>
        /// Primary account contact.
        /// </summary>
        [DescriptionAttribute("Primary account contact.")]
        Primary = 0,
        /// <summary>
        /// Secondary account contact.
        /// </summary>
        [DescriptionAttribute("Secondary account contact.")]
        Secondary = 1,
        /// <summary>
        /// Tertiary account contact.
        /// </summary>
        [DescriptionAttribute("Tertiary account contact.")]
        Tertiary = 2,
        /// <summary>
        /// Quaternary account contact.
        /// </summary>
        [DescriptionAttribute("Quaternary account contact.")]
        Quaternary = 3,
        /// <summary>
        /// Quinary account contact.
        /// </summary>
        [DescriptionAttribute("Quinary account contact.")]
        Quinary = 4,
        /// <summary>
        /// Senary account contact.
        /// </summary>
        [DescriptionAttribute("Senary account contact.")]
        Senary = 5,
        /// <summary>
        /// Septenary account contact.
        /// </summary>
        [DescriptionAttribute("Septenary account contact.")]
        Septenary = 6,
        /// <summary>
        /// Octonary account contact.
        /// </summary>
        [DescriptionAttribute("Octonary account contact.")]
        Octonary = 7,
        /// <summary>
        /// Nonary account contact.
        /// </summary>
        [DescriptionAttribute("Nonary account contact.")]
        Nonary = 8,
        /// <summary>
        /// Denary account contact.
        /// </summary>
        [DescriptionAttribute("Denary account contact.")]
        Denary = 9
    }
    /// <summary>
    /// Account base information.
    /// </summary>
    public class Account {
        /// <summary>
        /// Unique id of this account.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A place to temporarily store data.
        /// This object list is not persistent.
        /// </summary>
        public List<object> Items { get; set; }
        /// <summary>
        /// Contact information.
        /// </summary>
        public Contact Contact { get; set; }
        /// <summary>
        /// Contacts associated with this account.
        /// </summary>
        public List<Contact> Contacts { get; set; }
        /// <summary>
        /// Logon.
        /// </summary>
        public string Logon { get; set; }
        /// <summary>
        /// Hashed password.
        /// </summary>
        public string DigestPassword { get; set; }
    }
    /// <summary>
    /// Contact information.
    /// </summary>
    public class Contact : JsonMethods {
        #region Private backing fields
        private Guid _accountId;
        private Guid _id;
        private string _first;
        private string _middle;
        private string _last;
        private string _address;
        private string _address2;
        private string _city;
        private string _state;
        private string _zip;
        private string _email;
        private string _company;
        private string _title;
        private string _webAddress;
        private string _imAddress;
        private string _fax;
        private string _home;
        private string _work;
        private ContactType _type;
        private string _notes;
        private string _mobile;
        #endregion
        /// <summary>
        /// Gets a resource string from the static plugin reference.
        /// </summary>
        /// <param name="resourceString">The resource string.</param>
        /// <returns></returns>
        private static string GetResString(string resourceString) {
            return SessionInit.
                SessionInitRef.GetResourceString(resourceString);
        }
        #region JSON Methods
        /// <summary>
        /// Updates the contact.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="first">The first.</param>
        /// <param name="middle">The middle.</param>
        /// <param name="last">The last.</param>
        /// <param name="address">The address.</param>
        /// <param name="address2">The address2.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="zip">The zip.</param>
        /// <param name="email">The email.</param>
        /// <param name="company">The company.</param>
        /// <param name="title">The title.</param>
        /// <param name="webAddress">The web address.</param>
        /// <param name="imAddress">The instant messenger address.</param>
        /// <param name="fax">The fax.</param>
        /// <param name="home">The home.</param>
        /// <param name="work">The work.</param>
        /// <param name="mobile">The mobile.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static JsonResponse UpdateContact(string contactId, string accountId, string first, string middle, 
            string last, string address, string address2, string city, string state, string zip, 
            string email, string company, string title, string webAddress, string imAddress, 
            string fax, string home, string work, string mobile, string notes, Int64 type) {
            var j = new JsonResponse();
            //@ContactId, @AccountId, @First, @Middle, @Last, @Address, @Address2, @City, @State, 
            //@Zip, @Email, @Company, @Title, @WebAddress, @IMAddress, @Fax, 
            //@Home, @Work, @Mobile, @Notes, @Type
            var query = GetResString("/Sql/CreateUpdateContact.sql");
            using(var cmd = new SqlCommand(query, Sql.Connection)) {
                cmd.Parameters.Add("@ContactId", SqlDbType.UniqueIdentifier).Value = new Guid(contactId);
                cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = new Guid(accountId);
                cmd.Parameters.Add("@First", SqlDbType.VarChar).Value = first;
                cmd.Parameters.Add("@Middle", SqlDbType.VarChar).Value = middle;
                cmd.Parameters.Add("@Last", SqlDbType.VarChar).Value = last;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = address;
                cmd.Parameters.Add("@Address2", SqlDbType.VarChar).Value = address2;
                cmd.Parameters.Add("@City", SqlDbType.VarChar).Value = city;
                cmd.Parameters.Add("@State", SqlDbType.VarChar).Value = state;
                cmd.Parameters.Add("@Zip", SqlDbType.VarChar).Value = zip;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                cmd.Parameters.Add("@Company", SqlDbType.VarChar).Value = company;
                cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = title;
                cmd.Parameters.Add("@WebAddress", SqlDbType.VarChar).Value = webAddress;
                cmd.Parameters.Add("@IMAddress", SqlDbType.VarChar).Value = imAddress;
                cmd.Parameters.Add("@Fax", SqlDbType.VarChar).Value = fax;
                cmd.Parameters.Add("@Home", SqlDbType.VarChar).Value = home;
                cmd.Parameters.Add("@Work", SqlDbType.VarChar).Value = work;
                cmd.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = mobile;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = notes;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = (int)type;
                cmd.ExecuteNonQuery();
            }
            return j;
        }
        #endregion

        /// <summary>
        /// The account Id associated with this contact.
        /// </summary>
        public Guid AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        /// <summary>
        /// The unique Id of this contact.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// First name.
        /// </summary>
        public string First
        {
            get { return _first; }
            set { _first = value; }
        }

        /// <summary>
        /// Middle name or initial.
        /// </summary>
        public string Middle
        {
            get { return _middle; }
            set { _middle = value; }
        }

        /// <summary>
        /// Last name.
        /// </summary>
        public string Last
        {
            get { return _last; }
            set { _last = value; }
        }

        /// <summary>
        /// Address line 1.
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Address line 2.
        /// </summary>
        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        /// <summary>
        /// City.
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// State or province.
        /// </summary>
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// ZIP code.
        /// </summary>
        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        /// <summary>
        /// Primary email address.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Company name.
        /// </summary>
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        /// <summary>
        /// Position title.  E.g.: Programmer, CEO, CFO.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Web address.
        /// </summary>
        public string WebAddress
        {
            get { return _webAddress; }
            set { _webAddress = value; }
        }

        /// <summary>
        /// Instant messenger address.
        /// </summary>
        public string IMAddress
        {
            get { return _imAddress; }
            set { _imAddress = value; }
        }

        /// <summary>
        /// Fax phone number.
        /// </summary>
        public string Fax
        {
            get { return _fax; }
            set { _fax = value; }
        }

        /// <summary>
        /// Home phone number.
        /// </summary>
        public string Home
        {
            get { return _home; }
            set { _home = value; }
        }

        /// <summary>
        /// Work phone number.
        /// </summary>
        public string Work
        {
            get { return _work; }
            set { _work = value; }
        }

        /// <summary>
        /// Mobile phone number.
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        /// <summary>
        /// Notes.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        /// <summary>
        /// Type of contact.
        /// </summary>
        public ContactType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Contact" /> class.
        /// </summary>
        public Contact() {
            _accountId = Guid.Empty;
            _id = Guid.Empty;
            _first = "";
            _middle = "";
            _last = "";
            _address = "";
            _address2 = "";
            _city = "";
            _state = "";
            _zip = "";
            _email = "";
            _company = "";
            _title = "";
            _webAddress = "";
            _imAddress = "";
            _fax = "";
            _home = "";
            _work = "";
            _type = ContactType.Primary;
            _notes = "";
            _mobile = "";
        }
        /// <summary>
        /// Updates this contact writing the contact to the database.
        /// </summary>
        /// <returns>The contact being updated</returns>
        public Contact Update(SqlConnection cn, SqlTransaction trans) {
            UpdateContact(Id.ToString(), AccountId.ToString(), First, Middle, Last, Address, Address2,
                City, State, Zip, Email, Company, Title, WebAddress, IMAddress,
                Fax, Home, Work, Mobile, Notes, (int)Type);
            return this;
        }
    }
    #endregion
}
