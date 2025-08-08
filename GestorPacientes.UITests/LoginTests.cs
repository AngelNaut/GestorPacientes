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
            // options.AddArgument("--headless"); // Descomenta si quieres ejecutar sin abrir navegador
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
            // 1. Ir al login
            driver.Navigate().GoToUrl("https://localhost:7188/Usuario/Login");

            // 2. Ingresar usuario y contraseña válidos
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            driver.FindElement(By.Id("Contrasena")).SendKeys("123");

            // 3. Enviar formulario
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 4. Esperar redirección (ajusta si tu dashboard es diferente)
            wait.Until(drv => drv.Url.Contains("/Home"));

            // 5. Validar que se redirigió correctamente
            Assert.That(driver.Url, Does.Contain("/Home"));
        }

        [Test]
        public void LoginConCredencialesInvalidas_DeberiaMostrarError()
        {
            driver.Navigate().GoToUrl("https://localhost:7188/Usuario/Login");

            driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            driver.FindElement(By.Id("Contrasena")).SendKeys("claveIncorrecta");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Esperar y validar el mensaje de error exacto
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
