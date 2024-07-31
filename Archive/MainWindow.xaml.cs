using MahApps.Metro.Controls;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Forms = System.Windows.Forms; 

namespace Archive
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
  
        private void OpenArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new FileExplorerWindow();
            if (fileExplorer.ShowDialog() == true)
            {
                string selectedFile = fileExplorer.SelectedFile;
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    
                    using (var archive = ArchiveFactory.Open(selectedFile))
                    {
                        var files = new Dictionary<string, string>();

                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            files[entry.Key] = entry.Size.ToString();
                        }

                        FilesListView.ItemsSource = files;
                    }
                }
            }
        }

        
        private void ExtractArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new FileExplorerWindow();
            if (fileExplorer.ShowDialog() == true)
            {
                string selectedFile = fileExplorer.SelectedFile;
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    using (var archive = ArchiveFactory.Open(selectedFile))
                    {
                        var folderBrowser = new Forms.FolderBrowserDialog();
                        if (folderBrowser.ShowDialog() == Forms.DialogResult.OK)
                        {
                            string extractPath = folderBrowser.SelectedPath;

                            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                            {
                                entry.WriteToDirectory(extractPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }

                            System.Windows.MessageBox.Show("Архив успешно извлечен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }
    }
}
