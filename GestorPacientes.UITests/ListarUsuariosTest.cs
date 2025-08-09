using NUnit.Framework;
using OpenQA.Selenium;
using System.Linq;

namespace GestorPacientes.UITests
{
    [TestFixture]
    public class ListarUsuariosTest : BaseUiTest
    {
        [Test]
        public void ListadoUsuarios_DeberiaMostrarEstructuraCorrecta()
        {
            // 1) Asegurar sesión
            LoginSiEsNecesario();

            // 2) Ir al Index de usuarios
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

            // 3) Validar título
            var h2 = Wait.Until(d => d.FindElement(By.CssSelector("h2")));
            Assert.That(h2.Text, Does.Contain("Mantenimiento de Usuarios"), "No se encontró el título esperado.");

            // 4) Validar botón "Crear Usuario"
            var btnCrear = Wait.Until(d => d.FindElement(By.XPath("//a[contains(@class,'btn') and contains(., 'Crear Usuario')]")));
            Assert.That(btnCrear.Displayed, Is.True, "No se encontró el botón 'Crear Usuario'.");

            // 5) Validar tabla
            var tabla = Wait.Until(d => d.FindElement(By.CssSelector("table.table")));
            Assert.That(tabla.Displayed, Is.True, "No se encontró la tabla de usuarios.");

            // 6) Validar encabezados
            var headers = Driver.FindElements(By.CssSelector("table.table thead th"))
                                .Select(th => th.Text.Trim())
                                .ToList();

            Assert.That(headers.Count, Is.GreaterThanOrEqualTo(6), "La tabla no tiene la cantidad de columnas esperada.");
            Assert.Multiple(() =>
            {
                Assert.That(headers.Any(h => h.Contains("Nombre", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Nombre'.");
                Assert.That(headers.Any(h => h.Contains("Apellido", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Apellido'.");
                Assert.That(headers.Any(h => h.Contains("Correo", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Correo'.");
                Assert.That(headers.Any(h => h.Contains("NombreUsuario", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'NombreUsuario'.");
                Assert.That(headers.Any(h => h.Contains("TipoUsuario", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'TipoUsuario'.");
                Assert.That(headers.Any(h => h.Contains("Acciones", System.StringComparison.OrdinalIgnoreCase)), "Falta encabezado 'Acciones'.");
            });

            // 7) Validar que hay filas o el mensaje de vacío
            var filas = Driver.FindElements(By.CssSelector("table.table tbody tr"));
            bool hayFilasDatos = filas.Any(tr => tr.FindElements(By.CssSelector("td")).Count >= 6
                                                 && !tr.Text.Contains("No se encontraron usuarios.", System.StringComparison.OrdinalIgnoreCase));

            bool muestraVacio = Driver.PageSource.Contains("No se encontraron usuarios.", System.StringComparison.OrdinalIgnoreCase);

            Assert.That(hayFilasDatos || muestraVacio, Is.True,
                "El listado no muestra filas de usuarios ni el mensaje de 'No se encontraron usuarios.'");
        }
    }
}
