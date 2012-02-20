using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Mvc.ResumingActionResults
{
	public abstract class ResumingActionResultBase : ActionResult
	{
		[DefaultValue("<q1w2e3r4t5y6u7i8o9p0>")]
		public string MultipartBoundary { get; set; }
		public string ContentType { get; private set; }
		public DateTimeOffset? LastModified { get; set; }
		public string EntityTag { get; set; }
		public Stream FileContents { get; set; }

		protected ResumingActionResultBase(string contentType)
		{
			if (string.IsNullOrEmpty(contentType))
				throw new ArgumentException();

			this.ContentType = contentType;
		}

		public string CustomDisposition { get; set; }

        public string CustomDispositionType { get; set; }

        public Action<long, ResumingRequest> PartSentCallback { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");


			ResumingRequest resumingRequest = new ResumingRequest(context.HttpContext, FileContents.Length);
			ExecuteResultBody(context, resumingRequest);
		}

		public virtual void ExecuteResultBody(ControllerContext context, ResumingRequest resumingRequest)
		{
			WriteCommonHeaders(context, resumingRequest);

			if (ShouldProceedAfterEvaluatingPreconditions(context.HttpContext, resumingRequest))
			{
				using (FileContents)
				{
                    bool allSent;
					if (resumingRequest.IsRangeRequest)
                        allSent = WritePartialContent(context, FileContents, resumingRequest);
					else
                        allSent = WriteFullContent(context, FileContents);
                    if (PartSentCallback != null && allSent)
                        PartSentCallback(FileContents.Length, resumingRequest);
				}
                

			}
		}


		protected virtual bool ShouldProceedAfterEvaluatingPreconditions(HttpContextBase context, ResumingRequest resumingRequest)
		{
			var request = context.Request;
			string check;
			DateTimeOffset preconditionDateTime;

			if (!string.IsNullOrEmpty(check = (request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderIfRange)])))
			{
				if (DateTimeOffset.TryParse(check, out preconditionDateTime))
				{
					if ((LastModified.Value - preconditionDateTime).TotalSeconds > 1)
					{
						//The request had a date check; requested entity is newer so we return the full entity
						resumingRequest.Ranges = null;
					}
				}
				else
				{
					if (!check.Equals(EntityTag))
					{
						//The request had an entity tag; it didn't match our tag so we return the full entity
						resumingRequest.Ranges = null;
					}
				}
			}


			if (!string.IsNullOrEmpty(check = (request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderIfMatch)])))
			{
				IEnumerable<string> entitiesTags = check.Split(',');

				if ((string.IsNullOrEmpty(EntityTag) && entitiesTags.Count() > 0) ||
					 (!entitiesTags.Any(entity => entitiesTags.Equals(EntityTag))))
				{
					context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
					return false;
				}
			}

			//If we have an EntityTag and the header has * or a matching tag:
			//  If the request is GET or HEAD, we have to return NotModified;
			//  Else we have to return PreconditionFailed
			//Else we allow continued processing for If-Modified check
			if (!string.IsNullOrEmpty(check = (request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderIfNoneMatch)])))
			{
				IEnumerable<string> entitiesTag = check.Split(',');
				if ((!string.IsNullOrEmpty(EntityTag) && entitiesTag.Contains("*")) ||
					(entitiesTag.Any(entity => entity.Equals(this.EntityTag))))
				{
					if (context.Request.RequestType == "GET" ||
						 context.Request.RequestType == "HEAD")
					{
						context.Response.StatusCode = (int)HttpStatusCode.NotModified;
						return false;
					}
					else
					{
						context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
						return false;
					}
				}
			}


			if (!string.IsNullOrEmpty(check = (request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderIfUnmodifiedSince)])))
			{
				//If the resource changed since (valid) supplied date, return PreconditionFailed
				//Else proceed
				if (DateTimeOffset.TryParse(check, out preconditionDateTime))
				{
					if (!LastModified.HasValue ||
						((LastModified.Value - preconditionDateTime).TotalSeconds > 0))
					{
						//Resource is newer or we don't know for sure.
						context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
						return false;
					}
				}
			}


			if (!string.IsNullOrEmpty(check = (request.Headers[HttpWorkerRequest.GetKnownRequestHeaderName(HttpWorkerRequest.HeaderIfModifiedSince)])))
			{
				//If the resource changed since (valid) supplied date, return NotModified
				//Ele proceed
				if (DateTimeOffset.TryParse(check, out preconditionDateTime))
				{
					if (LastModified.HasValue)
					{
						if ((LastModified.Value - preconditionDateTime).TotalSeconds < 1)
						{
							context.Response.StatusCode = (int)HttpStatusCode.NotModified;
							return false;
						}
					}
				}
			}


			return true;
		}


		/// <summary>
		/// Writes the common headers to the response stream.
		/// </summary>
		/// <param name="context">The executing controller context.</param>
		protected virtual void WriteCommonHeaders(ControllerContext context, ResumingRequest resumingRequest)
		{
			if (resumingRequest.IsMultipartRequest)
			{
				context.HttpContext.Response.ContentType = string.Format("multipart/byteranges; boundary={0}", MultipartBoundary);
			}
			else
			{
				context.HttpContext.Response.ContentType = ContentType;
			}

			context.HttpContext.Response.AddHeader(
				 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderAcceptRanges),
				 "bytes");

			if (!string.IsNullOrEmpty(resumingRequest.FileName))
			{
					context.HttpContext.Response.AddHeader(
						  "Content-Disposition", string.Format((CustomDispositionType ?? "inline") + "; filename=\"{0}\"", 
						  !string.IsNullOrWhiteSpace(CustomDisposition) ? CustomDisposition
						  :resumingRequest.FileName));
			}

			if (!string.IsNullOrEmpty(this.EntityTag))
			{
				context.HttpContext.Response.AddHeader(
					 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderEtag),
					 this.EntityTag);
			}

			if (LastModified.HasValue)
			{
				context.HttpContext.Response.AddHeader(
					 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderLastModified),
					 LastModified.Value.ToString("R"));
			}
		}


		/// <summary>
		/// Write the full file contents to the output stream.
		/// </summary>
		/// <param name="context">The executing controller context.</param>
		/// <param name="fileContent">The stream from which the byte data is read.</param>
		public virtual bool WriteFullContent(ControllerContext context, Stream fileContent)
		{
			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

            return WriteBinaryData(context, fileContent, 0, fileContent.Length - 1) == 0;
		}


		/// <summary>
		/// Write specific byte ranges to the output stream specified by the ResumableDownloadRequest object.
		/// </summary>
		/// <param name="context">The executing controller context.</param>
		/// <param name="fileContent">The stream from which the byte data is read.</param>
		// See http://www.w3.org/Protocols/rfc1341/7_2_Multipart.html for multipart format information
		public virtual bool WritePartialContent(ControllerContext context, Stream fileContent, ResumingRequest resumingRequest)
		{
			var response = context.HttpContext.Response;

			response.StatusCode = (int)HttpStatusCode.PartialContent;

			if (!resumingRequest.IsMultipartRequest)
				context.HttpContext.Response.AddHeader(
					 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderContentRange),
					 string.Format("bytes {0}-{1}/{2}",
						  resumingRequest.Ranges.First().StartByte,
						  resumingRequest.Ranges.First().EndByte,
						  fileContent.Length)
					 );

            bool allSent = true;

			foreach (var range in resumingRequest.Ranges)
			{
				if (!response.IsClientConnected)
					return false;

				if (resumingRequest.IsMultipartRequest)
				{
					response.Output.WriteLine(string.Format("--{0}", MultipartBoundary));
					response.Output.WriteLine(string.Format("{0}: {1}",
						 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderContentType),
						 ContentType));
					response.Output.WriteLine(string.Format("{0}: bytes {1}-{2}/{3}",
						 HttpWorkerRequest.GetKnownResponseHeaderName(HttpWorkerRequest.HeaderContentRange),
						 resumingRequest.Ranges.First().StartByte,
						 resumingRequest.Ranges.First().EndByte,
						 fileContent.Length));
					response.Output.WriteLine();
				}

				allSent = WriteBinaryData(context, fileContent, range.StartByte, range.EndByte) == 0;

				if (resumingRequest.IsMultipartRequest)
					response.Output.WriteLine();

                if (!allSent) break;
			}

			if (resumingRequest.IsMultipartRequest)
			{
				response.Output.WriteLine(string.Format("--{0}--", MultipartBoundary));
				response.Output.WriteLine();
			}

            return allSent;
		}

		/// <summary>
		/// Writes byte data from <paramref name="fileContent"/> to the <paramref name="context"/> Response OutputStream
		/// starting at <paramref name="startIndex"/> and ending at <paramref name="endIndex"/> inclusive.
		/// </summary>
		/// <param name="context">The ControllerContext of the ActionResult.</param>
		/// <param name="fileContent">The Stream from which to write data to the Response.OutputStream.</param>
		/// <param name="startIndex">The position from which to start reading content.</param>
		/// <param name="endIndex">The last index position from which to read content.</param>
		private long WriteBinaryData(ControllerContext context, Stream fileContent, long startIndex, long endIndex)
		{
			var response = context.HttpContext.Response;

			byte[] buffer = new byte[0x1000];
			long totalToSend = endIndex - startIndex;
			int count = 0;

			long bytesRemaining = totalToSend + 1; //To make EndIndex inclusive
			fileContent.Seek(startIndex, SeekOrigin.Begin);

			while (response.IsClientConnected && bytesRemaining > 0)
			{
				try
				{
					if (bytesRemaining <= buffer.Length)
						count = fileContent.Read(buffer, 0, (int)bytesRemaining);
					else
						count = fileContent.Read(buffer, 0, buffer.Length);

					if (count == 0) //stream content is shorter than expected
						return -1;

					response.OutputStream.Write(buffer, 0, count);

					bytesRemaining -= count;
				}
				catch (IndexOutOfRangeException)
				{
					response.Flush();
					return -1;
				}
				finally
				{
					response.Flush();
				}
			}
            return bytesRemaining;
		}
	}
}
