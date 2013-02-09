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
using System.Text;

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
            get { return InternalPlugins.ToArray(); }
        }

        /// <summary>
        /// Gets a plugin by the plugins type.
        /// </summary>
        /// <param name="pluginType">Type of the plugin.</param>
        /// <returns></returns>
        public static object GetPlugin(Type pluginType)
        {
            return InternalPlugins.FirstOrDefault(plugin => plugin.GetType() == pluginType);
        }

        /// <summary>
        /// Gets a plugin by the plugins name.
        /// </summary>
        /// <param name="pluginName">Name of the plugin.</param>
        /// <returns></returns>
        public static object GetPlugin(string pluginName)
        {
            return InternalPlugins.FirstOrDefault(plugin => plugin.GetType().Name.Equals(pluginName));
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
            // make resourcePath resource string compatible
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
        /// The path is a combination of
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
        /// Gets the resource string starting from the project root.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        public string GetResourceString(string resourcePath)
        {
            // see if this is a real resource path, if not return
            var resName = FindManifestResourceName(resourcePath);
            if (resName.Equals(string.Empty))
            {
                return string.Empty;
            }
            // see if an inheritance file exists
            if (InheritanceFileExist(resourcePath))
            {
                using (var sr = new StreamReader(GetInheritanceFile(resourcePath)))
                {
                    return sr.ReadToEnd();
                }
            }
            var s = GetType().Assembly.GetManifestResourceStream(resName);
            if(s==null)
            {
                return string.Empty;
            }
            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets an embedded resource stream.
        /// </summary>
        /// <param name="resourcePath">The resource path starting from the project root.</param>
        /// <returns></returns>
        public Stream GetResource(string resourcePath)
        {
            // see if this is a real resource path, if not return
            var resName = FindManifestResourceName(resourcePath);
            if (resName.Equals(string.Empty))
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
            if (resName.Equals(string.Empty))
            {
                return null;
            }
            var s = GetType().Assembly.GetManifestResourceStream(resName);
            if(s==null)
            {
                return null;
            }
            // see if an inheritance file exists
            return InheritanceFileExist(resourcePath) ? new Bitmap(GetInheritanceFile(resourcePath)) : new Bitmap(s);
        }

        #region Private plugin field and plugin instantiator method

        /// <summary>
        /// Internal list of plugins.
        /// </summary>
        internal static List<object> InternalPlugins;

        /// <summary>
        /// A list of methods found who's class inherits the abstract class JsonMethods.
        /// </summary>
        internal static Dictionary<string, MethodInfo> InternalJsonMethods;

        private static void LoadJsonMethods(IEnumerable<Assembly> assemblies)
        {
            var jsonMethods = (from assembly in assemblies from type in assembly.GetTypes() where type.BaseType == typeof(JsonMethods) select type).ToList();
            foreach (var type in jsonMethods)
            {
                // get a list of JsonMethods to be called on demand
                var methods = type.GetMethods();
                foreach (var method in methods.
                    Where(method => method.ReturnType == typeof (JsonResponse) && method.IsStatic).
                    Where(method => !InternalJsonMethods.ContainsKey(type.Name + "." + method.Name)))
                {
                    InternalJsonMethods.Add(type.Name + "." + method.Name, method);
                }
            }
        }
        /// <summary>
        /// Used internally to load a plugin when dependencies are satisfied.
        /// </summary>
        /// <param name="type">The type to load.</param>
        /// <param name="unloadedPlugins">List of unloaded plugins.</param>
        /// <param name="loadedPlugins">List of loaded plugins.</param>
        /// <returns></returns>
        private static void LoadPlugin(Type type, ICollection<Type> unloadedPlugins, ICollection<Type> loadedPlugins)
        {
            try {
                var p = Activator.CreateInstance(type);
                InternalPlugins.Add(p);
                Core.Log.WriteLine(Core.GetAssemblyVersionString(Assembly.GetAssembly(type)),1);
                unloadedPlugins.Remove(type);
                loadedPlugins.Add(type);
            }catch(Exception e) {
                throw Core.GetInnermostException(e);
            }
        }
        /// <summary>
        /// Activates the plugins located in the host sites AppDomain (/bin).
        /// </summary>
        internal static void ActivatePlugins()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // load all types that have a base class of Oda.Plugin
            var unloadedPlugins = (from assembly in assemblies from type in assembly.GetTypes() where type.BaseType == typeof (Plugin) select type).ToList();
            var loadedPlugins = new List<Type>();
            // index counter for walking through dependencies
            var x = 0;
            while (unloadedPlugins.Count>0)
            {
                // if the index is too high, start over from the begining
                // this can happen if a dependencies are in an unusual order (e.g.: 1 requires 3, 3 requires 2, 2 requires 4)
                if(x>unloadedPlugins.Count-1)
                {
                    x = 0;
                }
                // get the first unloaded plugin
                var type = unloadedPlugins[x];
                // get the plugins dependencies from the classes PluginDependencies attribute 
                var d = (PluginDependencies[])type.GetCustomAttributes(typeof(PluginDependencies), true);
                // if there are no dependencies load
                // the plugin now and remove from the
                // unloaded plugins list.
                if(d.Length == 0)
                {
                    LoadPlugin(type, unloadedPlugins, loadedPlugins);
                    x = 0;
                    continue;
                } 
                // there are dependencies
                var dependencies = d[0].RequiredTypes;
                // next try and invalidate this boolean by ...
                var allDependenciesFound = true;
                // ... checking already loaded plugins for the presence of loaded dependancies
                foreach (var dependentType in dependencies.Where(dependentType => !loadedPlugins.Contains(dependentType)))
                {
                    allDependenciesFound = false;
                    // check to see if this dependency will _ever_ be loaded
                    if (unloadedPlugins.Contains(dependentType)) continue;
                    // there is a missing dependency. 
                    // Throw and exception telling the user what plugin needs it and what the needed plugin is called.
                    
                    // create a string list of the plugins required
                    var sb = new StringBuilder();
                    foreach(var dpn in dependencies)
                    {
                        sb.Append(dpn.FullName + ", ");
                    }
                    sb.Remove(sb.Length - 2, 2);// remove trailing ', '
                    Exception e = new DllNotFoundException(string.Format(@"The plugin {0} requires the following plugins {1}, however plugin {2} is missing.",
                                                                         type.FullName, sb, dependentType.FullName));
                    throw e;
                }
                if(allDependenciesFound)
                {
                    // All dependencies were found
                    // go ahead and load this type now.
                    LoadPlugin(type, unloadedPlugins, loadedPlugins);
                    x = 0;
                }else{
                    // one or more dependencies were not found
                    // skip loading this type for now until
                    // the proper dependency can be found
                    x++;
                }
            }
            // now load json methods
            LoadJsonMethods(assemblies);
        }
        #endregion
    }
    /// <summary>
    /// Attribute class for specifiying list of
    /// Oda plugins required by this plugin that should
    /// be loaded first.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginDependencies : Attribute
    {
        /// <summary>
        /// List of dependant types that this plugin requires
        /// loaded before instantiation.
        /// </summary>
        public Type[] RequiredTypes { get; private set; }
        /// <summary>
        /// List of dependant types that this plugin requires
        /// loaded before instantiation.
        /// </summary>
        /// <param name="types">Array of dependant types.</param>
        public PluginDependencies(Type[] types)
        {
            RequiredTypes = types;
        }
        /// <summary>
        /// List of dependant types that this plugin requires
        /// loaded before instantiation.
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <param name="type3"></param>
        /// <param name="type4"></param>
        /// <param name="type5"></param>
        /// <param name="type6"></param>
        /// <param name="type7"></param>
        /// <param name="type8"></param>
        /// <param name="type9"></param>
        /// <param name="type10"></param>
        public PluginDependencies(Type type1, Type type2 = null, Type type3 = null, Type type4 = null,
            Type type5 = null, Type type6 = null, Type type7 = null, Type type8 = null, Type type9 = null, Type type10 = null)
        {
            var typesToAdd = new List<Type>();
            if (type1 != null) { typesToAdd.Add(type1); }
            if (type2 != null) { typesToAdd.Add(type2); }
            if (type3 != null) { typesToAdd.Add(type3); }
            if (type4 != null) { typesToAdd.Add(type4); }
            if (type5 != null) { typesToAdd.Add(type5); }
            if (type6 != null) { typesToAdd.Add(type6); }
            if (type7 != null) { typesToAdd.Add(type7); }
            if (type8 != null) { typesToAdd.Add(type8); } 
            if (type9 != null) { typesToAdd.Add(type9); }
            if (type10 != null) { typesToAdd.Add(type10); }
            RequiredTypes = typesToAdd.ToArray();
        }
    }
}