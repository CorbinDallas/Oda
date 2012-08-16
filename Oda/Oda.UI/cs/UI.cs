using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Oda;
using System.Web;
namespace Oda {
    /// <summary>
    /// A class to create a refrence to fetch 
    /// embedded files from the plugin base class method
    /// </summary>
    public class UIPlugin : Oda.Plugin {
        internal static UIPlugin UIRef;
        /// <summary>
        /// Initializes a new instance of the <see cref="UIPlugin"/> class.
        /// </summary>
        public UIPlugin() {
            UIRef = this;
            Oda.Core.BeginHttpRequest += new EventHandler(Core_BeginHttpRequest);
        }
        /// <summary>
        /// Handles the BeginHttpRequest event of the Core control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Core_BeginHttpRequest(object sender, EventArgs e) {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            string lPath = request.Path.ToLower();
            string tPath = lPath.Substring(7);
            if(lPath.StartsWith("/oda.ui/")) {
                Stream AttachmentStream = new MemoryStream();
                string AttachmentContent = "";
                if(IsImageFile(tPath)) {
                    AttachmentStream = UIPlugin.UIRef.GetResrouce(tPath);
                } else {
                    AttachmentContent = UIPlugin.UIRef.GetResrouceString(tPath);
                }
                string mimeType = GetMimeType(tPath);
                if(AttachmentContent.Length > 0) {
                    response.AddHeader("Content-Length", AttachmentContent.Length.ToString());
                    response.ContentType = mimeType + ";charset=utf-8";
                    response.HeaderEncoding = Encoding.UTF8;
                    response.Write(AttachmentContent);
                } else if(AttachmentStream != null) {
                    response.AddHeader("Content-Length", AttachmentStream.Length.ToString());
                    response.ContentType = mimeType;
                    response.HeaderEncoding = Encoding.UTF8;
                    byte[] bytes = new byte[AttachmentStream.Length];
                    AttachmentStream.Read(bytes, 0, (int)AttachmentStream.Length);
                    response.BinaryWrite(bytes);
                }
                response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;// Only one file can be output at a time.
            }
        }
        /// <summary>
        /// Determines whether the path is an image file.
        /// </summary>
        /// <param name="lowerCasePath">The lower case path.</param>
        /// <returns>
        ///   <c>true</c> if  file is image file; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsImageFile(string lowerCasePath) {
            return lowerCasePath.EndsWith(".gif") ||
            lowerCasePath.EndsWith(".jpg") ||
            lowerCasePath.EndsWith(".jpeg") ||
            lowerCasePath.EndsWith(".png");
        }
        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetMimeType(string fileName) {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if(regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
    /// <summary>
    /// Methods related to UI controls.
    /// </summary>
    public class UI : JsonMethods {
        /// <summary>
        /// Gets an embedded file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static JsonResponse GetFile(string fileName) {
            JsonResponse j = new JsonResponse();
            string lName = fileName.ToLower();
            if(UIPlugin.IsImageFile(lName)) {
                j.AttachmentStream = UIPlugin.UIRef.GetResrouce(fileName);
            } else {
                j.AttachmentContent = UIPlugin.UIRef.GetResrouceString(fileName);
            }
            j.AttachmentFileName = fileName;
            j.ContentDisposition = JsonContentDisposition.Normal;
            j.ContentType = UIPlugin.GetMimeType(lName);
            return j;
        }
    }
}
