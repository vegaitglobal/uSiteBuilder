using System;
using System.Collections.Generic;
using System.Xml;

namespace Vega.USiteBuilder.Types
{
    //using umbraco.NodeFactory;

    /// <summary>
    /// Implements conversion from xml to RelatedLink collection
    /// </summary>
    public class RelatedLinksConvertor : ICustomTypeConvertor
    {
        #region ICustomTypeConvertor Members

        /// <summary>
        /// Gets the Type that this convertor converts to and from
        /// </summary>
        public Type ConvertType
        {
            get
            {
                return typeof(List<RelatedLink>);
            }
        }

        /// <summary>
        /// Converts inputValue which is xml to List of RelatedLink
        /// </summary>
        /// <param name="inputValue">Input value (for example string xml)</param>
        /// <returns>
        /// List of RelatedLink created from input xml or empty list if there are no links
        /// </returns>
        public object ConvertValueWhenRead(object inputValue)
        {
            // example of input value: <links><link title="google" link="http://www.google.com" type="external" newwindow="0" /></links>
            string inputXml = Convert.ToString(inputValue);
            try
            {
                List<RelatedLink> retVal = new List<RelatedLink>();

                if (!string.IsNullOrEmpty(inputXml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(inputXml);

                    foreach (XmlNode node in doc.SelectNodes("links/link"))
                    {
                        RelatedLink rl = new RelatedLink();
                        
                        rl.Title = node.Attributes["title"].Value;
                        rl.NewWindow = node.Attributes["newwindow"].Value != "0";

                        switch (node.Attributes["type"].Value)
                        {
                            case "external":
                                rl.Type = RelatedLink.RelatedLinkType.External;
                                rl.Url = node.Attributes["link"].Value;
                                break;
                            case "internal": // points to some node so Url is nodeid
                                rl.Type = RelatedLink.RelatedLinkType.Internal;
                                rl.RelatedNodeId = int.Parse(node.Attributes["link"].Value);
                                rl.Url = new umbraco.NodeFactory.Node((int)rl.RelatedNodeId).NiceUrl;
                                break;
                            case "media":
                                rl.Type = RelatedLink.RelatedLinkType.Media;
                                rl.RelatedNodeId = int.Parse(node.Attributes["link"].Value);
                                rl.Url = Util.GetMediaUrlById((int)rl.RelatedNodeId);
                                break;
                        }

                        retVal.Add(rl);
                    }
                }

                return retVal;
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Cannot convert '{0}' to List<RelatedLink>. Error: {1}",
                    inputXml, exc.Message));
            }
        }

        /// <summary>
        /// Converts inputValue (List of RelatedLinks) to XML
        /// </summary>
        /// <param name="inputValue">Input value (for example List of RelatedLinks)</param>
        /// <returns>Output value (Xml representing related links)</returns>
        public object ConvertValueWhenWrite(object inputValue)
        {
            string retVal = "<links>{0}</links>";

            string linksXml = "";

            if (inputValue != null)
            {
                List<RelatedLink> links = (List<RelatedLink>)inputValue;

                foreach (RelatedLink link in links)
                {
                    linksXml += string.Format("<link title=\"{0}\" link=\"{1}\" type=\"{2}\" newwindow=\"{3}\" />",
                        link.Title,
                        link.Type == RelatedLink.RelatedLinkType.External ? link.Url : link.RelatedNodeId.ToString(),
                        link.Type.ToString().ToLower(),
                        link.NewWindow ? "1" : "0");
                }
            }

            retVal = string.Format(retVal, linksXml);

            return retVal;
        }

        #endregion
    }
}
