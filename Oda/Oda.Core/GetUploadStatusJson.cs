using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Oda {
    /// <summary>
    /// Gets the upload status by the Id of the upload.
    /// </summary>
    public class GetUploadStatusJson : JsonMethods {
        public static JsonResponse GetUploadStatus(string qid) {
            var j = new JsonResponse();
            // try and turn the id into a guid
            Guid id;
            if (!Guid.TryParse(qid, out id)) {
                j.Message = "Id not a valid Guid.";
                j.Error = 1;
                return j;
            }
            if(!Core.UploadStatuses.ContainsKey(id)) {
                j.Message = "Upload does not exist or status has expired.";
                j.Error = 2;
                return j;
            }
            var u = Core.UploadStatuses[id];
            j.Add("BytesRead", u.BytesRead);
            j.Add("BytesTotal", u.BytesTotal);
            j.Add("Complete", u.Complete);
            j.Add("CurrentFile", u.CurrentFile);
            j.Add("Id", u.Id);
            j.Add("LastUpdated", u.LastUpdated);
            j.Add("Message", u.Message);
            j.Add("StartedOn", u.StartedOn);
            return j;
        }
    }
}
