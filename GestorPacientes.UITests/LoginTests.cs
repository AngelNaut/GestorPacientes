using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace GestorPacientes.UITests
{
    [TestFixture]
    public class LoginTests : BaseUiTest
    {
        [Test]
        public void LoginConCredencialesValidas_DeberiaRedirigirADashboard()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Login");

            Driver.FindElement(By.Id("NombreUsuario")).SendKeys(AdminUser);
            Driver.FindElement(By.Id("Contrasena")).SendKeys(AdminPass);

            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            Wait.Until(drv => drv.Url.Contains("/Home", StringComparison.OrdinalIgnoreCase));
            Assert.That(Driver.Url, Does.Contain("/Home"));
        }

        [Test]
        public void LoginConCredencialesInvalidas_DeberiaMostrarError()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Login");

            Driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            Driver.FindElement(By.Id("Contrasena")).SendKeys("claveIncorrecta");
            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Esperar mensaje en ValidationSummary
            bool hayError = Wait.Until(d =>
            {
                try
                {
                    var li = d.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    return li.Displayed && li.Text.Contains("Datos de acceso incorrecto", StringComparison.OrdinalIgnoreCase);
                }
                catch (StaleElementReferenceException) { return false; }
                catch (NoSuchElementException) { return false; }
            });

            Assert.IsTrue(hayError, "No se mostró el mensaje de 'Datos de acceso incorrecto'.");
        }
    }
}
