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
using System.Diagnostics;
using System.Threading;

namespace StoreFilesByName
{
    public class StoreFilesByNameViewModel : INotifyPropertyChanged
    {

        /* イベント */
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /* privateフィールド */
        private CancellationTokenSource cts;
        private CommonOpenFileDialog OpenFileDialog = new CommonOpenFileDialog();


        /* プロパティおよびプロパティ用のprivateフィールド */
        //private string _FolderName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private string _FolderName = "";

        /// <summary>
        /// 選択されたフォルダ名を取得または設定します。
        /// </summary>
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

        /// <summary>
        /// キーとする文字数を取得または設定します。
        /// </summary>
        public int Letters
        {
            get { return this._Letters; }
            set
            {
                this._Letters = value;
                this.NotifyPropertyChanged(nameof(Letters));
            }
        }

        private double _Progress = 0;

        /// <summary>
        /// 進捗率(0.0～100.0)を取得または設定します。
        /// </summary>
        public double Progress
        {
            get { return this._Progress; }
            set
            {
                this._Progress = value;
                this.NotifyPropertyChanged(nameof(Progress));
            }
        }

        private string _ProgressMessage = "";

        /// <summary>
        /// 進捗に関するメッセージを取得または設定します。
        /// </summary>
        public string ProgressMessage
        {
            get
            {
                return this._ProgressMessage;
            }
            set
            {
                this._ProgressMessage = value;
                this.NotifyPropertyChanged(nameof(ProgressMessage));
            }
        }

        private bool _IsStoring = false;
        
        /// <summary>
        /// ファイルを格納処理中であるかを取得または設定します。
        /// </summary>
        public bool IsStoring
        {
            get
            {
                return this._IsStoring;
            }
            set
            {
                this._IsStoring = value;
                this.NotifyPropertyChanged(nameof(IsStoring));
                this.NotifyPropertyChanged(nameof(IsIdling));
            }
        }

        public bool IsIdling
        {
            get
            {
                return !this._IsStoring;
            }
            set
            {
                this._IsStoring = !value;
                this.NotifyPropertyChanged(nameof(IsStoring));
                this.NotifyPropertyChanged(nameof(IsIdling));
            }
        }

        /* コンストラクタ */
        public StoreFilesByNameViewModel()
        {
            this.OpenFileDialog.Title = "Select Folder";
            this.OpenFileDialog.IsFolderPicker = true;
            //this.OpenFileDialog.InitialDirectory = this.FolderName;
        }

        /* メソッド */

        /* ボタンのコールバック */

        public void SelectFolderButtonClickCallback()
        {
            this.FolderName = this.SelectFolder(this.FolderName);
        }

        public async void StoreButtonClickCallback()
        {
            this.IsStoring = true;
            this.cts = new CancellationTokenSource();
            //MessageBox.Show("Letters:" + this.Letters.ToString());
            try
            {
                await Task.Run(() => this.StoreFiles(this.FolderName, this.Letters, this.cts.Token));
            }
            catch (OperationCanceledException)
            {
                /* キャンセルされた */
            }
            finally
            {
                this.IsStoring = false;
            }
        }

        public void CancelButtonClickCallback()
        {
            this.cts.Cancel();
        }

        /* privateメソッド  */
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

        private void StoreFiles(string directoryName, int letters, CancellationToken cancellationToken)
        {
            if (Directory.Exists(directoryName))
            {
                List<string> files = new List<string>(Directory.EnumerateFiles(directoryName));
                int fileNum = files.Count();
                int count = 0;
                if (MessageBox.Show(String.Format("ファイルの数は:{0}です。処理しますか？", fileNum), "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //files.ForEach(file => this.MoveFile(file, this.Letters));
                    foreach (string file in files)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        this.MoveFile(file, this.Letters);
                        count++;
                        this.Progress = (double)count / (double)fileNum * 100;
                        this.ProgressMessage = string.Format("{0} / {1}", count, fileNum);
                    }
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
                try
                {
                    if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
                    File.Move(FileName, directoryPath + "\\" + Path.GetFileName(FileName));
                }
                catch (Exception)
                {
                    /* ディレクトリを作成できない場合はファイルをそのままにする */
                    Debug.Print(directoryPath + "を作成できませんでした。");
                }
            }
        }
    }
}
