#region Directives

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NppPluginNET.Core.Helpers;
using NppPluginNET.Core.Formatter; 

#endregion

namespace NppPluginNET
{
    public partial class PluginBase
    {
        #region Fields

        private string _iniFilePath = null;
        // bool someSetting = false;
        // frmMyDlg frmMyDlg = null;
        public int _ReindentCmdId = 0;
        Bitmap toolBarButtonBmp = Properties.Resources.MRIButton_bmp;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current scintilla.
        /// </summary>
        /// <value>The current scintilla.</value>
        internal IntPtr CurrentScintilla
        {
            get
            {
                return GetCurrentScintilla();
            }
        }

        /// <summary>
        /// Gets the ini file path.
        /// </summary>
        /// <value>The ini file path.</value>
        internal string IniFilePath
        {
            get
            {
                // get path of plugin configuration
                StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
                Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
                _iniFilePath = sbIniFilePath.ToString();

                // if config path doesn't exist, we create it
                if (!Directory.Exists(_iniFilePath))
                {
                    Directory.CreateDirectory(_iniFilePath);
                }

                // make your plugin config file full file path name
                _iniFilePath = Path.Combine(_iniFilePath, _pluginBaseName + ".ini");

                return _iniFilePath;
            }
        }

        #endregion
        
        #region " StartUp/CleanUp "

        /// <summary>
        /// Initializes the menu items in Notepad++'s Command Menu under Plugin
        /// </summary>
        void CommandMenuInit()
        {
            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            SetCommand(0, "Reindent MRI Code", ReindentCode, new ShortcutKey(true, false, true, Keys.M));
        }

        /// <summary>
        /// Sets the tool bar icon which defaults to the ReIndent
        /// </summary>
        void SetToolBarIcon()
        {
            ToolbarIcons toolbarIcons = new ToolbarIcons();
            toolbarIcons.hToolbarBmp = toolBarButtonBmp.GetHbitmap();
            IntPtr toolbarIconsPointer = Marshal.AllocHGlobal(Marshal.SizeOf(toolbarIcons));
            Marshal.StructureToPtr(toolbarIcons, toolbarIconsPointer, false);
            Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON, _funcItems.Items[_ReindentCmdId]._cmdID, toolbarIconsPointer);
            Marshal.FreeHGlobal(toolbarIconsPointer);
        }

        void PluginCleanUp()
        {
            // Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }

        #endregion
        
        #region Menu functions

        /// <summary>
        /// Reindent code.
        /// </summary>
        void ReindentCode()
        {
            int selectionLength = (int)Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETSELTEXT, 0, 0);
            if (selectionLength <= 1)
            {
                //if no selection was made, just select all
                Win32.SendMessage(CurrentScintilla, SciMsg.SCI_SELECTALL, 0, 0);
                selectionLength = (int)Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETSELTEXT, 0, 0);
            }

            if(selectionLength <= 1)
            {
                //can't format no text.
                return;
            }

            StringBuilder selectionBuffer = new StringBuilder(selectionLength);
            Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETSELTEXT, 0, selectionBuffer);

            //format the text
            string reindentedString = GetReindentString(selectionBuffer);
            StringBuilder reindentedBuffer = new StringBuilder(reindentedString, reindentedString.Length);
            if (reindentedBuffer != null)
            {
                Win32.SendMessage(GetCurrentScintilla(), SciMsg.SCI_REPLACESEL, 0, reindentedBuffer);
            }
        }

        // void myDockableDialog()
        // {
            // if (frmMyDlg == null)
            // {
                // frmMyDlg = new frmMyDlg(this);

                // NppTbData _nppTbData = new NppTbData();
                // _nppTbData.hClient = frmMyDlg.Handle;
                // _nppTbData.pszName = "My dockable dialog";
                // _nppTbData.dlgID = idMyDlg;
                // _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT;
                // _nppTbData.pszModuleName = _pluginModuleName;
                // IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                // Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                // Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            // }
            // else
            // {
                // Win32.SendMessage(nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            // }
        // }

        #endregion

        #region Methods

        /// <summary>
        /// Reindents the buffer.
        /// </summary>
        /// <param name="selectionBuffer">The selection buffer.</param>
        /// <returns></returns>
        private string GetReindentString(StringBuilder selectionBuffer)
        {
            string formattedString;
            using (MRITextFormatter formatter = new MRITextFormatter(selectionBuffer))
            {
                formatter.Rules = ConfigurationHelper.LoadRuleFile(IniFilePath);
                formatter.TabWidth = (int)Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETTABWIDTH, 0, 0);
                formatter.AutoCaseFunctions = true;

                int selStartPos = (int)Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETSELECTIONSTART, 0, 0);
                int column = (int)Win32.SendMessage(CurrentScintilla, SciMsg.SCI_GETCOLUMN, selStartPos, 0);

                formatter.SetIndentLevel(column / formatter.TabWidth);

                //get the final formatted result
                if (formatter.IsLoaded)
                {
                    formatter.FormatText();
                    formattedString = formatter.FormattedBuffer.ToString();
                }
                else
                {
                    //just pass the selection
                    formattedString = selectionBuffer.ToString();
                }
            }
            return formattedString;
        }


        #endregion
    }
}