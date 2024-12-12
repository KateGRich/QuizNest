using DataDomain;
using LogicLayer;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
    /// Interaction logic for CreateEditQuestionWindow.xaml
    /// </summary>
    public partial class CreateEditQuestionWindow : Window
    {
        UserVM _user;
        QuizVM? _quiz;
        QuizVM? _editQuiz;
        IQuizManager _quizManager;
        IQuestionManager? _questionManager;

        QuizTopic? _quizTopic; // If this is not null, then it is a new QuizTopic & user is creating a new Quiz.

        List<string> _questionTypes = new List<string>();

        int _count = 0;
        List<Question> _questions = new List<Question>();
        List<Question> _newQuestions = new List<Question>(); // Used for editing - adding new questions to the existing quiz.

        List<string> _multiChoiceAnswers = new List<string> { "", "", "", "" };
        
        // For creating new Quiz Questions.
        public CreateEditQuestionWindow(UserVM user, QuizVM quiz, IQuizManager quizManager, IQuestionManager questionManager, QuizTopic quizTopic)
        {
            _user = user;
            _quiz = quiz;
            _quizManager = quizManager;
            _questionManager = questionManager;
            _quizTopic = quizTopic;

            InitializeComponent();
        }
        
        // For editing existing Quiz Questions.
        public CreateEditQuestionWindow(UserVM user, QuizVM quiz, IQuizManager quizManager, IQuestionManager questionManager)
        {
            _user = user;
            _editQuiz = quiz;
            _quizManager = quizManager;
            _questionManager = questionManager;

            InitializeComponent();
        }

        private void winCreateEditQuestion_Loaded(object sender, RoutedEventArgs e)
        {
            btnBack.Visibility = Visibility.Hidden;
            btnBack.IsEnabled = false;

            if(_editQuiz == null && _quiz != null)
            {
                winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";

                getAllQuestionTypes();

                dkpActive.Visibility = Visibility.Hidden;
            }
            else if(_editQuiz != null && _quiz == null)
            {
                winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";

                dkpActive.Visibility = Visibility.Visible;
                btnComplete.Content = "Save";

                // Get all questions by the _editQuiz's quizID.
                // Add all of these questions to the _questions list.
                try
                {
                    _questions = _questionManager.GetAllQuestionsByQuizID(_editQuiz.QuizID);
                }
                catch(Exception ex)
                {
                    string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                    MessageBox.Show(message);
                }

                // Populate the first question.
                loadPreviousNextCreatedQuestion();

                // Other questions will populate accordingly when pressing next & back.
                // Should be the same functionality as the current Next & Back buttons.
                // User can have the option to add more questions, once they are past the end of the list.
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to abandon this form?\nYour changes will not be saved.", (_editQuiz == null ? "Abandon Quiz and Questions?" : "Abandon Edit?"),
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // decide if you are at the end of the list or not
            if(_count == 0 && _questions.Count == 0) // at the beginning of adding questions, we have no questions yet, nowhere to go 
            {
                // Only for creating new Quiz Questions - only reason _questions.Count would equal 0.

                // validate and offer to save new, then go on if saved
                if(validateQuestionForm() == false)
                {
                    return;
                }
                _questions.Add(saveQuestion());

                ++_count;

                if(_count > 0)
                {
                    btnBack.Visibility = Visibility.Visible;
                    btnBack.IsEnabled = true;
                }

                winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";

                displayEmptyQuestion();
            }
            else if(_count + 1 > _questions.Count) // past the end of the list, so this is a new question to add to the list
            {
                // Whether creating new or editing - this should remain the same.

                // validate and offer to save new, then go on to another new question
                if(validateQuestionForm() == false)
                {
                    return;
                }
                
                if(_editQuiz == null)
                {
                    _questions.Add(saveQuestion());
                }
                else
                {
                    // Editing an existing quiz - adding new Questions.

                    // Do both - _newQuestions keeps track of newly added.
                    _questions.Add(saveQuestion());

                    _newQuestions.Add(saveQuestion());
                }

                ++_count;

                if(_count > 0)
                {
                    btnBack.Visibility = Visibility.Visible;
                    btnBack.IsEnabled = true;
                }

                if(_editQuiz == null)
                {
                    winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                }
                else
                {
                    winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                }

                displayEmptyQuestion();
            }
            else if(_count + 1 == _questions.Count) // this is the last question, moving to a new question
            {
                // validate and offer to overwrite the _question[_count], then go on to a new
                var result = MessageBox.Show("Would you like to save these changes, if any?\nYour current question will still be saved by clicking 'Yes', if no changes were made.",
                        "Overwrite Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    _questions[_count] = saveQuestion();

                    ++_count;

                    if(_count > 0)
                    {
                        btnBack.Visibility = Visibility.Visible;
                        btnBack.IsEnabled = true;
                    }
                    
                    if(_editQuiz == null)
                    {
                        winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                    }
                    else
                    {
                        winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                    }

                    // Display an empty question whether creating new or editing.
                    displayEmptyQuestion();
                }
            }
            else if(_count < _questions.Count) // this is an existing question with more questions after it, so going forward loads the next existing question
            {
                // validate and offer to overwrite the _question[_count], then go on to the next existing question
                var result = MessageBox.Show("Would you like to save these changes, if any?\nYour current question will still be saved by clicking 'Yes', if no changes were made.",
                        "Overwrite Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    _questions[_count] = saveQuestion();

                    ++_count;

                    if(_count > 0)
                    {
                        btnBack.Visibility = Visibility.Visible;
                        btnBack.IsEnabled = true;
                    }

                    if(_editQuiz == null)
                    {
                        winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                    }
                    else
                    {
                        winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                    }

                    // Load Next Question, regardless of creating new or editing.
                    loadPreviousNextCreatedQuestion();
                }
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // decide if you are at the end of the list or not
            if(_count == 0) // at the beginning of adding questions, we have no questions yet, nowhere to go 
            {
                // could just disable the back button when _count == 0
                btnBack.Visibility = Visibility.Hidden;
                btnBack.IsEnabled = false;
            }
            else if(_count + 1 > _questions.Count) // past the end of the list, so this is a new question to add to the list
            {
                // validate and offer to save new, then go back to an existing question
                var result = MessageBox.Show("Would you like to save this question to your quiz?",
                        "Save Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    --_count;

                    if(_editQuiz == null)
                    {
                        winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                    }
                    else
                    {
                        winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                    }

                    loadPreviousNextCreatedQuestion();
                }
                else
                {
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    if(_editQuiz == null)
                    {
                        // Creating a new quiz.
                        _questions.Add(saveQuestion());
                    }
                    else
                    {
                        // Editing an existing quiz - adding new Questions.

                        // Do both - _newQuestions just keeps track of what needs added to the DB.
                        _questions.Add(saveQuestion());

                        _newQuestions.Add(saveQuestion());
                    }

                    --_count;

                    if(_count == 0)
                    {
                        btnBack.Visibility = Visibility.Hidden;
                        btnBack.IsEnabled = false;
                    }

                    if(_editQuiz == null)
                    {
                        winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                    }
                    else
                    {
                        winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                    }

                    loadPreviousNextCreatedQuestion();
                }
            }
            else if(_count + 1 <= _questions.Count) // this is an existing question or the last question, moving back to an existing question
            {
                // validate and offer to overwrite _question[_count], then go back to the previous question
                var result = MessageBox.Show("Would you like to save these changes, if any?\nYour current question will still be saved by clicking 'Yes', if no changes were made.",
                        "Overwrite Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }
                    _questions[_count] = saveQuestion();

                    --_count;

                    if(_count == 0)
                    {
                        btnBack.Visibility = Visibility.Hidden;
                        btnBack.IsEnabled = false;
                    }

                    if(_editQuiz == null)
                    {
                        winCreateEditQuestion.Title = $"Create Quiz - {_quiz.Name} - Question {_count + 1}";
                    }
                    else
                    {
                        winCreateEditQuestion.Title = $"Edit Quiz - {_editQuiz.Name} - Question {_count + 1}";
                    }

                    loadPreviousNextCreatedQuestion();
                }
            }
        }
        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            // decide if you are at the end of the list or not
            if(_count == 0 && _questions.Count == 0) // at the beginning of adding questions, we have no questions yet, nowhere to go 
            {
                // Only for creating new quiz questions - only reason _questions.Count would equal 0.

                // Ask if they're sure they want to save the quiz & questions.
                var result = MessageBox.Show("Would you like to save this quiz & its questions?",
                        "Save Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    // validate and offer to save new, then save the quiz & question to the DB
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    // Save question to list.
                    _questions.Add(saveQuestion());

                    if(_quizTopic != null)
                    {
                        // This is a new topic, insert new QuizTopic to DB.
                        addNewQuizTopic();
                    }

                    // Insert quiz to DB & get the QuizID for the new Quiz.
                    int newQuizID = addNewQuiz();

                    // Insert questions per quiz to DB.
                    addNewQuizQuestions(newQuizID);
                }
            }
            else if(_count + 1 > _questions.Count) // past the end of the list, so this is a new question to add to the list
            {
                // validate and offer to save new, then save the quiz & question to the DB
                var result = MessageBox.Show("Would you like to save this question to your quiz?",
                        "Save Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    if(_editQuiz == null)
                    {
                        var quizResult = MessageBox.Show("Would you like to save this quiz & its questions?",
                        "Save Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(quizResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_quizTopic != null)
                            {
                                // This is a new topic, insert new QuizTopic to DB.
                                addNewQuizTopic();
                            }

                            // Insert quiz to DB & get the QuizID for the new Quiz.
                            int newQuizID = addNewQuiz();

                            // Insert questions per quiz to DB.
                            addNewQuizQuestions(newQuizID);
                        }
                    }
                    else
                    {
                        // Editing
                        var questionResult = MessageBox.Show("Would you like to save these questions?",
                        "Save Questions?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(questionResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_newQuestions.Count > 0)
                            {
                                int startAddingHere = (_questions.Count - _newQuestions.Count);
                                for(int i = startAddingHere; startAddingHere < _questions.Count; startAddingHere++)
                                {
                                    for(int j = 0; j < _newQuestions.Count; j++)
                                    {
                                        _newQuestions[j] = _questions[i];
                                    }
                                }

                                // Add any new questions that were added while editing.
                                addNewQuizQuestions(_editQuiz.QuizID);
                            }

                            // Update the existing questions to the new data.
                            saveQuizQuestions(_questions, _editQuiz.QuizID);
                        }
                    }
                }
                else
                {
                    // Yes, they want to save this question to the list.

                    // validate and offer to save new, then save the quiz & question to the DB
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    if(_editQuiz == null)
                    {
                        // Save question to list.
                        _questions.Add(saveQuestion());

                        // Be sure they want to save the quiz & questions.
                        var quizResult = MessageBox.Show("Your question has been added!\nWould like to save this quiz & its questions?",
                            "Save Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(quizResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_quizTopic != null)
                            {
                                // This is a new topic, insert new QuizTopic to DB.
                                addNewQuizTopic();
                            }

                            // Insert quiz to DB & get the QuizID for the new Quiz.
                            int newQuizID = addNewQuiz();

                            // Insert questions per quiz to DB.
                            addNewQuizQuestions(newQuizID);
                        }
                    }
                    else
                    {
                        // Editing an existing quiz - adding new Questions.
                        _newQuestions.Add(saveQuestion());

                        var questionResult = MessageBox.Show("Would you like to save these questions?",
                        "Save Questions?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(questionResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_newQuestions.Count > 0)
                            {
                                // Add any new questions that were added while editing..
                                addNewQuizQuestions(_editQuiz.QuizID);
                            }

                            // Update the existing questions to the new data.
                            saveQuizQuestions(_questions, _editQuiz.QuizID);
                        }
                    }
                }
            }
            else if(_count + 1 == _questions.Count) // this is the last question, moving to a new question
            {
                // validate and offer to overwrite the _question[_count], then go on to a new
                var result = MessageBox.Show("Would you like to save these changes, if any?\nYour current question will still be saved by clicking 'Yes', if no changes were made.",
                        "Overwrite Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    // validate and offer to save new, then save the quiz & question to the DB
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    // Replace question in the list.
                    _questions[_count] = saveQuestion();

                    if(_editQuiz == null)
                    {
                        // Be sure they want to save the quiz & questions.
                        var quizResult = MessageBox.Show("Your question has been added!\nWould like to save this quiz & its questions?",
                            "Save Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(quizResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_quizTopic != null)
                            {
                                // This is a new topic, insert new QuizTopic to DB.
                                addNewQuizTopic();
                            }

                            // Insert quiz to DB & get the QuizID for the new Quiz.
                            int newQuizID = addNewQuiz();

                            // Insert questions per quiz to DB.
                            addNewQuizQuestions(newQuizID);
                        }
                    }
                    else
                    {
                        var questionResult = MessageBox.Show("Would you like to save these questions?",
                        "Save Questions?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(questionResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_newQuestions.Count > 0)
                            {
                                int startAddingHere = (_questions.Count - _newQuestions.Count);
                                for(int i = startAddingHere; startAddingHere < _questions.Count; startAddingHere++)
                                {
                                    for(int j = 0; j < _newQuestions.Count; j++)
                                    {
                                        _newQuestions[j] = _questions[i];
                                    }
                                }

                                // Add any new questions that were added while editing..
                                addNewQuizQuestions(_editQuiz.QuizID);
                            }

                            // Update the existing questions to the new data.
                            saveQuizQuestions(_questions, _editQuiz.QuizID);
                        }
                    }
                }
            }
            else if(_count < _questions.Count) // this is an existing question with more questions after it, so going forward loads the next existing question
            {
                // validate and offer to overwrite the _question[_count], then go on to a new
                var result = MessageBox.Show("Would you like to save these changes, if any?\nYour current question will still be saved by clicking 'Yes', if no changes were made.",
                        "Overwrite Question?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    // validate and offer to save new, then save the quiz & question to the DB
                    if(validateQuestionForm() == false)
                    {
                        return;
                    }

                    // Replace question in the list.
                    _questions[_count] = saveQuestion();

                    if(_editQuiz == null)
                    {
                        // Be sure they want to save the quiz & questions.
                        var quizResult = MessageBox.Show("Your question has been added!\nWould like to save this quiz & its questions?",
                            "Save Quiz?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(quizResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_quizTopic != null)
                            {
                                // This is a new topic, insert new QuizTopic to DB.
                                addNewQuizTopic();
                            }

                            // Insert quiz to DB & get the QuizID for the new Quiz.
                            int newQuizID = addNewQuiz();

                            // Insert questions per quiz to DB.
                            addNewQuizQuestions(newQuizID);
                        }
                    }
                    else
                    {
                        // Editing

                        var questionResult = MessageBox.Show("Would you like to save these questions?",
                        "Save Questions?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(questionResult == MessageBoxResult.No)
                        {
                            return;
                        }
                        else
                        {
                            if(_newQuestions.Count > 0)
                            {
                                int startAddingHere = (_questions.Count - _newQuestions.Count);
                                for(int i = startAddingHere; startAddingHere < _questions.Count; startAddingHere++)
                                {
                                    for(int j = 0; j < _newQuestions.Count; j++)
                                    {
                                        _newQuestions[j] = _questions[i];
                                    }
                                }

                                // Add any new questions that were added while editing..
                                addNewQuizQuestions(_editQuiz.QuizID);
                            }

                            // Update the existing questions to the new data.
                            saveQuizQuestions(_questions, _editQuiz.QuizID);
                        }
                    }
                }
            }
        }

        private void cboQuestionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cboCorrectAnswer.IsEditable = false;

            if((cboQuestionType.SelectedItem as string) == "Multiple Choice")
            {
                toggleMultipleChoiceAnswers(true);

                cboCorrectAnswer.MaxDropDownHeight = 288;

                _multiChoiceAnswers[0] = txtAnswer1.Text;
                _multiChoiceAnswers[1] = txtAnswer2.Text;
                _multiChoiceAnswers[2] = txtAnswer3.Text;
                _multiChoiceAnswers[3] = txtAnswer4.Text;

                cboCorrectAnswer.ItemsSource = null;
                cboCorrectAnswer.ItemsSource = _multiChoiceAnswers;
            }
            else if((cboQuestionType.SelectedItem as string) == "True/False")
            {
                toggleMultipleChoiceAnswers(false);

                cboCorrectAnswer.MaxDropDownHeight = 288;
                cboCorrectAnswer.ItemsSource = null;
                cboCorrectAnswer.ItemsSource = new List<string> { "True", "False" };
            }
            else if((cboQuestionType.SelectedItem as string) == "Short Answer")
            {
                toggleMultipleChoiceAnswers(false);

                cboCorrectAnswer.MaxDropDownHeight = 0;
                cboCorrectAnswer.ItemsSource = null;
                cboCorrectAnswer.IsEditable = true;
            }
        }
        private void txtAnswer1_TextChanged(object sender, TextChangedEventArgs e)
        {
            _multiChoiceAnswers[0] = txtAnswer1.Text;
            cboCorrectAnswer.Items.Refresh();
        }

        private void txtAnswer2_TextChanged(object sender, TextChangedEventArgs e)
        {
            _multiChoiceAnswers[1] = txtAnswer2.Text;
            cboCorrectAnswer.Items.Refresh();
        }

        private void txtAnswer3_TextChanged(object sender, TextChangedEventArgs e)
        {
            _multiChoiceAnswers[2] = txtAnswer3.Text;
            cboCorrectAnswer.Items.Refresh();
        }

        private void txtAnswer4_TextChanged(object sender, TextChangedEventArgs e)
        {
            _multiChoiceAnswers[3] = txtAnswer4.Text;
            cboCorrectAnswer.Items.Refresh();
        }







        // Helper Methods
        private void getAllQuestionTypes()
        {
            try
            {
                _questionTypes = _quizManager.GetAllQuestionTypes();
                cboQuestionType.ItemsSource = _questionTypes;
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private void toggleMultipleChoiceAnswers(bool isVisible)
        {
            if(isVisible == false)
            {
                lblAnswer1.Visibility = Visibility.Hidden;
                txtAnswer1.Visibility = Visibility.Hidden;
                txtAnswer1.Text = null;
                txtAnswer1.IsEnabled = false;

                lblAnswer2.Visibility = Visibility.Hidden;
                txtAnswer2.Visibility = Visibility.Hidden;
                txtAnswer2.Text = null;
                txtAnswer2.IsEnabled = false;

                lblAnswer3.Visibility = Visibility.Hidden;
                txtAnswer3.Visibility = Visibility.Hidden;
                txtAnswer3.Text = null;
                txtAnswer3.IsEnabled = false;

                lblAnswer4.Visibility = Visibility.Hidden;
                txtAnswer4.Visibility = Visibility.Hidden;
                txtAnswer4.Text = null;
                txtAnswer4.IsEnabled = false;

                // Source: https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.grid.setrowspan?view=windows-app-sdk-1.6
                Grid.SetRow(lblPrompt, 3);
                Grid.SetRow(txtPrompt, 3);
                Grid.SetRowSpan(lblPrompt, 2);
                Grid.SetRowSpan(txtPrompt, 2);
            }
            else
            {
                lblAnswer1.Visibility = Visibility.Visible;
                txtAnswer1.Visibility = Visibility.Visible;
                txtAnswer1.IsEnabled = true;

                lblAnswer2.Visibility = Visibility.Visible;
                txtAnswer2.Visibility = Visibility.Visible;
                txtAnswer2.IsEnabled = true;

                lblAnswer3.Visibility = Visibility.Visible;
                txtAnswer3.Visibility = Visibility.Visible;
                txtAnswer3.IsEnabled = true;

                lblAnswer4.Visibility = Visibility.Visible;
                txtAnswer4.Visibility = Visibility.Visible;
                txtAnswer4.IsEnabled = true;

                // Source: https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.grid.setrowspan?view=windows-app-sdk-1.6
                Grid.SetRow(lblPrompt, 0);
                Grid.SetRow(txtPrompt, 0);
                Grid.SetRowSpan(lblPrompt, 1);
                Grid.SetRowSpan(txtPrompt, 1);
            }
        }
        private bool validateQuestionForm()
        {
            // Validate the Form Input
            if(txtPrompt.Text.Length < 2 || txtPrompt.Text.Length > 250)
            {
                MessageBox.Show("Invalid Prompt...");
                txtPrompt.Focus();
                txtPrompt.SelectAll();
                return false;
            }
            if(cboQuestionType.Text == "Multiple Choice")
            {
                if(txtAnswer1.Text.Length < 1 || txtAnswer1.Text.Length > 250)
                {
                    MessageBox.Show("Invalid Answer Option...");
                    txtAnswer1.Focus();
                    txtAnswer1.SelectAll();
                    return false;
                }
                if(txtAnswer2.Text.Length < 1 || txtAnswer2.Text.Length > 250)
                {
                    MessageBox.Show("Invalid Answer Option...");
                    txtAnswer2.Focus();
                    txtAnswer2.SelectAll();
                    return false;
                }
                if(txtAnswer3.Text.Length < 1 || txtAnswer3.Text.Length > 250)
                {
                    MessageBox.Show("Invalid Answer Option...");
                    txtAnswer3.Focus();
                    txtAnswer3.SelectAll();
                    return false;
                }
                if(txtAnswer4.Text.Length < 1 || txtAnswer4.Text.Length > 250)
                {
                    MessageBox.Show("Invalid Answer Option...");
                    txtAnswer4.Focus();
                    txtAnswer4.SelectAll();
                    return false;
                }
            }
            else if(cboQuestionType.Text == "Short Answer")
            {
                if(cboCorrectAnswer.Text.Length > 250)
                {
                    MessageBox.Show("Invalid Correct Answer...");
                    cboCorrectAnswer.Text = "";
                    cboCorrectAnswer.Focus();
                    return false;
                }
            }

            if((cboCorrectAnswer.Text == null) || (cboCorrectAnswer.Text == ""))
            {
                MessageBox.Show("You must enter or choose the correct answer to your prompt.");
                cboCorrectAnswer.Focus();
                return false;
            }

            return true;
        }

        private void loadPreviousNextCreatedQuestion()
        {
            // Pull their prompt & answers, whether they click Back or Next (if they go back, then forward again).
            txtPrompt.Text = _questions[_count].Prompt;

            getAllQuestionTypes();

            for(int i = 0; i < _questionTypes.Count; i++)
            {
                if(_questions[_count].QuestionTypeID == _questionTypes[i])
                {
                    cboQuestionType.Text = _questionTypes[i];
                    break;
                }
            }

            if((cboQuestionType.SelectedItem as string) == "Multiple Choice")
            {
                toggleMultipleChoiceAnswers(true);

                txtAnswer1.Text = _questions[_count].Answer1;
                txtAnswer2.Text = _questions[_count].Answer2;
                txtAnswer3.Text = _questions[_count].Answer3;
                txtAnswer4.Text = _questions[_count].Answer4;

                _multiChoiceAnswers[0] = _questions[_count].Answer1;
                _multiChoiceAnswers[1] = _questions[_count].Answer2;
                _multiChoiceAnswers[2] = _questions[_count].Answer3;
                _multiChoiceAnswers[3] = _questions[_count].Answer4;

                cboCorrectAnswer.ItemsSource = null;
                cboCorrectAnswer.ItemsSource = _multiChoiceAnswers;

                for(int i = 0; i < _multiChoiceAnswers.Count; i++)
                {
                    if(_questions[_count].CorrectAnswer == _multiChoiceAnswers[i])
                    {
                        cboCorrectAnswer.SelectedItem = _multiChoiceAnswers[i];
                    }
                }
            }
            if(cboQuestionType.Text == "True/False")
            {
                List<string> trueFalseList = new List<string> { "True", "False" };
                cboCorrectAnswer.ItemsSource = trueFalseList;

                for(int i = 0; i < trueFalseList.Count; i++)
                {
                    if(_questions[_count].CorrectAnswer == trueFalseList[i])
                    {
                        cboCorrectAnswer.SelectedItem = trueFalseList[i];
                    }
                }
            }
            if(cboQuestionType.Text == "Short Answer")
            {
                cboCorrectAnswer.Text = _questions[_count].CorrectAnswer;
            }

            if(_editQuiz != null)
            {
                if(_questions[_count].Active == true)
                {
                    chkActive.IsChecked = true;
                }
            }
        }
        private Question saveQuestion()
        {
            Question q;

            if(_editQuiz == null)
            {
                // No QuizID.

                // Assign the entered values to a Question object, which will get saved to the list.
                q = new Question
                {
                    QuestionTypeID = cboQuestionType.Text,
                    Prompt = txtPrompt.Text,
                    Answer1 = txtAnswer1.Text,
                    Answer2 = txtAnswer2.Text,
                    Answer3 = txtAnswer3.Text,
                    Answer4 = txtAnswer4.Text,
                    CorrectAnswer = cboCorrectAnswer.Text
                };
            }
            else
            {
                // Assign the QuizID.
                if(_count + 1 > _questions.Count)
                {
                    // New Question being added to the Quiz.
                    q = new Question
                    {
                        QuestionTypeID = cboQuestionType.Text,
                        QuizID = _editQuiz.QuizID,
                        Prompt = txtPrompt.Text,
                        Answer1 = txtAnswer1.Text,
                        Answer2 = txtAnswer2.Text,
                        Answer3 = txtAnswer3.Text,
                        Answer4 = txtAnswer4.Text,
                        CorrectAnswer = cboCorrectAnswer.Text,
                        Active = (chkActive.IsChecked == true) ? true : false
                    };
                }
                else
                {
                    // Assign the entered values to a Question object, which will get saved to the list.
                    q = new Question
                    {
                        QuestionID = _questions[_count].QuestionID,
                        QuestionTypeID = cboQuestionType.Text,
                        QuizID = _editQuiz.QuizID,
                        Prompt = txtPrompt.Text,
                        Answer1 = txtAnswer1.Text,
                        Answer2 = txtAnswer2.Text,
                        Answer3 = txtAnswer3.Text,
                        Answer4 = txtAnswer4.Text,
                        CorrectAnswer = cboCorrectAnswer.Text,
                        Active = (chkActive.IsChecked == true) ? true : false
                    };
                }
            }

            return q;
        }
        private void displayEmptyQuestion()
        {
            // Clear all fields after adding to the list.
            txtPrompt.Text = "";
            cboQuestionType.SelectedItem = null;
            txtAnswer1.Text = "";
            txtAnswer2.Text = "";
            txtAnswer3.Text = "";
            txtAnswer4.Text = "";
            cboCorrectAnswer.Text = "";
            cboCorrectAnswer.SelectedItem = null;
            cboCorrectAnswer.ItemsSource = null;
        }
        private void addNewQuizTopic()
        {
            try
            {
                bool topicResult = _quizManager.AddNewQuizTopic(_quizTopic.QuizTopicID, _quizTopic.Description);
                if(topicResult == false)
                {
                    throw new Exception("Quiz Topic Not Added...");
                }
                else
                {
                    MessageBox.Show("New Quiz Topic Added Successfully!");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
        private int addNewQuiz()
        {
            int newQuizID = 0;
            try
            {
                newQuizID = _quizManager.AddNewQuiz(_quiz.QuizTopicID, _quiz.Name, _quiz.CreatedBy, _quiz.Description);
                if(newQuizID == 0)
                {
                    throw new Exception("Quiz Not Added...");
                }
                else
                {
                    MessageBox.Show("New Quiz Added Successfully!");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }

            return newQuizID;
        }

        private void addNewQuizQuestions(int newQuizID)
        {
            try
            {
                if(_editQuiz == null)
                {
                    // Creating new Quiz.
                    for(int i = 0; i < _questions.Count; i++)
                    {
                        bool questionResult = _questionManager.AddNewQuizQuestion(_questions[i].QuestionTypeID, newQuizID,
                                                    _questions[i].Prompt, _questions[i].Answer1, _questions[i].Answer2,
                                                    _questions[i].Answer3, _questions[i].Answer4, _questions[i].CorrectAnswer);
                        if(questionResult == false)
                        {
                            throw new Exception("Quiz Question Not Added...");
                        }
                    }

                    // If no exception thrown in the loop, questions were added!
                    MessageBox.Show("New Quiz Questions Added Successfully!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    // Editing existing Quiz - just adding new questions to it.
                    for(int i = 0; i < _newQuestions.Count; i++)
                    {
                        bool questionResult = _questionManager.AddNewQuizQuestion(_newQuestions[i].QuestionTypeID, newQuizID,
                                                    _newQuestions[i].Prompt, _newQuestions[i].Answer1, _newQuestions[i].Answer2,
                                                    _newQuestions[i].Answer3, _newQuestions[i].Answer4, _newQuestions[i].CorrectAnswer);
                        if(questionResult == false)
                        {
                            throw new Exception("Quiz Question Not Added...");
                        }
                    }

                    MessageBox.Show("New Quiz Questions Added Successfully!");
                }
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }

        private void saveQuizQuestions(List<Question> questions, int quizID)
        {
            try
            {
                for(int i = 0; i < _questions.Count - _newQuestions.Count; i++)
                {
                    bool questionResult = _questionManager.EditQuestionInformation(questions[i].QuestionID, questions[i].QuestionTypeID, quizID,
                                                questions[i].Prompt, questions[i].Answer1, questions[i].Answer2, questions[i].Answer3,
                                                questions[i].Answer4, questions[i].CorrectAnswer, questions[i].Active);
                    if(questionResult == false)
                    {
                        throw new Exception("Quiz Question Not Updated...");
                    }
                }

                // If no exception thrown in the loop, questions were added!
                MessageBox.Show("Quiz Questions Updated Successfully!");
                this.DialogResult = true;
                this.Close();
            }
            catch(Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.Message + "\n\n" + ex.InnerException.Message;
                MessageBox.Show(message);
            }
        }
    }
}