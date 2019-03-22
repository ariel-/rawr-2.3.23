using System;
using System.Net;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Rawr
{
    public static class XMLUtils
    {

        public static XElement SelectSingleNode(this XDocument document, string path)
        {
            if (path[0] == '/') return SelectSingleNode(document.Root, path);
            else return SelectSingleNode(document.Root, path.Substring(path.IndexOf('/') + 1));
        }
        public static XElement SelectSingleNode(this XElement element, string path)
        {
            int startIndex;
            XElement current;
            if (path[0] == '/') { current = element.Document.Root; startIndex = 2; }
            else { current = element; startIndex = 0; }
            string[] pathArray = path.Split('/');
            for (int i = startIndex; i < pathArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(pathArray[i]))
                {
                    current = current.Element(pathArray[i]);
                }
                if (current == null) break;
            }
            return current;
        }

        public static IEnumerable<XElement> SelectNodes(this XDocument document, string path)
        {
            if (path[0] == '/') return SelectNodes(document.Root, path);
            else return SelectNodes(document.Root, path.Substring(path.IndexOf('/')+1));
        }
        public static IEnumerable<XElement> SelectNodes(this XElement element, string path)
        {
            int startIndex;
            XElement current;
            if (path[0] == '/') { current = element.Document.Root; startIndex = 2; }
            else { current = element; startIndex = 0; }
            string[] pathArray = path.Split('/');
            for (int i = startIndex; i < pathArray.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(pathArray[i]))
                {
                    current = current.Element(pathArray[i]);
                }
                if (current == null) break;
            }
            if (current == null || pathArray.Length == 0) return new XElement[0];
            else return current.Elements(pathArray[pathArray.Length - 1]);
        }

    }
}
