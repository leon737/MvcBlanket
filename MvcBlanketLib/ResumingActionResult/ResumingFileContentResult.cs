using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mvc.ResumingActionResults
{
    public class ResumingFileContentResult : ResumingActionResultBase
    {
        public ResumingFileContentResult(byte[] fileContents, string contentType)
            : base(contentType)
        {
            if (fileContents == null)
                throw new ArgumentNullException("fileContents");

            FileContents = new MemoryStream(fileContents);
        }
    }
}
