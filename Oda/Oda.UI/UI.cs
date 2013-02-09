using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Web;
namespace Oda.UI {
    /// <summary>
    /// A class to create a reference to fetch 
    /// embedded files from the plugin base class method
    /// </summary>
    public class UIPlugin : Plugin {
        internal static UIPlugin UIRef;
        /// <summary>
        /// Initializes a new instance of the <see cref="UIPlugin"/> class.
        /// </summary>
        public UIPlugin() {
            UIRef = this;
            Core.BeginHttpRequest += CoreBeginHttpRequest;
        }
        /// <summary>
        /// Handles the BeginHttpRequest event of the Core control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        static void CoreBeginHttpRequest(object sender, EventArgs e) {
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;
            var lPath = request.Path.ToLower();
            var tPath = lPath.Substring(7);
            if (!lPath.StartsWith("/oda.ui/")) return;
            Stream attachmentStream = new MemoryStream();
            var attachmentContent = "";
            if(IsImageFile(tPath)) {
                attachmentStream = UIRef.GetResource(tPath);
            } else {
                attachmentContent = UIRef.GetResourceString(tPath);
            }
            var mimeType = GetMimeType(tPath);
            if(attachmentContent.Length > 0) {
                response.AddHeader("Content-Length", attachmentContent.Length.ToString(CultureInfo.InvariantCulture));
                response.ContentType = mimeType + ";charset=utf-8";
                response.HeaderEncoding = Encoding.UTF8;
                response.Write(attachmentContent);
            } else if(attachmentStream != null) {
                response.AddHeader("Content-Length", attachmentStream.Length.ToString(CultureInfo.InvariantCulture));
                response.ContentType = mimeType;
                response.HeaderEncoding = Encoding.UTF8;
                var bytes = new byte[attachmentStream.Length];
                attachmentStream.Read(bytes, 0, (int)attachmentStream.Length);
                response.BinaryWrite(bytes);
            }
            response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
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
            var mimeType = "application/unknown";
            var ext = Path.GetExtension(fileName);
            if (ext == null) return mimeType;
            ext = ext.ToLower();
            var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
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
            var j = new JsonResponse();
            var lName = fileName.ToLower();
            if(UIPlugin.IsImageFile(lName)) {
                j.AttachmentStream = UIPlugin.UIRef.GetResource(fileName);
            } else {
                j.AttachmentContent = UIPlugin.UIRef.GetResourceString(fileName);
            }
            j.AttachmentFileName = fileName;
            j.ContentDisposition = JsonContentDisposition.Normal;
            j.ContentType = UIPlugin.GetMimeType(lName);
            return j;
        }
    }
}
