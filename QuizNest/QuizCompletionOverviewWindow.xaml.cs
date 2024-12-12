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
using static System.Formats.Asn1.AsnWriter;

namespace QuizNestPresentation
{
    /// <summary>
    /// Interaction logic for QuizCompletionOverviewWindow.xaml
    /// </summary>
    public partial class QuizCompletionOverviewWindow : Window
    {
        UserVM _user;

        int _quizRecordID;
        decimal _score;

        int _misssedQuestionCount = -1;

        QuizVM _quiz;
        QuizRecordVM _quizRecord;
        IQuestionManager _questionManager;
        IQuizRecordManager _quizRecordManager;

        // For completing a quiz.
        public QuizCompletionOverviewWindow(UserVM _user, int quizRecordID, decimal score, int missedQuestionCount, QuizVM quiz,
                IQuestionManager questionManager, IQuizRecordManager quizRecordManager)
        {
            this._user = _user;

            this._quizRecordID = quizRecordID;
            this._score = score;
            this._misssedQuestionCount = missedQuestionCount;

            this._quiz = quiz;
            this._questionManager = questionManager;
            this._quizRecordManager = quizRecordManager;

            InitializeComponent();
        }

        // For an overview of a quiz record.
        public QuizCompletionOverviewWindow(UserVM user, QuizRecordVM quizRecord, QuizVM quiz, IQuestionManager questionManager, IQuizRecordManager quizRecordManager)
        {
            this._user = user;

            this._quizRecordID = quizRecord.QuizRecordID;
            this._score = quizRecord.Score;

            this._quizRecord = quizRecord;
            this._quiz = quiz;
            this._questionManager = questionManager;
            this._quizRecordManager = quizRecordManager;

            InitializeComponent();
        }

        private void winQuizCompletionOverviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(_misssedQuestionCount == -1)
            {
                // Just reviewing, didn't actually take a quiz just now.

                winQuizCompletionOverviewWindow.Title = $"Overview - Quiz Record - {_quiz.Name}";

                lblYourScore.Content = "Your Score Was:";
                txtScore.Text = _score.ToString() + "%";

                lblYouMissed.Visibility = Visibility.Hidden;
                txtYouMissed.Visibility = Visibility.Hidden;

                // Get the Quiz Record - if it's Public, check the box; otherwise, don't.
                if(_quizRecord.IsPublic == true)
                {
                    chkActive.IsChecked = true;
                }
                else if(_quizRecord.IsPublic == false)
                {
                    chkActive.IsChecked = false;
                }
            }
            else
            {
                // Just took a quiz.

                winQuizCompletionOverviewWindow.Title = $"Your Score - Quiz {_quiz.Name}";

                lblYourScore.Content = "Your Score Is:";
                txtScore.Text = _score.ToString() + "%";

                lblYouMissed.Visibility = Visibility.Visible;
                txtYouMissed.Visibility = Visibility.Visible;
                txtYouMissed.Text = _misssedQuestionCount.ToString() + " questions";

                chkActive.IsChecked = true;
            }
            
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void chkActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isPublic = _quizRecordManager.EditQuizRecordIsPublicStatus(_quizRecordID, true);
                if(isPublic == true)
                {
                    MessageBox.Show("Your quiz record is now public!\nCheck it out on the leaderboard!");
                }
                else
                {
                    throw new Exception("Public Status Not Updated...");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void chkActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isNotPublic = _quizRecordManager.EditQuizRecordIsPublicStatus(_quizRecordID, false);
                if(isNotPublic == true)
                {
                    MessageBox.Show("Your quiz record is no longer public!\nIt will no longer appear on the leaderboard.");
                }
                else
                {
                    throw new Exception("Public Status Not Updated...");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            var reviewWindow = new ReviewRetakeWindow(_user, _quizRecordID, _quiz, true, _questionManager, _quizRecordManager);
            reviewWindow.ShowDialog();
        }

        private void btnRetake_Click(object sender, RoutedEventArgs e)
        {
            var retakeWindow = new ReviewRetakeWindow(_user, _quizRecordID, _quiz, false, _questionManager, _quizRecordManager);
            retakeWindow.ShowDialog();
        }
    }
}