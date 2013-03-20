using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Oda {
    public partial class Core {
        /// <summary>
        /// Gets the boundary data from a form post.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">Malformed form-data.  Boundary cannot be greater than 60 characters.</exception>
        static byte[] GetBoundary(byte[] b) {
            // read until the first CR
            var r = new List<byte>();
            var i = 0;
            while (b[i] != 13) {
                r.Add(b[i]);
                i++;
                if (i > 60) {
                    throw new FormatException("Malformed form-data.  Boundary cannot be greater than 60 characters.");
                }
            }
            return r.ToArray();
        }
        /// <summary>
        /// Finds the position of a bit array in in a bit stream.
        /// </summary>
        /// <param name="haystack">The haystack.</param>
        /// <param name="needle">The needle.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        static int FindPosition(Stream haystack, byte[] needle, long offset) {
            int b;
            var i = 0;
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
        /// <summary>
        /// Updates the upload status event.
        /// </summary>
        /// <param name="d">The <see cref="HttpUploadStatusEventArgs"/> instance containing the event data.</param>
        /// <param name="bytesRead">The bytes read.</param>
        /// <param name="message">The message.</param>
        void updateUploadStatus(ref HttpUploadStatusEventArgs  d, long bytesRead, string message) {
            d.LastUpdated = DateTime.Now;
            d.Message = message;
            d.BytesRead = bytesRead;
            RaiseOnUploadStatus(d);
        }
        /// <summary>
        /// Parses the request into files then parses the files into form fields and uploaded files.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <returns></returns>
        Mapper ParseRequest(HttpWorkerRequest r) {
            // get the ID of the upload from the querystring if any
            var qid = r.GetQueryString();
            Guid id;
            if(!Guid.TryParse(qid, out id)) {
                throw new HttpParseException("Upload contains no ID.");
            }
            var d = new HttpUploadStatusEventArgs() {
                Id = id,
                StartedOn = DateTime.Now, 
                LastUpdated = DateTime.Now, 
                Message = "Beginning Upload."
            };
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
            const int f = 131072; // 128kb = 131072b
            var l = r.GetTotalEntityBodyLength();
            d.BytesTotal = l;
            var p = r.GetPreloadedEntityBody();
            var b = GetBoundary(p);
            updateUploadStatus(ref d,l,"Uploading preloaded entity body.");
            // load stream into temp file
            var fst = Path.GetTempFileName();
            using (var fs = new FileStream(fst, FileMode.OpenOrCreate)) {
                // write preloaded body to file
                Debug.Assert(p != null, "GetPreloadedEntityBody != null");
                fs.Write(p, 0, p.Length);
                var c = p.Length;
                var q = 0;
                var u = new byte[f];
                updateUploadStatus(ref d, l, "Uploading.");
                while (l - c > q) {
                    q = r.ReadEntityBody(u, 0, f);
                    fs.Write(u, 0, q);
                    c += q;
                    updateUploadStatus(ref d, c, "Uploading.");
                }
                if (l - c > 0) {
                    var ux = new byte[l - c];
                    q = r.ReadEntityBody(ux, 0, l - c);
                    fs.Write(ux, 0, q);
                }
                fs.Flush();
                fs.Position = 0;
                updateUploadStatus(ref d, c, "Upload Complete, Parsing upload.");
                // read the entire file finding all boundaries
                var s = new List<long>();
                while (fs.Position < fs.Length) {
                    s.Add(FindPosition(fs, b, fs.Position));
                }
                fs.Position = 0;
                // the last boundary is the eof
                for (var i = 0; s.Count - 1 > i; i++) {
                    // indexes between boundaries - this is the new file size in bytes
                    if (s[i + 1] == -1) {
                        break;
                    }
                    var z = s[i + 1] - s[i];
                    // this is the size of the object between the boundaries (including current boundary)
                    var g = (z) < f ? z : f;
                    // chunk size (g) = 131072 (f) or next boundary pos - current boundary pos (z)
                    var n = Path.GetTempFileName();
                    fs.Position = s[i]; // start reading from the beginning position of the object (including boundary)
                    using (var a = new FileStream(n, FileMode.OpenOrCreate)) {
                        q = 0;
                        c = 0;
                        while (z - c > q) {
                            //read blocks while position - mod block size  is less than end position
                            q = fs.Read(u, 0, (int)g);
                            a.Write(u, 0, q);
                            c += q;
                        }
                        q = fs.Read(u, 0, (int)z - c);
                        a.Write(u, 0, q);
                        a.Position = 0;
                        // read in form data
                        if (!isMapFound) {
                            if (FindPosition(a, mapSig, 0) > -1) {
                                // this is the map field
                                var mapBytes = new byte[a.Length - a.Position - 2]; // -2 to drop the \r\n
                                a.Read(mapBytes, 0, mapBytes.Length);
                                m.Map = e.GetString(mapBytes);
                                isMapFound = true;
                                continue;
                            }
                        }
                        if (!isIdFound) {
                            if (FindPosition(a, idSig, 0) > -1) {
                                // id is always 36 bytes
                                var idBytes = new byte[36];
                                a.Read(idBytes, 0, 36);
                                m.Id = Guid.Parse(e.GetString(idBytes));
                                isIdFound = true;
                                continue;
                            }
                        }
                        // this must be a binary file attachment, rip file apart
                        a.Position = 0;
                        //1) remove boundary 
                        while (a.ReadByte() != 13) { }
                        a.Position++; // read past lf
                        //2) parse form data
                        var startOfFormData = a.Position;
                        while (a.ReadByte() != 13) { }
                        a.Position++; // read past lf
                        var endOfFormdata = a.Position;
                        a.Position = startOfFormData;
                        // create a byte array to hold form data
                        var formDataBuffer = new byte[endOfFormdata - startOfFormData];
                        a.Read(formDataBuffer, 0, formDataBuffer.Length);
                        var formData = e.GetString(formDataBuffer);
                        // form data looks like Content-Disposition: form-data; name="Authentication.CreateAccount_files_0_0"; filename="tiny.gif"
                        //3) parse content type
                        a.Position = endOfFormdata;
                        while (a.ReadByte() != 13) { }
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
                        var remainingChunckSize = binLength % bufferSize;
                        var bytesRead = 0L;
                        var tempFileName = Path.GetTempFileName();
                        using (var bin = new FileStream(tempFileName, FileMode.OpenOrCreate)) {
                            while (binLength - remainingChunckSize > bytesRead) {
                                var buffer = new byte[bufferSize];
                                bytesRead += a.Read(buffer, 0, (int)bufferSize);
                                bin.Write(buffer, 0, (int)bufferSize);
                            }
                            if (remainingChunckSize > 0) {
                                var buffer = new byte[remainingChunckSize];
                                a.Read(buffer, 0, (int)remainingChunckSize);
                                bin.Write(buffer, 0, (int)remainingChunckSize);
                            }
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
                            var fm = new UploadedFile {
                                ContentType = contentType.Replace("Content-Type: ", ""),
                                // remove the words "Content-Type: " from value
                                Path = tempFileName,
                                FileNumber = int.Parse(fileNumber),
                                Instance = int.Parse(methodInstance),
                                Method = method,
                                FieldNumber = int.Parse(fileField),
                                OriginalFileName = oFileName
                            };
                            m.Files.Add(fm);
                        }
                    }
                    // delete temp file
                    File.Delete(n);
                }
            }
            File.Delete(fst);
            d.BytesTotal = 0;
            d.BytesRead = 0;
            d.Complete = true;
            d.LastUpdated = DateTime.Now;
            d.Message = "Upload Complete.";
            RaiseOnUploadStatus(d);
            return m;
        }
    }
}   
