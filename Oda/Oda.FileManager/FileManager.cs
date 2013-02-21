using System;
using System.Collections.Generic;
using System.IO;
namespace Oda {
    public class FileManager: JsonMethods {
        public static JsonResponse Upload(string targetPath, IList<UploadedFile> files) {
            var j = new JsonResponse();
            if(files.Count>1) {
                j.Error = 1;
                j.Message = "FileManager.Upload only supports one file at a time.  Use FileManager.UploadFiles instead.";
                return j;
            }
            if (files.Count == 0) {
                j.Error = 2;
                j.Message = "Source file is missing from upload request.";
                return j;
            }
            try {
                targetPath = targetPath.Replace("~\\", Core.BaseDirectory) + files[0].OrginalFileName;
                if(File.Exists(targetPath)) {
                    File.Delete(targetPath);
                }
                File.Move(files[0].Path, targetPath);
            }catch(Exception e) {
                j.Error = e.Message.GetHashCode();
                j.Message = e.Message;
                return j;
            }
            j.Error = 0;
            j.Message = "File uploaded successfully.";
            return j;
        }
        public static JsonResponse UploadFiles(IList<object> targetPaths, IList<UploadedFile> files) {
            var j = new JsonResponse();
            if (files.Count == 0) {
                j.Error = 2;
                j.Message = "Source file is missing from upload request.";
                return j;
            }
            if (targetPaths.Count != files.Count) {
                j.Error = 3;
                j.Message = "Number of target paths does not match number of files uploaded.";
                return j;
            }
            foreach (var target in targetPaths) {
                var t = ((string)target).Replace("~\\", Core.BaseDirectory);
                if(!Directory.Exists(t)){
                    j.Error = 3;
                    j.Message = string.Format("The directory {0} does not exist.",t);
                    return j;
                };

            }
            var x = 0;
            foreach(var target in targetPaths) {
                try {
                    var f = files[x++];
                    var targetPath = ((string)target).Replace("~\\", Core.BaseDirectory) + f.OrginalFileName;
                    if (File.Exists(targetPath)) {
                        File.Delete(targetPath);
                    }
                    File.Move(f.Path, targetPath);
                } catch (Exception e) {
                    j.Error = e.Message.GetHashCode();
                    j.Message = e.Message;
                    return j;
                }
            }
            j.Error = 0;
            j.Message = "File(s) uploaded successfully.";
            return j;
        }
    }
}
