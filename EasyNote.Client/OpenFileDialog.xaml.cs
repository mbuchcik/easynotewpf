using EasyNote.Integration.EasyNoteAPI;
using EasyNote.Integration.EasyNoteAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyNote.Client
{
    /// <summary>
    /// Interaction logic for OpenFileDialog.xaml
    /// </summary>
    public partial class OpenFileDialog : Window
    {
        private readonly IEasyNoteService service;
        private IEnumerable<FileQueryResponse> allNotes;

        public OpenFileDialog(IEasyNoteService service)
        {
            this.service = service;
            InitializeComponent();

            try
            {
                allNotes = service.Get();
                allFiles.ItemsSource = allNotes;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        public OpenFileDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Globals.CurrentlyOpenedFile = allFiles.SelectedItem as FileQueryResponse;
            this.DialogResult = true;
        }
    }
}
