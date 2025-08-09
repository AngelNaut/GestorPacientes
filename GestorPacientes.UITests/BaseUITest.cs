using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.MarkupUtils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GestorPacientes.UITests
{
    public abstract class BaseUiTest
    {
        protected ChromeDriver Driver;
        protected WebDriverWait Wait;

       
        private static ExtentReports _extent;
        protected ExtentTest Test;

       
        private static string _runFolder;
        private static readonly object _lock = new object();

        protected const string BaseUrl = "https://localhost:7188";
        protected const string AdminUser = "angel";
        protected const string AdminPass = "123";

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            lock (_lock)
            {
                if (_extent != null) return;

                var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                _runFolder = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", $"Run_{timeStamp}");
                Directory.CreateDirectory(_runFolder);
                Directory.CreateDirectory(Path.Combine(_runFolder, "screenshots"));
                Directory.CreateDirectory(Path.Combine(_runFolder, "pagesource"));

                var htmlPath = Path.Combine(_runFolder, "ExtentReport.html");
                var reporter = new ExtentSparkReporter(htmlPath);
                reporter.Config.DocumentTitle = "GestorPacientes - Reporte UI";
                reporter.Config.ReportName = "Pruebas UI con Selenium";

                _extent = new ExtentReports();
                _extent.AttachReporter(reporter);
                _extent.AddSystemInfo("Framework", "NUnit");
                _extent.AddSystemInfo("Selenium", "WebDriver + Chrome");
                _extent.AddSystemInfo("Machine", Environment.MachineName);
                _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            }
        }

        [SetUp]
        public void SetUp()
        {
         
            Test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);

            var options = new ChromeOptions();
           
            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(25);

            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));

         
            try
            {
                var caps = (Driver as IHasCapabilities)?.Capabilities;
                if (caps != null)
                {
                    var browserName = caps?.GetCapability("browserName")?.ToString();
                    var browserVersion = caps?.GetCapability("browserVersion")?.ToString()
                                         ?? caps?.GetCapability("version")?.ToString();
                    Test.Info($"Navegador: {browserName} {browserVersion}");

                }
            }
            catch {  }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var error = TestContext.CurrentContext.Result.FailCount > 0
                            ? (TestContext.CurrentContext.Result.Message ?? "") + "\n" + (TestContext.CurrentContext.Result.StackTrace ?? "")
                            : string.Empty;

              
                var pngPath = SaveScreenshot($"{status}");
                var htmlPath = SavePageSource();

              
                if (!string.IsNullOrEmpty(pngPath))
                {
                    var relShot = GetRelativePath(pngPath, _runFolder);
                    Test.AddScreenCaptureFromPath(relShot);
                }
                if (!string.IsNullOrEmpty(htmlPath))
                {
                    var relHtml = GetRelativePath(htmlPath, _runFolder);
                    Test.Info($"HTML: {relHtml}");
                    Test.Info(MarkupHelper.CreateLabel("Se adjuntó el HTML de la página al finalizar la prueba.", ExtentColor.Blue));
                }

                switch (status)
                {
                    case NUnit.Framework.Interfaces.TestStatus.Passed:
                        Test.Pass("✅ Test Passed");
                        break;
                    case NUnit.Framework.Interfaces.TestStatus.Failed:
                        Test.Fail("❌ Test Failed").Fail(error);
                        break;
                    case NUnit.Framework.Interfaces.TestStatus.Skipped:
                        Test.Skip("⏭️ Test Skipped");
                        break;
                    default:
                        Test.Info("ℹ️ Test Finished with status: " + status);
                        break;
                }
            }
            catch (Exception ex)
            {
                Test.Warning("Error en TearDown/Reporte: " + ex.Message);
            }
            finally
            {
                try { Driver?.Quit(); } finally { Driver?.Dispose(); }
                Driver = null;
            }
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            try { _extent?.Flush(); } catch { }
        }

       

        private string SaveScreenshot(string suffix)
        {
            try
            {
                var fileName = $"{SanitizeFileName(TestContext.CurrentContext.Test.Name)}_{suffix}_{DateTime.Now:HHmmss}.png";
                var full = Path.Combine(_runFolder, "screenshots", fileName);
                var ss = ((ITakesScreenshot)Driver).GetScreenshot();
                File.WriteAllBytes(full, ss.AsByteArray); 
                return full;
            }
            catch { return string.Empty; }
        }

        private string SavePageSource()
        {
            try
            {
                var fileName = $"{SanitizeFileName(TestContext.CurrentContext.Test.Name)}_{DateTime.Now:HHmmss}.html";
                var full = Path.Combine(_runFolder, "pagesource", fileName);
                File.WriteAllText(full, Driver.PageSource);
                return full;
            }
            catch { return string.Empty; }
        }

        protected void LogStep(string message)
        {
            Test.Info(message);
        }

        protected void StepScreenshot(string label)
        {
            var path = SaveScreenshot(label);
            if (!string.IsNullOrEmpty(path))
            {
                var rel = GetRelativePath(path, _runFolder);
                Test.Info($"Paso: {label}");
                Test.AddScreenCaptureFromPath(rel);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return name;
        }

        private static string GetRelativePath(string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                folder += Path.DirectorySeparatorChar;

            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

   
        protected void LoginSiEsNecesario()
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Index");

            if (Driver.Url.Contains("/Usuario/Login", StringComparison.OrdinalIgnoreCase)
                || Driver.PageSource.Contains("Iniciar sesión", StringComparison.OrdinalIgnoreCase))
            {
                Driver.Navigate().GoToUrl($"{BaseUrl}/Usuario/Login");

                Wait.Until(d => d.FindElement(By.Id("NombreUsuario")));
                Driver.FindElement(By.Id("NombreUsuario")).Clear();
                Driver.FindElement(By.Id("NombreUsuario")).SendKeys(AdminUser);

                Driver.FindElement(By.Id("Contrasena")).Clear();
                Driver.FindElement(By.Id("Contrasena")).SendKeys(AdminPass);

                Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                Wait.Until(d => d.Url.Contains("/Home", StringComparison.OrdinalIgnoreCase));
                Test.Info("Login exitoso como administrador.");
            }
        }
    }
}
