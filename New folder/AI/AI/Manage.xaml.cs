using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;

namespace AI
{
    /// <summary>
    /// Interaction logic for Manage.xaml
    /// </summary>
    
    public partial class Manage : Window
    {
        private const string subscriptionKey = "5237e26734d043cf868add67ea47460c";
        private const string baseUri = "https://smfaceverify.cognitiveservices.azure.com/";

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });
        String[] faceDescriptions;
        double resizeFactor;

        public Manage()
        {
            InitializeComponent();
            faceClient.Endpoint = baseUri;
        }        

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {            
            string persongroupName = "profiles";
            CreatePersonGroup(persongroupName);
            MessageBox.Show("New Person Group 'profiles' created!", "AI");
        }

        private async void CreatePersonGroup( string persongroupName)
        {
            try
            {
                await faceClient.PersonGroup.CreateAsync(persongroupName, persongroupName);
                IList<PersonGroup> persongroupList = await faceClient.PersonGroup.ListAsync();               
            }
            catch (APIErrorException f)
            {
                MessageBox.Show(f.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private async void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            // Get list of all people groups
            IList<PersonGroup> persongroupList = await faceClient.PersonGroup.ListAsync();
            foreach (PersonGroup pg in persongroupList)
            {
                // Delete person group 
                MessageBox.Show("Deleting " + pg.Name);
                await faceClient.PersonGroup.DeleteAsync(pg.PersonGroupId);
            }
            MessageBox.Show("Delete completed.");            
        }
       
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();
            this.Close();
        }

        private void btnCreateFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@"C:\ScienceProject\Profiles"))
            {
                Directory.CreateDirectory(@"C:\ScienceProject\Profiles");
                MessageBox.Show("Created Folders!", "AI");
            }
        }
    }
}
