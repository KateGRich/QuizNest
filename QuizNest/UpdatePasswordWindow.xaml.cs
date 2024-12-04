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
    /// Interaction logic for UpdatePasswordWindow.xaml
    /// </summary>
    public partial class UpdatePasswordWindow : Window
    {
        UserVM _user;
        IUserManager _userManager;

        bool _isNewUser;

        public UpdatePasswordWindow(UserVM user, IUserManager userManager, bool isNewUser = false)
        {
            this._user = user;
            this._userManager = userManager;

            this._isNewUser = isNewUser;

            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(_isNewUser == true)
            {
                var resultNewUser = MessageBox.Show("You must change your password to enter the QuizNest!", "Password Update Required",
                            MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if(resultNewUser == MessageBoxResult.Cancel)
                {
                    this.Close();
                }
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to abandon this form?\nYour changes will not be saved.", "Abandon Password Update?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void winUpdatePasswordWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate user's email, whether or not they are a new user.
            txtEmail.Text = _user.Email;
            txtEmail.IsEnabled = false;

            if(_isNewUser == true)
            {
                // If this is a new user, populate their current email & password.
                // Put focus on the new password field.
                winUpdatePasswordWindow.Title = "Update Your Password";
                pwdCurrentPassword.Password = "P@ssw0rd";
                pwdCurrentPassword.IsEnabled = false;
                pwdNewPassword.Focus();
            }
            else
            {
                // If not a new user, put focus on the current password field.
                pwdCurrentPassword.Focus();
            }
            
            btnSave.IsDefault = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Verify all form data.
            if(txtEmail.Text != _user.Email)
            {
                MessageBox.Show("Incorrect Email...");
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            if(pwdCurrentPassword.Password.Length < 7)
            {
                MessageBox.Show("You must enter your correct current password.", "Invalid Password");
                pwdCurrentPassword.Focus();
                pwdCurrentPassword.SelectAll();
                return;
            }

            if(pwdNewPassword.Password == pwdCurrentPassword.Password)
            {
                MessageBox.Show("Your new password cannot be the same as your current password.", "Invalid Password");
                resetNewPassword();
                return;
            }
            else if(pwdNewPassword.Password == "P@ssw0rd")
            {
                MessageBox.Show("Your new password cannot be the same as your first password.", "Invalid Password");
                resetNewPassword();
                return;
            }
            
            if(pwdNewPassword.Password.Length < 7)
            {
                MessageBox.Show("Your password must be at least 7 characters long.", "Invalid Password");
                resetNewPassword(); ;
                return;
            }
            if(string.Compare(pwdNewPassword.Password, pwdConfirmPassword.Password) != 0)
            {
                MessageBox.Show("Your new password and confirmed password must match.", "Passwords Must Match");
                resetNewPassword();
                return;
            }

            // Assign the values, now that everything has been verified.
            string email = txtEmail.Text;
            string currentPassword = pwdCurrentPassword.Password;
            string newPassword = pwdNewPassword.Password;
            try
            {
                if(_userManager.UpdatePassword(email, currentPassword, newPassword))
                {
                    MessageBox.Show("Your Password was Updated Successfully!");
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

        // Helper Methods
        private void resetNewPassword()
        {
            pwdConfirmPassword.Password = "";
            pwdNewPassword.Focus();
            pwdNewPassword.SelectAll();
        }
    }
}