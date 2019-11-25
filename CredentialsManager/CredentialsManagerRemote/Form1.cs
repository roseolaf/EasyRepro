using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;
using Draeger.Testautomation.CredentialsManagerRemote;

namespace CredentialsManagerRemote
{
    public partial class Form1 : Form
    {
        private SynchronizationContext _synchronizationContext;
        private readonly KeyVaultConnector _keyVaultConnector;
        private BindingList<UserListItem> binding;

        public Form1()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            _keyVaultConnector = new KeyVaultConnector
            {
                //TODO: find out if it's possible to log in via username and password instead of using service principal
                ClientId = Environment.GetEnvironmentVariable("akvClientId"), 
                ClientSecret = Environment.GetEnvironmentVariable("akvClientSecret")
            };
            _keyVaultConnector.Connect();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            binding = new BindingList<UserListItem>();
            UpdateBinding();
            lbUsers.DataSource = binding;
            lbUsers.ValueMember = "Username";
            lbUsers.DisplayMember = "DisplayName";
        }

        private void UpdateBinding()
        {
            var users = CredentialsManager.GetUsers(_keyVaultConnector);
            
            binding.Clear();
            foreach (var user in users)
            {
                binding.Add(new UserListItem
                {
                    Username = user.Key,
                    UserGroup = user.Value
                });
            }

        }

        private async void btAdd_Click(object sender, EventArgs e)
        {
            var dia = new UserCreateEditDialog();
            if (dia.ShowDialog() != DialogResult.OK) return;
            await CredentialsManager.Instance.AddUserCredentials(_keyVaultConnector, dia.Username, dia.Password, dia.UserGroup);
            UpdateBinding();
        }

        private async void btEdit_Click(object sender, EventArgs e)
        {
            if (lbUsers.SelectedIndex < 0) return;

            var dia = new UserCreateEditDialog();
            dia.Username = lbUsers.SelectedValue.ToString();
            dia.UserGroup = ((UserListItem) lbUsers.SelectedItem).UserGroup;
            if (dia.ShowDialog() != DialogResult.OK) return;
            await CredentialsManager.Instance.UpdateUserCredentials(_keyVaultConnector, lbUsers.SelectedValue.ToString(), dia.Username, dia.Password, dia.UserGroup);
            UpdateBinding();
        }

        private async void btRemove_Click(object sender, EventArgs e)
        {
            if (lbUsers.SelectedIndex < 0) return;
            await CredentialsManager.Instance.DeleteUserCredentials(_keyVaultConnector, lbUsers.SelectedValue.ToString());
            UpdateBinding();
        }
    }
}
