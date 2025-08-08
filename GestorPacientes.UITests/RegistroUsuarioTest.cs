using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.UITests
{
    [TestFixture]
    public class RegistroUsuarioTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private const string BaseUrl = "https://localhost:7188"; 
        private const string AdminUser = "angel";                
        private const string AdminPass = "123";

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
          
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));

            LoginSiEsNecesario();
        }

        [TearDown]
        public void TearDown()
        {
            try { driver?.Quit(); } finally { driver?.Dispose(); }
            driver = null;
        }

        private void LoginSiEsNecesario()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

          
            if (driver.Url.Contains("/Usuario/Login", StringComparison.OrdinalIgnoreCase)
                || driver.PageSource.Contains("Iniciar sesión", StringComparison.OrdinalIgnoreCase))
            {
               
                driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Login");

                wait.Until(d => d.FindElement(By.Id("NombreUsuario")));
                driver.FindElement(By.Id("NombreUsuario")).Clear();
                driver.FindElement(By.Id("NombreUsuario")).SendKeys(AdminUser);

                driver.FindElement(By.Id("Contrasena")).Clear();
                driver.FindElement(By.Id("Contrasena")).SendKeys(AdminPass);

                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                
                wait.Until(d => d.Url.Contains("/Home", StringComparison.OrdinalIgnoreCase));
            }
        }

      
        [Test]
        public void RegistrarUsuario_SinCamposObligatorios_DeberiaMostrarValidationSummary()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");
            wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        
            bool hayErrores = wait.Until(d =>
            {
                try
                {
                    var li = d.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    return li.Displayed && !string.IsNullOrWhiteSpace(li.Text);
                }
                catch (NoSuchElementException) { return false; }
                catch (StaleElementReferenceException) { return false; }
            });

            Assert.That(hayErrores, Is.True, "No se mostraron errores en el ValidationSummary al dejar campos obligatorios vacíos.");
        }

       
        [Test]
        public void RegistrarUsuario_CorreoInvalido_DeberiaMostrarErrorEnCorreo()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            driver.FindElement(By.Id("Apellido")).SendKeys("Uno");
            driver.FindElement(By.Id("Correo")).SendKeys("correo-invalido"); 
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("userMailBad" + Guid.NewGuid().ToString("N").Substring(0, 4));
            driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("Password123!");

            new SelectElement(driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

          
            ((IJavaScriptExecutor)driver).ExecuteScript(@"
        var f = document.querySelector('form');
        if (f) { f.setAttribute('novalidate','novalidate'); f.submit(); }
    ");

           
            wait.Until(d => d.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase));

           
            bool hayErrorCorreo = wait.Until(d =>
            {
                try
                {
                  
                    var span = d.FindElement(By.CssSelector("span[data-valmsg-for='Correo']"));
                    var cls = span.GetAttribute("class") ?? "";
                    if (cls.Contains("field-validation-error", StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(span.Text))
                        return true;

                   
                    var sibling = d.FindElements(By.CssSelector("#Correo, input[name='Correo']")).Count > 0
                        ? d.FindElement(By.CssSelector("#Correo, input[name='Correo']")).FindElement(By.XPath("following::span[contains(@class,'text-danger')][1]"))
                        : null;

                    if (sibling != null && !string.IsNullOrWhiteSpace(sibling.Text))
                        return true;

                   
                    var li = d.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    return !string.IsNullOrWhiteSpace(li.Text);
                }
                catch (NoSuchElementException) { return false; }
                catch (StaleElementReferenceException) { return false; }
            });

           
            Assert.That(hayErrorCorreo, Is.True, "No se mostraron mensajes de validación para Correo.");

          
            try
            {
                var span = driver.FindElement(By.CssSelector("span[data-valmsg-for='Correo']"));
                if (!string.IsNullOrWhiteSpace(span.Text))
                    Assert.That(span.Text, Does.Contain("correo válido").IgnoreCase);
                else
                {
                    var li = driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    Assert.That(li.Text, Does.Contain("correo válido").IgnoreCase);
                }
            }
            catch (NoSuchElementException)
            {
                var li = driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                Assert.That(li.Text, Does.Contain("correo válido").IgnoreCase);
            }
        }



      
        [Test]
        public void RegistrarUsuario_PasswordsNoCoinciden_DeberiaMostrarErrorEnConfirmacion()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            driver.FindElement(By.Id("Apellido")).SendKeys("Dos");
            driver.FindElement(By.Id("Correo")).SendKeys($"qa{Guid.NewGuid():N}@test.com");
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("userPwdMismatch" + Guid.NewGuid().ToString("N").Substring(0, 4));
            driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("PasswordX"); // no coincide

            new SelectElement(driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

          
            var spanConfirm = wait.Until(d =>
                d.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='ConfirmarContrasena'], span.field-validation-error[data-valmsg-for='ConfirmarContrasena']"))
            );

            Assert.That(spanConfirm.Text, Does.Contain("no coinciden").IgnoreCase,
                "No se mostró el mensaje de validación esperado para confirmación de contraseña.");
        }

      
        [Test]
        public void RegistrarUsuario_SinTipoUsuario_DeberiaMostrarErrorEnTipoUsuario()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            driver.FindElement(By.Id("Apellido")).SendKeys("Tres");
            driver.FindElement(By.Id("Correo")).SendKeys($"qa{Guid.NewGuid():N}@test.com");
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("userSinTipo" + Guid.NewGuid().ToString("N").Substring(0, 4));
            driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("Password123!");

        

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

          
            var spanTipo = wait.Until(d =>
                d.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='TipoUsuario'], span.field-validation-error[data-valmsg-for='TipoUsuario']"))
            );

            Assert.That(spanTipo.Text, Does.Contain("obligatorio").IgnoreCase,
                "No se mostró el mensaje de validación esperado para TipoUsuario requerido.");
        }
    }
}
