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
    /// Interaction logic for ReviewRetakeWindow.xaml
    /// </summary>
    public partial class ReviewRetakeWindow : Window
    {
        UserVM _user;
        QuizVM _quiz;

        IQuestionManager _questionManager;
        IQuizRecordManager _quizRecordManager;

        int _quizRecordID;

        bool _isReview;



        public ReviewRetakeWindow(UserVM user, int quizRecordID, QuizVM quiz, bool _isReview, IQuestionManager questionManager, IQuizRecordManager quizRecordManager)
        {
            this._user = user;

            this._quizRecordID = quizRecordID;
            this._quiz = quiz;
            this._isReview = _isReview;

            this._questionManager = questionManager;
            this._quizRecordManager = quizRecordManager;

            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void winReviewRetakeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnAll.Content = "Yes";
            btnCancel.Content = "No";

            if(_isReview == true)
            {
                winReviewRetakeWindow.Title = $"Review Quiz - {_quiz.Name}";
                txtReviewRetake.Text = "Do you want to review the answers to this quiz?";
            }
            else
            {
                winReviewRetakeWindow.Title = $"Retake Quiz - {_quiz.Name}";
                txtReviewRetake.Text = "Do you want to retake this quiz?";
            }
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            if(_isReview == true)
            {
                var reviewWindow = new TakeReviewQuizWindow(_user, _quiz, _questionManager, _quizRecordManager, false);
                reviewWindow.ShowDialog();
            }
            else
            {
                var retakeQuizWindow = new TakeReviewQuizWindow(_user, _quiz, _questionManager, _quizRecordManager, "Retake");
                retakeQuizWindow.ShowDialog();
            }
        }
    }
}