using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;


namespace Common.Utilities
{
    public static class SeleniumExtensions
    {
        public static IWebElement GetElementIfVisibleAndEnabled(this DefaultWait<IWebDriver> fluentWait, TimeSpan timeoutWait, TimeSpan pollingInterval, By by)
        {
            fluentWait.Timeout = timeoutWait;
            fluentWait.PollingInterval = pollingInterval;
            IWebElement element = fluentWait.Until(condition =>
            {
                try
                {
                    var e = condition.FindElement(by);
                    if (e.Displayed && e.Enabled)
                        return e;
                    else
                        return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (WebDriverTimeoutException)
                {
                    return null;
                }
            });
            return element;
        }

        public static IWebElement GetElementIfVisibleAndEnabled(this WebDriverWait wait, By by)
        {
            IWebElement element = wait.Until(condition =>
            {
                try
                {
                    var e = condition.FindElement(by);
                    if (e.Displayed && e.Enabled)
                        return e;
                    else
                        return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (WebDriverTimeoutException)
                {
                    return null;
                }
            });
            return element;
        }

        public static IWebElement GetElementIfEnabled(this WebDriverWait wait, By by)
        {
            IWebElement element = wait.Until(condition =>
            {
                try
                {
                    var e = condition.FindElement(by);
                    if (e.Enabled)
                        return e;
                    else
                        return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (WebDriverTimeoutException)
                {
                    return null;
                }
            });
            return element;
        }

        public static IWebElement GetElementIfNotDisplayedAndEnabled(this WebDriverWait wait, By by)
        {

            IWebElement element = wait.Until(condition =>
            {
                try
                {
                    var e = condition.FindElement(by);
                    if (!e.Displayed && e.Enabled)
                        return e;
                    else
                        return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (WebDriverTimeoutException)
                {
                    return null;
                }
            });
            return element;
        }

        public static bool IsWebElementFound(this WebDriverWait wait, By by)
        {
            bool result = true;
            try
            {
                IWebElement buttonStaySigninSubmit = wait.GetElementIfVisibleAndEnabled(by);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static void WaitForElementToBecomeStaleOrUnavailable(this IWebDriver driver, By by, int maxWaitInSeconds)
        {
            for (int i = 0; i <= maxWaitInSeconds; i++)
            {
                try
                {
                    driver.FindElement(by);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch (StaleElementReferenceException)
                { return; }
                catch (NoSuchElementException)
                { return; }
            }
        }

        public static void WaitForElementToBecomeNotDisplayed(this IWebElement element, int maxWaitInSeconds)
        {
            for (int i = 0; i <= maxWaitInSeconds; i++)
            {
                try
                {
                    if (!element.Displayed)
                    { break; }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch (StaleElementReferenceException)
                { return; }
                catch (NoSuchElementException)
                { return; }
            }
        }

        public static IWebElement FindElementNullable(this IWebDriver driver, By by)
        {
            IWebElement result = null;
            try
            { result = driver.FindElement(by); }
            catch (Exception)
            { }
            return result;
        }
    }
}
