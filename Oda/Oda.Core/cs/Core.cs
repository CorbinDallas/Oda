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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Reflection;
using System.ComponentModel;
namespace Oda {
    #region Enum
    /// <summary>
    /// The state that Oda.Core is in.
    /// </summary>
    public enum StartupState {
        /// <summary>
        /// Oda has yet to begin starting.
        /// </summary>
        [DescriptionAttribute("Oda has yet to begin starting.")]
        NotYetStarted,
        /// <summary>
        /// Oda is starting up.
        /// </summary>
        [DescriptionAttribute("Oda is starting up.")]
        Starting,
        /// <summary>
        /// Oda is finished starting up.
        /// </summary>
        [DescriptionAttribute("Oda is finished starting up.")]
        Started
    }
    #endregion
    #region Core
    /// <summary>
    /// The core of Oda.
    /// </summary>
    public partial class Core {
        #region Module Name For Sandcastle
        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public static string ModuleName {
            get {
                return "Oda";
            }
        }
        #endregion
        /// <summary>
        /// Logging utility.
        /// </summary>
        public static Log Log { get; internal set; }
        /// <summary>
        /// Gets or sets the log verbosity.  0 = less detail 10 = more detail.  Default is 5. Max value is Int32.MaxValue.
        /// This is set via the Web.Config application setting LogVerbosity.
        /// </summary>
        /// <seealso cref="Oda.Log"/>
        /// <seealso cref="LogTimestamp"/>
        /// <seealso cref="LogPath"/>
        public static int LogVerbosity { get; internal set; }
        /// <summary>
        /// When <c>true</c> then a timestamp will be included in the log output.
        /// This is set via the Web.Config application setting LogTimestamp.
        /// </summary>
        /// <seealso cref="Oda.Log"/>
        /// <seealso cref="LogVerbosity"/>
        /// <seealso cref="LogPath"/>
        public static bool LogTimestamp { get; internal set; }
        /// <summary>
        /// The path the log will write to.
        /// This is set via the Web.Config application setting LogPath.
        /// </summary>
        /// <seealso cref="Oda.Log"/>
        /// <seealso cref="LogVerbosity"/>
        /// <seealso cref="LogTimestamp"/>
        public static string LogPath { get; internal set; }
        /// <summary>
        /// Path to the executing assembly file.
        /// </summary>
        public static string DynamicDirectory { get; internal set; }
        /// <summary>
        /// Physical path to root of the site that is running Oda.Core.
        /// </summary>
        public static string BaseDirectory { get; internal set; }
        /// <summary>
        /// Physical path to the directory that contains the Oda.Core.dll file and plugins.
        /// </summary>
        public static string RelativeSearchPath { get; internal set; }
        /// <summary>
        /// Gets version information about Oda Core.
        /// </summary>
        public static string Version { get; internal set; }
        /// <summary>
        /// Gets the state of Oda Core.
        /// </summary>
        /// <value>
        /// The state of Oda Core.
        /// </value>
        public static StartupState StartupState { get; internal set; }
        /// <summary>
        /// Gets or sets the Http cookie mode.
        /// </summary>
        /// <value>
        /// The Http cookie mode.
        /// </value>
        public static HttpCookieMode HttpCookieMode { get; set; }
        /// <summary>
        /// Gets the Http application.
        /// </summary>
        public static HttpApplication HttpApplication { get; internal set; }
        /// <summary>
        /// Internal field for Items.
        /// </summary>
        private static List<object> _items;
        /// <summary>
        /// A box to put your things in.
        /// </summary>
        public static List<object> Items {
            get {
                return _items;
            }
        }
        /// <summary>
        /// The private field for the JsonMethodUrl property.
        /// </summary>
        private static string _jsonMethodUrl;
        /// <summary>
        /// Gets or sets the Json method Url.
        /// </summary>
        /// <value>
        /// The Json method URL.
        /// </value>
        public static string JsonMethodUrl {
            get { return _jsonMethodUrl ?? (_jsonMethodUrl = "/responder"); }
            set {
                _jsonMethodUrl = value;
            }
        }
        /// <summary>
        /// Hashes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static string Hash(string message){
            var md5 = new MD5CryptoServiceProvider();
            var digest = md5.ComputeHash(Encoding.UTF8.GetBytes(message));
            var base64Digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64Digest.Substring(0, base64Digest.Length - 2);
        }
        /// <summary>
        /// Resolves the embedded assemblies.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        private static Assembly ResolveEmbeddedAssembiles(object sender, EventArgs args) {
            var a = (ResolveEventArgs)args;
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (
                var name in
                from name in names 
                let cleanName = name.Replace("Oda.lib.", "").Replace(".dll", "") 
                where a.Name.Contains(cleanName) select name)
            {
                using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) {
                    if (stream == null) continue;
                    var assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the innermost exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        internal static Exception GetInnermostException(Exception ex) {
            while(ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
    #endregion
}