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
    public partial class Core : IHttpModule, IHttp {
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
        private static List<object> _Items;
        /// <summary>
        /// A box to put your things in.
        /// </summary>
        public static List<object> Items {
            get {
                return _Items;
            }
        }
        /// <summary>
        /// The private field for the JsonMethodUrl property.
        /// </summary>
        private static string _JsonMethodUrl;
        /// <summary>
        /// Gets or sets the Json method Url.
        /// </summary>
        /// <value>
        /// The json method URL.
        /// </value>
        public static string JsonMethodUrl {
            get {
                if(_JsonMethodUrl == null) {
                    _JsonMethodUrl = "/responder";
                }
                return _JsonMethodUrl;
            }
            set {
                _JsonMethodUrl = value;
            }
        }
        /// <summary>
        /// Resolves the embedded assembiles.
        /// </summary>
        /// <param name="_sender">The _sender.</param>
        /// <param name="_args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private static Assembly resolveEmbeddedAssembiles(object _sender, EventArgs _args) {
            ResolveEventArgs args = (ResolveEventArgs)_args;
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach(string name in names) {
                string cleanName = name.Replace("Oda.lib.", "").Replace(".dll", "");
                if(args.Name.Contains(cleanName)) {
                    using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) {
                        Byte[] assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
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