using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace DatabaseLib
{
    public static class OpenExplorer
    {
        public static bool Open(string title, string fileType, out string fileName)
        {
            fileName = "";
            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = $"{fileType} files (*.{fileType})|*.{fileType}|All files (*.*)|*.*"
            };
            if(openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
            }
            return fileName != "";
        }
    }
}
