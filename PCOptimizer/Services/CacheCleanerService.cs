using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32;

namespace PCOptimizer.Services
{
	public static class CacheCleanerService
	{
		// Método principal para limpar cache - DEVE SER STATIC
		public static void ExecutarLimpezaCompleta()
		{
			if (!IsRunningAsAdministrator())
			{
				Console.WriteLine("ERRO: Execute este programa como Administrador!");
				Console.WriteLine("Clique com botão direito -> Executar como Administrador");
				return;
			}

			Console.WriteLine("Iniciando limpeza de cache e cookies...");

			try
			{
				// para testar cada função só comentar cada função
				LimparCacheNavegadores();
				// LimparCacheSistema();
				// LimparCacheTemporario();
				LimparCookiesNavegadores();
				LimparCacheDNS();

				Console.WriteLine("Limpeza concluída com sucesso!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro durante a limpeza: {ex.Message}");
			}
		}

		// Verifica se está executando como administrador - DEVE SER STATIC
		private static bool IsRunningAsAdministrator()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Limpa cache dos navegadores principais - DEVE SER STATIC
		private static void LimparCacheNavegadores()
		{
			Console.WriteLine("\n[1/5] Limpando cache de navegadores...");

			var navegadores = new Dictionary<string, string[]>
			{
                // Chrome
                {
					"Chrome", new[]
					{
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cache",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cache2",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Code Cache",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\GPUCache"
					}
				},
                /*// Edge
                {
                    "Edge", new[]
                    {
                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cache",
                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cache2",
                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Code Cache",
                        $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\GPUCache"
                    }
                },*/
                // Firefox
                {
					"Firefox", new[]
					{
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Mozilla\Firefox\Profiles",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Mozilla\Firefox\Profiles"
					}
				},
                // Opera
                {
					"Opera", new[]
					{
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Opera Software\Opera Stable\Cache",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Opera Software\Opera Stable\Cache"
					}
				}
			};

			foreach (var navegador in navegadores)
			{
				Console.WriteLine($"  Processando {navegador.Key}...");

				foreach (var caminho in navegador.Value)
				{
					if (Directory.Exists(caminho))
					{
						try
						{
							LimparDiretorio(caminho);
							Console.WriteLine($"    ✓ Cache limpo: {caminho}");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"    ✗ Erro em {caminho}: {ex.Message}");
						}
					}
				}
			}
		}

		// Limpa cookies dos navegadores - DEVE SER STATIC
		private static void LimparCookiesNavegadores()
		{
			Console.WriteLine("\n[2/5] Limpando cookies...");

			var cookiesPaths = new[]
			{
                // Chrome Cookies
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cookies",
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cookies-journal",
                
                /*// Edge Cookies
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cookies",
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cookies-journal",
                */
                // Firefox Cookies (arquivo SQLite)
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Mozilla\Firefox\Profiles"
			};

			foreach (var caminho in cookiesPaths)
			{
				try
				{
					if (File.Exists(caminho))
					{
						File.Delete(caminho);
						Console.WriteLine($"    ✓ Cookies removidos: {Path.GetFileName(caminho)}");
					}
					else if (Directory.Exists(caminho) && caminho.Contains("Firefox"))
					{
						// Para Firefox, procura arquivos cookies.sqlite
						var cookieFiles = Directory.GetFiles(caminho, "cookies.sqlite", SearchOption.AllDirectories);
						foreach (var file in cookieFiles)
						{
							File.Delete(file);
							File.Delete(file + "-journal"); // Remove também o journal
							Console.WriteLine($"    ✓ Cookies Firefox removidos");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"    ✗ Erro ao remover cookies: {ex.Message}");
				}
			}
		}

		// Limpa cache do sistema - DEVE SER STATIC
		private static void LimparCacheSistema()
		{
			Console.WriteLine("\n[3/5] Limpando cache do sistema...");

			var systemPaths = new[]
			{
                // Temp do sistema
                Path.GetTempPath(),
                
                // Temp do usuário
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Temp",
                
                // Windows Temp
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\Temp",
                
                // Prefetch (Windows)
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\Prefetch",
                
                // Thumbnails
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Windows\Explorer",
                
                // Recent files
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.Recent)}"
			};

			foreach (var caminho in systemPaths)
			{
				if (Directory.Exists(caminho))
				{
					try
					{
						LimparDiretorio(caminho);
						Console.WriteLine($"    ✓ Sistema limpo: {Path.GetFileName(caminho)}");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"    ✗ Erro em {caminho}: {ex.Message}");
					}
				}
			}
		}

		// Limpa diretório temporário - DEVE SER STATIC
		private static void LimparCacheTemporario()
		{
			Console.WriteLine("\n[4/5] Limpando arquivos temporários...");

			try
			{
				// Limpa arquivos .tmp
				var tempFiles = Directory.GetFiles(Path.GetTempPath(), "*.tmp", SearchOption.AllDirectories);
				foreach (var file in tempFiles.Take(1000)) // Limita para não travar
				{
					try
					{
						File.Delete(file);
					}
					catch { }
				}
				Console.WriteLine($"    ✓ {tempFiles.Length} arquivos .tmp removidos");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"    ✗ Erro ao limpar temporários: {ex.Message}");
			}
		}

		// Limpa cache DNS - DEVE SER STATIC
		private static void LimparCacheDNS()
		{
			Console.WriteLine("\n[5/5] Limpando cache DNS...");

			try
			{
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
				{
					WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
					FileName = "cmd.exe",
					Arguments = "/C ipconfig /flushdns",
					Verb = "runas" // Executa como administrador
				};
				process.StartInfo = startInfo;
				process.Start();
				process.WaitForExit();

				Console.WriteLine("    ✓ Cache DNS limpo");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"    ✗ Erro ao limpar DNS: {ex.Message}");
			}
		}

		// Método auxiliar para limpar diretório - DEVE SER STATIC
		private static void LimparDiretorio(string caminho)
		{
			if (!Directory.Exists(caminho))
				return;

			var dirInfo = new DirectoryInfo(caminho);

			// Limpa arquivos
			foreach (var file in dirInfo.GetFiles())
			{
				try
				{
					file.Delete();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"      Não foi possível excluir {file.Name}: {ex.Message}");
				}
			}

			// Limpa subdiretórios
			foreach (var dir in dirInfo.GetDirectories())
			{
				try
				{
					dir.Delete(true);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"      Não foi possível excluir {dir.Name}: {ex.Message}");
				}
			}
		}

		// Método para limpar cache de um aplicativo específico - DEVE SER STATIC
		public static void LimparCacheAplicativo(string nomeApp)
		{
			Console.WriteLine($"\nLimpando cache do {nomeApp}...");

			var appPaths = new[]
			{
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nomeApp}",
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{nomeApp}",
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\{nomeApp}"
			};

			foreach (var caminho in appPaths)
			{
				if (Directory.Exists(caminho))
				{
					try
					{
						LimparDiretorio(caminho);
						Console.WriteLine($"    ✓ {nomeApp} limpo: {caminho}");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"    ✗ Erro em {nomeApp}: {ex.Message}");
					}
				}
			}
		}
	}
}