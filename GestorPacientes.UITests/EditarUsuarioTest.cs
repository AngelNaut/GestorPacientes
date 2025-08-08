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
    public class EditarUsuarioTest
    {
       
        private ChromeDriver driver;
        private WebDriverWait wait;

        private const string BaseUrl = "https://localhost:7188";

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless=new"); // útil en CI
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(12));
        }

        [TearDown] // NUnit1032: disponer correctamente
        public void TearDown()
        {
            try { driver?.Quit(); }
            finally { driver?.Dispose(); }
            driver = null;
        }

        private void Login()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Login");
            wait.Until(d => d.FindElement(By.Id("NombreUsuario")));
            driver.FindElement(By.Id("NombreUsuario")).SendKeys("angel");
            driver.FindElement(By.Id("Contrasena")).SendKeys("123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            wait.Until(d => d.Url.Contains("/Home", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void EditarUsuarioValido_DeberiaActualizarDatosEnIndex()
        {
            Login();

            driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

            // Editar el primer usuario de la tabla
            wait.Until(d => d.FindElement(By.CssSelector("a.btn.btn-sm.btn-warning"))).Click();
            wait.Until(d => d.Url.Contains("/Usuario/Edit", StringComparison.OrdinalIgnoreCase));

            var nuevoNombre = "NombreEditado" + DateTime.Now.ToString("mmss");

            var nombre = wait.Until(d => d.FindElement(By.Id("Nombre")));
            nombre.Clear();
            nombre.SendKeys(nuevoNombre);

            var apellido = driver.FindElement(By.Id("Apellido"));
            apellido.Clear();
            apellido.SendKeys("ApellidoEditado");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            wait.Until(d => d.Url.Contains("/Usuario/Index", StringComparison.OrdinalIgnoreCase));

            var tabla = wait.Until(d => d.FindElement(By.CssSelector("table.table")));
            Assert.That(tabla.Text, Does.Contain(nuevoNombre));
            Assert.That(tabla.Text, Does.Contain("ApellidoEditado"));
        }
    }
}
