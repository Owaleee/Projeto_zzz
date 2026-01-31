using System;
using PCOptimizer.Services;

namespace PCOptimizer
{
    internal class Program
    {
        /// <summary>
        /// Método principal que inicia a aplicação.
        /// </summary>
        static void Main()
        {

            /*
            // Define o título do console para melhor UX
            Console.Title = "PC Optimizer - Ferramentas de Sistema";

            // Chama o serviço SFC. Toda a lógica de verificação está isolada aqui.
            SfcService.Run();

            // Impede que o console feche imediatamente após o término
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();*/

            // TESTE 1: Mensagem inicial para confirmar que este código roda
            Console.Title = "PC OPTIMIZER - TESTE CACHE CLEANER";
            Console.WriteLine("=== INICIANDO APENAS CACHE CLEANER ===\n");

            // TESTE 2: Verificar se chega aqui
            Console.WriteLine("1. Executando CacheCleanerService...");

            // Executa a limpeza de cache
            CacheCleanerService.ExecutarLimpezaCompleta();

            // TESTE 3: Mensagem final
            Console.WriteLine("\n=== PROGRAMA FINALIZADO ===");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();



        }
    }
}