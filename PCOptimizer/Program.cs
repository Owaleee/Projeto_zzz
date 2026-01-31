using System;
using System.Security.Principal;
using PCOptimizer.Services;

namespace PCOptimizer
{
    internal class Program
    {
        static void Main()
        {
            Console.Title = "PC Optimizer - Central de Manutenção";

            // Verifica se o processo atual possui privilégios elevados (necessário para DISM/SFC)
            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERRO: Execute como ADMINISTRADOR!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            bool sair = false;
            while (!sair)
            {
                ExibirMenu();
                string opcao = Console.ReadLine();
                Console.Clear();

                switch (opcao)
                {
                    case "1":
                        DismService.Run(); // Repara a imagem de recuperação (origem dos arquivos)
                        Concluir();
                        break;
                    case "2":
                        SfcService.Run(); // Verifica e corrige os arquivos ativos do sistema
                        Concluir();
                        break;
                    case "3":
                        // Fluxo completo: Primeiro conserta a base (DISM), depois o sistema (SFC)
                        Logger.Log("Iniciando Manutenção Completa (Combo)");
                        DismService.Run();
                        Console.WriteLine("\n" + new string('-', 40));
                        SfcService.Run();
                        Concluir();
                        break;
                    case "0":
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
        }

        static void ExibirMenu()
        {
            Console.Clear();
            Console.WriteLine("=========================================");
            Console.WriteLine("      MENU DE MANUTENÇÃO DO WINDOWS     ");
            Console.WriteLine("=========================================");
            Console.WriteLine("1. Reparo Inicial (DISM)");
            Console.WriteLine("2. Reparo Avançado (SFC)");
            Console.WriteLine("3. Manutenção Completa (DISM + SFC)");
            Console.WriteLine("0. Sair");
            Console.WriteLine("=========================================");
            Console.Write("Escolha uma opção: ");
        }

        static void Concluir()
        {
            // Orienta o usuário sobre a necessidade de reboot para persistir reparos em arquivos em uso
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n-----------------------------------------");
            Console.WriteLine("DICA: Recomendamos reiniciar o computador");
            Console.WriteLine("para aplicar todas as correções de log.");
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
            Console.WriteLine("\nPressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        // Método via Windows API para validar se o usuário clicou em "Executar como Administrador"
        static bool IsAdministrator()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}