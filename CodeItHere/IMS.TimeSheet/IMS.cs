﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IMS.TimeSheet
{
    public class IMS
    {
        private readonly IWebDriver _driver;
        private readonly IJavaScriptExecutor _javaScriptExecutor;
        public IMS()
        {
            _driver = new ChromeDriver();
            _javaScriptExecutor = (IJavaScriptExecutor)_driver;
        }

        private void WaitForLoading()
        {
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 5, 0));
            wait.Until(x => !IsElementPresent(By.Id("jquery-overlay")));
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public void BookMyLog(string sprint, string range)
        {
            //got to ims
            _driver.Url = "http://ims.pitsolutions.com/_layouts/TimesheetEntry_Forms/TimesheetEntry.aspx";
            WaitForLoading();
            _driver.FindElement(By.Id("Timelog")).Click();
            WaitForLoading();
            IList<LogData> details = new List<LogData>();

            if (string.IsNullOrWhiteSpace(range))
            {
                details = GetLogDetailsFromTimeSheet();
            }
            else
            {
                details = GetLogDetailsFromRange(range);
            }

        }
        private IList<LogData> GetLogDetailsFromRange(string range, int hours = 8, int minutes = 0)
        {
            IList<LogData> result = null;
            Regex regex = new Regex(@"(\d{1,2}\/\d{1,2}\/\d{4}-\d{1,2}\/\d{1,2}\/\d{4})|\d{1,2}\/\d{1,2}\/\d{4}");
            var matches = regex.Matches(range);
            if (matches.Count > 0)
            {
                result = new List<LogData>();
                foreach (var item in matches)
                {
                    var splits = item.ToString().Split('-');
                    var start = DateTime.ParseExact(splits[0], "d/M/yyyy", CultureInfo.InvariantCulture);
                    if (splits.Length < 2)
                    {
                        result.Add(new LogData
                        {
                            Date = start,
                            LoggedHours = hours,
                            LoggedMinutes = minutes
                        });
                    }
                    else
                    {

                        var end = DateTime.ParseExact(splits[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                        if (start < end)
                        {
                            while (start <= end)
                            {
                                result.Add(new LogData
                                {
                                    Date = start,
                                    LoggedHours = hours,
                                    LoggedMinutes = minutes
                                });
                                start = start.AddDays(1);
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"{item.ToString()} Specified date range is invalid. Hence ignored");
                            Console.ResetColor();
                        }

                    }
                }
            }

            return result;
        }
        private string GetSprint()
        {
            return null;
        }
        private IList<LogData> GetLogDetailsFromTimeSheet()
        {
            IList<LogData> result = null;
            var elements = _driver.FindElements(By.CssSelector("#Timelogtable tr"));
            if (elements.Count > 1)
            {
                result = new List<LogData>();
                for (int i = 1; i < elements.Count; i++)
                {
                    var tds = elements[i].FindElements(By.TagName("td"));
                    if (string.IsNullOrWhiteSpace(tds[0].Text) || string.IsNullOrWhiteSpace(tds[4].Text) || string.IsNullOrWhiteSpace(tds[5].Text))
                        continue;
                    var total = tds[4].Text.Split(':');
                    var logged = tds[5].Text.Split('.');
                    DateTime.TryParseExact(tds[0].Text, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                    result.Add(new LogData
                    {
                        Date = date,
                        TotalHours = Convert.ToInt32(total[0]),
                        TotalMinutes = total.Length > 1 ? Convert.ToInt32(total[1]) : 0,
                        LoggedHours = Convert.ToInt32(logged[0]),
                        LoggedMinutes = logged.Length > 1 ? Convert.ToInt32(logged[1]) : 0,
                    });
                }
            }

            return result;
        }
        public class LogData
        {
            public DateTime Date { get; set; }
            public int LoggedHours { get; set; }
            public int LoggedMinutes { get; set; }
            public int TotalHours { get; set; }
            public int TotalMinutes { get; set; }
            public override string ToString()
            {
                return Date.ToString("dd-MM-yyyy"); 
            }
        }
    }
}
