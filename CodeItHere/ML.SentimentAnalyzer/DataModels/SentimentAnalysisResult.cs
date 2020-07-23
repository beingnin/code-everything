using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.SentimentAnalyzer.DataModels
{
    public class SentimentAnalysisResult
    {
        [ColumnName("PredictedLabel")]
        public bool Negative { get; set; }
        public float Score { get; set; }
    }
}
