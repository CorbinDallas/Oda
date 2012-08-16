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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using System.Reflection;
namespace Oda {
    #region Json enumerators
    /// <summary>
    /// The type of Content-Disposition returned in the Http headers.
    /// </summary>
    public enum JsonContentDisposition {
        /// <summary>
        /// Content is a Json response.
        /// </summary>
        [DescriptionAttribute("Content is a Json response")]
        Json,
        /// <summary>
        /// Content should be written to the Http response stream or string.
        /// </summary>
        [DescriptionAttribute("Content is not a file attachment")]
        Normal,
        /// <summary>
        /// Content is a file attachment.
        /// </summary>
        [DescriptionAttribute("Content is a file attachment")]
        Attachment
    }
    #endregion
    #region Json Method Result Class
    /// <summary>
    /// The result of a Json method execution.  
    /// All Json methods in Oda are required to return this type.
    /// </summary>
    public class JsonResponse : Dictionary<string, object> {
        /// <summary>
        /// Gets or sets the Http cookie to return.
        /// </summary>
        /// <value>
        /// The HTTP cookie.
        /// </value>
        public HttpCookie HttpCookie {get; set;}
        /// <summary>
        /// The private field for Status
        /// </summary>
        private string status;
        /// <summary>
        /// Gets or sets the Http header status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status {
            get {
                if(status == null) {
                    status = "200 OK";
                }
                return status;
            }
            set {
                status = value;
            }
        }
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to supress a response.
        /// </summary>
        /// <value>
        ///   <c>true</c> to supress this response; otherwise, <c>false</c>.
        /// </value>
        public bool SupressResponse { get; set; }
        /// <summary>
        /// The private field for ContentType.
        /// </summary>
        private string contentType;
        /// <summary>
        /// Gets or sets the type of the content returned in the Http header ContentType.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { 
            get{
                if(contentType == null) {
                    contentType = "application/json; charset=utf-8";
                }
                return contentType;
            }
            set {
                contentType = value;
            }
        }
        /// <summary>
        /// Private field for HeaderEncoding
        /// </summary>
        private Encoding _headerEncoding;
        /// <summary>
        /// Gets or sets the header encoding.
        /// </summary>
        /// <value>
        /// The header encoding.
        /// </value>
        public Encoding HeaderEncoding {
            get {
                if(_headerEncoding == null) {
                    _headerEncoding = Encoding.UTF8;
                }
                return _headerEncoding;
            }
            set {
                _headerEncoding = value;
            }
        }
        /// <summary>
        /// Private field for ContentEncoding
        /// </summary>
        private Encoding _contentEncoding;
        /// <summary>
        /// Gets or sets the content encoding.
        /// </summary>
        /// <value>
        /// The content encoding.
        /// </value>
        public Encoding ContentEncoding {
            get {
                if(_contentEncoding == null) {
                    _contentEncoding = Encoding.UTF8;
                }
                return _contentEncoding;
            }
            set {
                _contentEncoding = value;
            }
        }
        /// <summary>
        /// Gets or sets the content disposition.
        /// </summary>
        /// <value>
        /// The content disposition.
        /// </value>
        public JsonContentDisposition ContentDisposition { get; set; }
        /// <summary>
        /// Gets or sets the length of the content.
        /// </summary>
        /// <value>
        /// The length of the content.
        /// </value>
        public int ContentLength { get; set; }
        /// <summary>
        /// The private field for AttachmentFileName.
        /// </summary>
        private string attachmentFileName;
        /// <summary>
        /// Gets or sets the name of the file attachment.
        /// </summary>
        /// <value>
        /// The name of the file attachment.
        /// </value>
        public string AttachmentFileName {
            get {
                if(attachmentFileName == null) {
                    attachmentFileName = "";
                }
                return attachmentFileName;
            }
            set {
                attachmentFileName = value;
            }
        }
        /// <summary>
        /// private field for AttachmentContent.
        /// </summary>
        private string attachmentContent;
        /// <summary>
        /// Gets or sets the content of the attachment.
        /// </summary>
        /// <value>
        /// The content of the attachment.
        /// </value>
        public string AttachmentContent {
            get {
                if(attachmentContent == null) {
                    attachmentContent = "";
                }
                return attachmentContent;
            }
            set {
                attachmentContent = value;
            }
        }
        /// <summary>
        /// Gets or sets the attachment stream.
        /// </summary>
        /// <value>
        /// The attachment stream.
        /// </value>
        public Stream AttachmentStream { get; set; }
        /// <summary>
        /// The private field for Id.
        /// </summary>
        private Guid id;
        /// <summary>
        /// Gets or sets the unique id of the requested method.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id {
            get {
                // avoid null refrences
                if(id == null) {
                    id = Guid.Empty;
                }
                return id;
            }
            set {
                id = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string MethodName { 
            get{
                if(!this.ContainsKey("MethodName")) {
                    this.Add("MethodName", "");
                }
                return this["MethodName"].ToString();
            }
            set{
                if(!this.ContainsKey("MethodName")) {
                    this.Add("MethodName", value);
                } else {
                    this["MethodName"] = value;
                }
            } 
        }
        /// <summary>
        /// Gets or sets the error number this method returns.
        /// Typically 0 means no errors, any other number
        /// indicates an error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public int Error {
            get {
                return (int)this["Error"];
            }
            set {
                this["Error"] = value;
            }
        }
        /// <summary>
        /// Gets or sets the message returned by this Json method.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message {
            get {
                return (string)this["Message"];
            }
            set {
                this["Message"] = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        public JsonResponse() {
            this.Add("Error", 0);
            this.Add("Message", "");
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="Error">The error number of this Json method.  
        /// Typically 0 means no errors, any other number
        /// indicates an error.</param>
        /// <param name="Message">Sets the message returned by this Json method.</param>
        public JsonResponse(int Error, string Message) {
            this.Add("Error", Error);
            this.Add("Message", Message);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="Error">The error number of this Json method.  
        /// Typically 0 means no errors, any other number
        /// indicates an error.</param>
        /// <param name="Message">Sets the message returned by this Json method.</param>
        /// <param name="Items">Addtional items returned by this Json method.</param>
        public JsonResponse(int Error, string Message, Dictionary<string, object> Items) {
            this.Add("Error", Error);
            this.Add("Message", Message);
            foreach(KeyValuePair<string, object> item in Items) {
                this.Add(item.Key, item.Value);
            }
        }
        #region Json Invoke Methods
        internal static JsonResponse[] InvokeJsonMethods(string decodedMethodRequest) {
            List<JsonResponse> results = new List<JsonResponse>();
            JArray requestedMethods = JsonConvert.DeserializeObject<JArray>(decodedMethodRequest);
            foreach(JToken methodToken in requestedMethods) {
                string methodName = methodToken[0].ToString();
                if(Plugin._JsonMethods.ContainsKey(methodName)) {
                    MethodInfo method = Plugin._JsonMethods[methodName];
                    List<object> args = (List<object>)JsonResponse.JTokenToGeneric((JArray)methodToken[1]);
                    try {
                        JsonResponse result = (JsonResponse)method.Invoke(null, args.ToArray());
                        result.MethodName = methodName;
                        results.Add(result);
                    }catch(Exception ex){
                        Exception iex = Core.GetInnermostException(ex);
                        JsonResponse errorResult = new JsonResponse(2, string.Format("Error calling method {1}{0}Source: {2}{0} Message: {3}{0}Stack Trace: {4}",
                            Environment.NewLine, methodName, iex.Source ,iex.Message, iex.StackTrace));
                        errorResult.MethodName = methodName;
                        results.Add(errorResult);
                    }
                } else {
                    JsonResponse errorResult = new JsonResponse(1,string.Format("Method {0} not found.",methodName));
                    errorResult.MethodName = methodName;
                    results.Add(errorResult);
                }
            }
            return results.ToArray();
        }
        #endregion
        #region Json converter methods
        /// <summary>
        /// Turns Dictionary&lt;string,object&gt; or a List&lt;object&gt; into a Json string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToJson(object obj) {
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
            json.NullValueHandling = NullValueHandling.Include;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Error;
            json.Error += delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) {
                string errObject = args.ErrorContext.OriginalObject.GetType().ToString();
                args.ErrorContext.Handled = true;
            };
            string output = "";
            using(StringWriter sw = new StringWriter()) {
                Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                writer.QuoteChar = '"';
                json.Serialize(writer, obj);
                output = sw.ToString();
            }
            return output;
        }
        /// <summary>
        /// Turns a JToken into a Dictionary&lt;string,object&gt; or a List&lt;object&gt;.
        /// </summary>
        /// <param name="jobject">The jobject.</param>
        /// <returns></returns>
        public static object JTokenToGeneric(object jobject) {
            if(jobject.GetType() == typeof(JValue)) {
                JValue o = (JValue)jobject;
                return o.Value;
            } else if(jobject.GetType() == typeof(JArray)) {
                List<object> j = new List<object>();
                foreach(object b in jobject as JArray) {
                    j.Add(JTokenToGeneric(b));
                }
                return j;
            } else {
                Dictionary<string, object> j = new Dictionary<string, object>();
                JObject o = (JObject)jobject;
                foreach(KeyValuePair<string, JToken> i in o) {
                    j.Add(i.Key, JTokenToGeneric(i.Value));
                }
                return j;
            }
        }
        #endregion
    }
    #endregion
}