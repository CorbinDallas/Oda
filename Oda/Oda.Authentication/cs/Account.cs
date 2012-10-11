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
        public Guid Id;
        /// <summary>
        /// A place to temporarily store data.
        /// This object list is not persistent.
        /// </summary>
        public List<object> Items;
        /// <summary>
        /// Contact information.
        /// </summary>
        public Contact Contact;
        /// <summary>
        /// Contacts associated with this account.
        /// </summary>
        public List<Contact> Contacts = new List<Contact>();
        /// <summary>
        /// Logon.
        /// </summary>
        public string Logon = "";
        /// <summary>
        /// Hashed password.
        /// </summary>
        public string DigestPassword = "";
    }
    /// <summary>
    /// Contact information.
    /// </summary>
    public class Contact : JsonMethods {
        /// <summary>
        /// Gets a resource string from the static plugin reference.
        /// </summary>
        /// <param name="resourceString">The resource string.</param>
        /// <returns></returns>
        private static string GetResString(string resourceString) {
            return SessionInit.
                SessionInitRef.GetResrouceString(resourceString);
        }
        #region JSON Methods
        /// <summary>
        /// Updates the contact.
        /// </summary>
        /// <param name="ContactId">The contact id.</param>
        /// <param name="AccountId">The account id.</param>
        /// <param name="First">The first name.</param>
        /// <param name="Middle">The middle name.</param>
        /// <param name="Last">The last name.</param>
        /// <param name="Address">The address line 1.</param>
        /// <param name="Address2">The address line 2.</param>
        /// <param name="City">The city.</param>
        /// <param name="State">The state.</param>
        /// <param name="Zip">The ZIP.</param>
        /// <param name="Email">The email.</param>
        /// <param name="Company">The company.</param>
        /// <param name="Title">The title.</param>
        /// <param name="WebAddress">The web address.</param>
        /// <param name="IMAddress">The IM address.</param>
        /// <param name="Fax">The fax phone.</param>
        /// <param name="Home">The home phone.</param>
        /// <param name="Work">The work phone.</param>
        /// <param name="Mobile">The mobile phone.</param>
        /// <param name="Notes">The notes.</param>
        /// <param name="Type">The type.</param>
        /// <returns></returns>
        public static JsonResponse UpdateContact(string ContactId, string AccountId, string First, string Middle, 
            string Last, string Address, string Address2, string City, string State, string Zip, 
            string Email, string Company, string Title, string WebAddress, string IMAddress, 
            string Fax, string Home, string Work, string Mobile, string Notes, Int64 Type) {
            JsonResponse j = new JsonResponse();
            //@ContactId, @AccountId, @First, @Middle, @Last, @Address, @Address2, @City, @State, 
            //@Zip, @Email, @Company, @Title, @WebAddress, @IMAddress, @Fax, 
            //@Home, @Work, @Mobile, @Notes, @Type
            string query = GetResString("/Sql/CreateUpdateContact.sql");
            using(SqlCommand cmd = new SqlCommand(query, Sql.Connection)) {
                cmd.Parameters.Add("@ContactId", SqlDbType.UniqueIdentifier).Value = new Guid(ContactId);
                cmd.Parameters.Add("@AccountId", SqlDbType.UniqueIdentifier).Value = new Guid(AccountId);
                cmd.Parameters.Add("@First", SqlDbType.VarChar).Value = First;
                cmd.Parameters.Add("@Middle", SqlDbType.VarChar).Value = Middle;
                cmd.Parameters.Add("@Last", SqlDbType.VarChar).Value = Last;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = Address;
                cmd.Parameters.Add("@Address2", SqlDbType.VarChar).Value = Address2;
                cmd.Parameters.Add("@City", SqlDbType.VarChar).Value = City;
                cmd.Parameters.Add("@State", SqlDbType.VarChar).Value = State;
                cmd.Parameters.Add("@Zip", SqlDbType.VarChar).Value = Zip;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@Company", SqlDbType.VarChar).Value = Company;
                cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;
                cmd.Parameters.Add("@WebAddress", SqlDbType.VarChar).Value = WebAddress;
                cmd.Parameters.Add("@IMAddress", SqlDbType.VarChar).Value = IMAddress;
                cmd.Parameters.Add("@Fax", SqlDbType.VarChar).Value = Fax;
                cmd.Parameters.Add("@Home", SqlDbType.VarChar).Value = Home;
                cmd.Parameters.Add("@Work", SqlDbType.VarChar).Value = Work;
                cmd.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = Mobile;
                cmd.Parameters.Add("@Notes", SqlDbType.VarChar).Value = Notes;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = (int)Type;
                cmd.ExecuteNonQuery();
            }
            return j;
        }
        #endregion
        /// <summary>
        /// The account Id associated with this contact.
        /// </summary>
        public Guid AccountId;
        /// <summary>
        /// The unique Id of this contact.
        /// </summary>
        public Guid Id;
        /// <summary>
        /// First name.
        /// </summary>
        public string First = "";
        /// <summary>
        /// Middle name or initial.
        /// </summary>
        public string Middle = "";
        /// <summary>
        /// Last name.
        /// </summary>
        public string Last = "";
        /// <summary>
        /// Address line 1.
        /// </summary>
        public string Address = "";
        /// <summary>
        /// Address line 2.
        /// </summary>
        public string Address2 = "";
        /// <summary>
        /// City.
        /// </summary>
        public string City = "";
        /// <summary>
        /// State or province.
        /// </summary>
        public string State = "";
        /// <summary>
        /// ZIP code.
        /// </summary>
        public string Zip = "";
        /// <summary>
        /// Primary email address.
        /// </summary>
        public string Email = "";
        /// <summary>
        /// Company name.
        /// </summary>
        public string Company = "";
        /// <summary>
        /// Position title.  E.g.: Programmer, CEO, CFO.
        /// </summary>
        public string Title = "";
        /// <summary>
        /// Web address.
        /// </summary>
        public string WebAddress = "";
        /// <summary>
        /// Instant messenger address.
        /// </summary>
        public string IMAddress = "";
        /// <summary>
        /// Fax phone number.
        /// </summary>
        public string Fax = "";
        /// <summary>
        /// Home phone number.
        /// </summary>
        public string Home = "";
        /// <summary>
        /// Work phone number.
        /// </summary>
        public string Work = "";
        /// <summary>
        /// Mobile phone number.
        /// </summary>
        public string Mobile = "";
        /// <summary>
        /// Notes.
        /// </summary>
        public string Notes = "";
        /// <summary>
        /// Type of contact.
        /// </summary>
        public ContactType Type = ContactType.Primary;
        /// <summary>
        /// Updates this contact writing the contact to the database.
        /// </summary>
        /// <returns>The contact being updated</returns>
        public Contact Update(SqlConnection cn, SqlTransaction trans) {
            UpdateContact(this.Id.ToString(), this.AccountId.ToString(), this.First, this.Middle, this.Last, this.Address, this.Address2,
                this.City, this.State, this.Zip, this.Email, this.Company, this.Title, this.WebAddress, this.IMAddress,
                this.Fax, this.Home, this.Work, this.Mobile, this.Notes, (int)this.Type);
            return this;
        }
    }
    #endregion
}
