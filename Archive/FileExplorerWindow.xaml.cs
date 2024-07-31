using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Archive
{
    public partial class FileExplorerWindow : Window
    {
        public string SelectedFile { get; private set; }

        public FileExplorerWindow()
        {
            InitializeComponent();
            LoadFileSystem();
        }

        private void LoadFileSystem()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                var driveNode = new TreeViewItem { Header = drive.Name, Tag = drive.RootDirectory.FullName };
                driveNode.Items.Add(null);
                driveNode.Expanded += Folder_Expanded;
                FileTreeView.Items.Add(driveNode);
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var node = sender as TreeViewItem;
            if (node.Items.Count == 1 && node.Items[0] == null)
            {
                node.Items.Clear();
                try
                {
                    var dir = new DirectoryInfo(node.Tag.ToString());
                    foreach (var directory in dir.GetDirectories())
                    {
                        var dirNode = new TreeViewItem { Header = directory.Name, Tag = directory.FullName };
                        dirNode.Items.Add(null);
                        dirNode.Expanded += Folder_Expanded;
                        node.Items.Add(dirNode);
                    }

                    foreach (var file in dir.GetFiles())
                    {
                        var fileNode = new TreeViewItem { Header = file.Name, Tag = file.FullName };
                        node.Items.Add(fileNode);
                    }
                }
                catch { }
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = FileTreeView.SelectedItem as TreeViewItem;
            if (selectedItem != null)
            {
                SelectedFile = selectedItem.Tag.ToString();
                DialogResult = true;
                Close();
            }
        }
    }
}
