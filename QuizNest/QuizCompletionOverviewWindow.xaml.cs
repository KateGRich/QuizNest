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
    /// Interaction logic for QuizCompletionOverviewWindow.xaml
    /// </summary>
    public partial class QuizCompletionOverviewWindow : Window
    {
        int _quizRecordID;
        decimal _score;
        int _misssedQuestionCount;

        QuizVM _quiz;
        IQuizRecordManager _quizRecordManager;

        public QuizCompletionOverviewWindow(int quizRecordID, decimal score, int missedQuestionCount, QuizVM quiz, IQuizRecordManager quizRecordManager)
        {
            this._quizRecordID = quizRecordID;
            this._score = score;
            this._misssedQuestionCount = missedQuestionCount;

            this._quiz = quiz;
            this._quizRecordManager = quizRecordManager;

            InitializeComponent();
        }

        private void winQuizCompletionOverviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            winQuizCompletionOverviewWindow.Title = $"Your Score - Quiz {_quiz.Name}";

            txtScore.Text = _score.ToString();
            txtYouMissed.Text = _misssedQuestionCount.ToString() + " questions";

            chkActive.IsChecked = true;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}