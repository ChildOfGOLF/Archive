using MahApps.Metro.Controls;
using Microsoft.Win32;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Forms = System.Windows.Forms; // Добавляем псевдоним для System.Windows.Forms

namespace Archive
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик нажатия кнопки "Открыть архив"
        private void OpenArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Archive files (*.zip, *.rar)|*.zip;*.rar"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Открытие архива и отображение его содержимого в ListView
                using (var archive = ArchiveFactory.Open(openFileDialog.FileName))
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

        // Обработчик нажатия кнопки "Извлечь архив"
        private void ExtractArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Archive files (*.zip, *.rar)|*.zip;*.rar"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (var archive = ArchiveFactory.Open(openFileDialog.FileName))
                {
                    var folderBrowser = new Forms.FolderBrowserDialog(); // Используем псевдоним
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
