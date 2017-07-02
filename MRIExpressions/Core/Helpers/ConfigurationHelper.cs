using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using NppPluginNET.Core.Rules;

namespace NppPluginNET.Core.Helpers
{
    class ConfigurationHelper
    {
        #region Static Methods
        
        /// <summary>
        /// Loads the rule file.
        /// </summary>
        /// <returns></returns>
        internal static FormatRules LoadRuleFile(string iniFilePath)
        {
            //default xml path is at the same location as the assembly
            string assemblyPath = Assembly.GetAssembly(typeof(ConfigurationHelper)).Location;
            assemblyPath = assemblyPath.Substring(0, assemblyPath.LastIndexOf(@"\"));
            string ruleFile = assemblyPath + @"\MRIFormatRules.xml";
            FormatRules rules = new FormatRules();
            if (iniFilePath != null)
            {
                StringBuilder filePathBuffer = new StringBuilder(Win32.MAX_PATH);
                Win32.GetPrivateProfileString("Format Rules", "RuleFileName", string.Empty, filePathBuffer, Win32.MAX_PATH, iniFilePath);
                if (!string.IsNullOrEmpty(filePathBuffer.ToString().Trim()))
                {
                    ruleFile = filePathBuffer.ToString().Trim();
                }

                try
                {
                    rules.LoadRules(ruleFile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Rules could not be loaded.\n\n{0}", e.Message), "Error indenting code");
                }
            }

            return rules;
        } 

        #endregion
    }
}
