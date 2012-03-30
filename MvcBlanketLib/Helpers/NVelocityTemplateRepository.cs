/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/


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
