using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace IMS.WorkFromHome
{
    public class IMS
    {
        private readonly IWebDriver _driver;
        private IJavaScriptExecutor _javaScriptExecutor;
        public IMS()
        {
            _driver = new ChromeDriver();
        }

        private void WaitForLoading()
        {
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 5, 0));
            Thread.Sleep(1000);

            while (IsElementPresent(By.Id("jquery-overlay")))
            {
                Thread.Sleep(500);
            }

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
        private void WaitTill(Func<bool> condition)
        {
            while (!condition.Invoke())
            {
                Thread.Sleep(500);
            }
        }

        public bool RequestWFH(string range, string reason = "Work From Home")
        {
            _driver.Url = "http://ims.pitsolutions.com/Lists/LogChangeRequest/UserView.aspx";
            WaitTill(() => IsElementPresent(By.Id("idHomePageNewItem")));
            _driver.FindElement(By.Id("idHomePageNewItem")).Click();

            WaitTill(() => IsElementPresent(By.CssSelector(".ms-dlgFrameContainer iframe")));
            _driver.SwitchTo().Frame(_driver.FindElement(By.CssSelector(".ms-dlgFrameContainer iframe")));
            _javaScriptExecutor = (IJavaScriptExecutor)(_driver);

            WaitTill(() => IsElementPresent(By.Id("ctl00_PlaceHolderMain_tr1")));
            var select = new SelectElement(_driver.FindElement(By.Id("ctl00_PlaceHolderMain_ddIssue")));
            select.SelectByValue("2");

            WaitTill(() => IsElementPresent(By.Id("ctl00_PlaceHolderMain_ddProject")));
            _javaScriptExecutor.ExecuteScript("$('#ctl00_PlaceHolderMain_ddProject').attr('onchange','')");
            select = new SelectElement(_driver.FindElement(By.Id("ctl00_PlaceHolderMain_ddProject")));
            select.SelectByValue("396");

            _driver.FindElement(By.Id("ctl00_PlaceHolderMain_txtReason")).SendKeys(reason);

            var dates = GetDates(range);

            int updatedCount = 0;
            foreach (var date in dates)
            {
                if (updatedCount > 10)
                {
                    Thread.Sleep(3000);
                }
                else
                {
                    WaitTill(() => _driver.FindElements(By.CssSelector("#ctl00_PlaceHolderMain_gvLogDetails tbody tr")).Count == updatedCount + 1);
                }
                _javaScriptExecutor.ExecuteScript("$('#ctl00_PlaceHolderMain_ddEntryHour').attr('onchange','')");
                _javaScriptExecutor.ExecuteScript("$('#ctl00_PlaceHolderMain_ddEntrryMinute').attr('onchange','')");
                _javaScriptExecutor.ExecuteScript("$('#ctl00_PlaceHolderMain_ddEntrySecond').attr('onchange','')");
                _driver.FindElement(By.Id("ctl00_PlaceHolderMain_txtEditDate")).Clear();
                _driver.FindElement(By.Id("ctl00_PlaceHolderMain_txtEditDate")).SendKeys(date.ToString("MM'/'dd'/'yyyy"));
                select = new SelectElement(_driver.FindElement(By.Id("ctl00_PlaceHolderMain_ddEntryHour")));
                select.SelectByValue("08");
                select = new SelectElement(_driver.FindElement(By.Id("ctl00_PlaceHolderMain_ddEntrryMinute")));
                select.SelectByValue("0");
                select = new SelectElement(_driver.FindElement(By.Id("ctl00_PlaceHolderMain_ddEntrySecond")));
                select.SelectByValue("0");
                _driver.FindElement(By.Id("ctl00_PlaceHolderMain_btnOk")).Click();
                ++updatedCount;

            }
            _driver.FindElement(By.Id("ctl00_PlaceHolderMain_btnSave")).Click();
            return true;
        }

        private IList<DateTime> GetDates(string range)
        {
            IList<DateTime> result = null;
            Regex regex = new Regex(@"(\d{1,2}\/\d{1,2}\/\d{4}-\d{1,2}\/\d{1,2}\/\d{4})|\d{1,2}\/\d{1,2}\/\d{4}");
            var matches = regex.Matches(range);
            if (matches.Count > 0)
            {
                result = new List<DateTime>();
                foreach (var item in matches)
                {
                    var splits = item.ToString().Split('-');
                    var start = DateTime.ParseExact(splits[0], "d/M/yyyy", CultureInfo.InvariantCulture);
                    if (splits.Length < 2)
                    {
                        result.Add(start);
                    }
                    else
                    {

                        var end = DateTime.ParseExact(splits[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                        if (start < end)
                        {
                            while (start <= end)
                            {
                                result.Add(start);
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
    }
}
