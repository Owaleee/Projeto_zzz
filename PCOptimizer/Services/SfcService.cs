using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Runtime.Versioning;

namespace PCOptimizer.Services
{
    [SupportedOSPlatform("windows")]
    public static class SfcService
    {
        /// <summary>
        /// Executa a ferramenta System File Checker (SFC) para reparar arquivos de sistema.
        /// </summary>
        public static void Run()
        {
            // Valida se o SO é Windows antes de prosseguir
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Este recurso é suportado apenas no Windows.");
                return;
            }

            // O SFC exige privilégios elevados para modificar arquivos de sistema
            if (!IsAdministrator())
            {
                Console.WriteLine("ERRO: Este recurso requer permissões de ADMINISTRADOR.");
                Console.WriteLine("Por favor, execute o terminal ou o app como administrador.");
                return;
            }

            Console.WriteLine("Iniciando verificação do sistema (SFC)...\n");

            // Configuração do processo CMD que chamará o SFC
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c sfc /scannow", // /c executa e termina
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    // Para Windows PT-BR, o Encoding.Default ou UTF8 costumam resolver caracteres especiais
                    StandardOutputEncoding = Encoding.UTF8 
                }
            };

            try 
            {
                process.Start();

                // Lê a saída em tempo real para capturar o progresso (%)
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    ParseOutput(line);
                }

                process.WaitForExit();
                Console.WriteLine("\nVerificação finalizada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar o processo: {ex.Message}");
            }
        }

        /// <summary>
        /// Filtra a saída do CMD para exibir apenas informações relevantes ao usuário.
        /// </summary>
        private static void ParseOutput(string line)
        {
            // Busca por padrões de porcentagem (ex: 10%, 25%)
            var progressMatch = Regex.Match(line, @"(\d+)%");
            if (progressMatch.Success)
            {
                Console.Write($"\rProgresso: {progressMatch.Groups[1].Value}%"); // \r limpa a linha atual
                return;
            }

            // Exibe mensagens de status final da Proteção de Recursos
            if (line.Contains("Proteção de Recursos") || line.Contains("Windows Resource Protection"))
            {
                Console.WriteLine($"\n{line}");
            }

            // Informa onde o log detalhado foi salvo, se aplicável
            if (line.Contains("CBS.log"))
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Verifica se o processo atual possui privilégios de administrador.
        /// </summary>
        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}