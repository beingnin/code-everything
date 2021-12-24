using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace HRMS.Timesheet
{
    public class Timesheet
    {
        private readonly IWebDriver _driver;
        private readonly IJavaScriptExecutor _javaScriptExecutor;
        public Timesheet()
        {
            //ChromeOptions options = new ChromeOptions();
            //options.AddArguments("start-maximized");
            //options.AddArguments("disable-infobars");
            //options.AddArguments("--disable-extensions");
            _driver = new ChromeDriver();
            _javaScriptExecutor = (IJavaScriptExecutor)_driver;
        }
        public IList<LogData> GetLogFromRange(string range, int hours = 8, int minutes = 0)
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
        private void Log(string sprint, LogData logData)
        {


            //project
            WaitUntilExists(By.CssSelector("form .row.mx-1:nth-child(1)")).Click();
            Thread.Sleep(2000);
            var projectSection = WaitUntilExists(By.CssSelector(".v-menu__content.menuable__content__active"));
            var elements = projectSection.FindElements(By.CssSelector(".v-list-item"));
            foreach (var item in elements)
            {
                var option = item.FindElement(By.CssSelector(".v-list-item__content .v-list-item__title"));
                if (option.Text == "Sharjah Police")
                {
                    item.Click();
                }
            }

            //sub project
            WaitUntilExists(By.CssSelector("form .row.mx-1:nth-child(2)")).Click();
            Thread.Sleep(2000);
            var subProjectSection = WaitUntilExists(By.CssSelector(".v-menu__content.menuable__content__active"));
            elements = subProjectSection.FindElements(By.CssSelector(".v-list-item"));
            foreach (var item in elements)
            {
                var option = item.FindElement(By.CssSelector(".v-list-item__content .v-list-item__title"));
                if (option.Text == "SPSA\\"+sprint)
                {
                    item.Click();
                }
            }

            //tasks
            WaitUntilExists(By.CssSelector("form .row.mx-1:nth-child(3)")).Click();
            Thread.Sleep(2000);
            var taskSection = WaitUntilExists(By.CssSelector(".v-menu__content.menuable__content__active"));
            elements = taskSection.FindElements(By.CssSelector(".v-list-item"));
            foreach (var item in elements)
            {
                var option = item.FindElement(By.CssSelector(".v-list-item__content .v-list-item__title"));
                if (option.Text == "Technical support")
                {
                    item.Click();
                }
            }

            //date
            _javaScriptExecutor.ExecuteScript($"document.getElementById('txtDate').value='{logData.Date.ToString("dd-MM-yyyy")}'");
            var hours = logData.LoggedHours.ToString();
            var minutes = logData.LoggedMinutes.ToString();
            hours = hours.Length == 1 ? "0" + hours : hours;
            minutes = minutes.Length == 1 ? "0" + minutes : minutes;
            WaitUntilExists(By.CssSelector("input[data-test-id='ddlTimeTaken']")).SendKeys($"{hours}:{minutes}");
            WaitUntilExists(By.Id("txaDescription")).SendKeys("Support to team members");
            Thread.Sleep(2000);
            _driver.FindElement(By.CssSelector("button[data-test-id='btnSaveNormalTask']")).Click();

        }
        public void Log(string sprint, IList<LogData> logDatas)
        {
            _driver.Url = "https://hrms.pitsolutions.com";
            var btn = WaitUntilExists(By.ClassName("alineAddTimesheet"));
            btn.Click();
            var failedDates = string.Empty;
            foreach (var item in logDatas)
            {
                try
                {
                    Log(sprint, item);

                }
                catch (Exception ex)
                {
                    failedDates += "," + item.Date.ToString("dd'/'MM'/'yyyy");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
            if(!string.IsNullOrEmpty(failedDates))
            {
                Console.WriteLine("The following dates failed to log");
                Console.WriteLine(failedDates);
            }

        }
        #region helpers
        private IWebElement WaitUntilExists(By by, int timeOutMinutes = 5)
        {
            WebDriverWait webDriverWait = new WebDriverWait(_driver, TimeSpan.FromMinutes(timeOutMinutes));
            return webDriverWait.Until(ExpectedConditions.ElementExists(by));
        }
        #endregion
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
