using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace GestorPacientes.UITests
{
    public class LoginTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
          
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }

        [Test]
        public void LoginConCredencialesValidas_DeberiaRedirigirADashboard()
        {
           
            driver.Navigate().GoToUrl("https://localhost:7188/Usuario/Login");

            
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            driver.FindElement(By.Id("Contrasena")).SendKeys("123");

           
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            wait.Until(drv => drv.Url.Contains("/Home"));

           
            Assert.That(driver.Url, Does.Contain("/Home"));
        }

        [Test]
        public void LoginConCredencialesInvalidas_DeberiaMostrarError()
        {
            driver.Navigate().GoToUrl("https://localhost:7188/Usuario/Login");

            driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            driver.FindElement(By.Id("Contrasena")).SendKeys("claveIncorrecta");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

         
            wait.Until(driver =>
            {
                try
                {
                    var error = driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    return error.Displayed && error.Text.Contains("Datos de acceso incorrecto", StringComparison.OrdinalIgnoreCase);
                }
                catch (StaleElementReferenceException) { return false; }
                catch (NoSuchElementException) { return false; }
            });
        }
    }
}
