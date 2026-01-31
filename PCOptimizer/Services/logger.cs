using System;
using System.IO;

namespace PCOptimizer.Services
{
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

                string fileName = $"Manutencao_{DateTime.Now:yyyy-MM-dd}.log";
                string filePath = Path.Combine(LogPath, fileName);
                
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";

                // Escreve no arquivo (Append) e também no Console se necessário
                File.AppendAllText(filePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao gravar log: {ex.Message}");
            }
        }
    }
}