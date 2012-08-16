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
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
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
        /// <param name="_app">The _app.</param>
        public HttpEventArgs(HttpApplication _app) {
            HttpApplication = _app;
        }
    }
    #endregion
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
        internal void raiseOnApplicationError(HttpEventArgs args) {
            if(ApplicationError != null) { ApplicationError(this, args); };
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
        internal void raiseOnEndRequest(HttpEventArgs args) {
            if(EndHttpRequest != null) { EndHttpRequest(this, args); };
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
        internal void raiseOnBeginRequest(HttpEventArgs args) {
            if(BeginHttpRequest != null) { BeginHttpRequest(this, args); };
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
        internal void raiseOnInitialize(HttpEventArgs args) {
            if(Initialize != null) { Initialize(this, args); };
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
        #region Event : Dispose
        /// <summary>
        /// Raises the on dispose.
        /// </summary>
        /// <param name="args">The <see cref="Oda.HttpEventArgs"/> instance containing the event data.</param>
        internal void raiseOnDispose(HttpEventArgs args) {
            if(DisposeHttpApplication != null) { DisposeHttpApplication(this, args); };
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
        /// <summary>
        /// Inits the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Init(HttpApplication sender) {
            // make a global refrence
            Core.HttpApplication = (HttpApplication)sender;
            // bind events
            Core.HttpApplication.BeginRequest += new EventHandler(BeginRequest);
            Core.HttpApplication.EndRequest += new EventHandler(EndRequest);
            Core.HttpApplication.Error += new EventHandler(AppError);
            // resolve embedded assemblies
            AppDomain.CurrentDomain.AssemblyResolve += resolveEmbeddedAssembiles;
            if(Core.StartupState == Oda.StartupState.NotYetStarted) {
                Core.StartupState = Oda.StartupState.Starting;
                // create box to put stuff in
                _Items = new List<object>();
                // init plugins
                Plugin._Plugins = new List<object>();
                Plugin._JsonMethods = new Dictionary<string, System.Reflection.MethodInfo>();
                Plugin.ActivatePlugins();
                // raise init event
                HttpEventArgs args = new HttpEventArgs(Core.HttpApplication);
                raiseOnInitialize(args);
                Core.StartupState = Oda.StartupState.Started;
            }
        }
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose() {
            HttpEventArgs args = new HttpEventArgs(Core.HttpApplication);
            raiseOnDispose(args);
        }
        /// <summary>
        /// The start of an Http request
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void BeginRequest(object sender, EventArgs e) {
            // begin request events
            HttpEventArgs hArgs = new HttpEventArgs(Core.HttpApplication);
            raiseOnBeginRequest(hArgs);
            List<JsonResponse> results = new List<JsonResponse>();
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            // check for Json requests made to the JsonMethodUrl
            // and clear the requests
            if(request.FilePath == JsonMethodUrl) {
                // a request was made to the responder Url
                // deserialize the request and execute the requested methods
                // gather the results and add them to the results collection
                //
                // frist do QueryString (get)
                if(request.QueryString.Keys.Count == 1) {
                    try {
                        JsonResponse[] rs = JsonResponse.InvokeJsonMethods(HttpUtility.UrlDecode(request.QueryString.ToString()));
                        results.AddRange(rs);
                    } catch(Exception ex) {
                        Exception iex = GetInnermostException(ex);
                        JsonResponse errorResult = new JsonResponse(3, string.Format("Error invoking method. Source: {1}{0} Message: {2}{0} Stack Trace: {3}",
                           Environment.NewLine, iex.Source, iex.Message, iex.StackTrace));
                        results.Add(errorResult);
                    }
                }
                // then do Form (post)
                if(request.Form.Keys.Count == 1) {
                    try {
                        JsonResponse[] rs = JsonResponse.InvokeJsonMethods(HttpUtility.UrlDecode(request.Form.ToString()));
                        results.AddRange(rs);
                    } catch(Exception ex) {
                        Exception iex = GetInnermostException(ex);
                        JsonResponse errorResult = new JsonResponse(3, string.Format("Error invoking method. Source: {1}{0} Message: {2}{0} Stack Trace: {3}", 
                            Environment.NewLine, iex.Source ,iex.Message, iex.StackTrace));
                        results.Add(errorResult);
                    }
                }
            }
            // if one or more Json methods returned a result
            // respond with that result
            if(results.Count > 0) {
                Dictionary<string, JsonResponse> outputRestults = new Dictionary<string, JsonResponse>();
                foreach(JsonResponse result in results) {
                    response.Status = result.Status;
                    response.StatusCode = result.StatusCode;
                    response.ContentType = result.ContentType;
                    response.ContentEncoding = result.ContentEncoding;
                    response.HeaderEncoding = result.HeaderEncoding;
                    if(result.HttpCookie != null) {
                        response.AppendCookie(result.HttpCookie);
                    }
                    if(!result.SupressResponse) {
                        if(result.ContentDisposition == JsonContentDisposition.Attachment || result.ContentDisposition == JsonContentDisposition.Normal) {
                            if(result.ContentDisposition == JsonContentDisposition.Attachment) {
                                response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}""", result.AttachmentFileName));
                            }
                            if(result.AttachmentContent.Length > 0) {
                                result.ContentLength = result.AttachmentContent.Length;
                                response.AddHeader("Content-Length", result.ContentLength.ToString());
                                response.Write(result.AttachmentContent);
                            } else if(result.AttachmentStream.Length > 0) {
                                result.ContentLength = (int)result.AttachmentStream.Length;
                                response.AddHeader("Content-Length", result.ContentLength.ToString());
                                byte[] bytes = new byte[result.AttachmentStream.Length];
                                result.AttachmentStream.Read(bytes, 0, (int)result.AttachmentStream.Length);
                                response.BinaryWrite(bytes);
                            }
                            response.Flush();
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return;// Only one file can be output at a time.
                        }
                        string methodName = result.MethodName;
                        int methodNameCounter = 0;
                        // make sure and create a unique name
                        // for the returned method
                        while(outputRestults.ContainsKey(methodName + "_" + methodNameCounter)) {
                            methodNameCounter++;
                        }
                        if(methodNameCounter>0) {
                            outputRestults.Add(methodName + "_" + methodNameCounter, result);
                        }else{
                            outputRestults.Add(methodName, result);
                        }
                    }
                }
                response.Write(JsonResponse.ToJson(outputRestults));
                response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
        /// <summary>
        /// The end of the Http request
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void EndRequest(object sender, EventArgs e) {
            HttpEventArgs args = new HttpEventArgs(Core.HttpApplication);
            raiseOnEndRequest(args);
        }
        /// <summary>
        /// Application error
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void AppError(object sender, EventArgs e) {
            HttpEventArgs args = new HttpEventArgs(Core.HttpApplication);
            raiseOnApplicationError(args);
        }
        #endregion
    }
}