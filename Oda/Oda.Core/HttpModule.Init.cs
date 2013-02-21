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
    public class HttpUploadStatusEventArgs : EventArgs {
        public long BytesRead { get; internal set; }
        public long BytesTotal { get; internal set; }
        public DateTime StartedOn { get; internal set; }
        public DateTime LastUpdated { get; internal set; }
        public bool Complete { get; internal set; }
        public Guid Id { get; internal set; }
    }
    #endregion
    class Mapper {
        public List<UploadedFile> Files { get; set; }
        public string Map { get; set; }
        public Guid Id { get; set; }
        public Mapper() {
            Files = new List<UploadedFile>();
        }
    }
    public class UploadedFile {
        public string Method { get; set; }
        public int Instance { get; set; }
        public int FileNumber { get; set; }
        public int FieldNumber { get; set; }
        public string Path { get; set; }
        public string OrginalFileName { get; set; }
        public string ContentType { get; set; }
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
            StartupState = StartupState.Started;
        }
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() {
            var args = new HttpEventArgs(HttpApplication);
            RaiseOnDispose(args);
        }
        byte[] getBoundary(byte[] b) {
            // read until the first CR
            var r = new List<byte>();
            var i = 0;
            while(b[i] != 13){
                r.Add(b[i]);
                i++;
                if(i>60) {
                    throw new FormatException("Malformed form-data.  Boundary cannot be greater than 60 characters.");
                }
            }
            return r.ToArray();
        }
        static int FindPosition(Stream haystack, byte[] needle, long offset) {
            int b;
            int i = 0;
            haystack.Position = offset;
            while ((b = haystack.ReadByte()) != -1) {
                if (b == needle[i++]) {
                    if (i == needle.Length) {
                        return (int)(haystack.Position - needle.Length);
                    }
                } else {
                    i = 0;
                }
            }
            return -1;
        }
        Mapper createTempFilesAndMapFromPost(HttpWorkerRequest r) {
            var isIdFound = false;
            var isMapFound = false;
            var e = new UTF8Encoding();
            // bytes for "Content-Disposition: form-data; "
            var mapSig = new byte[] {
67,111,110,116,101,110,116,45,68,105,115,112,111,
115,105,116,105,111,110,58,32,102,111,114,109,
45,100,97,116,97,59,32,110,97,109,101,61,
34,109,97,112,34,13,10,13,10
            };
            var idSig = new byte[] { 
67,111,110,116,101,110,116,45,68,105,115,112,111,115,105,116,105,
111,110,58,32,102,111,114,109,45,100,97,116,97,59,32,110,
97,109,101,61,34,105,100,34,13,10,13,10};
            var m = new Mapper();
            // first 46 bytes = boundary signature
            const int f = 4096;
            var p = r.GetPreloadedEntityBody();
            var b = getBoundary(p);
            var l = r.GetTotalEntityBodyLength();
            // load stream into temp file
            var fst = Path.GetTempFileName();
            var fs = new FileStream(fst, FileMode.OpenOrCreate);
            // write preloaded body to file
            fs.Write(p,0,p.Length);
            var c = p.Length;
            var q = 0;
            var u = new byte[f];
            while(l-c>q) {
                q = r.ReadEntityBody(u, 0, f);
                fs.Write(u, 0, q);
                c += q;
            }
            if (l-c > 0) {
                var ux = new byte[l - c];
                q = r.ReadEntityBody(ux, 0, (int)l - c);
                fs.Write(ux, 0, q);
            }
            fs.Flush();
            fs.Position = 0;
            // read the entire file finding all boundaries
            var s = new List<long>();
            while (fs.Position<fs.Length) {
                s.Add(FindPosition(fs, b, fs.Position));
            }
            fs.Position = 0;
            // load each boundary into seperate files
            var j = new List<string>();
            // the last boundary is the eof
            for(var i=0;s.Count-1>i;i++) {
                // indexes between boundaries - this is the new file size in bytes
                if (s[i + 1] == -1) {
                    break;
                }
                var z = s[i + 1] - s[i]; // this is the size of the object between the boundaries (including current boundary)
                var x = s[i + 1]; // end position is the begining of the next boundary -1
                var g = (z) < f ? z : f; // chunk size (g) = 4096 (f) or next boundary pos - current boundary pos (z)
                var h = z%g; // get remaining bytes to wrte at the end of the while loop
                var n = Path.GetTempFileName();
                fs.Position = s[i]; // start reading from the begining position of the object (including boundary)
                using(var a = new FileStream(n, FileMode.OpenOrCreate)) {
                    q = 0;
                    c = 0;
                    while (z-c>q){ //read blocks while position - mod block size  is less than end position
                        q = fs.Read(u, 0, (int)g);
                        a.Write(u, 0, q);
                        c += q;
                    }
                    q = fs.Read(u, 0, (int)z-c);
                    a.Write(u, 0, q);
                    a.Position = 0;
                    // read in form data
                    if(!isMapFound){
                        if(FindPosition(a, mapSig, 0)>-1) {
                            // this is the map field
                            var mapBytes = new byte[a.Length - a.Position - 2]; // -2 to drop the \r\n
                            a.Read(mapBytes, 0, mapBytes.Length);
                            m.Map = e.GetString(mapBytes);
                            a.Close();
                            File.Delete(n);
                            isMapFound = true;
                            continue; ;
                        }
                    } 
                    if (!isIdFound) {
                        if(FindPosition(a, idSig, 0) > -1) {
                            // id is always 36 bytes
                            var idBytes = new byte[36];
                            a.Read(idBytes, 0, 36);
                            m.Id = Guid.Parse(e.GetString(idBytes));
                            a.Close();
                            File.Delete(n);
                            isIdFound = true;
                            continue;
                        }
                    } 

                    // this must be a binary file attachment, rip file apart

                    a.Position = 0;

                    //1) remove boundary 
                    while (a.ReadByte() != 13) {}
                    a.Position++; // read past lf
                    //2) parse form data
                    var startOfFormData = a.Position;
                    while(a.ReadByte()!=13) {}
                    a.Position++; // read past lf
                    var endOfFormdata = a.Position;
                    a.Position = startOfFormData;
                    // create a byte array to hold form data
                    var formDataBuffer = new byte[endOfFormdata-startOfFormData];
                    a.Read(formDataBuffer, 0, formDataBuffer.Length);
                    var formData = e.GetString(formDataBuffer);
                    // form data looks like Content-Disposition: form-data; name="Authentication.CreateAccount_files_0_0"; filename="tiny.gif"

                    //3) parse content type
                    a.Position = endOfFormdata;
                    while(a.ReadByte()!=13) {}
                    a.Position++; // read past lf
                    var endOfContentType = a.Position - 2; // -2 so we don't capture \r\n in content type string
                    var contentTypeBuffer = new byte[endOfContentType - endOfFormdata];
                    a.Position = endOfFormdata;
                    a.Read(contentTypeBuffer, 0, contentTypeBuffer.Length);
                    var contentType = e.GetString(contentTypeBuffer);
                        
                    //4) remove extra \r\n\r\n
                    a.Position = endOfContentType + 4;
                        
                    //5) the rest is the binary file 
                    // read it and store it in a new file
                    var binLength = a.Length - a.Position;
                    var bufferSize = f > binLength ? binLength : f;
                    var remainingChunckSize = binLength%bufferSize;
                    var bytesRead = 0L;
                    var tempFileName = Path.GetTempFileName();
                    using(var bin = new FileStream(tempFileName,FileMode.OpenOrCreate)){
                        while (binLength - remainingChunckSize > bytesRead) {
                            var buffer = new byte[bufferSize];
                            bytesRead += a.Read(buffer, 0, (int)bufferSize);
                            bin.Write(buffer,0,(int)bufferSize);
                        }
                        if(remainingChunckSize>0) {
                            var buffer = new byte[remainingChunckSize];
                            a.Read(buffer, 0, (int)remainingChunckSize);
                            bin.Write(buffer, 0, (int) remainingChunckSize);
                        }
                        // delete temp file
                        a.Close();
                        File.Delete(n);
                        // make mapper ref
                        var regex = new Regex(@".*name=""([^""]+)""; filename=""([^""]+)""");
                        var matches = regex.Match(formData);
                        var methodSig = matches.Groups[1].Value;
                        var oFileName = matches.Groups[2].Value;
                        regex = new Regex(@"file:::(.*)_(\d)_files_(\d+)_(\d+)");
                        var matchesMetthod = regex.Match(methodSig);
                        var method = matchesMetthod.Groups[1].Value;
                        var methodInstance = matchesMetthod.Groups[2].Value;
                        var fileField = matchesMetthod.Groups[3].Value;
                        var fileNumber = matchesMetthod.Groups[4].Value;
                        var fm = new UploadedFile() {
                            ContentType = contentType.Replace("Content-Type: ", ""), // remove the words "Content-Type: " from value
                            Path = tempFileName,
                            FileNumber = int.Parse(fileNumber),
                            Instance = int.Parse(methodInstance),
                            Method = method,
                            FieldNumber = int.Parse(fileField),
                            OrginalFileName = oFileName
                        };
                        m.Files.Add(fm);
                    }
                    
                };
            }
            fs.Close();
            fs.Dispose();
            File.Delete(fst);
            return m;
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
                    var filePathsAndMap = createTempFilesAndMapFromPost(workerRequest);
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