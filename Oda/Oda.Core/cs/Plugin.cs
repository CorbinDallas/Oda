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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Oda
{
    /// <summary>
    /// An abstract class that Oda plugin types
    /// can have when they want their methods
    /// exposed to the Json request / response system
    /// all methods within the class that inherits JsonMethods
    /// must return the Type JsonResponse.
    /// </summary>
    public abstract class JsonMethods
    {
    }

    /// <summary>
    /// An Oda plugin.  All plugins have this class as their base class.
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Gets a list of all the instantiated plugins.
        /// </summary>
        public static object[] Plugins
        {
            get { return _Plugins.ToArray(); }
        }

        /// <summary>
        /// Gets a plugin by the plugins type.
        /// </summary>
        /// <param name="pluginType">Type of the plugin.</param>
        /// <returns></returns>
        public static object GetPlugin(Type pluginType)
        {
            return _Plugins.Cast<Plugin>().FirstOrDefault(plugin => plugin.GetType().FullName == pluginType.FullName);
        }

        /// <summary>
        /// Gets a plugin by the plugins name.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns></returns>
        public static object GetPlugin(string pluginName)
        {
            return _Plugins.Cast<Plugin>().FirstOrDefault(plugin => plugin.GetType().Name == pluginName);
        }

        /// <summary>
        /// Finds the name of the manifest resource by 
        /// scanning the embedded manifest resource names
        /// looking for the closest match.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        private string FindManifestResourceName(string resourcePath)
        {
            // make resourcePath resource string compatable
            string cResPath = resourcePath.Replace("\\", ".").Replace("/", ".").ToLower();
            string[] names = GetType().Assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name.ToLower().EndsWith(cResPath))
                {
                    return name;
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the inheritance file path.
        /// The path is a combonation of
        /// Physical application path + Plugin Assembly name + Resource Path
        /// For example: A resource named /Sql/CreateSession.sql in the plugin
        /// Oda.Authentication in an application in C:\inetpub\www.mysite.com\ would 
        /// appear as C:\inetpub\www.mysite.com\Oda.Authentication\Sql\CreateSession.sql.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        private string GetInheritanceFilePath(string resourcePath)
        {
            return (AppDomain.CurrentDomain.BaseDirectory +
                    GetType().Assembly.FullName.Split(',')[0] +
                    resourcePath.Replace("/", @"\")).Replace(@"\\", @"\");
        }

        /// <summary>
        /// Check if the inheritances the file exist for this resource path.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        private bool InheritanceFileExist(string resourcePath)
        {
            var inheritanceFile = GetInheritanceFilePath(resourcePath);
            return File.Exists(inheritanceFile);
        }

        /// <summary>
        /// Gets the inheritance file matching this resource path.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        private Stream GetInheritanceFile(string resourcePath)
        {
            var inheritanceFile = GetInheritanceFilePath(resourcePath);
            if (!File.Exists(inheritanceFile)){
                return null;
            }
            var ms = new MemoryStream(File.ReadAllBytes(inheritanceFile));
            return ms;

        }

        /// <summary>
        /// Gets the resrouce string starting from the project root.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        public string GetResrouceString(string resourcePath)
        {
            // see if this is a real resource path, if not return
            string resName = FindManifestResourceName(resourcePath);
            if (resName == "")
            {
                return "";
            }
            // see if an inheritance file exists
            if (InheritanceFileExist(resourcePath))
            {
                using (var sr = new StreamReader(GetInheritanceFile(resourcePath)))
                {
                    return sr.ReadToEnd();
                }
            }
            using (var sr = new StreamReader(GetType().Assembly.GetManifestResourceStream(resName)))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets an embedded resrouce stream.
        /// </summary>
        /// <param name="resourcePath">The resource path starting from the project root.</param>
        /// <returns></returns>
        public Stream GetResrouce(string resourcePath)
        {
            // see if this is a real resource path, if not return
            var resName = FindManifestResourceName(resourcePath);
            if (resName == "")
            {
                return null;
            }
            // see if an inheritance file exists
            return InheritanceFileExist(resourcePath) ? GetInheritanceFile(resourcePath) : GetType().Assembly.GetManifestResourceStream(resName);
        }

        /// <summary>
        /// Gets an embedded resource image.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        public Bitmap GetResourceImage(string resourcePath)
        {
            // see if this is a real resource path, if not return
            var resName = FindManifestResourceName(resourcePath);
            if (resName == "")
            {
                return null;
            }
            // see if an inheritance file exists
            return InheritanceFileExist(resourcePath) ? new Bitmap(GetInheritanceFile(resourcePath)) : new Bitmap(GetType().Assembly.GetManifestResourceStream(resName));
        }

        #region Private plugin field and plugin instantiator method

        /// <summary>
        /// Internal list of plugins.
        /// </summary>
        internal static List<object> _Plugins;

        /// <summary>
        /// A list of methods found whos class inherits the abstract class JsonMethods.
        /// </summary>
        internal static Dictionary<string, MethodInfo> _JsonMethods;

        /// <summary>
        /// Activates the plugins located in the host sites AppDomain (/bin).
        /// </summary>
        internal static void ActivatePlugins()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Type type in from assembly in assemblies from type in assembly.GetTypes() where type.BaseType != null select type)
            {
                // add plugins that are instantiated now
                if (type.BaseType.FullName == typeof (Plugin).FullName)
                {
                    var p = Activator.CreateInstance(type);
                    _Plugins.Add(p);
                }
                // get a list of JsonMethods to be called on demand
                if (type.BaseType.FullName != typeof (JsonMethods).FullName) continue;
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (method.ReturnType.FullName != typeof (JsonResponse).FullName || !method.IsStatic) continue;
                    if (!_JsonMethods.ContainsKey(type.Name + "." + method.Name))
                    {
                        _JsonMethods.Add(type.Name + "." + method.Name, method);
                    }
                }
            }
        }

        #endregion
    }
}