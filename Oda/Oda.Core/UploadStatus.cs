using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oda {
    /// <summary>
    /// Holds status messages for uploads
    /// </summary>
    internal class UploadStatus {
        /// <summary>
        /// Gets the bytes read so far.
        /// </summary>
        /// <value>
        /// The bytes read.
        /// </value>
        public long BytesRead { get; internal set; }
        /// <summary>
        /// Gets the bytes total being uploaded.
        /// </summary>
        /// <value>
        /// The bytes total.
        /// </value>
        public long BytesTotal { get; internal set; }
        /// <summary>
        /// Gets the time the upload started on.
        /// </summary>
        /// <value>
        /// The upload start time.
        /// </value>
        public DateTime StartedOn { get; internal set; }
        /// <summary>
        /// Gets the time last updated.
        /// </summary>
        /// <value>
        /// The last updated time.
        /// </value>
        public DateTime LastUpdated { get; internal set; }
        /// <summary>
        /// Gets the status message from the server.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; internal set; }
        /// <summary>
        /// Gets the current file.
        /// </summary>
        /// <value>
        /// The current file.
        /// </value>
        public string CurrentFile { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="HttpUploadStatusEventArgs"/> is complete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if complete; otherwise, <c>false</c>.
        /// </value>
        public bool Complete { get; internal set; }
        /// <summary>
        /// Gets the unique id of this upload request.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; internal set; }
    }
}
