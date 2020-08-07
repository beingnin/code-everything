using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.ML;
using Microsoft.ML.Data;
using ML.DetectFakeJobPosts.DataModels;

namespace ML.DetectFakeJobPosts
{
    public class Program
    {
        static void Main(string[] args)
        {
            var analyzer = new Analyzer();
            analyzer.Train();
        }
    }
    public class Analyzer
    {
        private MLContext _context;
        public Analyzer()
        {
            _context = new MLContext();
        }
        public void Train()
        {
            var root = new System.IO.DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            var path = root + @"\DataSets\fake_job_postings.csv";


            var dataView = _context.Data.LoadFromTextFile<JobPostInput>(path: path, hasHeader: true, separatorChar: ',',allowQuoting:true);
            var dataTable = dataView.ToDataTable();
            //string to data vectors
            var pipeline = _context.Transforms.Categorical.OneHotEncoding("ec_title", "title")
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_location", "location"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_department", "department"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_salary_range", "salary_range"))
                .Append(_context.Transforms.Text.FeaturizeText("ec_company_profile", "company_profile"))
                .Append(_context.Transforms.Text.FeaturizeText("ec_description", "description"))
                .Append(_context.Transforms.Text.FeaturizeText("ec_requirements", "requirements"))
                .Append(_context.Transforms.Text.FeaturizeText("ec_benefits", "benefits"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_employment_type", "employment_type"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_required_experience", "required_experience"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_required_education", "required_education"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_industry", "industry"))
                .Append(_context.Transforms.Categorical.OneHotEncoding("ec_function", "function"))

                //drop unnecessary columns from view
                .Append(_context.Transforms.DropColumns("title", "location", "department", "salary_range", "company_profile", "description", "requirements", "benefits", "employment_type", "required_experience", "required_education", "industry", "function", "telecommuting", "has_company_logo", "has_questions"))

                //concate features
                .Append(_context.Transforms.Concatenate("Features", "ec_title", "ec_location", "ec_department", "ec_salary_range", "ec_company_profile", "ec_description", "ec_requirements", "ec_benefits", "ec_employment_type", "ec_required_experience", "ec_required_education", "ec_industry", "ec_function"))
                //set label/prediction column
                .Append(_context.Transforms.Conversion.ConvertType("Label", "fraudulent", DataKind.Boolean))
                //select a trainer
                .Append(_context.BinaryClassification.Trainers.SdcaLogisticRegression());


            var model = pipeline.Fit(dataView);

            var engine = _context.Model.CreatePredictionEngine<JobPostInput, FraudDetectionResult>(model);
            _context.Model.Save(model,dataView.Schema, root + @"\DataSets\triained_model");
        }
    }
    public static class DataViewHelper
    {
        public static DataTable ToDataTable(this IDataView dataView)
        {
            DataTable dt = null;
            if (dataView != null)
            {
                dt = new DataTable();
                var preview = dataView.Preview();
                dt.Columns.AddRange(preview.Schema.Select(x => new DataColumn(x.Name)).ToArray());
                foreach (var row in preview.RowView)
                {
                    var r = dt.NewRow();
                    foreach (var col in row.Values)
                    {
                        r[col.Key] = col.Value;
                    }
                    dt.Rows.Add(r);

                }
            }
            return dt;
        }
    }
}
