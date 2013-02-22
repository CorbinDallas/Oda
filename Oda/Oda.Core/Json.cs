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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
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
    [Serializable]
    public class JsonResponse : Dictionary<string, object>, ISerializable {
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
        private string _status;
        /// <summary>
        /// Gets or sets the Http header status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status {
            get { return _status ?? (_status = "200 OK"); }
            set {
                _status = value;
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
        /// Gets or sets a value indicating whether to suppress a response.
        /// </summary>
        /// <value>
        ///   <c>true</c> to suppress this response; otherwise, <c>false</c>.
        /// </value>
        public bool SupressResponse { get; set; }
        /// <summary>
        /// The private field for ContentType.
        /// </summary>
        private string _contentType;
        /// <summary>
        /// Gets or sets the type of the content returned in the Http header ContentType.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { 
            get { return _contentType ?? (_contentType = "application/json; charset=utf-8"); }
            set {
                _contentType = value;
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
            get { return _headerEncoding ?? (_headerEncoding = Encoding.UTF8); }
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
            get { return _contentEncoding ?? (_contentEncoding = Encoding.UTF8); }
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
        private string _attachmentFileName;
        /// <summary>
        /// Gets or sets the name of the file attachment.
        /// </summary>
        /// <value>
        /// The name of the file attachment.
        /// </value>
        public string AttachmentFileName {
            get { return _attachmentFileName ?? (_attachmentFileName = ""); }
            set {
                _attachmentFileName = value;
            }
        }
        /// <summary>
        /// private field for AttachmentContent.
        /// </summary>
        private string _attachmentContent;
        /// <summary>
        /// Gets or sets the content of the attachment.
        /// </summary>
        /// <value>
        /// The content of the attachment.
        /// </value>
        public string AttachmentContent {
            get { return _attachmentContent ?? (_attachmentContent = ""); }
            set {
                _attachmentContent = value;
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
        /// Gets or sets the instance of this method.  When more than one method with the same
        /// name is called an instance greater than zero is possible.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public int Instance { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the requested method.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string MethodName { 
            get{
                if(!ContainsKey("MethodName")) {
                    Add("MethodName", "");
                }
                return this["MethodName"].ToString();
            }
            set{
                if(!ContainsKey("MethodName")) {
                    Add("MethodName", value);
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
            Add("Error", 0);
            Add("Message", "");
        }
        /// <summary>
        /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" /> instance.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" /> instance.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2" /> instance.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info,context);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        protected JsonResponse(SerializationInfo info, StreamingContext context) : base(info,context) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="error">The error number of this Json method.  
        /// Typically 0 means no errors, any other number
        /// indicates an error.</param>
        /// <param name="message">Sets the message returned by this Json method.</param>
        public JsonResponse(int error, string message) {
            Add("Error", error);
            Add("Message", message);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="error">The error number of this Json method.  
        /// Typically 0 means no errors, any other number
        /// indicates an error.</param>
        /// <param name="message">Sets the message returned by this Json method.</param>
        /// <param name="items">Additional items returned by this Json method.</param>
        public JsonResponse(int error, string message, Dictionary<string, object> items) {
            Add("Error", error);
            Add("Message", message);
            foreach(var item in items) {
                Add(item.Key, item.Value);
            }
        }
        #region Json Invoke Methods
        internal static void InvokeJsonMethods(Mapper mapper, ref Dictionary<string, JsonResponse> results) {
            var requestedMethods = JsonConvert.DeserializeObject<JArray>(mapper.Map);
            foreach(var methodToken in requestedMethods) {
                var orignalMethodName = methodToken[0].ToString();
                var methodName = orignalMethodName;
                if(Plugin.InternalJsonMethods.ContainsKey(methodName)) {
                    var method = Plugin.InternalJsonMethods[methodName];
                    var args = (List<object>)JTokenToGeneric(methodToken[1]);
                    // make sure and create a unique name
                    // for the returned method
                    var methodNameCounter = 0;
                    if (results.ContainsKey(methodName)) {
                        // if the key with no number exists, rename the key to key_0
                        var temp = results[methodName];
                        results.Add(methodName + "_0", temp);
                        results.Remove(methodName);
                    }
                    while (results.ContainsKey(methodName + "_" + methodNameCounter)) {
                        //find the first non conflicting number for the new key
                        methodNameCounter++;
                    }
                    if (methodNameCounter > 0) {
                        methodName = methodName + "_" + methodNameCounter;
                    }
                    // check if this method has files attached to it
                    var x = mapper.Files.Where(f => f.Method == methodName && f.Instance == methodNameCounter).ToArray();
                    if (x.Length > 0) {
                        // replace sub file holders with refrence to files uploaded
                        for (var z=0;args.Count>z;z++) {
                            var t = args[z];
                            if (t is List<object> || t is string) {
                                var a = new List<object>();
                                var fa = new List<UploadedFile>();
                                if (t is List<object>) {
                                    a.AddRange((List<object>) t);
                                }
                                else {
                                    a.Add(t);
                                }
                                if (a[0] is string){;
                                    if(((string) a[0]).Contains("file:::")) {
                                        for (var j = 0; a.Count > j; j++) {
                                            // becuase HTML 5 file fileds can support more than one file per field, we must require the same
                                            var fileSigs = Regex.Split((string) a[j], "file:::");
                                            foreach (var sig in fileSigs) {
                                                if (sig.Length == 0) {
                                                    continue;
                                                }
                                                // this is a file argument.  Replace it with an uploaded file.
                                                var regex = new Regex(@"(.*)_(\d)_files_(\d+)_(\d+)");
                                                var m = regex.Match(sig);
                                                var fieldNumber = int.Parse(m.Groups[3].Value);
                                                var fileNumber = int.Parse(m.Groups[4].Value);
                                                foreach (var f in x) {
                                                    if (f.FieldNumber == fieldNumber && f.FileNumber == fileNumber) {
                                                        fa.Add(f);
                                                    }
                                                }
                                            }
                                        }
                                        if (a.Count > 0) {
                                            args[z] = fa.ToArray();
                                        }
                                    }
                                }
                            };
                        }
                    }
                    try {
                        var result = (JsonResponse)method.Invoke(null, args.ToArray());
                        result.MethodName = orignalMethodName;
                        result.Instance = methodNameCounter;
                        result.Id = Guid.NewGuid();
                        results.Add(methodName, result);
                    }catch(Exception ex){
                        var iex = Core.GetInnermostException(ex);
                        var errorResult = new JsonResponse(2, string.Format("Error calling method {1}{0}Source: {2}{0} Message: {3}{0}Stack Trace: {4}",
                            Environment.NewLine, methodName, iex.Source ,iex.Message, iex.StackTrace))
                                              {MethodName = methodName};
                        results.Add(methodName, errorResult);
                    }
                } else {
                    var errorResult = new JsonResponse(1,string.Format("Method {0} not found.",methodName)) {MethodName = methodName};
                    results.Add(methodName, errorResult);
                }
            }
        }
        #endregion
        #region Json converter methods
        /// <summary>
        /// Turns Dictionary&lt;string,object&gt; or a List&lt;object&gt; into a Json string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToJson(object obj) {
            var json = new JsonSerializer
                           {
                               NullValueHandling = NullValueHandling.Include,
                               ObjectCreationHandling = ObjectCreationHandling.Replace,
                               MissingMemberHandling = MissingMemberHandling.Ignore,
                               ReferenceLoopHandling = ReferenceLoopHandling.Error
                           };
            json.Error += delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) {
                args.ErrorContext.Handled = true;
            };
            string output;
            using(var sw = new StringWriter()) {
                var writer = new JsonTextWriter(sw) {Formatting = Formatting.Indented, QuoteChar = '"'};
                json.Serialize(writer, obj);
                output = sw.ToString();
            }
            return output;
        }
        /// <summary>
        /// Turns a JToken into a Dictionary&lt;string,object&gt; or a List&lt;object&gt;.
        /// </summary>
        /// <param name="jobject">The JObject.</param>
        /// <returns></returns>
        public static object JTokenToGeneric(object jobject)
        {
            if(jobject.GetType() == typeof(JValue)) {
                var o = (JValue)jobject;
                return o.Value;
            }
            if(jobject.GetType() == typeof(JArray))
            {
                return (from object b in jobject as JArray select JTokenToGeneric(b)).ToList();
            }
            var j = new Dictionary<string, object>();
            var jo = (JObject)jobject;
            foreach(var i in jo) {
                j.Add(i.Key, JTokenToGeneric(i.Value));
            }
            return j;
        }

        #endregion
    }
    #endregion
}