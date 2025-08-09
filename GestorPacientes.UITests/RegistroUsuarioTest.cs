using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace GestorPacientes.UITests
{
    [TestFixture]
    public class RegistroUsuarioTest : BaseUiTest
    {
        [SetUp]
        public void BeforeEach() => LoginSiEsNecesario();

        [Test]
        public void RegistrarUsuario_SinCamposObligatorios_DeberiaMostrarValidationSummary()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            Wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']"))).Click();

            bool hayErrores = Wait.Until(d =>
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
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            Driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            Driver.FindElement(By.Id("Apellido")).SendKeys("Uno");
            Driver.FindElement(By.Id("Correo")).SendKeys("correo-invalido"); // formato incorrecto
            Driver.FindElement(By.Id("NombreUsuario")).SendKeys("userMailBad" + Guid.NewGuid().ToString("N")[..4]);
            Driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            Driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("Password123!");
            new SelectElement(Driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

            // Desactivar HTML5 y forzar submit para ver mensajes del servidor/cliente
            ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                var f = document.querySelector('form');
                if (f) { f.setAttribute('novalidate','novalidate'); f.submit(); }
            ");

            // Debe permanecer en Create por modelo inválido
            Wait.Until(d => d.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase));

            bool hayErrorCorreo = Wait.Until(d =>
            {
                try
                {
                    var span = d.FindElement(By.CssSelector("span[data-valmsg-for='Correo']"));
                    var cls = span.GetAttribute("class") ?? "";
                    if (cls.Contains("field-validation-error", StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(span.Text)) return true;

                    var li = d.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    return !string.IsNullOrWhiteSpace(li.Text);
                }
                catch (NoSuchElementException) { return false; }
                catch (StaleElementReferenceException) { return false; }
            });

            Assert.That(hayErrorCorreo, Is.True, "No se mostraron mensajes de validación para Correo.");

            // Intentar validar el texto esperado
            try
            {
                var span = Driver.FindElement(By.CssSelector("span[data-valmsg-for='Correo']"));
                if (!string.IsNullOrWhiteSpace(span.Text))
                    Assert.That(span.Text, Does.Contain("correo válido").IgnoreCase);
                else
                {
                    var li = Driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                    Assert.That(li.Text, Does.Contain("correo válido").IgnoreCase);
                }
            }
            catch (NoSuchElementException)
            {
                var li = Driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                Assert.That(li.Text, Does.Contain("correo válido").IgnoreCase);
            }
        }

        [Test]
        public void RegistrarUsuario_PasswordsNoCoinciden_DeberiaMostrarErrorEnConfirmacion()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            Driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            Driver.FindElement(By.Id("Apellido")).SendKeys("Dos");
            Driver.FindElement(By.Id("Correo")).SendKeys($"qa{Guid.NewGuid():N}@test.com");
            Driver.FindElement(By.Id("NombreUsuario")).SendKeys("userPwdMismatch" + Guid.NewGuid().ToString("N")[..4]);
            Driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            Driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("PasswordX"); // no coincide
            new SelectElement(Driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var spanConfirm = Wait.Until(d =>
                d.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='ConfirmarContrasena'], span.field-validation-error[data-valmsg-for='ConfirmarContrasena']"))
            );

            Assert.That(spanConfirm.Text, Does.Contain("no coinciden").IgnoreCase,
                "No se mostró el mensaje de validación esperado para confirmación de contraseña.");
        }

        [Test]
        public void RegistrarUsuario_SinTipoUsuario_DeberiaMostrarErrorEnTipoUsuario()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Create");

            Driver.FindElement(By.Id("Nombre")).SendKeys("Test");
            Driver.FindElement(By.Id("Apellido")).SendKeys("Tres");
            Driver.FindElement(By.Id("Correo")).SendKeys($"qa{Guid.NewGuid():N}@test.com");
            Driver.FindElement(By.Id("NombreUsuario")).SendKeys("userSinTipo" + Guid.NewGuid().ToString("N")[..4]);
            Driver.FindElement(By.Id("Contrasena")).SendKeys("Password123!");
            Driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys("Password123!");

            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var spanTipo = Wait.Until(d =>
                d.FindElement(By.CssSelector("span.text-danger[data-valmsg-for='TipoUsuario'], span.field-validation-error[data-valmsg-for='TipoUsuario']"))
            );

            Assert.That(spanTipo.Text, Does.Contain("obligatorio").IgnoreCase,
                "No se mostró el mensaje de validación esperado para TipoUsuario requerido.");
        }
    }
}
