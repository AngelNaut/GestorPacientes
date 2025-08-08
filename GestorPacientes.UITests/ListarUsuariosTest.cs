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
    public class ListarUsuariosTest
    {
        private ChromeDriver driver;
        private WebDriverWait wait;

        private const string BaseUrl = "https://localhost:7188"; // ⚠️ ajusta si cambia
        private const string AdminUser = "angel";
        private const string AdminPass = "123";

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless=new"); // opcional CI
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(12));

            LoginSiEsNecesario();
        }

        [TearDown]
        public void TearDown()
        {
            try { driver?.Quit(); }
            finally { driver?.Dispose(); }
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
        public void ListadoUsuarios_DeberiaMostrarEstructuraCorrecta()
        {
            // 1) Ir al Index de usuarios
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

            // 2) Validar título
            var h2 = wait.Until(d => d.FindElement(By.CssSelector("h2")));
            Assert.That(h2.Text, Does.Contain("Mantenimiento de Usuarios"), "No se encontró el título esperado.");

            // 3) Validar botón "Crear Usuario"
            var btnCrear = wait.Until(d => d.FindElement(By.XPath("//a[contains(@class,'btn') and contains(., 'Crear Usuario')]")));
            Assert.That(btnCrear.Displayed, Is.True, "No se encontró el botón 'Crear Usuario'.");

            // 4) Validar tabla existe
            var tabla = wait.Until(d => d.FindElement(By.CssSelector("table.table")));
            Assert.That(tabla.Displayed, Is.True, "No se encontró la tabla de usuarios.");

            // 5) Validar encabezados (tolerante a cambios menores)
            var headers = driver.FindElements(By.CssSelector("table.table thead th")).Select(th => th.Text.Trim()).ToList();
            Assert.That(headers.Count, Is.GreaterThanOrEqualTo(6), "La tabla no tiene la cantidad de columnas esperada.");
            Assert.Multiple(() =>
            {
                Assert.That(headers.Any(h => h.Contains("Nombre", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Nombre'.");
                Assert.That(headers.Any(h => h.Contains("Apellido", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Apellido'.");
                Assert.That(headers.Any(h => h.Contains("Correo", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Correo'.");
                Assert.That(headers.Any(h => h.Contains("NombreUsuario", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'NombreUsuario'.");
                Assert.That(headers.Any(h => h.Contains("TipoUsuario", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'TipoUsuario'.");
                Assert.That(headers.Any(h => h.Contains("Acciones", StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Acciones'.");
            });

            // 6) Validar que hay filas o el mensaje de vacío
            var filas = driver.FindElements(By.CssSelector("table.table tbody tr"));
            bool hayFilasDatos = filas.Any(tr => tr.FindElements(By.CssSelector("td")).Count >= 6
                                                 && !tr.Text.Contains("No se encontraron usuarios.", StringComparison.OrdinalIgnoreCase));

            bool muestraVacio = driver.PageSource.Contains("No se encontraron usuarios.", StringComparison.OrdinalIgnoreCase);

            Assert.That(hayFilasDatos || muestraVacio, Is.True,
                "El listado no muestra filas de usuarios ni el mensaje de 'No se encontraron usuarios.'");
        }
    }
}
