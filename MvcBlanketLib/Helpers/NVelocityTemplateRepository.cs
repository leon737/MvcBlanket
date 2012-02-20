/*******************************************************************\
* Module Name: Lt.Helpers
* 
* File Name: Helpers/NVelocityTemplateRepository.cs
*
* Warnings:
*
* Issues:
*
* Created:  09 Jul 2011
* Author:   Leonid Gordo  [ leonardpt@gmail.com ]
*
\***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Commons.Collections;
using System.IO;
using NVelocity;
using NVelocity.Runtime;
using NVelocity.App;

namespace MvcBlanketLib.Helpers
{
	public interface ITemplateRepository
	{
		string RenderTemplate(string templateName, IDictionary<string, object> data);
		string RenderTemplate(string masterPage, string templateName, IDictionary<string, object> data);
	}

	public class NVelocityTemplateRepository : ITemplateRepository
	{
		private readonly string _templatesPath;

		public NVelocityTemplateRepository(string templatesPath)
		{
			_templatesPath = templatesPath;
		}

		public string RenderTemplate(string templateName, IDictionary<string, object> data)
		{
			return RenderTemplate(null, templateName, data);
		}

		public string RenderTemplate(string masterPage, string templateName, IDictionary<string, object> data)
		{
			if (string.IsNullOrEmpty(templateName))
			{
				throw new ArgumentException("The \"templateName\" parameter must be specified", "templateName");
			}

			var name = !string.IsNullOrEmpty(masterPage)
				 ? masterPage : templateName;

			var engine = new VelocityEngine();
			var props = new ExtendedProperties();
			props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, _templatesPath);
			engine.Init(props);
			var template = engine.GetTemplate(name);
			template.Encoding = Encoding.UTF8.BodyName;
			var context = new VelocityContext();

			var templateData = data ?? new Dictionary<string, object>();
			foreach (var key in templateData.Keys)
			{
				context.Put(key, templateData[key]);
			}

			if (!string.IsNullOrEmpty(masterPage))
			{
				context.Put("childContent", templateName);
			}

			using (var writer = new StringWriter())
			{
				engine.MergeTemplate(name, context, writer);
				return writer.GetStringBuilder().ToString();
			}
		}

		public string RenderTemplateContent(string templateContent, IDictionary<string, object> data)
		{
			if (string.IsNullOrEmpty(templateContent))
				throw new ArgumentException("Template content cannot be null", "templateContent");

			var engine = new VelocityEngine();
			engine.Init();

			var context = new VelocityContext();

			var templateData = data ?? new Dictionary<string, object>();
			foreach (var key in templateData.Keys)
			{
				context.Put(key, templateData[key]);
			}

		
			using (var writer = new StringWriter())
			{
				engine.Evaluate(context, writer, "", templateContent);
				return writer.GetStringBuilder().ToString();
			}
		}

	}

}
