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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
namespace Oda {
    #region Http Event Interface
    /// <summary>
    /// Contains events for the HttpModle.
    /// Standard Http events like Begin Request, End Request and Initialize.
    /// </summary>
    public interface IHttp {
        /// <summary>
        /// Occurs when the HttpModule initializes.  
        /// This can happen more than once per application.
        /// </summary>
        event EventHandler Initialize;
        /// <summary>
        /// Occurs when the HttpModule is disposed.
        /// </summary>
        event EventHandler DisposeHttpApplication;
        /// <summary>
        /// Occurs when an Http request Begins.
        /// </summary>
        event EventHandler BeginHttpRequest;
        /// <summary>
        /// Occurs when an Http request ends.
        /// </summary>
        event EventHandler EndHttpRequest;
        /// <summary>
        /// Occurs when an Http application error occurs.
        /// </summary>
        event EventHandler ApplicationError;
        /// <summary>
        /// Occurs each time the memory buffer of a large upload is flushed.
        /// </summary>
        event EventHandler UploadStatus;
    }
    #endregion
    #region Http Event Arguments
    /// <summary>
    /// Arguments for an Http Event
    /// </summary>
    public class HttpEventArgs : EventArgs {
        /// <summary>
        /// The Http Application associated with the event.
        /// </summary>
        public HttpApplication HttpApplication { get; internal set; }
        /// <summary>
        /// Gets or sets items.
        /// Put anything you like into this array.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public object[] Items { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpEventArgs"/> class.
        /// </summary>
        /// <param name="app">The _app.</param>
        public HttpEventArgs(HttpApplication app) {
            HttpApplication = app;
        }
    }
    /// <summary>
    /// Arguments for the Http upload event.
    /// </summary>
    public class HttpUploadStatusEventArgs : EventArgs {
        /// <summary>
        /// Gets the bytes read so far.
        /// </summary>
        /// <value>
        /// The bytes read.
        /// </value>
        public long BytesRead { get; internal set; }
        /// <summary>
        /// Gets the bytes total being uploaded.
        /// </summary>
        /// <value>
        /// The bytes total.
        /// </value>
        public long BytesTotal { get; internal set; }
        /// <summary>
        /// Gets the time the upload started on.
        /// </summary>
        /// <value>
        /// The upload start time.
        /// </value>
        public DateTime StartedOn { get; internal set; }
        /// <summary>
        /// Gets the time last updated.
        /// </summary>
        /// <value>
        /// The last updated time.
        /// </value>
        public DateTime LastUpdated { get; internal set; }
        /// <summary>
        /// Gets the status message from the server.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; internal set; }
        /// <summary>
        /// Gets the current file.
        /// </summary>
        /// <value>
        /// The current file.
        /// </value>
        public string CurrentFile { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="HttpUploadStatusEventArgs"/> is complete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if complete; otherwise, <c>false</c>.
        /// </value>
        public bool Complete { get; internal set; }
        /// <summary>
        /// Gets the unique id of this upload request.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; internal set; }
    }
    #endregion
    /// <summary>
    /// Used to map data from HTTP JSON requests to methods.
    /// </summary>
    class Mapper {
        /// <summary>
        /// Gets or sets the list of files uploaded.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<UploadedFile> Files { get; set; }
        /// <summary>
        /// Gets or sets the JSON method map.
        /// </summary>
        /// <value>
        /// The map.
        /// </value>
        public string Map { get; set; }
        /// <summary>
        /// Gets or sets the unique id of this request.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Mapper"/> class.
        /// </summary>
        public Mapper() {
            Files = new List<UploadedFile>();
        }
    }
    /// <summary>
    /// Represents a file uploaded from a HTTP client.
    /// </summary>
    public class UploadedFile {
        /// <summary>
        /// The method used by the upload stream.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method { get; internal set; }
        /// <summary>
        /// The instance number of the method if there is more than one method.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public int Instance { get; internal set; }
        /// <summary>
        /// The file number if this file was in a field with other files.
        /// </summary>
        /// <value>
        /// The file number.
        /// </value>
        public int FileNumber { get; internal set; }
        /// <summary>
        /// The field number if this file was in a collection that contained more than one file upload field.
        /// </summary>
        /// <value>
        /// The field number.
        /// </value>
        public int FieldNumber { get; internal set; }
        /// <summary>
        /// The path to the uploaded file.  This file will be deleted at the end of the request.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; internal set; }
        /// <summary>
        /// The original file name
        /// </summary>
        /// <value>
        /// The name of the original file.
        /// </value>
        public string OriginalFileName { get; internal set; }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; internal set; }
    }
    /// <summary>
    /// The main Http module.
    /// </summary>
    public partial class Core : IHttpModule, IHttp {
        #region Events
        #region Event : AppError
        /// <summary>
        /// Raises the on app error.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnApplicationError(HttpEventArgs args) {
            if(ApplicationError != null) ApplicationError(this, args);
        }
        /// <summary>
        /// Occurs when an Http application error occurs.
        /// </summary>
        public static event EventHandler ApplicationError;
        /// <summary>
        /// Occurs when an Http application error occurs.
        /// </summary>
        event EventHandler IHttp.ApplicationError {
            add {
                lock(ApplicationError) {
                    ApplicationError += value;
                }
            }
            remove {
                lock(ApplicationError) {
                    ApplicationError -= value;
                }
            }
        }
        #endregion
        #region Event : EndRequest
        /// <summary>
        /// Raises the on end request.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnEndRequest(HttpEventArgs args) {
            if(EndHttpRequest != null) EndHttpRequest(this, args);
        }
        /// <summary>
        /// Occurs when an Http request ends.
        /// </summary>
        public static event EventHandler EndHttpRequest;
        /// <summary>
        /// Occurs when an Http request ends.
        /// </summary>
        event EventHandler IHttp.EndHttpRequest {
            add {
                lock(EndHttpRequest) {
                    EndHttpRequest += value;
                }
            }
            remove {
                lock(EndHttpRequest) {
                    EndHttpRequest -= value;
                }
            }
        }
        #endregion
        #region Event : BeginRequest
        /// <summary>
        /// Raises the on begin request.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnBeginRequest(HttpEventArgs args) {
            if(BeginHttpRequest != null) BeginHttpRequest(this, args);
        }
        /// <summary>
        /// Occurs when an Http request Begins.
        /// </summary>
        public static event EventHandler BeginHttpRequest;
        /// <summary>
        /// Occurs when an Http request Begins.
        /// </summary>
        event EventHandler IHttp.BeginHttpRequest {
            add {
                lock(BeginHttpRequest) {
                    BeginHttpRequest += value;
                }
            }
            remove {
                lock(BeginHttpRequest) {
                    BeginHttpRequest -= value;
                }
            }
        }
        #endregion
        #region Event : Init
        /// <summary>
        /// Raises the on init.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnInitialize(HttpEventArgs args) {
            if(Initialize != null) Initialize(this, args);
        }
        /// <summary>
        /// Occurs when the HttpModule initializes.
        /// This can happen more than once per application.
        /// </summary>
        public static event EventHandler Initialize;
        /// <summary>
        /// Occurs when the HttpModule initializes.
        /// This can happen more than once per application.
        /// </summary>
        event EventHandler IHttp.Initialize {
            add {
                lock(Initialize) {
                    Initialize += value;
                }
            }
            remove {
                lock(Initialize) {
                    Initialize -= value;
                }
            }
        }
        #endregion
        #region Event : UploadStatus
        /// <summary>
        /// Raises the Upload Status event.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpUploadStatusEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnUploadStatus(HttpUploadStatusEventArgs args) {
            if (UploadStatus != null) UploadStatus(this, args);
        }
        /// <summary>
        /// Occurs each time the memory buffer of a large upload is flushed.
        /// </summary>
        public static event EventHandler UploadStatus;
        /// <summary>
        /// Occurs each time the memory buffer of a large upload is flushed.
        /// </summary>
        event EventHandler IHttp.UploadStatus {
            add {
                lock (UploadStatus) {
                    UploadStatus += value;
                }
            }
            remove {
                lock (UploadStatus) {
                    UploadStatus -= value;
                }
            }
        }
        #endregion
        #region Event : Dispose
        /// <summary>
        /// Raises the on dispose.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void RaiseOnDispose(HttpEventArgs args) {
            if(DisposeHttpApplication != null) DisposeHttpApplication(this, args);
        }
        /// <summary>
        /// Occurs when the HttpModule is disposed.
        /// </summary>
        public static event EventHandler DisposeHttpApplication;
        /// <summary>
        /// Occurs when the HttpModule is disposed.
        /// </summary>
        event EventHandler IHttp.DisposeHttpApplication {
            add {
                lock(DisposeHttpApplication) {
                    DisposeHttpApplication += value;
                }
            }
            remove {
                lock(DisposeHttpApplication) {
                    DisposeHttpApplication -= value;
                }
            }
        }
        #endregion
        #endregion
        #region HttpModule Event Interface Subscriber
        private static void GetWebConfigSettings() {
            LogVerbosity = GetWebConfigSetting("LogVerbosity", 5);
            LogTimestamp = GetWebConfigSetting("LogTimestamp", true);
            LogPath = GetWebConfigSetting("LogPath", "~/log").Replace("~/", BaseDirectory);
        }
        private static void LogVersionInformation() {
            Version = GetAssemblyVersionString(Assembly.GetExecutingAssembly());
            Log.WriteLine(Version);
        }
        /// <summary>
        /// Gets the assembly name, version and copyright information.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string GetAssemblyVersionString(Assembly asm) {
            var c = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var copyright = ((AssemblyCopyrightAttribute)c[0]).Copyright;
            return String.Format("{0} {1}",asm.FullName, copyright);
        }
        /// <summary>
        /// Starts up Oda HttpModule.  Starts logging, instantiates plugins, connects to events and creates Json mapper.
        /// </summary>
        /// <param name="sender">The HTTP application.</param>
        public void Init(HttpApplication sender) {
            // bind events
            sender.BeginRequest += BeginRequest;
            sender.EndRequest += EndRequest;
            sender.Error += AppError;
            // only init the http module one time
            if (HttpApplication != null) return;
            HttpApplication = sender;
            var domain = AppDomain.CurrentDomain;
            BaseDirectory = domain.BaseDirectory;
            RelativeSearchPath = domain.RelativeSearchPath;
            DynamicDirectory = domain.DynamicDirectory;
            GetWebConfigSettings();
            // start logging
            var logFileName = string.Format("{0}.log", DateTime.Now.ToString("G").Replace("/", ".").Replace(":", ".").Replace(" ", "_"));
            Log = new Log(System.IO.Path.Combine(LogPath, logFileName), LogVerbosity, LogTimestamp);
            // write some info about the assembly
            LogVersionInformation();
            // resolve embedded assemblies
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEmbeddedAssembiles;
            if (StartupState != StartupState.NotYetStarted) return;
            StartupState = StartupState.Starting;
            // create box to put stuff in
            _items = new List<object>();
            // init plugins
            Plugin.InternalPlugins = new List<object>();
            Plugin.InternalJsonMethods = new Dictionary<string, System.Reflection.MethodInfo>();
            Plugin.ActivatePlugins();
            // raise init event
            var args = new HttpEventArgs(HttpApplication);
            RaiseOnInitialize(args);
            UploadStatus += (o, eventArgs) => {
                var ev = (HttpUploadStatusEventArgs) eventArgs;
                if(UploadStatuses.ContainsKey(ev.Id)) {
                    // update existing key
                    UploadStatuses[ev.Id].BytesRead = ev.BytesRead;
                    UploadStatuses[ev.Id].BytesTotal = ev.BytesTotal;
                    UploadStatuses[ev.Id].Complete = ev.Complete;
                    UploadStatuses[ev.Id].LastUpdated = ev.LastUpdated;
                    UploadStatuses[ev.Id].StartedOn = ev.StartedOn;
                    UploadStatuses[ev.Id].Message = ev.Message;
                    UploadStatuses[ev.Id].CurrentFile = ev.CurrentFile;
                    if (ev.Complete) {
                        UploadStatuses.Remove(ev.Id);
                    }
                }else {
                    UploadStatuses.Add(ev.Id, new UploadStatus() {
                        BytesRead = ev.BytesRead,
                        BytesTotal = ev.BytesTotal,
                        Complete = ev.Complete,
                        Id = ev.Id,
                        LastUpdated = ev.LastUpdated,
                        StartedOn = ev.StartedOn,
                        Message = ev.Message,
                        CurrentFile = ev.CurrentFile
                    });
                }
            };
            StartupState = StartupState.Started;
        }
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() {
            var args = new HttpEventArgs(HttpApplication);
            RaiseOnDispose(args);
        }
        /// <summary>
        /// The start of an Http request
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void BeginRequest(object sender, EventArgs e) {
            // begin request events
            var hArgs = new HttpEventArgs(HttpApplication);
            RaiseOnBeginRequest(hArgs);
            var results = new Dictionary<string, JsonResponse>();
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;
            // check for Json requests made to the JsonMethodUrl
            // and clear the requests
            if(request.FilePath.Equals(JsonMethodUrl)) {
                // a request was made to the responder Url
                // deserialize the request and execute the requested methods
                // gather the results and add them to the results collection
                // do post - Don't use the request.Form object because it is silly.
                var workerRequest = (HttpWorkerRequest)HttpContext.Current.GetType().GetProperty("WorkerRequest",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(HttpContext.Current, null);
                var bodyLength = workerRequest.GetPreloadedEntityBodyLength();
                if (bodyLength>0) {
                    var contentType = workerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);
                    if (contentType == null) { throw new NullReferenceException("Header Content-Type cannot be empty.  Acceptable post values are multipart/form-data or application/json."); }
                    var filePathsAndMap = ParseRequest(workerRequest);
                    try {
                        JsonResponse.InvokeJsonMethods(filePathsAndMap, ref results);
                        // after methods have been invoked, remove files from temp
                        foreach (var f in filePathsAndMap.Files) {
                            File.Delete(f.Path);
                        }
                    } catch(Exception ex) {
                        var iex = GetInnermostException(ex);
                        var errorResult = new JsonResponse(3, string.Format("Error invoking method. Source: {1}{0} Message: {2}{0} Stack Trace: {3}", 
                            Environment.NewLine, iex.Source ,iex.Message, iex.StackTrace));
                        results.Add("Exception", errorResult);
                    }
                } else {
                    // if there was no post
                    // do QueryString (get)
                    if (request.QueryString.Keys.Count == 1) {
                        try {
                            var m = new Mapper() {Map = HttpUtility.UrlDecode(request.QueryString.ToString())};
                            JsonResponse.InvokeJsonMethods(m, ref results);
                        } catch (Exception ex) {
                            var iex = GetInnermostException(ex);
                            var errorResult = new JsonResponse(3, string.Format("Error invoking method. Source: {1}{0} Message: {2}{0} Stack Trace: {3}",
                               Environment.NewLine, iex.Source, iex.Message, iex.StackTrace));
                            results.Add("Exception", errorResult);
                        }
                    }
                }
            }
            // if one or more Json methods returned a result
            // respond with that result
            if (results.Count <= 0) return;
            foreach(var r in results) {
                var result = r.Value;
                if (result.SupressResponse) continue;
                response.Status = result.Status;
                response.StatusCode = result.StatusCode;
                response.ContentType = result.ContentType;
                response.ContentEncoding = result.ContentEncoding;
                response.HeaderEncoding = result.HeaderEncoding;
                if(result.HttpCookie != null) {
                    response.AppendCookie(result.HttpCookie);
                }
                if(result.ContentDisposition == JsonContentDisposition.Attachment || result.ContentDisposition == JsonContentDisposition.Normal) {
                    if(result.ContentDisposition == JsonContentDisposition.Attachment) {
                        response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}""", result.AttachmentFileName));
                    }
                    if(result.AttachmentContent.Length > 0) {
                        result.ContentLength = result.AttachmentContent.Length;
                        response.AddHeader("Content-Length", result.ContentLength.ToString(CultureInfo.InvariantCulture));
                        response.Write(result.AttachmentContent);
                    } else if(result.AttachmentStream.Length > 0) {
                        result.ContentLength = (int)result.AttachmentStream.Length;
                        response.AddHeader("Content-Length", result.ContentLength.ToString(CultureInfo.InvariantCulture));
                        var bytes = new byte[result.AttachmentStream.Length];
                        result.AttachmentStream.Read(bytes, 0, (int)result.AttachmentStream.Length);
                        response.BinaryWrite(bytes);
                    }
                    response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return;// Only one file can be output at a time.
                }
            }
            response.Write(JsonResponse.ToJson(results));
            response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        /// <summary>
        /// The end of the Http request
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void EndRequest(object sender, EventArgs e) {
            var args = new HttpEventArgs(HttpApplication);
            RaiseOnEndRequest(args);
        }
        /// <summary>
        /// Application error
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void AppError(object sender, EventArgs e) {
            var args = new HttpEventArgs(HttpApplication);
            RaiseOnApplicationError(args);
        }
        #endregion
        #region GetWebConfigSetting Overloads
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static bool GetWebConfigSetting(string appSettingKeyName, bool defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            bool output;
            return keyValue == null ? defaultValue : (bool.TryParse(keyValue, out output) ? output : defaultValue);
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static int GetWebConfigSetting(string appSettingKeyName, int defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            int output;
            return keyValue == null ? defaultValue : (int.TryParse(keyValue, out output) ? output : defaultValue);
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static Guid GetWebConfigSetting(string appSettingKeyName, Guid defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            Guid output;
            return keyValue == null ? defaultValue : (Guid.TryParse(keyValue, out output) ? output : defaultValue);
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static DateTime GetWebConfigSetting(string appSettingKeyName, DateTime defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            DateTime output;
            return keyValue == null ? defaultValue : (DateTime.TryParse(keyValue, out output) ? output : defaultValue);
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static string GetWebConfigSetting(string appSettingKeyName, string defaultValue) {
            return ConfigurationManager.AppSettings[appSettingKeyName] ?? defaultValue;
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static decimal GetWebConfigSetting(string appSettingKeyName, decimal defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            decimal output;
            return keyValue == null ? defaultValue : (decimal.TryParse(keyValue, out output) ? output : defaultValue);
        }
        /// <summary>
        /// Gets a web config setting, converts the string to the target type and returns an object type.
        /// </summary>
        /// <param name="appSettingKeyName">The sender.</param>
        /// <param name="defaultValue">The sender.</param>
        public static Int64 GetWebConfigSetting(string appSettingKeyName, Int64 defaultValue) {
            var keyValue = ConfigurationManager.AppSettings[appSettingKeyName];
            Int64 output;
            return keyValue == null ? defaultValue : (Int64.TryParse(keyValue, out output) ? output : defaultValue);
        }
        #endregion
    }
}