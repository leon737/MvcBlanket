using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mvc.ResumingActionResults
{
    public class ResumingFileStreamResult : ResumingActionResultBase
    {
        public ResumingFileStreamResult(Stream fileStream, string contentType)
            : base(contentType)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            FileContents = fileStream;
        }
    }
}
