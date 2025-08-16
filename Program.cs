using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Question
{
    public string Text { get; set; }
    public string Topic { get; set; }
    public string Difficulty { get; set; }

    public Question(string text, string topic, string difficulty)
    {
        Text = text;
        Topic = topic;
        Difficulty = difficulty;
    }
}

public class QuizBuilder
{
    private List<Question> _questionBank;
    private Random _random;

    public QuizBuilder(List<Question> questionBank)
    {
        _questionBank = questionBank;
    }

    public List<Question> BuildQuiz(Dictionary<string, Dictionary<string, int>> blueprint, int seed)
    {
        _random = new Random(seed);
        List<Question> selectedQuestions = new List<Question>();

        foreach (var topic in blueprint)
        {
            string topicName = topic.Key;
            foreach (var difficulty in topic.Value)
            {
                string difficultyLevel = difficulty.Key;
                int requiredCount = difficulty.Value;

                var questions = _questionBank
                    .Where(q => q.Topic == topicName && q.Difficulty == difficultyLevel)
                    .ToList();

                if (questions.Count < requiredCount)
                {
                    throw new InvalidOperationException($"Not enough questions for topic '{topicName}' and difficulty '{difficultyLevel}'. Required: {requiredCount}, Available: {questions.Count}");
                }

                var selected = questions.OrderBy(q => _random.Next()).Take(requiredCount).ToList();
                selectedQuestions.AddRange(selected);
            }
        }

        return selectedQuestions;
    }
}

namespace Quiz_Randomizer_with_Blueprint
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Question> questionBank = new List<Question>
        {
            new Question("What is 2 + 2?", "Math", "Easy"),
            new Question("What is the capital of France?", "Geography", "Easy"),
            new Question("What is 5 * 6?", "Math", "Medium"),
            new Question("What is the largest ocean?", "Geography", "Medium"),
            new Question("What is the derivative of x^2?", "Math", "Hard"),
            new Question("What is the capital of Japan?", "Geography", "Hard"),
        };
            Dictionary<string, Dictionary<string, int>> blueprint = new Dictionary<string, Dictionary<string, int>>
        {
            { "Math", new Dictionary<string, int> { { "Easy", 1 }, { "Medium", 1 }, { "Hard", 1 } } },
            { "Geography", new Dictionary<string, int> { { "Easy", 1 }, { "Medium", 1 }, { "Hard", 1 } } }
        };
            QuizBuilder quizBuilder = new QuizBuilder(questionBank);

            int seed = 42;
            try
            {
                List<Question> quiz = quizBuilder.BuildQuiz(blueprint, seed);
                Console.WriteLine("Quiz Questions:");
                foreach (var question in quiz)
                {
                    Console.WriteLine($"- {question.Text} (Topic: {question.Topic}, Difficulty: {question.Difficulty})");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
