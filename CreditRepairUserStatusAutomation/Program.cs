using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Configuration;

namespace CreditRepairUserStatusAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverPath"]);
            driver.Url = ConfigurationManager.AppSettings["ApplicationUrl"];
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            driver.FindElement(By.Id("username")).SendKeys(ConfigurationManager.AppSettings["UserName"]);
            driver.FindElement(By.Id("password")).SendKeys(ConfigurationManager.AppSettings["Password"]);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.FindElement(By.Id("signin")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            WebElement we = (WebElement)driver.FindElement(By.Id("mainPopupbox"));
            if (we != null)
                we.FindElement(By.Name("got questionimg")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            WebElement navigationMenu = (WebElement)driver.FindElement(By.Id("navigation"));
            if (navigationMenu != null)
                navigationMenu.FindElement(By.LinkText("Clients")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            WebElement tblClients = (WebElement)driver.FindElement(By.Id("datatable-buttons"));
            var clientsTableBody = tblClients.FindElement(By.TagName("tbody"));
            if (clientsTableBody != null)
            {
                var clientRows = clientsTableBody.FindElements(By.TagName("tr"));
                var count = 0;
                if (clientRows.Count > 0)
                {
                    foreach (var client in clientRows)
                    {
                        var row = (WebElement)client;
                        if (row.GetAttribute("class").Contains("odd"))
                        {
                            Actions actions = new Actions(driver);
                            actions.MoveToElement(row);
                            actions.Perform();
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                            Console.WriteLine(row.Text);
                            var userHyperlink = row.FindElement(By.TagName("td")).FindElement(By.TagName("a"));
                            Console.WriteLine(userHyperlink.GetAttribute("href"));
                            var url = userHyperlink.GetAttribute("href");
                            String originalWindow = driver.CurrentWindowHandle;
                            var clientDriver = driver.SwitchTo().NewWindow(WindowType.Window);
                            clientDriver.Url = url;
                            clientDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(50);

                            var progressbarElem = (WebElement)clientDriver.FindElement(By.Id("client-progress-bar-indasboard"));
                            Actions action = new Actions(clientDriver);
                            action.MoveToElement(progressbarElem);
                            action.Perform();
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                            var progressSteps = progressbarElem.FindElements(By.ClassName("client-progress-step"));
                            Console.WriteLine(progressSteps[3].GetAttribute("class"));
                            if (!progressSteps[Convert.ToInt32(ConfigurationManager.AppSettings["OnboardStage"])].GetAttribute("class").Contains("is-active"))
                            {
                                Console.WriteLine("Not onboarded");
                            }
                            else
                            {
                                Console.WriteLine("onboarding complete");
                            }
                            clientDriver.Close();
                            driver.SwitchTo().Window(originalWindow);
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
