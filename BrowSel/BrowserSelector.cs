using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Windows.Forms;

namespace BrowSel
{
    public class BrowserSelector
    {
        private List<KeyData> sites;
        private IniData data;
        public BrowserSelector()
            : this(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),"config.ini"))
        {
        }

        public BrowserSelector(string configFile)
        {
            var parser = new FileIniDataParser();
            data = parser.ReadFile(configFile);
            sites = new List<KeyData>();
            foreach (KeyData val in data["sites"])
            {
                sites.Add(val);
            }
            sites.Sort(new LengthComparer());
        }

        class LengthComparer : IComparer<KeyData>
        {
            public int Compare(KeyData x, KeyData y)
            {
                int lengthComparison = x.KeyName.Length.CompareTo(y.KeyName.Length);
                if (lengthComparison == 0)
                {
                    return x.KeyName.CompareTo(y.KeyName);
                }
                else
                {
                    return -lengthComparison;
                }
            }
        }

        public BrowserInfo FindBrowser(string url)
        {
            url = url.Substring(url.IndexOf("://") + 3);
            foreach (KeyData element in sites)
            {
                if (url.StartsWith(element.KeyName))
                {
                    if (!data["browsers"].ContainsKey(element.Value))
                    {
                        throw new ConfigException("The browser for this site is not configured");
                    }
                    return GetBrowserInfo(element.Value);
                }
            }
            return GetDefaultBrowser();
        }

        private BrowserInfo GetBrowserInfo(string browser)
        {
            BrowserInfo bi = new BrowserInfo();
            if (data.Sections.Where(s => (s.SectionName == browser)).Count() != 0)
            {
                if (data[browser].ContainsKey("path"))
                {
                    bi.Path = data[browser]["path"];
                }
                else
                {
                    throw new ConfigException("Path is needed for Browser");
                }
                if (data[browser].ContainsKey("flags"))
                {
                    bi.Flags = data[browser]["flags"];
                }
                else
                {
                    bi.Flags = "";
                }
            }
            else
            {
                if (data["browsers"].ContainsKey(browser))
                {
                    bi.Flags = "";
                    bi.Path = data["browsers"][browser];
                }
                else
                {
                    throw new ConfigException("Cannot find browser config");
                }
            }
            return bi;

        }

        private BrowserInfo GetDefaultBrowser()
        {
            if (!(data.Sections.Where(s => (s.SectionName == "browsers")).Count() == 0 || data["browsers"].Count == 0))
            {
                if (data["browsers"].ContainsKey("default"))
                {
                    return GetBrowserInfo(data["browsers"]["default"]);
                }
                return GetBrowserInfo(data["browsers"].GetEnumerator().Current.KeyName);
            }
            IEnumerable<SectionData> sd = data.Sections.Where(s => (s.SectionName != "browsers" && s.SectionName != "sites"));
            if (sd.Count() != 0)
            {
                return GetBrowserInfo(sd.First().SectionName);
            }
            throw new ConfigException("No Browser Configured");
        }



    }
}
