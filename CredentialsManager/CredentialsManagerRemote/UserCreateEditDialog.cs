using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Draeger.Testautomation.CredentialsManagerCore;
using Draeger.Testautomation.CredentialsManagerCore.Extensions;

namespace Draeger.Testautomation.CredentialsManagerRemote
{
    public partial class UserCreateEditDialog : Form
    {
        private bool _allowRadioButtonEvents;

        public UserCreateEditDialog()
        {
            InitializeComponent();
        }

        public string Password
        {
            get => tbPassword.Text;
            private set => tbPassword.Text = value;
        }

        public string Username
        {
            get => tbUsername.Text;
            set => tbUsername.Text = value;
        }

        public UserGroup UserGroup
        {
            get =>
                rbAdmin.Checked ? UserGroup.Admin :
                rbService.Checked ? UserGroup.Service :
                rbSales.Checked ? UserGroup.Sales : UserGroup.Undefined;
            set
            {
                _allowRadioButtonEvents = false;
                switch (value)
                {
                    case UserGroup.Admin:
                        rbAdmin.Checked = true;
                        break;
                    case UserGroup.Service:
                        rbService.Checked = true;     
                        break;
                    case UserGroup.Sales:
                        rbSales.Checked = true;
                        break;
                    case UserGroup.Undefined:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _allowRadioButtonEvents = true;
            }
        }

        private void rbAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (!_allowRadioButtonEvents) return;
            if ((RadioButton) sender != rbAdmin) return;
            if (rbAdmin.Checked)
            {
                UserGroup = UserGroup.Admin;
            }
        }

        private void rbSales_CheckedChanged(object sender, EventArgs e)
        {
            if (!_allowRadioButtonEvents) return;
            if ((RadioButton)sender != rbSales) return;
            if (rbSales.Checked)
            {
                UserGroup = UserGroup.Sales;
            }
        }

        private void rbService_CheckedChanged(object sender, EventArgs e)
        {
            if (!_allowRadioButtonEvents) return;
            if ((RadioButton)sender != rbService) return;
            if (rbService.Checked)
            {
                UserGroup = UserGroup.Service;
            }
        }

        private void rbUnassigned_CheckedChanged(object sender, EventArgs e)
        {
            if (!_allowRadioButtonEvents) return;
            if ((RadioButton)sender != rbUnassigned) return;
            if (rbUnassigned.Checked)
            {
                UserGroup = UserGroup.Undefined;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {

        }
    }
}
