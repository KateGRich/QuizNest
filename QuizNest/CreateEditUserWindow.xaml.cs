using DataDomain;
using LogicLayer;
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

namespace QuizNestPresentation
{
    /// <summary>
    /// Interaction logic for CreateEditUserWindow.xaml
    /// </summary>
    public partial class CreateEditUserWindow : Window
    {
        private UserVM? _adminUser;
        private IUserManager _userManager;

        private UserVM? _editUser;

        public CreateEditUserWindow(UserVM user, IUserManager userManager, bool isEditingSelf)
        {
            if(isEditingSelf == false)
            {
                // For Creating New Users - Admin
                this._adminUser = user;
                this._userManager = userManager;
                this._editUser = null;
            }
            else
            {
                // For Editting Your Own Info - All Users
                this._adminUser = null;
                this._userManager = userManager;
                this._editUser = user;
            }

            InitializeComponent();
        }

        // For Editing a Current User's Info - Admin
        public CreateEditUserWindow(UserVM user, IUserManager userManager, UserVM userEdit)
        {
            this._adminUser = user;
            this._userManager = userManager;
            this._editUser = userEdit; // User whose info is being editted.

            InitializeComponent();
        }

        private void winCreateEditUser_Loaded(object sender, RoutedEventArgs e)
        {
            txtUpdateEmail.Visibility = Visibility.Hidden;

            if(_adminUser == null && _editUser != null)
            {
                // User is editing their own info.
                txtUpdateEmail.Visibility= Visibility.Visible;

                if(_editUser.Roles.Contains("Admin"))
                {
                    // Admins can use the Users tab to update their own email.
                    txtUpdateEmail.Text = "If you need to update your email, please do so through the 'Users' tab!";
                }

                lblRoles.Visibility = Visibility.Hidden;
                dkpRoles.Visibility = Visibility.Hidden;
                dkpRoles.IsEnabled = false;
                stkActive.Visibility = Visibility.Hidden;
                stkActive.IsEnabled = false;

                txtGivenName.Text = _editUser.GivenName;
                txtFamilyName.Text = _editUser.FamilyName;
                txtEmail.Text = _editUser.Email;
                txtEmail.IsEnabled = false;
                txtPhoneNumber.Text = _editUser.PhoneNumber;

                winCreateEditUser.Title = "Edit My Information";
                btnCreateEditUser.Content = "Save";
            }
            else if(_adminUser != null && _editUser != null)
            {
                // Admin is editing an existing User's info.

                winCreateEditUser.Title = "Edit " + _editUser.Name + "'s Information";
                btnCreateEditUser.Content = "Save";

                txtGivenName.Text = _editUser.GivenName;
                txtFamilyName.Text = _editUser.FamilyName;
                txtEmail.Text = _editUser.Email;
                txtPhoneNumber.Text = _editUser.PhoneNumber;

                if(_editUser.Roles.Contains(chkAdmin.Content.ToString()))
                {
                    // Admins cannot take away Admin Role from other Admins.
                    chkAdmin.IsChecked = true;
                    chkAdmin.IsEnabled = false;

                    if(_editUser.UserID == _adminUser.UserID)
                    {
                        // Admin is updating themselves through this form.
                        chkActive.IsEnabled = false;
                        dtpkReactivationDate.IsEnabled = false;
                    }
                }
                if(_editUser.Roles.Contains(chkQuizMaker.Content.ToString()))
                {
                    chkQuizMaker.IsChecked = true;
                }
                if(_editUser.Roles.Contains(chkQuizTaker.Content.ToString()))
                {
                    chkQuizTaker.IsChecked = true;
                }

                stkActive.Visibility = Visibility.Visible;
                if(_editUser.Active == true)
                {
                    chkActive.IsChecked = true;
                }
                else
                {
                    chkActive.IsChecked = false;
                    dtpkReactivationDate.SelectedDate = _editUser.ReactivationDate;
                }
            }
            else
            {
                // Admin is creating a new User.

                stkActive.Visibility = Visibility.Hidden;
                stkActive.IsEnabled = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to abandon this form?\nYour changes will not be saved.", "Abandon Form?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnCreateEditUser_Click(object sender, RoutedEventArgs e)
        {
            // Validate the Form Input
            if(txtGivenName.Text.Length < 2 || txtGivenName.Text.Length > 50)
            {
                MessageBox.Show("Invalid Given Name...");
                txtGivenName.Focus();
                txtGivenName.SelectAll();
                return;
            }
            if(txtFamilyName.Text.Length < 2 || txtFamilyName.Text.Length > 50)
            {
                MessageBox.Show("Invalid Family Name...");
                txtFamilyName.Focus();
                txtFamilyName.SelectAll();
                return;
            }
            if(txtEmail.Text.Length < 5 || txtEmail.Text.Length > 250)
            {
                MessageBox.Show("Invalid Email...");
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            if(txtPhoneNumber.Text.Length > 15)
            {
                MessageBox.Show("Invalid Phone Number...");
                txtPhoneNumber.Focus();
                txtPhoneNumber.SelectAll();
                return;
            }


            List<string> roles = new List<string>();
            if(_adminUser != null)
            {
                if(chkAdmin.IsChecked == false && chkQuizMaker.IsChecked == false && chkQuizTaker.IsChecked == false)
                {
                    MessageBox.Show("You must assign at least one role to each user.");
                    dkpRoles.Focus();
                    return;
                }

                if(chkAdmin.IsChecked == true)
                {
                    roles.Add(chkAdmin.Content.ToString());
                }
                if(chkQuizMaker.IsChecked == true)
                {
                    roles.Add(chkQuizMaker.Content.ToString());
                }
                if(chkQuizTaker.IsChecked == true)
                {
                    roles.Add(chkQuizTaker.Content.ToString());
                }
            }

            // Assign the values, once we know all input is valid.
            string givenName = txtGivenName.Text;
            string familyName = txtFamilyName.Text;
            string email = txtEmail.Text;
            string? phoneNumber = txtPhoneNumber.Text;

            if(_adminUser == null && _editUser != null)
            {
                // User is editing their own info.
                bool newActive = true;
                DateTime? newReactivationDate = null;

                try
                {
                    bool result = _userManager.EditUserInformation(givenName, familyName, email, phoneNumber, newActive, newReactivationDate, _editUser, _editUser.Roles);
                    if(result == false)
                    {
                        throw new Exception("Update Failed...");
                    }
                    else
                    {
                        MessageBox.Show("Your Information was Updated Successfully!");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch(Exception ex)
                {
                    string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                    MessageBox.Show(message);
                }
            }
            else if(_adminUser != null && _editUser != null)
            {
                // Admin is editing an existing User
                
                bool newActive;
                DateTime? newReactivationDate = null;

                if(chkActive.IsChecked == false)
                {
                    newActive = false;

                    newReactivationDate = dtpkReactivationDate.SelectedDate;
                    if(newReactivationDate <= DateTime.Now)
                    {
                        MessageBox.Show("Invalid Reactivation Date...");
                        dtpkReactivationDate.Focus();
                        return;
                    }
                }
                else
                {
                    newActive = true;
                }

                try
                {
                    bool result = _userManager.EditUserInformation(givenName, familyName, email, phoneNumber, newActive, newReactivationDate, _editUser, roles);
                    if(result == false)
                    {
                        throw new Exception("Update Failed...");
                    }
                    else
                    {
                        MessageBox.Show("User Information Updated Successfully!");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch(Exception ex)
                {
                    string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                    MessageBox.Show(message);
                }
            }
            else
            {
                // Admin is creating a new User.
                try
                {
                    bool result = _userManager.AddNewUser(givenName, familyName, email, phoneNumber, roles);
                    if(result == false)
                    {
                        throw new Exception("User not added...");
                    }
                    else
                    {
                        MessageBox.Show("User Created Successfully!");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch(Exception ex)
                {
                    string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                    MessageBox.Show(message);
                }
            }
        }
    }
}