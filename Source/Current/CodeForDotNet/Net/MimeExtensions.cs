using System;
using System.Text;
using CodeForDotNet.Collections;

namespace CodeForDotNet.Net
{
    /// <summary>
    /// Provides helper methods and constants useful for processing MIME data types and streams.
    /// </summary>
    public static class MimeExtensions
    {
        #region Constants

        /// <summary>
        /// HTTP Method used for file upload post-backs.
        /// </summary>
        public const string FileUploadHttpMethod = "post";

        /// <summary>
        /// Content Type header name.
        /// </summary>
        public const string HeaderContentType = "Content-Type";

        /// <summary>
        /// Content Length header name.
        /// </summary>
        public const string HeaderContentLength = "Content-Length";

        /// <summary>
        /// Content Disposition header name.
        /// </summary>
        public const string HeaderContentDisposition = "Content-Disposition";

        /// <summary>
        /// Content Disposition header value for an in-line file (open within browser if supported, e.g. Adobe PDF viewed in browser via ActiveX).
        /// Formatted string, only parameter is the filename without path.
        /// </summary>
        public const string HeaderContentDispositionInlineValueFormat = "inline; filename=\"{0}\"";

        /// <summary>
        /// Content Disposition header value for an attached file (opened in a new browser window, prompts the user to open/save normally).
        /// Formatted string, only parameter is the filename without path.
        /// </summary>
        public const string HeaderContentDispositionAttachmentValueFormat = "attachment; filename=\"{0}\"";

        /// <summary>
        /// Root MIME type for file uploads.
        /// </summary>
        public const string FileUploadMimeType = "multipart/form-data";

        /// <summary>
        /// MIME boundary prefix characters, also used to end the entire MIME request.
        /// </summary>
        public const string MimeBoundaryPrefix = "--";

        #endregion

        #region Methods

        /// <summary>
        /// Gets the header value alone, without any attachments or white space.
        /// </summary>
        public static string GetHeaderValue(string header)
        {
            // Validate
            if (String.IsNullOrEmpty(header))
                throw new ArgumentNullException("header");

            // Strip header value
            int firstAttribute = header.IndexOf(';');
            if (firstAttribute > 0)
                header = header.Substring(0, firstAttribute);
            return header.Trim();
        }

        /// <summary>
        /// Gets an attribute value from a header string.
        /// Attributes are appended after a semicolon at the end of header values.
        /// e.g. Content-type: multipart/mixed; boundary="BoundaryText"
        /// </summary>
        public static string GetHeaderAttribute(string header, string name)
        {
            // Validate
            if (String.IsNullOrEmpty(header))
                throw new ArgumentNullException("header");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            // Split header value and following attributes, separated by semicolons
            var attributes = header.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Search all attributes and return value if found
            for (var i = 0; i < attributes.Length; i++)
            {
                // Split into key = value
                var attributeParts = attributes[i].Split('=');

                // Ignore invalid attributes
                if (attributeParts.Length != 2)
                    continue;

                // Get key, return value if found
                var trimChars = new[] { ' ', '\"' };
                var key = attributeParts[0].Trim(trimChars);
                if (String.Compare(key, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return attributeParts[1].Trim(trimChars);
            }

            // Return null when not found
            return null;
        }

        /// <summary>
        /// Tests a MIME data buffer for a complete ASCII line.
        /// Returns the length of the current line without any CR and the LF,
        /// and the offset to the next line including any CR and the LF.
        /// </summary>
        /// <param name="buffer">Buffer to search.</param>
        /// <param name="offset">Offset to start searching the buffer.</param>
        /// <param name="length">Maximum length to search in the buffer from the offset.</param>
        /// <param name="lineSize">Returns the length of the current line without any CR and the LF, when found, else 0.</param>
        /// <param name="nextLine">Returns the index of the next line including any CR and the LF, when found, else -1.</param>
        /// <returns>
        /// True when a complete line was found, otherwise false.
        /// Blank lines are also true but have a lineSize of 0.
        /// </returns>
        public static bool FindLine(byte[] buffer, int offset, int length, out int lineSize, out int nextLine)
        {
            // Validate
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (length <= 0) throw new ArgumentOutOfRangeException("length");

            // Initialize
            lineSize = 0;
            nextLine = -1;

            // Get the length of the current line (return null when no complete line ending with LF)
            int newLineIndex = Array.IndexOf(buffer, (byte)'\n', offset, length);
            if (newLineIndex < 0)
                return false;

            // Get line size, ignoring any CR before LF
            if (newLineIndex > 0 && buffer[newLineIndex - 1] == (byte)'\r')
                lineSize = newLineIndex - offset - 1;
            else
                lineSize = newLineIndex - offset;

            // Get offset to next line
            nextLine = newLineIndex + 1;

            // Return result
            return nextLine > 0;
        }

        /// <summary>
        /// Finds a MIME boundary.
        /// </summary>
        /// <param name="buffer">Buffer to search.</param>
        /// <param name="offset">Offset to start searching the buffer.</param>
        /// <param name="length">Maximum length to search in the buffer from the offset.</param>
        /// <param name="boundary">Boundary to search for within the buffer.</param>
        /// <param name="contentEnd">Returns the index of the new line at the end of the content, before the boundary. Set to -1 when no boundary was found.</param>
        /// <param name="boundaryEnd">
        /// Returns the index of the position after the boundary end and any CR and the LF.
        /// At the end of a MIME post the "--" MIME terminator will follow the boundary without any CR and the LF,
        /// in which case the position is set to the first character of the terminator (first character after the boundary).
        /// When no boundary was found this is set to -1 when no boundary was found.</param>
        /// <returns>True if the boundary was found, otherwise false.</returns>
        public static bool FindBoundary(byte[] buffer, int offset, int length, byte[] boundary, out int contentEnd, out int boundaryEnd)
        {
            // Validate
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (length <= 0) throw new ArgumentOutOfRangeException("length");
            if (boundary == null) throw new ArgumentNullException("boundary");

            // Initialize
            contentEnd = -1;
            boundaryEnd = -1;

            // Search buffer for boundary
            var search = offset;
            while (search < offset + length - boundary.Length + 2)
            {
                // Find new line
                if (!FindLine(buffer, search, offset + length - search, out int lineSize, out int nextLine))
                    return false;

                // Check for boundary
                if (ArrayExtensions.AreEqual(buffer, nextLine, boundary, 0, boundary.Length))
                {
                    // Found boundary, is it followed by the MIME terminator?
                    boundaryEnd = nextLine + boundary.Length;
                    if (!ArrayExtensions.AreEqual(buffer, boundaryEnd, Encoding.UTF8.GetBytes(MimeBoundaryPrefix), 0, MimeBoundaryPrefix.Length))
                    {
                        // No terminator, set end after any CR and the LF
                        if (buffer[boundaryEnd] == (byte)'\r')
                            boundaryEnd++;
                        if (buffer[boundaryEnd] == (byte)'\n')
                            boundaryEnd++;
                    }
                    contentEnd = search + lineSize;
                    return true;
                }

                // Try next line...
                search = nextLine;
            }

            // Not found
            return false;
        }

        #endregion
    }
}
