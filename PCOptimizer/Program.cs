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
            // Define o título do console para melhor UX
            Console.Title = "PC Optimizer - Ferramentas de Sistema";

            // Chama o serviço SFC. Toda a lógica de verificação está isolada aqui.
            SfcService.Run();

            // Impede que o console feche imediatamente após o término
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}