using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using EmulateContacts.Resources;
using System.IO;
using Windows.Phone.PersonalInformation;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace EmulateContacts
{
    public class RemoteContact
    {
        public string RemoteId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CodeName { get; set; }
        public Stream photo { get; set; }

        public override string ToString()
        {
            return String.Format(" {0}\n {1}\n {2}\n {3}\n {4}\n {5}", RemoteId, GivenName, FamilyName, DisplayName, Email, CodeName);
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnAddContacts_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Importing contacts...";
           
            CreateContacts();

            StatusTextBlock.Text = "Done.";
        }

        private async void CreateContacts()
        {
            try
            { 
                int remoteId = 0;
                int DN = 0;
                int mail = 0;
                int n;
                n = int.Parse(txtNumberOfContacts.Text);

                for (int i = 0; i < n; i++)
                {
                    Stream myStream = App.GetResourceStream(new Uri(@"Sample.jpg", UriKind.RelativeOrAbsolute)).Stream;
                    var remoteContact = new RemoteContact
                    {
                        RemoteId = remoteId.ToString(),
                        GivenName = "S",
                        FamilyName = "NCR",
                        DisplayName = "SNCR" + DN,
                        Email = "Synchronoss" + mail + "@gmail.com",
                        CodeName = "R",
                        photo = myStream,
                        
                    };
                    remoteId++; DN++; mail++;

                    await AddContact(remoteContact);
                }
         }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task AddContact(RemoteContact remoteContact)
        {
            try 
            { 
                 ContactStore store = await ContactStore.CreateOrOpenAsync();

                 StoredContact contact = new StoredContact(store);
                 
                 RemoteIdHelper remoteIDHelper = new RemoteIdHelper();
                 contact.RemoteId = await remoteIDHelper.GetTaggedRemoteId(store, remoteContact.RemoteId);

                 contact.GivenName = remoteContact.GivenName;
                 contact.FamilyName = remoteContact.FamilyName;

                 IDictionary<string, object> props = await contact.GetPropertiesAsync();
                 props.Add(KnownContactProperties.Email, remoteContact.Email);

                 IDictionary<string, object> extprops = await contact.GetExtendedPropertiesAsync();
                 extprops.Add("Codename", remoteContact.CodeName);

               
                 if (remoteContact.DisplayName != null) contact.DisplayName = remoteContact.DisplayName;
                 IInputStream I = remoteContact.photo.AsInputStream();
                 await contact.SetDisplayPictureAsync(I);

                 Debug.WriteLine(contact.Store);

                 await contact.SaveAsync();
                 Debug.WriteLine(String.Format("Adding:\n{0}", remoteContact.ToString()));
            }

            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }
    
}

