using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.SentimentAnalyzer.DataModels
{
    public class SentimentInput
    {
        [LoadColumn(2)]
        public string Sentiment { get; set; }
        [LoadColumn(1)]
        public bool Negative { get; set; }
    }
}
