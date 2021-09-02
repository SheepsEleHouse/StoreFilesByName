using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection;

namespace StoreFilesByName
{
    public class StoreFilesByNameViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private string _FolderName = Assembly.GetEntryAssembly().Location;

        public string FolderName
        {
            get { return this._FolderName; }
            set
            { 
                this._FolderName = value;
                this.NotifyPropertyChanged(nameof(FolderName));
            }
        }

        private int _Letters = 1;

        public int Letters
        {
            get { return this._Letters; }
            set
            {
                this._Letters = value;
                this.NotifyPropertyChanged(nameof(Letters));
            }
        }


        public void SelectFolderButtonClickCallback()
        {
            this.FolderName = this.SelectFolder(this.FolderName);
        }

        private string SelectFolder(string currentFolderName)
        {
            string newFolderName = currentFolderName;
            using (var dlg = new CommonOpenFileDialog()
            {
                Title = "Select Folder",
                InitialDirectory = newFolderName,
                IsFolderPicker = true
            })
            {
                if(dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    newFolderName = dlg.FileName;
                }
            }
            return newFolderName;
        }

        public void StoreButtonClickCallback()
        {
            //MessageBox.Show("Letters:" + this.Letters.ToString());
            this.StoreFiles(this.FolderName, this.Letters);
        }

        private void StoreFiles(string directoryName, int letters)
        {
            if (Directory.Exists(directoryName))
            {
                List<string> files = new List<string>(Directory.EnumerateFiles(directoryName));
                if (MessageBox.Show(String.Format("ファイルの数は:{0}です。処理しますか？", files.Count()),"確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    files.ForEach(file => this.MoveFile(file, this.Letters));
                    MessageBox.Show("完了しました。", "メッセージ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(directoryName + "は存在しません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MoveFile(String FileName, int letters)
        {
            if (File.Exists(FileName))
            {
                int letterNum = letters;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);
                string root = Path.GetDirectoryName(FileName) + "\\";
                if(fileNameWithoutExtension.Length < letterNum) { letterNum = fileNameWithoutExtension.Length; }
                string directoryName = fileNameWithoutExtension.Substring(0, letterNum);
                string directoryPath = root + directoryName;
                if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
                File.Move(FileName, directoryPath + "\\" + Path.GetFileName(FileName));
            }
        }
    }
}
