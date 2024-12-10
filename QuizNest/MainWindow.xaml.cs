using Azure.Core;
using DataAccessFakes;
using DataDomain;
using LogicLayer;
using Microsoft.Win32;
using QuizNestPresentation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizNest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserVM _user = null;

        IUserManager _userManager = new UserManager();
        List<UserVM> _users;

        IQuizManager _quizManager = new QuizManager();
        List<QuizVM> _quizzes;

        IQuestionManager _questionManager = new QuestionManager();


        IQuizRecordManager _quizRecordManager = new QuizRecordManager();
        List<QuizRecordVM> _takenQuizzes;

        IChatManager _chatManager = new ChatManager();
        // For Users who have started Chats with an Admin
        List<ChatVM> _startedChats;
        // For Admins
        List<ChatVM> _receivedChats;

        public MainWindow()
        {
            InitializeComponent();
            txtEmail.Text = "phill@tomlandryms.edu";
            pwdPassword.Password = "P@ssw0rd";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            resetLogIn();
        }

        // Log In & Out Methods
        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            // Test with Data Fake
            //var userManager = new UserManager(new UserAccessorFake());

            string email = txtEmail.Text;
            string password = pwdPassword.Password;

            // Check that entered values are at least 8 characters long ("P@ssw0rd") and not longer than is allowed in the DB.
            if(email.Length < 8 || email.Length > 250)
            {
                MessageBox.Show("Invalid Email...");
                txtEmail.Focus();
                txtEmail.SelectAll();
                return;
            }
            if(password.Length < 8 || password.Length > 100)
            {
                MessageBox.Show("Invalid Password...");
                pwdPassword.Focus();
                pwdPassword.SelectAll();
                return;
            }

            // Log In
            try
            {
                // Verify the user's account.
                _user = _userManager.LogInUser(email, password);

                // Force New User to Reset their Password
                if(password == "P@ssw0rd")
                {
                    var updatePasswordWindow = new UpdatePasswordWindow(_user, _userManager, isNewUser : true);
                    if(updatePasswordWindow.ShowDialog() == false)
                    {
                        return;
                    }
                }

                statMessage.Content = $"Welcome, {_user.GivenName}!"; // String interpolation

                toggleLogInControls(false);

                btnLogOut.Visibility = Visibility.Visible;

                showUserTabs();
                
            }
            catch(Exception ex)
            {
                string message = ex.Message;
                if(ex.InnerException != null)
                {
                    message += "\n\n" + ex.InnerException.Message;
                }
                MessageBox.Show(message, "Failed to Log In...");
            }
        }
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            _user = null;

            toggleLogInControls(true);
            statMessage.Content = "Log in to get cozy in the QuizNest!";

            resetLogIn();
        }


        // User Tab Methods - Admin Only
        private void grdUsers_Loaded(object sender, RoutedEventArgs e)
        {
            showAllUsers();
        }

        private void btnCreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            var createUserWindow = new CreateEditUserWindow(_user, _userManager, false);
            var result = createUserWindow.ShowDialog();
            if(result == true)
            {
                _users = _userManager.GetAllUsers();
                grdUsers.ItemsSource = _users;
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if(grdUsers.SelectedItem != null)
            {
                var editUserWindow = new CreateEditUserWindow(_user, _userManager, (grdUsers.SelectedItem as UserVM));
                var result = editUserWindow.ShowDialog();
                if(result == true)
                {
                    if((grdUsers.SelectedItem as UserVM).UserID == _user.UserID)
                    {
                        // If the Admin is updating their own information through this form,
                        // refresh their information on the Profile tab.
                        _user = _userManager.GetUserByUserID(_user.UserID);
                        _user.Roles = _userManager.GetUserRoles(_user.UserID);
                    }

                    _users = _userManager.GetAllUsers();
                    grdUsers.ItemsSource = _users;
                }
            }
            else
            {
                MessageBox.Show("You must select a user in order to edit their information.", "No Selection");
            }
            
        }


        // Quiz Tab Methods
        private void btnCreateNewQuiz_Click(object sender, RoutedEventArgs e)
        {
            var createQuizWindow = new CreateEditQuizWindow(_user, _quizManager, _questionManager);
            var result = createQuizWindow.ShowDialog();
            if(result == true)
            {
                showCreatedQuizzes();
            }
        }
        private void btnEditQuiz_Click(object sender, RoutedEventArgs e)
        {
            if(grdMyQuizzes.SelectedItem != null)
            {
                var editQuizWindow = new CreateEditQuizWindow(_user, (grdMyQuizzes.SelectedItem as QuizVM), _quizManager, _questionManager);
                var result = editQuizWindow.ShowDialog();
                if(result == true)
                {
                    showCreatedQuizzes();
                }
            }
            else
            {
                MessageBox.Show("You must select a quiz in order to edit one.", "No Selection");
            }
        }



        private void btnLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            if(grdAllQuizzes.SelectedItem != null)
            {
                var quiz = grdAllQuizzes.SelectedItem as Quiz;

                var leaderboardWindow = new LeaderboardWindow(_quizRecordManager, quiz);
                leaderboardWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must select a quiz in order to view a leaderboard.", "No Selection");
            }
        }
        private void btnTakeQuiz_Click(object sender, RoutedEventArgs e)
        {
            var takeQuizWindow = new TakeQuizWindow();
            takeQuizWindow.ShowDialog();
        }



        private void btnRetakeQuiz_Click(object sender, RoutedEventArgs e)
        {
            if(grdTakenQuizzes.SelectedItem != null)
            {
                var reviewRetakeWindow = new ReviewRetakeWindow();
                reviewRetakeWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must select a quiz in order to retake one.", "No Selection");
            }
        }



        // Profile Tab Methods
        private void tabProfile_GotFocus(object sender, RoutedEventArgs e)
        {
            txtGivenName.Text = _user.GivenName;
            txtFamilyName.Text = _user.FamilyName;
            txtUserEmail.Text = _user.Email;
            txtPhoneNumber.Text = _user.PhoneNumber;
            txtRoles.Text = _user.RoleList;
        }
        private void btnEditInfo_Click(object sender, RoutedEventArgs e)
        {
            var createEditUserWindow = new CreateEditUserWindow(_user, _userManager, true);
            var result = createEditUserWindow.ShowDialog();
            if(result == true)
            {
                // Refresh their information on the Profile tab.
                _user = (UserVM)_userManager.GetUserByEmail(_user.Email);
                _user.Roles = _userManager.GetUserRoles(_user.UserID);

                txtGivenName.Text = _user.GivenName;
                txtFamilyName.Text = _user.FamilyName;
                txtPhoneNumber.Text = _user.PhoneNumber;
            }
        }
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            var updatePasswordWindow = new UpdatePasswordWindow(_user, _userManager);
            updatePasswordWindow.ShowDialog();
        }
        private void btnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account?", "Delete Account?",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                // Save the current user into a new variable.
                UserVM user = _user;

                // Force log-out the current user.
                this.btnLogOut_Click(sender, e);

                // Let them know their account has been deleted/deactivated successfully.
                MessageBox.Show("Your account was successfully deleted!");

                // Reuse this method to explicitly save their current info, but set their Active status to false & their ReactivationDate to null.
                _userManager.EditUserInformation(user.GivenName, user.FamilyName, user.Email, user.PhoneNumber, false, null, user, user.Roles);
            }
        }



        // Chat Tab Methods
        private void btnViewMessages_Click(object sender, RoutedEventArgs e)
        {
            if(grdMyChats.SelectedItem != null)
            {
                var viewSendMessagesWindow = new ViewSendMessagesWindow();
                viewSendMessagesWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must select a chat in order to view messages.", "No Selection");
            }
        }


        // Tab Set Selection Change Methods
        private void tabSetMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

                switch(tabItem)
                {
                    case "Users":
                        break;
                    case "Quizzes":
                        break;
                    case "My Profile":
                        break;
                    case "Chat With Admin":
                        break;
                    case "Chats":
                        break;
                }
            }
        }
        private void tabSetQuizzes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

                switch(tabItem)
                {
                    case "My Quizzes":
                        showCreatedQuizzes();
                        break;
                    case "All Quizzes":
                        showActiveQuizzes();
                        break;
                    case "Taken Quizzes":
                        showTakenQuizzes();
                        break;
                }
            }
        }
        private void tabSetChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

                switch(tabItem)
                {
                    case "Start New":
                        break;
                    case "My Chats":
                        if(_user.Roles.Contains("Admin"))
                        {
                            showReceivedChats();
                        }
                        else
                        {
                            showStartedChats();
                        }
                        break;
                }
            }
        }


        // Helper Methods
        void toggleLogInControls(bool isVisible)
        {
            lblEmail.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            txtEmail.Text = "";
            txtEmail.IsEnabled = isVisible ? true : false;
            txtEmail.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

            lblPassword.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            pwdPassword.Password = "";
            pwdPassword.IsEnabled = isVisible ? true : false;
            pwdPassword.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

            btnLogIn.IsDefault = isVisible ? true : false;
            btnLogIn.IsEnabled = isVisible ? true : false;
            btnLogIn.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }
        private void resetLogIn()
        {
            txtEmail.Focus();
            btnLogIn.IsDefault = true;
            btnLogOut.Visibility = Visibility.Hidden;

            tabContainer.Visibility = Visibility.Hidden;
            foreach(TabItem item in tabSetMain.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach(TabItem item in tabSetQuizzes.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach(TabItem item in tabSetChats.Items)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }
        private void showUserTabs()
        {
            tabQuizzes.Visibility = Visibility.Visible;
            tabAllQuizzes.Visibility = Visibility.Visible;

            // Role-based Access.
            foreach(string role in _user.Roles)
            {
                switch(role)
                {
                    case "Admin":
                        tabUsers.Visibility = Visibility.Visible;
                        tabUsers.IsSelected = true;

                        tabQuizzes.Visibility = Visibility.Visible;
                        tabAllQuizzes.IsSelected = true;
                        tabMyQuizzes.Visibility = Visibility.Collapsed;
                        tabTakenQuizzes.Visibility = Visibility.Collapsed;
                        btnTakeQuiz.Visibility = Visibility.Hidden;
                        btnTakeQuiz.IsEnabled = false;

                        tabStartChat.Visibility = Visibility.Collapsed; // Admins cannot start chats.
                        tabMyChats.Visibility = Visibility.Visible;
                        tabMyChats.IsSelected = true;
                        break;
                    case "Quiz Maker":
                        tabQuizzes.IsSelected = true;
                        tabTakenQuizzes.Visibility = Visibility.Collapsed;
                        btnTakeQuiz.Visibility = Visibility.Hidden;
                        btnTakeQuiz.IsEnabled = false;
                        tabMyQuizzes.Visibility = Visibility.Visible;
                        tabMyQuizzes.IsSelected = true;

                        tabStartChat.Visibility = Visibility.Visible;
                        tabMyChats.Visibility = Visibility.Visible;
                        tabStartChat.IsSelected = true;
                        break;
                    case "Quiz Taker":
                        tabQuizzes.IsSelected = true;
                        btnTakeQuiz.Visibility = Visibility.Visible;
                        btnTakeQuiz.IsEnabled = true;
                        tabMyQuizzes.Visibility = Visibility.Collapsed;
                        tabTakenQuizzes.Visibility = Visibility.Visible;
                        tabTakenQuizzes.IsSelected = true;

                        tabStartChat.Visibility = Visibility.Visible;
                        tabMyChats.Visibility = Visibility.Visible;
                        tabStartChat.IsSelected = true;
                        break;
                }
            }

            if(_user.Roles.Contains("Admin"))
            {
                // If user is an Admin, but has multiple roles - this will force them to
                // default to the Users tab, despite their other roles.
                tabUsers.IsSelected = true;
                tabChat.Header = "Chats";

                // Admins cannot start chats, even if they have multiple roles.
                tabStartChat.Visibility = Visibility.Collapsed;
                tabMyChats.IsSelected = true;
            }
            else
            {
                // If an admin was logged in, but doesn't exit the application completely,
                // reset the Header on tabChat to what non-Admins would see.
                tabChat.Header = "Chat With Admin";
            }

            if(_user.Roles.Contains("Quiz Maker"))
            {
                // User has multiple roles, but is a Quiz Maker - default the Quizzes tab to My Quizzes.
                tabMyQuizzes.Visibility = Visibility.Visible;
                tabMyQuizzes.IsSelected = true;
            }
            else if(_user.Roles.Contains("Quiz Taker"))
            {
                // User has multiple roles, but is NOT a Quiz Maker - default the Quizzes tab to Taken Quizzes.
                tabMyQuizzes.Visibility = Visibility.Collapsed;
                tabTakenQuizzes.IsSelected = true;
            }

            tabProfile.Visibility = Visibility.Visible;
            tabChat.Visibility = Visibility.Visible;
            tabContainer.Visibility = Visibility.Visible;
        }

        private void showAllUsers()
        {
            try
            {
                _users = _userManager.GetAllUsers();
                grdUsers.ItemsSource = _users;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void showCreatedQuizzes()
        {
            try
            {
                _quizzes = _quizManager.GetQuizzesByCreator(_user.UserID);
                grdMyQuizzes.ItemsSource = _quizzes;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void showActiveQuizzes()
        {
            try
            {
                _quizzes = _quizManager.GetAllActiveQuizes();
                grdAllQuizzes.ItemsSource = _quizzes;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void showTakenQuizzes()
        {
            try
            {
                txtNoTakenQuizzes.Visibility = Visibility.Hidden;

                _takenQuizzes = _quizRecordManager.GetTakenQuizzes(_user.UserID);
                if(_takenQuizzes.Count == 0)
                {
                    grdTakenQuizzes.Visibility = Visibility.Hidden;
                    btnRetakeQuiz.Visibility = Visibility.Hidden;
                    btnRetakeQuiz.IsEnabled = false;
                    btnReviewAnswers.Visibility = Visibility.Hidden;
                    btnReviewAnswers.IsEnabled = false;

                    txtNoTakenQuizzes.Visibility = Visibility.Visible;
                }
                else
                {
                    grdTakenQuizzes.Visibility = Visibility.Visible;
                    btnRetakeQuiz.Visibility = Visibility.Visible;
                    btnRetakeQuiz.IsEnabled = true;
                    btnReviewAnswers.Visibility = Visibility.Visible;
                    btnReviewAnswers.IsEnabled = true;

                    txtNoTakenQuizzes.Visibility = Visibility.Hidden;

                    grdTakenQuizzes.ItemsSource = _takenQuizzes;
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void showStartedChats()
        {
            try
            {
                txtNoReceivedChats.Visibility = Visibility.Hidden;

                _startedChats = _chatManager.GetStartedChats(_user.UserID);
                if(_startedChats.Count == 0)
                {
                    grdMyChats.Visibility = Visibility.Hidden;
                    btnViewMessages.Visibility = Visibility.Hidden;
                    btnViewMessages.IsEnabled = false;

                    txtNoStartedChats.Visibility = Visibility.Visible;
                }
                else
                {
                    grdMyChats.Visibility = Visibility.Visible;
                    btnViewMessages.Visibility = Visibility.Visible;
                    btnViewMessages.IsEnabled = true;

                    txtNoStartedChats.Visibility = Visibility.Hidden;
                    grdColSender.Visibility = Visibility.Hidden;
                    grdColSentTo.Visibility = Visibility.Visible;
                    grdMyChats.ItemsSource = _startedChats;
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void showReceivedChats()
        {
            try
            {
                txtNoStartedChats.Visibility = Visibility.Hidden;

                _receivedChats = _chatManager.GetReceivedChats(_user.UserID);
                if(_receivedChats.Count == 0)
                {
                    grdMyChats.Visibility = Visibility.Hidden;
                    btnViewMessages.Visibility = Visibility.Hidden;
                    btnViewMessages.IsEnabled = false;

                    txtNoReceivedChats.Visibility = Visibility.Visible;
                }
                else
                {
                    grdMyChats.Visibility = Visibility.Visible;
                    btnViewMessages.Visibility = Visibility.Visible;
                    btnViewMessages.IsEnabled = true;

                    txtNoReceivedChats.Visibility = Visibility.Hidden;
                    grdColSentTo.Visibility = Visibility.Hidden;
                    grdMyChats.ItemsSource = _receivedChats;
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