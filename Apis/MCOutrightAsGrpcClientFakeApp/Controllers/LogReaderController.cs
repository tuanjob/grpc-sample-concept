using Microsoft.AspNetCore.Mvc;

namespace MCOutrightAsGrpcClientFakeApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogReaderController : ControllerBase
    {
        private readonly ILogger<LogReaderController> _logger;
        private readonly string _logFilePath = "logs";

        public LogReaderController(ILogger<LogReaderController> logger)
        {
            _logger = logger;
        }

        [HttpGet("logFiles")]
        public IActionResult LogFiles()
        {
            try
            {
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFilePath);
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

                var logFiles = new List<string>();
                foreach (string file in files)
                {
                    // Extract the subfolder path (relative to the parent directory)
                    string subfolderPath = file.Substring(file.IndexOf("logs") + 5);
                    logFiles.Add(subfolderPath);
                }

                return Ok(logFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving log entries: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult GetLog(string filePath = "2024-05-03\\INFO_Program.log")
        {
            try
            {
                var logEntries = ReadLogEntriesFromFile($"{_logFilePath}\\{filePath}");
                return Ok(logEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving log entries: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private List<string> ReadLogEntriesFromFile(string filePath)
        {
            // Read log entries from the log file
            var logEntries = new List<string>(); 
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);


            if (System.IO.File.Exists(logFilePath))
            {
                var lines = System.IO.File.ReadAllLines(logFilePath);
                logEntries.AddRange(lines);
            }

            return logEntries;
        }
    }
}
