using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Versioning;

namespace PCOptimizer.Services
{
    [SupportedOSPlatform("windows")]
    public static class SfcService
    {
        public static void Run()
        {
            if (!OperatingSystem.IsWindows()) return;

            Logger.Log(">>> INICIANDO VERIFICAÇÃO DE ARQUIVOS (SFC /SCANNOW)");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Iniciando Verificação de Integridade do Sistema (SFC)...");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Aguarde... Corrigindo encoding e filtrando resultados.");
            Console.WriteLine("-----------------------------------------------------------\n");
            Console.ResetColor();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c chcp 65001 > nul && sfc /scannow",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            try
            {
                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    line = line.Replace("\0", "");

                    ParseOutput(line);
                }

                process.WaitForExit();
                Logger.Log("SFC: Finalizado. Código: " + process.ExitCode);
            }
            catch (Exception ex)
            {
                Logger.Log("ERRO SFC: " + ex.Message);
            }
        }

        private static void ParseOutput(string line)
        {
            // 1. Detecta progresso
            // Usamos @ para a string do Regex para evitar problemas de escape
            var progressMatch = Regex.Match(line, @"(\d+)%");
            if (progressMatch.Success)
            {
                string p = progressMatch.Groups[1].Value;
                // Mudamos a forma de escrever para evitar que o compilador ache que [] é um atributo
                Console.Write("\rProgresso SFC: [" + p + "%] ");
                return; 
            }

            // 2. Filtra apenas mensagens importantes
            if (line.Contains("Proteção") || line.Contains("Resource Protection") || 
                line.Contains("corrompidos") || line.Contains("reparou"))
            {
                string msg = line.Trim();
                // Usando concatenação simples para garantir que o contexto seja entendido como string
                Console.WriteLine("\n[SFC]: " + msg);
                Logger.Log("SFC Detalhe: " + msg);
            }
        }
    }
}