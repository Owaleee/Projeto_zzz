using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Diagnostics;
using System.Linq;
using PCOptimizer.Services;

namespace PCOptimizer
{
    internal class Program
    {
        static void Main()
        {
            Console.Title = "PC Optimizer - Central de Manutenção";

            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=====================================================");
                Console.WriteLine("ERRO: O programa deve ser executado como ADMINISTRADOR");
                Console.WriteLine("=====================================================");
                Console.ResetColor();
                Console.WriteLine("Por favor, clique com o botão direito e selecione 'Executar como administrador'.");
                Console.ReadKey();
                return;
            }

            bool sair = false;
            while (!sair)
            {
                ExibirMenuPrincipal();
                string opcao = Console.ReadLine();
                Console.Clear();

                switch (opcao)
                {
                    case "1":
                        DismService.Run();
                        Concluir();
                        break;
                    case "2":
                        SfcService.Run();
                        Concluir();
                        break;
                    case "3":
                        Console.WriteLine("Iniciando Manutenção Completa (DISM + SFC)...");
                        DismService.Run();
                        Console.WriteLine("\n" + new string('-', 40));
                        SfcService.Run();
                        Concluir();
                        break;
                    case "4":
                        SubMenuLimpeza(); 
                        break;
                    case "0":
                        sair = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Opção inválida! Tente novamente.");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1500);
                        break;
                }
            }
        }

        static void ExibirMenuPrincipal()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");
            Console.WriteLine("      MENU DE MANUTENÇÃO DO WINDOWS      ");
            Console.WriteLine("=========================================");
            Console.ResetColor();
            Console.WriteLine("1. Reparo Inicial (DISM)");
            Console.WriteLine("2. Reparo Avançado (SFC)");
            Console.WriteLine("3. Manutenção Completa (DISM + SFC)");
            Console.WriteLine("4. Central de Limpeza (Cache, Cookies, DNS)");
            Console.WriteLine("0. Sair");
            Console.WriteLine("=========================================");
            Console.Write("Escolha uma opção: ");
        }

        static void SubMenuLimpeza()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=========================================");
                Console.WriteLine("       OPÇÕES DE LIMPEZA DE CACHE        ");
                Console.WriteLine("=========================================");
                Console.ResetColor();
                Console.WriteLine("1. Limpeza Completa (Recomendado)");
                Console.WriteLine("2. Apenas Navegadores (Cache e Cookies)");
                Console.WriteLine("3. Apenas Sistema (Temps e Prefetch)");
                Console.WriteLine("4. Apenas Cache DNS");
                Console.WriteLine("0. Voltar ao Menu Principal");
                Console.WriteLine("=========================================");
                Console.Write("Escolha o tipo de limpeza: ");

                string subOpcao = Console.ReadLine();
                Console.Clear();

                if (subOpcao == "1" || subOpcao == "2")
                {
                    VerificarNavegadoresAbertos();
                }

                switch (subOpcao)
                {
                    case "1":
                        CacheCleanerService.ExecutarLimpezaCompleta();
                        Aguardar();
                        break;
                    case "2":
                        CacheCleanerService.TestarLimpezaNavegadores();
                        CacheCleanerService.TestarLimpezaCookies();
                        Aguardar();
                        break;
                    case "3":
                        CacheCleanerService.TestarLimpezaSistema();
                        Aguardar();
                        break;
                    case "4":
                        CacheCleanerService.TestarLimpezaDNS();
                        Aguardar();
                        break;
                    case "0":
                        voltar = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        Aguardar();
                        break;
                }
            }
        }

        // --- MÉTODOS AUXILIARES ATUALIZADOS ---

        static void VerificarNavegadoresAbertos()
        {
            // Mapeamento de processo -> Nome real
            var navegadoresMap = new Dictionary<string, string>
            {
                { "chrome", "Google Chrome" },
                { "msedge", "Microsoft Edge" },
                { "firefox", "Mozilla Firefox" },
                { "opera", "Opera Browser" },
                { "brave", "Brave Browser" }
            };

            var processosParaFechar = new List<string>();
            var nomesParaExibir = new List<string>();

            // Verifica quais estão abertos
            foreach (var item in navegadoresMap)
            {
                if (Process.GetProcessesByName(item.Key).Length > 0)
                {
                    processosParaFechar.Add(item.Key);
                    nomesParaExibir.Add(item.Value);
                }
            }

            if (processosParaFechar.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=========================================================");
                Console.WriteLine("⚠️  ATENÇÃO: NAVEGADORES ABERTOS DETECTADOS");
                Console.WriteLine("=========================================================");
                
                // Exibe os nomes convertidos (ex: Microsoft Edge)
                Console.WriteLine($"Navegadores ativos: {string.Join(", ", nomesParaExibir)}");
                
                Console.WriteLine("\nAVISO IMPORTANTE:");
                Console.WriteLine("Para uma limpeza total, os navegadores precisam ser fechados.");
                Console.WriteLine("NÃO NOS RESPONSABILIZAMOS POR PERDA DE DADOS, TRABALHOS");
                Console.WriteLine("NÃO SALVOS OU ABAS FECHADAS DURANTE ESTE PROCESSO.");
                Console.WriteLine("=========================================================");
                Console.ResetColor();

                Console.Write("\nDeseja fechar os navegadores automaticamente agora? (S/N): ");
                string resposta = Console.ReadLine()?.Trim().ToUpper();

                if (resposta == "S")
                {
                    Console.WriteLine("\nEncerrando navegadores...");
                    foreach (var nomeProc in processosParaFechar)
                    {
                        foreach (var p in Process.GetProcessesByName(nomeProc))
                        {
                            try { p.Kill(); } catch { }
                        }
                    }
                    System.Threading.Thread.Sleep(1500);
                    Console.WriteLine("Navegadores encerrados. Prosseguindo...\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nNavegadores mantidos abertos.");
                    Console.WriteLine("A limpeza prosseguirá, mas alguns arquivos podem não ser limpos.");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        static void Aguardar()
        {
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void Concluir()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("DICA: Recomendamos reiniciar o computador");
            Console.WriteLine("para aplicar todas as correções.");
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
            Console.WriteLine("\nPressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        static bool IsAdministrator()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}