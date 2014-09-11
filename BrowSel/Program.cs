using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace BrowSel
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                MessageBox.Show("This tool can only be called with one parameter");
                return;
            }
            try
            {
                BrowserSelector bs = new BrowserSelector();
                BrowserInfo browser = bs.FindBrowser(args[0]);
                try
                {
                    Process.Start(browser.Path,
                        string.Format("{0} \"{1}\"", browser.Flags, args[0]));
                }
                catch (Win32Exception we)
                {
                    throw new ConfigException("The browser " + browser + " was not found! " + we.ToString());
                }
            }
            catch (ConfigException e)
            {
                MessageBox.Show("The config is incorrect: " + e.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
