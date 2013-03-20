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
                targetPath = targetPath.Replace("~\\", Core.BaseDirectory) + files[0].OriginalFileName;
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
                    var targetPath = ((string)target).Replace("~\\", Core.BaseDirectory) + f.OriginalFileName;
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
