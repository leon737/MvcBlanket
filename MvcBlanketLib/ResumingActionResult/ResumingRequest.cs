using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Mvc.ResumingActionResults
{
    public class ResumingRequest
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public ByteRange[] Ranges { get; set; }

        public bool IsRangeRequest
        {
            get { return (Ranges != null && Ranges.Length > 0); }
        }

        public bool IsMultipartRequest
        {
            get { return (Ranges != null && Ranges.Length > 1); }
        }


        public ResumingRequest(HttpContextBase context, long contentLength)
        {
            HttpRequestBase request = context.Request;

            ContentType = request.ContentType;
            
            if (!string.IsNullOrEmpty(request.FilePath))
                FileName = VirtualPathUtility.GetFileName(request.FilePath);

            ParseRequestHeaderRanges(context, contentLength);
        }


        protected virtual void ParseRequestHeaderRanges(HttpContextBase context, long contentSize)
        {
            var request = context.Request;
            string rangeHeader = request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderRange)];

            if (!string.IsNullOrEmpty(rangeHeader))
            {
                // rangeHeader contains the value of the Range HTTP Header and can have values like:
                //      Range: bytes=0-1            * Get bytes 0 and 1, inclusive
                //      Range: bytes=0-500          * Get bytes 0 to 500 (the first 501 bytes), inclusive
                //      Range: bytes=400-1000       * Get bytes 500 to 1000 (501 bytes in total), inclusive
                //      Range: bytes=-200           * Get the last 200 bytes
                //      Range: bytes=500-           * Get all bytes from byte 500 to the end
                //
                // Can also have multiple ranges delimited by commas, as in:
                //      Range: bytes=0-500,600-1000 * Get bytes 0-500 (the first 501 bytes), inclusive plus bytes 600-1000 (401 bytes) inclusive

                // Remove "Ranges" and break up the ranges
                string[] ranges = rangeHeader.Replace("bytes=", string.Empty).Split(",".ToCharArray());

                Ranges = new ByteRange[ranges.Length];

                for (int i = 0; i < ranges.Length; i++)
                {
                    const int START = 0, END = 1;
                    long parsedValue;

                    string[] currentRange = ranges[i].Split("-".ToCharArray());

                    if (long.TryParse(currentRange[END], out parsedValue))
                        Ranges[i].EndByte = parsedValue;
                    else
                        Ranges[i].EndByte = contentSize - 1;


                    if (long.TryParse(currentRange[START], out parsedValue))
                        Ranges[i].StartByte = parsedValue;
                    else
                    {
                        // No beginning specified, get last n bytes of file
                        // We already parsed end, so subtract from total and
                        // make end the actual size of the file
                        Ranges[i].StartByte = contentSize - Ranges[i].EndByte;
                        Ranges[i].EndByte = contentSize - 1;
                    }
                }
            }
        }
    }
}
