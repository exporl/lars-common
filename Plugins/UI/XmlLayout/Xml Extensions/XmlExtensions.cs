using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UI.Xml
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Convert a standard .Net XmlAttributeCollection to an XmlLayout AttributeDictionary
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static AttributeDictionary ToAttributeDictionary(this XmlAttributeCollection attributes)
        {
            AttributeDictionary dictionary = new AttributeDictionary();

            for (var x = 0; x < attributes.Count; x++)
            {
                dictionary.Add(attributes[x].Name.ToLower(), attributes[x].Value);
            }

            return dictionary;
        }
    }
}
