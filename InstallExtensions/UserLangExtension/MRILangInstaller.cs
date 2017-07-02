#region Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace UserLangExtension
{
    [RunInstaller(true)]
    public partial class MRILangInstaller : Installer
    {
        #region Constructors and Destructors
        
        public MRILangInstaller()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, performs the installation.
        /// </summary>
        /// <param name="stateSaver">An <see cref="T:System.Collections.IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="stateSaver"/> parameter is null. </exception>
        /// <exception cref="T:System.Exception">An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.-or- An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection. </exception>
        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            string appDataFolder = this.Context.Parameters["appdatafolder"];
#if DEBUG
            MessageBox.Show(appDataFolder); //to launch the debugger
#endif
            string filepath = string.Format(@"{0}\userDefineLang.xml", appDataFolder);
            try
            {
                XDocument langDocument = XDocument.Load(filepath);
                XDocument embeddedDoc = XDocument.Parse(Properties.Resources.userDefineLang, LoadOptions.None);

                XElement currentLang = langDocument.Descendants("UserLang").FirstOrDefault(element => element.Attribute("name").Value == "MRILang");
                XElement embeddedLang = embeddedDoc.Descendants("UserLang").FirstOrDefault(element => element.Attribute("name").Value == "MRILang");

                if (currentLang != null)
                {
                    currentLang.ReplaceWith(embeddedLang);
                }
                else
                {
                    //add the lang
                    langDocument.Root.Add(embeddedLang);
                }
                
                //write the changed file contents
                StreamWriter toStream = new StreamWriter(filepath);
                toStream.Write(langDocument.ToString(SaveOptions.None));
                toStream.Close();
            }
            catch (FileNotFoundException)
            {
                //the file could not be found, write the embedded file to disk instead.
                StreamWriter toStream = new StreamWriter(filepath);
                toStream.Write(Properties.Resources.userDefineLang);
                toStream.Close();
            }
            catch (NullReferenceException)
            {
                //install failed. mention to place the userdefine lang in the folder manually
                //we don't want to crash the install
            }
        }

        /// <summary>
        /// When overridden in a derived class, removes an installation.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after the installation was complete.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        ///   </exception>
        ///   
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred while uninstalling. This exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstallation completes.
        ///   </exception>
        [SecurityPermission(SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            //reomve the userlang from disk
            string appDataFolder = this.Context.Parameters["appdatafolder"];
            string filepath = string.Format(@"{0}\userDefineLang.xml", appDataFolder);
            try
            {
                XDocument langDocument = XDocument.Load(filepath);
                XElement currentLang = langDocument.Descendants("UserLang").FirstOrDefault(element => element.Attribute("name").Value == "MRILang");

                if (currentLang != null)
                {
                    currentLang.Remove();
                }

                if (langDocument.Descendants("UserLang").Count() > 0)
                {
                    //write the changed file contents
                    StreamWriter toStream = new StreamWriter(filepath);
                    toStream.Write(langDocument.ToString(SaveOptions.None));
                    toStream.Close();
                }
                else
                {
                    //delete the file because we do not want a blank lang file
                    File.Delete(filepath);
                }
            }
            catch (Exception)
            {
                //pretend as if nothing happened hehe
            }
        }

        #endregion
    }
}
