using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.DetectFakeJobPosts.DataModels
{
    public class JobPostInput
    {
        [LoadColumn(0)]
        [ColumnName("job_id")]
        public int JobId { get; set; }
        [LoadColumn(1)]
        [ColumnName("title")]
        public string Title { get; set; }
        [LoadColumn(2)]
        [ColumnName("location")]
        public string Location { get; set; }
        [LoadColumn(3)]
        [ColumnName("department")]
        public string Department { get; set; }
        [LoadColumn(4)]
        [ColumnName("salary_range")]
        public string SalaryRange { get; set; }
        [LoadColumn(5)]
        [ColumnName("company_profile")]
        public string CompanyProfile { get; set; }
        [LoadColumn(6)]
        [ColumnName("description")]
        public string Description { get; set; }
        [LoadColumn(7)]
        [ColumnName("requirements")]
        public string Requirements { get; set; }
        [LoadColumn(8)]
        [ColumnName("benefits")]
        public string Benefits { get; set; }
        [LoadColumn(9)]
        [ColumnName("telecommuting")]
        public float Telecommuting { get; set; }
        [LoadColumn(10)]
        [ColumnName("has_company_logo")]
        public float HasCompanyLogo { get; set; }
        [LoadColumn(11)]
        [ColumnName("has_questions")]
        public float HasQuestions { get; set; }
        [LoadColumn(12)]
        [ColumnName("employment_type")]
        public string EmployementType { get; set; }
        [LoadColumn(13)]
        [ColumnName("required_experience")]
        public string ExperienceRequired { get; set; }
        [LoadColumn(14)]
        [ColumnName("required_education")]
        public string RequiredEducation { get; set; }
        [LoadColumn(15)]
        [ColumnName("industry")]
        public string Industry { get; set; }
        [LoadColumn(16)]
        [ColumnName("function")]
        public string Function { get; set; }
        [LoadColumn(17)]
        [ColumnName("fraudulent")]
        public float IsFraud { get; set; }
    }
}
