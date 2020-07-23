using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using ML.SentimentAnalyzer.DataModels;

namespace ML.SentimentAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var analyzer = new SentimentAnalyzer();
            analyzer.Train();
            var exit = "";
            while (exit!="y")
            {
                Console.WriteLine("\nWrite a comment\n");
                var sentiment = Console.ReadLine();
                var negative= analyzer.Analyse(sentiment);
                Console.WriteLine($"Your comment is a {(negative?string.Empty:"non")} negative one");
                Console.WriteLine("Do you want to exit? press y for YES and n for NO");
                exit = Console.ReadLine().ToLower();
            }
        }
    }

    class SentimentAnalyzer
    {
        private readonly MLContext _context = null;
        private PredictionEngine<SentimentInput, SentimentAnalysisResult> _engine;
        public SentimentAnalyzer()
        {
            _context = new MLContext();
        }
        public void Train()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Training model....");
            Console.ResetColor();
            var path = new System.IO.DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + @"\DataSets\train.csv";
            var testPath = new System.IO.DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + @"\DataSets\test.csv";

            IDataView dataView = _context.Data.LoadFromTextFile<SentimentInput>(path: path, hasHeader: true, separatorChar: ',');
            IDataView testDataView = _context.Data.LoadFromTextFile<SentimentInput>(path: testPath, hasHeader: true, separatorChar: ',');
            var pre = dataView.Preview();

            var pipeline = _context.Transforms.Text.FeaturizeText("Features", nameof(SentimentInput.Sentiment))
                        .Append(_context.Transforms.Conversion.ConvertType("Label", nameof(SentimentInput.Negative), DataKind.Boolean))
                        .Append(_context.BinaryClassification.Trainers.LinearSvm());

            var model = pipeline.Fit(dataView);

            _engine = _context.Model.CreatePredictionEngine<SentimentInput, SentimentAnalysisResult>(model);


        }

        public bool Analyse(string sentiment)
        {
            return _engine.Predict(new SentimentInput { Sentiment = sentiment }).Negative;
        }
    }
}
