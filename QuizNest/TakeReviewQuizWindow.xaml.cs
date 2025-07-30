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
    /// Interaction logic for TakeReviewQuizWindow.xaml
    /// </summary>
    public partial class TakeReviewQuizWindow : Window
    {
        UserVM _user;
        QuizVM _quiz;

        string? _attemptType;

        bool _isReviewingMissedOnly = false;

        IQuestionManager _questionManager;
        IQuizRecordManager _quizRecordManager;

        int _count = 0;

        List<QuestionVM> _questions = new List<QuestionVM>();
        List<string> _answers = new List<string>();

        List<Question> _missedQuestions = new List<Question>();

        // For taking & retaking quizzes.
        public TakeReviewQuizWindow(UserVM user, QuizVM quiz, IQuestionManager questionManager, IQuizRecordManager quizRecordManager, string attemptType)
        {
            this._user = user;
            this._quiz = quiz;
            this._questionManager = questionManager;
            this._quizRecordManager = quizRecordManager;
            this._attemptType = attemptType;

            InitializeComponent();
        }

        // For reviewing quizzes.
        public TakeReviewQuizWindow(UserVM user, QuizVM quiz, IQuestionManager questionManager, IQuizRecordManager quizRecordManager, bool isReviewingMissedOnly)
        {
            this._user = user;
            this._quiz = quiz;
            this._questionManager = questionManager;
            this._quizRecordManager = quizRecordManager;

            this._isReviewingMissedOnly = isReviewingMissedOnly;

            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(_attemptType == null)
            {
                this.Close();
            }
            else
            {
                var result = MessageBox.Show("Are you sure you want to abandon this quiz?\nYour score will not be saved.", "Abandon Quiz?",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
        }

        private void winTakeQuizWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnBack.Visibility = Visibility.Hidden;
            btnBack.IsEnabled = false;

            btnSubmit.Visibility = Visibility.Hidden;
            btnSubmit.IsEnabled = false;

            setPageTitle();

            // Get all questions by the quiz's quizID.
            // Add all of these questions to the _questions list.
            try
            {
                if(_isReviewingMissedOnly == false)
                {
                    _questions = _questionManager.GetActiveQuestionsByQuizID(_quiz.QuizID);
                }
                if(_isReviewingMissedOnly == true)
                {
                    // GET MISSED QUESTIONS INSTEAD
                    //_questions = _quizRecordManager.GetMissedQuestions();
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }

            // Populate the first question.
            loadPreviousNextCreatedQuestion();

            if(_attemptType == null)
            {
                // Reviewing a quiz.

                btnCancel.Visibility = Visibility.Hidden;
                btnCancel.IsEnabled = false;

                btnSubmit.Content = "Done";
                btnSubmit.Visibility= Visibility.Visible;
                btnSubmit.IsEnabled = true;

                rdoAnswer1.IsEnabled = false;
                rdoAnswer2.IsEnabled = false;
                rdoAnswer3.IsEnabled = false;
                rdoAnswer4.IsEnabled = false;

                lblYourAnswer.Content = "Correct Answer:";
                txtYourAnswer.IsEnabled = false;

                loadAnswer();
            }
        }

        

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            string answer = "";

            if(_attemptType == null)
            {
                ++_count;

                if(_count > 0)
                {
                    btnBack.Visibility = Visibility.Visible;
                    btnBack.IsEnabled = true;
                }

                setPageTitle();

                clearAnswer();
                loadPreviousNextCreatedQuestion();
                loadAnswer();
            }

            else if(_count == 0 && _answers.Count == 0) // at the beginning of adding answers, we have no answers yet, nowhere to go 
            {
                // Verify they answered the question.
                if(validateAnswer(ref answer) == false)
                {
                    return;
                }

                // Add their answer to the list
                _answers.Add(answer);

                ++_count;

                if(_count > 0)
                {
                    btnBack.Visibility = Visibility.Visible;
                    btnBack.IsEnabled = true;
                }

                setPageTitle();

                // Clear the selected answer, then load next question.
                clearAnswer();
                loadPreviousNextCreatedQuestion();
            }

            // If _count + 1 < _questions.Count, they're not at the end of the quiz.
            // They must answer the question.
            else if(_count + 1 <= _questions.Count)
            {
                if(_count + 1 <= _answers.Count)
                {
                    // There are existing answers in the list.

                    if(_answers.Count == _questions.Count) // they have answered all questions already - went backwards, then came back forward.
                    {
                        // Ask if they want to overwrite their answer.
                        var result = MessageBox.Show("Would you like to overwrite this answer?\nYour current answer will still be saved, if no changes were made.",
                            "Overwrite Answer?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(result == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            // They clicked Yes.

                            // Validate their answer.
                            if(validateAnswer(ref answer) == false)
                            {
                                return;
                            }

                            // Replace existing answer.
                            _answers[_count] = answer;

                            ++_count;

                            if(_count > 0)
                            {
                                btnBack.Visibility = Visibility.Visible;
                                btnBack.IsEnabled = true;
                            }

                            setPageTitle();

                            clearAnswer();
                            loadPreviousNextCreatedQuestion();

                            // Load next saved answer.
                            loadAnswer();
                        }
                    }
                    else if(_answers.Count < _questions.Count) // they haven't answered all questions yet
                    {
                        if(_count + 1 < _answers.Count) // We are behind the number of answers - overwrite.
                        {
                            // Validate their answer.
                            if(validateAnswer(ref answer) == false)
                            {
                                return;
                            }

                            // Replace the answer in the list.
                            _answers[_count] = answer;

                            ++_count;

                            if(_count > 0)
                            {
                                btnBack.Visibility = Visibility.Visible;
                                btnBack.IsEnabled = true;
                            }

                            setPageTitle();

                            // Clear the selected answer, then load next question.
                            clearAnswer();
                            loadPreviousNextCreatedQuestion();

                            loadAnswer();
                        }
                        else if(_count + 1 == _answers.Count) // We are at the last answered question.
                        {
                            // ONLY REPLACE, DO NOT loadAnswer()!!

                            // Validate their answer.
                            if(validateAnswer(ref answer) == false)
                            {
                                return;
                            }

                            // Replace the answer in the list.
                            _answers[_count] = answer;

                            ++_count;

                            if(_count > 0)
                            {
                                btnBack.Visibility = Visibility.Visible;
                                btnBack.IsEnabled = true;
                            }

                            setPageTitle();

                            // Clear the selected answer, then load next question.
                            clearAnswer();
                            loadPreviousNextCreatedQuestion();
                        }
                        else
                        {
                            // Add new to the end of the list.

                            // Validate their answer.
                            if(validateAnswer(ref answer) == false)
                            {
                                return;
                            }

                            // Replace the answer in the list.
                            _answers.Add(answer);

                            ++_count;

                            if(_count > 0)
                            {
                                btnBack.Visibility = Visibility.Visible;
                                btnBack.IsEnabled = true;
                            }

                            setPageTitle();

                            // Clear the selected answer, then load next question.
                            clearAnswer();
                            loadPreviousNextCreatedQuestion();
                        }
                    }
                }
                else
                {
                    // Adding a new answer.
                    // Validate their answer.
                    if(validateAnswer(ref answer) == false)
                    {
                        return;
                    }

                    // Add new answer to the list.
                    _answers.Add(answer);

                    ++_count;

                    if(_count > 0)
                    {
                        btnBack.Visibility = Visibility.Visible;
                        btnBack.IsEnabled = true;
                    }

                    setPageTitle();

                    // Clear the selected answer, then load next question.
                    clearAnswer();
                    loadPreviousNextCreatedQuestion();
                }
            }

            // If _count + 1 == _questions.Count, disable btnNext & enable btnSubmit.
            if(_count + 1 == _questions.Count)
            {
                // Disable btnNext
                btnNext.Visibility = Visibility.Hidden;
                btnNext.IsEnabled = false;

                // Enable btnSubmit
                btnSubmit.Visibility = Visibility.Visible;
                btnSubmit.IsEnabled = true;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            string answer = "";

            if(_attemptType == null)
            {
                --_count;

                if(_count == 0)
                {
                    btnBack.Visibility = Visibility.Hidden;
                    btnBack.IsEnabled = false;
                }
                if(_count > 0)
                {
                    btnBack.Visibility = Visibility.Visible;
                    btnBack.IsEnabled = true;
                }
                if(_count < _questions.Count)
                {
                    btnNext.Visibility = Visibility.Visible;
                    btnNext.IsEnabled = true;
                }

                setPageTitle();

                clearAnswer();
                loadPreviousNextCreatedQuestion();
                loadAnswer();
            }

            else if(_count + 1 == _questions.Count) // this is the last question to answer, no more after this - but able to go back
            {
                // Can only move backwards.
                if(_count + 1 == _answers.Count)
                {
                    // There is an existing answer in the list.
                    var result = MessageBox.Show("Would you like to overwrite this answer, before moving back?\nYour current answer will still be saved, if no changes were made.",
                            "Overwrite Answer?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.No)
                    {
                        return;
                    }
                    else
                    {
                        // They clicked Yes.

                        // Validate their answer.
                        if(validateAnswer(ref answer) == false)
                        {
                            return;
                        }

                        // Replace existing answer.
                        _answers[_count] = answer;
                    }
                }
                else
                {
                    // Ask if they want to save their answer before moving backwards.
                    var result = MessageBox.Show("Would you like to save this answer, before moving back?",
                            "Save Answer?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        // They clicked Yes.

                        // Validate their answer.
                        if(validateAnswer(ref answer) == false)
                        {
                            return;
                        }

                        // Add new answer to the list.
                        _answers.Add(answer);
                    }
                    // If no, do not add the answer.
                }

                // Decrement _count, reset title, & disable/enable buttons either way.
                --_count;

                setPageTitle();

                // Disable btnSubmit
                btnSubmit.Visibility = Visibility.Hidden;
                btnSubmit.IsEnabled = false;

                // Enable btnNext
                btnNext.Visibility = Visibility.Visible;
                btnNext.IsEnabled = true;

                clearAnswer();
                loadPreviousNextCreatedQuestion();

                // Load next saved answer.
                loadAnswer();
            }

            // If _count + 1 < _questions.Count, they're not at the end of the quiz.
            else if(_count + 1 < _questions.Count)
            {
                if(_count + 1 <= _answers.Count)
                {
                    // Ask if they want to save their current answer to _answers list.
                    var result = MessageBox.Show("Would you like to overwrite this answer, before moving back?\nClicking 'Yes' will save your current answer, regardless of any changes you made.",
                            "Overwrite Answer?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.No)
                    {
                        return;
                    }

                    else
                    {
                        // They clicked Yes.
                        // If yes, save their answer to _answers, or replace the existing answer in the list.

                        // Validate their answer.
                        if(validateAnswer(ref answer) == false)
                        {
                            return;
                        }

                        // Replace existing answer.
                        _answers[_count] = answer;
                    }
                }
                else if(_count + 1 > _answers.Count)
                {
                    // Ask if they want to save their current answer to _answers list.
                    var result = MessageBox.Show("Would you like to save this answer, before moving back?",
                            "Save Answer?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        // They clicked Yes.
                        // If yes, save their answer to _answers, or replace the existing answer in the list.

                        // Validate their answer.
                        if(validateAnswer(ref answer) == false)
                        {
                            return;
                        }

                        // Replace existing answer.
                        _answers.Add(answer);
                    }
                    
                    // If they click No, do not add it.
                }

                // Decrement _count & reset title.
                --_count;

                setPageTitle();

                // If _count == 0, disable btnBack.
                if(_count == 0)
                {
                    btnBack.Visibility = Visibility.Hidden;
                    btnBack.IsEnabled = false;
                }

                clearAnswer();
                loadPreviousNextCreatedQuestion();

                // Load next saved answer.
                loadAnswer();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(_attemptType == null)
            {
                this.Close();
            }
            else
            {
                string answer = "";

                // See if the current (last) question is answered.
                // If not, they must answer it - take them back to it.
                // Validate their answer.
                if(validateAnswer(ref answer) == false)
                {
                    return;
                }

                // If it is answered, save their current answer to the _answers list.
                // Add new answer to the list.
                if(_answers.Count < _questions.Count)
                {
                    _answers.Add(answer);
                }

                // If it is already in the _answers list - _answers.Count == _questions.Count.
                // Overwrite the answer!
                else if(_answers.Count == _questions.Count)
                {
                    _answers[_count] = answer;
                }


                // Submit the test.
                // Check they actually answered all questions.
                if(_answers.Count == _questions.Count)
                {
                    // Calculate their score - how many they got right, & how many they missed.

                    // How many question did they miss?
                    for(int i = 0; i < _questions.Count; i++)
                    {
                        // Count through, if their answer does NOT equal the correct answer,
                        // add it to _missedQuestions.
                        if(_questions[i].CorrectAnswer.ToLower() != _answers[i].ToLower())
                        {
                            _missedQuestions.Add(_questions[i]);
                        }
                    }

                    // Divide number of correct questions by the quiz question count.
                    decimal score = (decimal)(_questions.Count - _missedQuestions.Count) / (decimal)_questions.Count;

                    // Round, to calculate it to 2 decimal places.
                    score = Decimal.Round((score *= 100), 2);

                    // Insert their QuizRecord.
                    // Get the ID, in case they missed any questions.
                    int newRecordID = addQuizRecord(score);

                    // If _missedQuestions.Count > 0, then insert all missed questions into the DB.
                    if(_missedQuestions.Count > 0)
                    {
                        foreach(Question q in _missedQuestions)
                        {
                            // Add to DB.
                            addMissedQuestion(newRecordID, q);
                        }
                    }

                    // Go to QuizCompletionOverviewWindow & display their score.
                    // They have the option to Retake the Quiz & Review the correct answers there.
                    var quizCompletionWindow = new QuizCompletionOverviewWindow(_user, newRecordID, score, _missedQuestions.Count, _quiz, _questionManager, _quizRecordManager);
                    var result = quizCompletionWindow.ShowDialog();
                    if(result == false)
                    {
                        this.DialogResult = false;
                        this.Close();
                    }
                }
            }
        }











        // Helper Methods
        private void loadPreviousNextCreatedQuestion()
        {
            // Pull the question prompt & answers, whether they click Back or Next (if they go back, then forward again).
            txtPrompt.Text = _questions[_count].Prompt;

            if(_questions[_count].QuestionTypeID == "Multiple Choice")
            {
                lblYourAnswer.Visibility = Visibility.Hidden;
                txtYourAnswer.Visibility = Visibility.Hidden;

                stkAnswers.Visibility = Visibility.Visible;

                rdoAnswer1.Visibility = Visibility.Visible;
                rdoAnswer1.Content = _questions[_count].Answer1;

                rdoAnswer2.Content = _questions[_count].Answer2;
                rdoAnswer3.Content = _questions[_count].Answer3;

                rdoAnswer4.Visibility = Visibility.Visible;
                rdoAnswer4.Content = _questions[_count].Answer4;
            }
            if(_questions[_count].QuestionTypeID == "True/False")
            {
                lblYourAnswer.Visibility = Visibility.Hidden;
                txtYourAnswer.Visibility = Visibility.Hidden;

                stkAnswers.Visibility = Visibility.Visible;

                rdoAnswer1.Visibility = Visibility.Hidden;

                rdoAnswer2.Content = "True";
                rdoAnswer3.Content = "False";

                rdoAnswer4.Visibility = Visibility.Hidden;
            }
            if(_questions[_count].QuestionTypeID == "Short Answer")
            {
                stkAnswers.Visibility = Visibility.Hidden;

                lblYourAnswer.Visibility = Visibility.Visible;
                txtYourAnswer.Visibility = Visibility.Visible;
            }
        }
        private void loadAnswer()
        {
            if(_attemptType == null)
            {
                // Load the correct answer.

                if(_questions[_count].QuestionTypeID == "Multiple Choice")
                {
                    if(rdoAnswer1.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer1.IsChecked = true;
                    }
                    else if(rdoAnswer2.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer2.IsChecked = true;
                    }
                    else if(rdoAnswer3.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer3.IsChecked = true;
                    }
                    else if(rdoAnswer4.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer4.IsChecked = true;
                    }
                }
                if(_questions[_count].QuestionTypeID == "True/False")
                {
                    if(rdoAnswer2.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer2.IsChecked = true;
                    }
                    else if(rdoAnswer3.Content as string == _questions[_count].CorrectAnswer)
                    {
                        rdoAnswer3.IsChecked = true;
                    }
                }
                if(_questions[_count].QuestionTypeID == "Short Answer")
                {
                    txtYourAnswer.Text = _questions[_count].CorrectAnswer;
                }
            }
            else
            {
                // Load a question they answered.

                if(_questions[_count].QuestionTypeID == "Multiple Choice")
                {
                    if(rdoAnswer1.Content as string == _answers[_count])
                    {
                        rdoAnswer1.IsChecked = true;
                    }
                    else if(rdoAnswer2.Content as string == _answers[_count])
                    {
                        rdoAnswer2.IsChecked = true;
                    }
                    else if(rdoAnswer3.Content as string == _answers[_count])
                    {
                        rdoAnswer3.IsChecked = true;
                    }
                    else if(rdoAnswer4.Content as string == _answers[_count])
                    {
                        rdoAnswer4.IsChecked = true;
                    }
                }
                if(_questions[_count].QuestionTypeID == "True/False")
                {
                    if(rdoAnswer2.Content as string == _answers[_count])
                    {
                        rdoAnswer2.IsChecked = true;
                    }
                    else if(rdoAnswer3.Content as string == _answers[_count])
                    {
                        rdoAnswer3.IsChecked = true;
                    }
                }
                if(_questions[_count].QuestionTypeID == "Short Answer")
                {
                    txtYourAnswer.Text = _answers[_count];
                }
            }
        }
        private void clearAnswer()
        {
            rdoAnswer1.IsChecked = false;
            rdoAnswer2.IsChecked = false;
            rdoAnswer3.IsChecked = false;
            rdoAnswer4.IsChecked = false;
            txtYourAnswer.Text = "";
        }
        private bool validateAnswer(ref string answer)
        {
            // If there are multiple answer options.
            if(stkAnswers.Visibility == Visibility.Visible)
            {
                // If it's a true/false question.
                if(rdoAnswer1.Visibility == Visibility.Hidden)
                {
                    // Verify they answered it.
                    if(rdoAnswer2.IsChecked == true)
                    {
                        answer = rdoAnswer2.Content as string;
                    }
                    else if(rdoAnswer3.IsChecked == true)
                    {
                        answer = rdoAnswer3.Content as string;
                    }
                    else
                    {
                        MessageBox.Show("You must answer this question to continue.");
                        stkAnswers.Focus();
                        rdoAnswer2.Focus();
                        return false;
                    }
                }
                // If it's a multiple choice question.
                else if(rdoAnswer1.Visibility == Visibility.Visible)
                {
                    // Verify they answered it.
                    if(rdoAnswer1.IsChecked == true)
                    {
                        answer = rdoAnswer1.Content as string;
                    }
                    else if(rdoAnswer2.IsChecked == true)
                    {
                        answer = rdoAnswer2.Content as string;
                    }
                    else if(rdoAnswer3.IsChecked == true)
                    {
                        answer = rdoAnswer3.Content as string;
                    }
                    else if(rdoAnswer4.IsChecked == true)
                    {
                        answer = rdoAnswer4.Content as string;
                    }
                    else
                    {
                        MessageBox.Show("You must answer this question to continue.");
                        stkAnswers.Focus();
                        rdoAnswer1.Focus();
                        return false;
                    }
                }
            }
            // If it's a short answer question.
            else if(txtYourAnswer.Visibility == Visibility.Visible)
            {
                // Verify they answered it.
                if(txtYourAnswer.Text == null || txtYourAnswer.Text == "")
                {
                    MessageBox.Show("You must answer this question to continue.");
                    txtYourAnswer.Focus();
                    return false;
                }
                else
                {
                    answer = txtYourAnswer.Text;
                }
            }

            return true;
        }
        private int addQuizRecord(decimal score)
        {
            int newRecordID = 0;
            QuizRecord newQuizRecord = new QuizRecord()
            {
                AttemptTypeID = _attemptType,
                UserID = _user.UserID,
                QuizID = _quiz.QuizID,
                Score = score,
                IsPublic = true
            };
            try
            {
                newRecordID = _quizRecordManager.AddQuizRecord(newQuizRecord);
                if(newRecordID == 0)
                {
                    throw new Exception("Record Not Added...");
                }
                else
                {
                    MessageBox.Show("New Record Added Successfully!");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }

            return newRecordID;
        }
        private void addMissedQuestion(int newRecordID, Question q)
        {
            try
            {
                bool result = _quizRecordManager.AddMissedQuestion(newRecordID, q.QuestionID);
                if(result == false)
                {
                    throw new Exception("Missed Question Not Recorded...");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void setPageTitle()
        {
            if(_attemptType == "First")
            {
                winTakeReviewQuizWindow.Title = $"Take Quiz - {_quiz.Name} - Question {_count + 1}";
            }
            if(_attemptType == "Retake")
            {
                winTakeReviewQuizWindow.Title = $"Retake Quiz - {_quiz.Name} - Question {_count + 1}";
            }
            if(_attemptType == "Missed Only")
            {
                winTakeReviewQuizWindow.Title = $"Retake Quiz - {_quiz.Name} | Missed Only - Question {_count + 1}";
            }
            if(_attemptType == null)
            {
                if(_isReviewingMissedOnly == true)
                {
                    winTakeReviewQuizWindow.Title = $"Review Quiz - {_quiz.Name} | Missed Only - Question {_count + 1}";
                }
                if(_isReviewingMissedOnly == false)
                {
                    winTakeReviewQuizWindow.Title = $"Review Quiz - {_quiz.Name} - Question {_count + 1}";
                }
            }
        }
    }
}