using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.DetectFakeJobPosts.DataModels
{
    public class FraudDetectionResult
    {
        [ColumnNameAttribute("PredictedValue")]
        public bool IsFraud { get; set; }
        public float Score { get; set; }
    }
}
