using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dashboard.DaoFactory
{
    public class ContextHandler : Spring.Context.Support.ContextHandler, System.Configuration.IConfigurationSectionHandler
    {
        #region Create
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public new object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Spring.Context.IApplicationContext IApplicationContext = getApplicationContext(section);
            return IApplicationContext;
        }
        #endregion

        #region getApplicationContext
        private Spring.Context.IApplicationContext _IApplicationContext;
        /// <summary>
        /// getApplicationContext
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private Spring.Context.IApplicationContext getApplicationContext(System.Xml.XmlNode section)
        {
            System.Xml.XmlNodeList nodes = section.SelectNodes("resource");
            string[] xmlPath = new string[nodes.Count];

            for (int index = 0; index < nodes.Count; index++)
            {
                foreach (System.Xml.XmlAttribute attribute in nodes[index].Attributes)
                {
                    if (attribute.Name.Trim().ToUpper() == "URI" || attribute.Name.Trim().ToUpper() == "URL")
                    {
                        xmlPath[index] = attribute.Value.Trim();
                        break;
                    }
                }
            }
            for (int index = 0; index < xmlPath.Length; index++)
            {
                xmlPath[index] = System.AppDomain.CurrentDomain.BaseDirectory + xmlPath[index];
            }

            if (_IApplicationContext == null)
            {
                _IApplicationContext = new Spring.Context.Support.XmlApplicationContext(xmlPath);
            }
            Spring.Context.Support.ContextRegistry.RegisterContext(_IApplicationContext);
            return _IApplicationContext;
        }
        #endregion

    }
}
