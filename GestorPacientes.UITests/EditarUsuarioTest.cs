using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace GestorPacientes.UITests
{
    public class EditarUsuarioTest : BaseUiTest
    {
        [Test]
        public void EditarUsuarioValido_DeberiaActualizarDatosEnIndex()
        {
            // 1) Login (usa el helper de la base)
            LoginSiEsNecesario();

            // 2) Ir al listado
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

            // 3) Click en "Editar" del primer usuario (botón amarillo)
            Wait.Until(d => d.FindElement(By.CssSelector("a.btn.btn-sm.btn-warning"))).Click();

            // 4) Asegurar que estamos en Edit
            Wait.Until(d => d.Url.Contains("/Usuario/Edit", StringComparison.OrdinalIgnoreCase));

            // 5) Editar campos
            var nuevoNombre = "NombreEditado" + DateTime.Now.ToString("mmss");

            var nombre = Wait.Until(d => d.FindElement(By.Id("Nombre")));
            nombre.Clear();
            nombre.SendKeys(nuevoNombre);

            var apellido = Driver.FindElement(By.Id("Apellido"));
            apellido.Clear();
            apellido.SendKeys("ApellidoEditado");

            // 6) Guardar
            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // 7) Volver a Index y verificar
            Wait.Until(d => d.Url.Contains("/Usuario/Index", StringComparison.OrdinalIgnoreCase));

            var tabla = Wait.Until(d => d.FindElement(By.CssSelector("table.table")));
            Assert.That(tabla.Text, Does.Contain(nuevoNombre));
            Assert.That(tabla.Text, Does.Contain("ApellidoEditado"));
        }
    }
}
