using MahApps.Metro.Controls;
using Microsoft.Win32; // Для использования SaveFileDialog
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using System.Collections.Generic;
using System.IO;
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

        // Обработчик нажатия кнопки "Открыть архив"
        private void OpenArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileExplorer = new FileExplorerWindow();
            if (fileExplorer.ShowDialog() == true)
            {
                string selectedFile = fileExplorer.SelectedFile;
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    OpenArchive(selectedFile);
                }
            }
        }

        // Обработчик нажатия кнопки "Открыть по пути"
        private void OpenByPathButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (File.Exists(filePath))
            {
                OpenArchive(filePath);
            }
            else
            {
                System.Windows.MessageBox.Show("Указанный файл не существует. Проверьте путь и попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик нажатия кнопки "Извлечь архив"
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

        // Обработчик нажатия кнопки "Создать архив"
        private void CreateArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == Forms.DialogResult.OK)
            {
                string selectedFolder = folderBrowser.SelectedPath;

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Zip Archive (*.zip)|*.zip",
                    Title = "Сохранить архив"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string archivePath = saveFileDialog.FileName;

                    // Создание архива
                    using (var archive = ZipArchive.Create())
                    {
                        var options = new WriterOptions(CompressionType.Deflate)
                        {
                            LeaveStreamOpen = false
                        };

                        archive.AddAllFromDirectory(selectedFolder); // Убираем аргумент options для AddAllFromDirectory
                        archive.SaveTo(archivePath, options); // Используем options только в SaveTo
                    }

                    System.Windows.MessageBox.Show("Архив успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Метод для открытия архива и отображения его содержимого
        private void OpenArchive(string filePath)
        {
            using (var archive = ArchiveFactory.Open(filePath))
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
