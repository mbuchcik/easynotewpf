using Autofac;
using EasyNote.Integration.EasyNoteAPI;
using EasyNote.Integration.EasyNoteAPI.Model;
using EasyNote.Integration.EasyNoteWebSockets;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyNote.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool TextChanged { get; set; }
        public bool FileExistsInServer { get; set; }

        private readonly IEasyNoteService service;
        private readonly EasyNoteHubService signalr;

        public MainWindow()
        {
            InitializeComponent();
            var container = InitializeIoC();
            this.service = container.Resolve<IEasyNoteService>();

            this.SetWindowName("new");
            fileContent.TextChanged += fileContent_TextChanged;

            bool loginSuccess = false;
            while (!loginSuccess)
            {
                var loginDialog = new LoginDialog();
                if (loginDialog.ShowDialog() == true)
                {
                    try
                    {
                        var logonInfo = service.Login(Globals.Credentials);
                        signalr = new EasyNoteHubService();

                        signalr.connection.On<string>("FileGotLocked", message => {
                        if (Globals.CurrentlyOpenedFile != null && Globals.CurrentlyOpenedFile.Id == int.Parse(message))
                            {
                                if (!TextChanged)
                                {
                                    Globals.CurrentlyOpenedFile.IsLocked = true;
                                    Globals.IsReadOnly = true;
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        fileContent.IsReadOnly = true;
                                    });
                                }
                                
                            }
                        });

                        signalr.connection.On<string>("FileGotUnlocked", message => {
                            if (Globals.CurrentlyOpenedFile != null && Globals.CurrentlyOpenedFile.Id == int.Parse(message))
                            {
                                Globals.CurrentlyOpenedFile.IsLocked = false;
                                Globals.IsReadOnly = false;
                                this.Dispatcher.Invoke(() =>
                                {
                                    fileContent.IsReadOnly = false;
                                 });
                            }
                        });

                        signalr.connection.On<string,string>("UnlockFileRequested", (fileId, requestor) => {
                            if (Globals.CurrentlyOpenedFile != null && Globals.CurrentlyOpenedFile.Id == int.Parse(fileId)
                            && !(requestor.ToString() == Globals.Credentials.Email || requestor.ToString() == Globals.Credentials.UserName))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    var choice = MessageBox.Show($"{requestor} wants to unlock this file, do You allow?","Unlocking request",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

                                    switch (choice)
                                    {
                                        case MessageBoxResult.Yes:
                                            signalr.UnlockFile(fileId);
                                            break;
                                        case MessageBoxResult.No:
                                            signalr.ForbidUnlockingFile(fileId, Globals.Credentials.UserName);
                                            break;
                                        default: break;
                                    }
                                });
                            }
                        });

                        signalr.connection.On<string, string>("UnlockingFileForbidden", (fileId, forbidder) => {
                            if (Globals.CurrentlyOpenedFile != null && Globals.CurrentlyOpenedFile.Id == int.Parse(fileId))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show($"{forbidder} did not allow to unlock this file.", "Unlocking forbidden",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                                });
                            }
                        });

                        signalr.connection.On("FilesListChanged", () => {
                            MessageBox.Show("File list got updated!");
                        });

                        signalr.Connect().ContinueWith(a=> { MessageBox.Show(a.Result.ToString()); });
                            loginSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        loginSuccess = false;
                        MessageBox.Show(ex.Message);
                    }

                }
                loginDialog.Close();
            }

        }
        private void SetWindowName(string v)
        {
            this.Title = v + " - Easy Note";
        }

        private IContainer InitializeIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new Integration.EasyNoteAPI.IoC.Module());
            builder.RegisterModule(new IoC.Module());

            return builder.Build();
        }

        private void fileContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged)
                if (Globals.CurrentlyOpenedFile != null)
                    signalr.LockFile(JsonConvert.SerializeObject(Globals.CurrentlyOpenedFile));

            if (!TextChanged)
            {
                TextChanged = true;
               
            }
            

        }

        private void openButton_click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog(service);
            if (openFileDialog.ShowDialog() == true)
            {
                this.Title = Globals.CurrentlyOpenedFile.Name;
                fileContent.Text = Globals.CurrentlyOpenedFile.Content;
                fileContent.IsReadOnly = Globals.CurrentlyOpenedFile.IsLocked;
                TextChanged = false;

                if (fileContent.IsReadOnly)
                {
                    var choice = MessageBox.Show($"File is currently locked, do You want to ask for unlocking?", "Unlocking request",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

                    switch (choice)
                    {
                        case MessageBoxResult.Yes:
                            signalr.RequestUnlockingFile(Globals.CurrentlyOpenedFile.Id.ToString(), Globals.Credentials.UserName);
                            break;
                        case MessageBoxResult.No:
                            break;
                        default: break;
                    }
                }
                
            }
            TextChanged = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Title == "new - Easy Note")
            {
                var newFileDialog = new NewFileDialog();
                if (newFileDialog.ShowDialog() == true)
                    this.Title = newFileDialog.Answer;
            }
            SaveOrUpdate();
        }

        private void SaveOrUpdate()
        {
            try
            {

                if (Globals.CurrentlyOpenedFile != null)
                {
                    service.Update(new UpdateFileCommand
                    {
                        Id = Globals.CurrentlyOpenedFile.Id,
                        Author = Globals.CurrentlyOpenedFile.Author,
                        Content = fileContent.Text,
                        Name = Globals.CurrentlyOpenedFile.Name
                    });
                    signalr.UnlockFile(JsonConvert.SerializeObject(Globals.CurrentlyOpenedFile));
                }
                else
                {
                    service.Add(new CreateFileCommand
                    {
                        Author = Globals.Credentials.Email,
                        Content = fileContent.Text,
                        Name = this.Title
                    });

                    var allNotes = service.Get();
                    Globals.CurrentlyOpenedFile = allNotes.Last();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShouldSaveChanges())
                this.Close();
        }

        private void newFile_Click(object sender, RoutedEventArgs e)
        {
            if (ShouldSaveChanges())
            {
                Globals.CurrentlyOpenedFile = null;
                this.SetWindowName("new");
                fileContent.Text = string.Empty;
                Globals.CurrentlyOpenedFile = null;
                TextChanged = false;

                var newFile = new NewFileDialog();
                if (newFile.ShowDialog() == true)
                {
                    this.Title = newFile.Answer;
                    newFile.Close();
                }

            }

        }

        private bool ShouldSaveChanges()
        {
            if (TextChanged)
            {
                var choice = MessageBox.Show("Do You want to save currently opened file?", "", MessageBoxButton.YesNoCancel);
                switch (choice)
                {
                    case MessageBoxResult.Yes:
                        SaveOrUpdate();
                        return true;
                    case MessageBoxResult.No:
                        return true;
                    default: return false;
                }
            }
            return true;
        }
    }
}
