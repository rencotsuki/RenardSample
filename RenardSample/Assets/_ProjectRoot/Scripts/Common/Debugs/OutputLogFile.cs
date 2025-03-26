using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace SignageHADO
{
    public class OutputLogFile
    {
        public bool Recording { get; private set; } = false;

        private const string fileExtension = "txt";

        private string _outputPath = string.Empty;
        private StringBuilder _logData = new StringBuilder();

        public OutputLogFile(string outputPath)
        {
            _outputPath = outputPath;
        }

        ~OutputLogFile()
        {
            Stop();
        }

        private string GetFileName()
            => $"{Application.productName}_log_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}";

        public void Stop()
        {
            if (!Recording)
                return;

            Recording = false;

            Debug.Log($"{typeof(OutputLogFile).Name}::Stop");

            if (_logData.Length > 0)
                OutputFile(GetFileName());
        }

        public void Rec()
        {
            if (Recording)
                return;

            Recording = true;
            _logData.Length = 0;

            Debug.Log($"{typeof(OutputLogFile).Name}::Rec");
        }

        public void UpdateLog(string log)
        {
            if (!Recording)
                return;

            _logData?.AppendLine(log);
        }

        private void OutputFile(string fileName)
        {
            try
            {
                if (!Directory.Exists(_outputPath))
                    Directory.CreateDirectory(_outputPath);

                // UTF8のBomに注意!!
                File.WriteAllText($"{_outputPath}/{fileName}.{fileExtension}", _logData.ToString(), new UTF8Encoding(false));

                Debug.Log($"{typeof(OutputLogFile).Name}::OutputFile - path={_outputPath}/{fileName}.{fileExtension}");
            }
            catch (Exception ex)
            {
                Debug.Log($"{typeof(OutputLogFile).Name}::OutputFile - {ex.Message}");
            }
        }
    }
}
