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
    public class EliminarUsuarioTest
    {
        private ChromeDriver driver;           // CA1859: tipo concreto
        private WebDriverWait wait;

        private const string BaseUrl = "https://localhost:7188"; // ⚠️ ajusta si cambia
        private const string AdminUser = "angel";
        private const string AdminPass = "123";

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless=new"); // útil en CI/CD
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

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

        /// <summary>
        /// Crea un usuario por UI y devuelve el NombreUsuario creado.
        /// Flujo robusto: espera regreso a Index y reintenta encontrar la fila.
        /// </summary>
        private string CrearUsuarioRapido()
        {
            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var nombre = "Nom" + guid;
            var apellido = "Ape" + guid;
            var correo = $"qa{guid}@test.com";
            var usuario = "usr" + guid;
            var pass = "Password123!";

            // Ir a Index y abrir Create
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
            wait.Until(d => d.FindElement(By.XPath("//a[contains(., 'Crear Usuario')]"))).Click();

            wait.Until(d => d.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase));

            // Llenar formulario
            driver.FindElement(By.Id("Nombre")).SendKeys(nombre);
            driver.FindElement(By.Id("Apellido")).SendKeys(apellido);
            driver.FindElement(By.Id("Correo")).SendKeys(correo);
            driver.FindElement(By.Id("NombreUsuario")).SendKeys(usuario);
            driver.FindElement(By.Id("Contrasena")).SendKeys(pass);
            driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys(pass);
            new SelectElement(driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Esperar que vuelva al listado (o al menos ver la tabla)
            bool enListado = wait.Until(d =>
            {
                try
                {
                    if (d.Url.Contains("/Usuario/Index", StringComparison.OrdinalIgnoreCase) ||
                        d.Url.EndsWith("/Usuario", StringComparison.OrdinalIgnoreCase)) return true;
                    if (d.FindElements(By.CssSelector("table.table")).Any()) return true;
                    return false;
                }
                catch (StaleElementReferenceException) { return false; }
            });

            if (!enListado || !driver.Url.Contains("/Usuario", StringComparison.OrdinalIgnoreCase))
            {
                driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
                wait.Until(d => d.FindElements(By.CssSelector("table.table")).Any());
            }

            // Verificar que aparece en la tabla (con pequeños reintentos + refresh)
            bool existe = false;
            Exception? lastEx = null;
            for (int i = 0; i < 4 && !existe; i++)
            {
                try
                {
                    if (i > 0) driver.Navigate().Refresh();

                    var fila = wait.Until(d => d.FindElement(By.XPath(
                        $"//table//tr[td[contains(normalize-space(.), '{usuario}')]]"
                    )));
                    existe = fila != null;
                    if (!existe) System.Threading.Thread.Sleep(700);
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    System.Threading.Thread.Sleep(700);
                }
            }

            if (!existe)
            {
                Console.WriteLine("URL actual tras crear: " + driver.Url);
                var html = driver.PageSource;
                Console.WriteLine(html.Length > 3000 ? html.Substring(0, 3000) : html);

                if (driver.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var li = driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
                        Console.WriteLine("ValidationSummary: " + li.Text);
                    }
                    catch { /* ignore */ }
                }

                if (lastEx != null) throw lastEx;
                Assert.Fail($"El usuario de prueba '{usuario}' no apareció en el listado.");
            }

            return usuario;
        }

        /// <summary>
        /// Devuelve el href del enlace "Eliminar" para la fila que contiene el NombreUsuario dado.
        /// </summary>
        private string GetHrefEliminarPorUsuario(string nombreUsuario)
        {
            var fila = wait.Until(d => d.FindElement(By.XPath(
                $"//table//tr[td[contains(normalize-space(.), '{nombreUsuario}')]]"
            )));
            var link = fila.FindElement(By.XPath(".//a[contains(., 'Eliminar')]"));
            return link.GetAttribute("href");
        }

        [Test]
        public void EliminarUsuario_DeberiaDesaparecerDelListado()
        {
            // 1) Asegurar login y crear usuario de prueba
            LoginSiEsNecesario();
            string usuario = CrearUsuarioRapido();

            // Reafirmar autenticación por si la sesión cambió
            LoginSiEsNecesario();

            // 2) Ir a Index y obtener el href del enlace Eliminar para ese usuario
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
            var hrefDelete = GetHrefEliminarPorUsuario(usuario);

            // 3) Navegar directamente a /Usuario/Delete/{id}
            driver.Navigate().GoToUrl(hrefDelete);

            // Si te manda a Login, reautenticar y volver a esa URL
            if (driver.Url.Contains("/Usuario/Login", StringComparison.OrdinalIgnoreCase))
            {
                LoginSiEsNecesario();
                driver.Navigate().GoToUrl(hrefDelete);
            }

            // 4) Confirmar que estás en Delete
            wait.Until(d => d.Url.Contains("/Usuario/Delete", StringComparison.OrdinalIgnoreCase));

            // 5) Confirmar eliminación (botón "Aceptar")
            wait.Until(d => d.FindElement(By.CssSelector("form button.btn.btn-danger"))).Click();

            // 6) Volver al Index (y si te manda a Login, reautenticar y volver)
            bool enIndex = wait.Until(d =>
                d.Url.Contains("/Usuario/Index", StringComparison.OrdinalIgnoreCase) ||
                d.FindElements(By.CssSelector("table.table")).Any()
            );
            if (!enIndex)
            {
                LoginSiEsNecesario();
                driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
                wait.Until(d => d.FindElements(By.CssSelector("table.table")).Any());
            }

            // 7) Verificar que YA NO está en la tabla (con pequeños reintentos)
            bool sigue = false;
            for (int i = 0; i < 3 && !sigue; i++)
            {
                var tabla = wait.Until(d => d.FindElement(By.CssSelector("table.table")));
                sigue = tabla.Text.Contains(usuario, StringComparison.OrdinalIgnoreCase);
                if (sigue) System.Threading.Thread.Sleep(700);
            }

            Assert.IsFalse(sigue, $"El usuario '{usuario}' todavía aparece en el listado después de eliminarlo.");
        }
    }
}
