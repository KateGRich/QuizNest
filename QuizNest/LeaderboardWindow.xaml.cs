using DataDomain;
using LogicLayer;
using Microsoft.IdentityModel.Tokens;
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
    /// Interaction logic for LeaderboardWindow.xaml
    /// </summary>
    public partial class LeaderboardWindow : Window
    {
        private IQuizRecordManager _quizRecordManager;
        private List<QuizRecordVM>? _leaderboard;
        private Quiz _quiz;
        
        public LeaderboardWindow(IQuizRecordManager quizRecordManager, QuizVM quiz)
        {
            _quizRecordManager = quizRecordManager;
            _quiz = quiz;

            InitializeComponent();
        }

        private void winLeaderboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                winLeaderboardWindow.Title += _quiz.Name;

                _leaderboard = _quizRecordManager.GetQuizLeaderboard(_quiz.QuizID);
                if(_leaderboard.Count == 0)
                {
                    grdLeaderboard.Visibility = Visibility.Hidden;
                    txtNoLeaderboard.Visibility = Visibility.Visible;
                }
                else
                {
                    txtNoLeaderboard.Visibility = Visibility.Hidden;
                    grdLeaderboard.ItemsSource = _leaderboard;
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
