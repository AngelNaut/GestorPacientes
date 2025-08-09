using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace GestorPacientes.UITests
{
    public class EliminarUsuarioTest : BaseUiTest
    {
        /// <summary>
        /// Crea un usuario por UI y devuelve el NombreUsuario creado.
        /// Usa Driver/Wait de BaseUiTest.
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
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
            Wait.Until(d => d.FindElement(By.XPath("//a[contains(., 'Crear Usuario')]"))).Click();

            Wait.Until(d => d.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase));

            // Llenar formulario
            Driver.FindElement(By.Id("Nombre")).SendKeys(nombre);
            Driver.FindElement(By.Id("Apellido")).SendKeys(apellido);
            Driver.FindElement(By.Id("Correo")).SendKeys(correo);
            Driver.FindElement(By.Id("NombreUsuario")).SendKeys(usuario);
            Driver.FindElement(By.Id("Contrasena")).SendKeys(pass);
            Driver.FindElement(By.Id("ConfirmarContrasena")).SendKeys(pass);
            new SelectElement(Driver.FindElement(By.Id("TipoUsuario"))).SelectByText("Asistente");

            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Esperar que vuelva al listado (o al menos ver la tabla)
            bool enListado = Wait.Until(d =>
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

            if (!enListado || !Driver.Url.Contains("/Usuario", StringComparison.OrdinalIgnoreCase))
            {
                Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
                Wait.Until(d => d.FindElements(By.CssSelector("table.table")).Any());
            }

            // Verificar que aparece en la tabla (reintentos + refresh)
            bool existe = false;
            Exception? lastEx = null;
            for (int i = 0; i < 4 && !existe; i++)
            {
                try
                {
                    if (i > 0) Driver.Navigate().Refresh();

                    var fila = Wait.Until(d => d.FindElement(By.XPath(
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
                Console.WriteLine("URL actual tras crear: " + Driver.Url);
                var html = Driver.PageSource;
                Console.WriteLine(html.Length > 3000 ? html.Substring(0, 3000) : html);

                if (Driver.Url.Contains("/Usuario/Create", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var li = Driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors ul li"));
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
            var fila = Wait.Until(d => d.FindElement(By.XPath(
                $"//table//tr[td[contains(normalize-space(.), '{nombreUsuario}')]]"
            )));
            var link = fila.FindElement(By.XPath(".//a[contains(., 'Eliminar')]"));
            return link.GetAttribute("href");
        }

        [Test]
        public void EliminarUsuario_DeberiaDesaparecerDelListado()
        {
            // 1) Login (helper de la base)
            LoginSiEsNecesario();

            // 2) Crear usuario de prueba
            string usuario = CrearUsuarioRapido();

            // Reafirmar autenticación por si la sesión cambió
            LoginSiEsNecesario();

            // 3) Ir a Index y capturar href del enlace Eliminar
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
            var hrefDelete = GetHrefEliminarPorUsuario(usuario);

            // 4) Ir directo a /Usuario/Delete/{id}
            Driver.Navigate().GoToUrl(hrefDelete);

            // Si te manda a Login, reautenticar y volver a esa URL
            if (Driver.Url.Contains("/Usuario/Login", StringComparison.OrdinalIgnoreCase))
            {
                LoginSiEsNecesario();
                Driver.Navigate().GoToUrl(hrefDelete);
            }

            // 5) Confirmar que estás en Delete
            Wait.Until(d => d.Url.Contains("/Usuario/Delete", StringComparison.OrdinalIgnoreCase));

            // 6) Confirmar eliminación
            Wait.Until(d => d.FindElement(By.CssSelector("form button.btn.btn-danger"))).Click();

            // 7) Volver a Index (y si te manda a Login, reautenticar y volver)
            bool enIndex = Wait.Until(d =>
                d.Url.Contains("/Usuario/Index", StringComparison.OrdinalIgnoreCase) ||
                d.FindElements(By.CssSelector("table.table")).Any()
            );
            if (!enIndex)
            {
                LoginSiEsNecesario();
                Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");
                Wait.Until(d => d.FindElements(By.CssSelector("table.table")).Any());
            }

            // 8) Verificar que YA NO está en la tabla
            bool sigue = false;
            for (int i = 0; i < 3 && !sigue; i++)
            {
                var tabla = Wait.Until(d => d.FindElement(By.CssSelector("table.table")));
                sigue = tabla.Text.Contains(usuario, StringComparison.OrdinalIgnoreCase);
                if (sigue) System.Threading.Thread.Sleep(700);
            }

            Assert.IsFalse(sigue, $"El usuario '{usuario}' todavía aparece en el listado después de eliminarlo.");
        }
    }
}
