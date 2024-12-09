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
    /// Interaction logic for CreateEditQuizWindow.xaml
    /// </summary>
    public partial class CreateEditQuizWindow : Window
    {
        UserVM _user;
        QuizVM? _quiz;

        List<QuizTopic> _quizTopics = new List<QuizTopic>();
        List<string> _topics = new List<string>();

        IQuizManager _quizManager;

        // For creating a new Quiz.
        public CreateEditQuizWindow(UserVM user, IQuizManager quizManager)
        {
            this._user = user;
            this._quizManager = quizManager;

            InitializeComponent();
        }

        // For editing an existing Quiz.
        public CreateEditQuizWindow(UserVM user, QuizVM quiz, IQuizManager quizManager)
        {
            this._user = user;
            this._quiz = quiz;
            this._quizManager = quizManager;

            InitializeComponent();
        }

        private void winCreateEditQuiz_Loaded(object sender, RoutedEventArgs e)
        {
            if(_quiz == null)
            {
                // Creating a new Quiz
                dkpActive.Visibility = Visibility.Hidden;
                getAllQuizTopics();
            }
            else
            {
                // Editing an existing Quiz
                getAllQuizTopics();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to abandon this form?\nYour changes will not be saved.", "Abandon Quiz?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnCreateQuestions_Click(object sender, RoutedEventArgs e)
        {
            QuizTopic? quizTopic = null;

            // Validate the Form Input
            if(txtQuizName.Text.Length < 2 || txtQuizName.Text.Length > 50)
            {
                MessageBox.Show("Invalid Quiz Name...");
                txtQuizName.Focus();
                txtQuizName.SelectAll();
                return;
            }
            if(cboQuizTopic.Text.Length < 2 || cboQuizTopic.Text.Length > 50)
            {
                MessageBox.Show("Invalid Quiz Topic...");
                cboQuizTopic.Focus();
                return;
            }
            if(txtTopicDescription.Text.Length < 5 || txtTopicDescription.Text.Length > 250)
            {
                MessageBox.Show("Invalid Topic Description...");
                txtTopicDescription.Focus();
                txtTopicDescription.SelectAll();
                return;
            }
            if(txtQuizDescription.Text.Length < 5 || txtQuizDescription.Text.Length > 250)
            {
                MessageBox.Show("Invalid Quiz Description...");
                txtQuizDescription.Focus();
                txtQuizDescription.SelectAll();
                return;
            }

            if(!_topics.Contains(cboQuizTopic.Text))
            {
                // Check if it's a new topic & have then enter a description if it is.
                if((txtTopicDescription.Text == null || txtTopicDescription.Text == ""))
                {
                    MessageBox.Show("Please enter a description for this new Quiz Topic!");
                    txtTopicDescription.Focus();
                    return;
                }
                
                // Create the new Quiz Topic.
                quizTopic = new QuizTopic
                {
                    QuizTopicID = cboQuizTopic.Text,
                    Description = txtTopicDescription.Text
                };
            }

            // Set the QuizVM object's values, once valdated.
            _quiz = new QuizVM{
                QuizTopicID = cboQuizTopic.Text,
                Name = txtQuizName.Text,
                CreatedBy = _user.UserID,
                Description = txtQuizDescription.Text
            };

            var createQuestionWindow = new CreateEditQuestionWindow(_user, _quiz, _quizManager, quizTopic);
            var result = createQuestionWindow.ShowDialog();
            if(result == false)
            {
                // If user abandons Question Creation, also close this window.
                // Make them start completely over.
                this.Close();
            }
            else if(result == true)
            {
                // Quiz & Question Creation were successful.
                // Close this window, also.
                this.DialogResult = true;
                this.Close();
            }
        }

        private void cboQuizTopic_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTopicDescription.Text = "";
            if(cboQuizTopic.SelectedItem != null && _topics.Contains(cboQuizTopic.Text))
            {
                txtTopicDescription.IsEnabled = false;
            }
            else
            {
                txtTopicDescription.IsEnabled = true;
            }
            populateTopicDescription();
        }


        // Helper Methods
        private void getAllQuizTopics()
        {
            try
            {
                _topics = new List<string>();

                _quizTopics = _quizManager.GetAllQuizTopics();

                foreach(QuizTopic topic in _quizTopics)
                {
                    _topics.Add(topic.QuizTopicID);
                }
                cboQuizTopic.ItemsSource = _topics;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void populateTopicDescription()
        {
            if(cboQuizTopic.SelectedItem != null && _topics.Contains(cboQuizTopic.Text))
            {
                var result = _quizTopics.FirstOrDefault(qt => qt.QuizTopicID == cboQuizTopic.Text);
                txtTopicDescription.Text = result?.Description;
            }
        }
    }
}