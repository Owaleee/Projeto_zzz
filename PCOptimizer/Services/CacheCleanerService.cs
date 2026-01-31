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
		// Método principal para limpar cache
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
				// Para testar cada função só comentar cada função
				bool navegadoresOk = LimparCacheNavegadores();
				bool sistemaOk = LimparCacheSistema();
				bool tempOk = LimparCacheTemporario();
				bool cookiesOk = LimparCookiesNavegadores();
				bool dnsOk = LimparCacheDNS();

				// Resumo da execução
				Console.WriteLine("\n" + new string('=', 50));
				Console.WriteLine("RESUMO DA LIMPEZA:");
				Console.WriteLine(new string('=', 50));
				Console.WriteLine($"✓ Cache de navegadores: {(navegadoresOk ? "SUCESSO" : "FALHA PARCIAL")}");
				Console.WriteLine($"✓ Cache do sistema: {(sistemaOk ? "SUCESSO" : "FALHA PARCIAL")}");
				Console.WriteLine($"✓ Arquivos temporários: {(tempOk ? "SUCESSO" : "FALHA PARCIAL")}");
				Console.WriteLine($"✓ Cookies: {(cookiesOk ? "SUCESSO" : "FALHA PARCIAL")}");
				Console.WriteLine($"✓ Cache DNS: {(dnsOk ? "SUCESSO" : "FALHA")}");
				Console.WriteLine(new string('=', 50));

				Console.WriteLine("\n✅ Limpeza concluída!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n❌ Erro durante a limpeza: {ex.Message}");
			}
		}

		// Verifica se está executando como administrador
		private static bool IsRunningAsAdministrator()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Limpa cache dos navegadores principais - Retorna true se tudo OK, false se algum erro
		private static bool LimparCacheNavegadores()
		{
			Console.WriteLine("\n[1/5] Limpando cache de navegadores...");
			bool sucessoTotal = true;
			int navegadoresProcessados = 0;
			int pastasLimpas = 0;

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
				// Edge
				{
					"Edge", new[]
					{
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cache",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cache2",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Code Cache",
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\GPUCache"
					}
				},
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
				navegadoresProcessados++;

				foreach (var caminho in navegador.Value)
				{
					if (Directory.Exists(caminho))
					{
						try
						{
							bool sucesso = LimparDiretorio(caminho);
							if (sucesso)
							{
								Console.WriteLine($"    ✓ Cache limpo: {Path.GetFileName(caminho)}");
								pastasLimpas++;
							}
							else
							{
								Console.WriteLine($"    ⚠ Cache parcialmente limpo: {Path.GetFileName(caminho)}");
								sucessoTotal = false;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine($"    ✗ Erro em {Path.GetFileName(caminho)}: {ex.Message}");
							sucessoTotal = false;
						}
					}
				}
			}

			Console.WriteLine($"  → Resultado: {navegadoresProcessados} navegadores processados, {pastasLimpas} pastas limpas");
			return sucessoTotal;
		}

		// Limpa cookies dos navegadores - Retorna true se tudo OK, false se algum erro
		private static bool LimparCookiesNavegadores()
		{
			Console.WriteLine("\n[2/5] Limpando cookies...");
			bool sucessoTotal = true;
			int arquivosRemovidos = 0;

			var cookiesPaths = new[]
			{
				// Chrome Cookies
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cookies",
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Cookies-journal",
				
				// Edge Cookies
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cookies",
				$@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Microsoft\Edge\User Data\Default\Cookies-journal",
				
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
						arquivosRemovidos++;
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
							arquivosRemovidos += 2;
						}
						if (cookieFiles.Length > 0)
							Console.WriteLine($"    ✓ Cookies Firefox removidos: {cookieFiles.Length} arquivos");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"    ✗ Erro ao remover cookies: {ex.Message}");
					sucessoTotal = false;
				}
			}

			Console.WriteLine($"  → Resultado: {arquivosRemovidos} arquivos de cookies removidos");
			return sucessoTotal;
		}

		// Limpa cache do sistema - Retorna true se tudo OK, false se algum erro
		private static bool LimparCacheSistema()
		{
			Console.WriteLine("\n[3/5] Limpando cache do sistema...");
			bool sucessoTotal = true;
			int pastasLimpas = 0;

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
						bool sucesso = LimparDiretorio(caminho);
						if (sucesso)
						{
							Console.WriteLine($"    ✓ Sistema limpo: {Path.GetFileName(caminho)}");
							pastasLimpas++;
						}
						else
						{
							Console.WriteLine($"    ⚠ Sistema parcialmente limpo: {Path.GetFileName(caminho)}");
							sucessoTotal = false;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"    ✗ Erro em {Path.GetFileName(caminho)}: {ex.Message}");
						sucessoTotal = false;
					}
				}
			}

			Console.WriteLine($"  → Resultado: {pastasLimpas} pastas do sistema limpas");
			return sucessoTotal;
		}

		// Limpa diretório temporário - Retorna true se tudo OK, false se algum erro
		private static bool LimparCacheTemporario()
		{
			Console.WriteLine("\n[4/5] Limpando arquivos temporários...");
			int arquivosRemovidos = 0;
			bool sucesso = true;

			try
			{
				// Limpa arquivos .tmp
				var tempFiles = Directory.GetFiles(Path.GetTempPath(), "*.tmp", SearchOption.AllDirectories);
				foreach (var file in tempFiles.Take(1000)) // Limita para não travar
				{
					try
					{
						File.Delete(file);
						arquivosRemovidos++;
					}
					catch
					{
						sucesso = false;
					}
				}
				Console.WriteLine($"    ✓ {arquivosRemovidos} arquivos .tmp removidos");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"    ✗ Erro ao limpar temporários: {ex.Message}");
				sucesso = false;
			}

			Console.WriteLine($"  → Resultado: {arquivosRemovidos} arquivos temporários removidos");
			return sucesso;
		}

		// Limpa cache DNS - Retorna true se tudo OK, false se falhar
		private static bool LimparCacheDNS()
		{
			Console.WriteLine("\n[5/5] Limpando cache DNS...");
			bool sucesso = false;

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

				// Verifica se o processo terminou com sucesso (código 0)
				if (process.ExitCode == 0)
				{
					Console.WriteLine("    ✓ Cache DNS limpo");
					sucesso = true;
				}
				else
				{
					Console.WriteLine($"    ✗ Erro ao limpar DNS (código: {process.ExitCode})");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"    ✗ Erro ao limpar DNS: {ex.Message}");
			}

			Console.WriteLine($"  → Resultado: {(sucesso ? "DNS limpo com sucesso" : "Falha ao limpar DNS")}");
			return sucesso;
		}

		// Método auxiliar para limpar diretório - Retorna true se tudo OK, false se algum erro
		private static bool LimparDiretorio(string caminho)
		{
			if (!Directory.Exists(caminho))
				return true; // Se não existe, consideramos "sucesso"

			var dirInfo = new DirectoryInfo(caminho);
			bool sucessoTotal = true;
			int arquivosRemovidos = 0;
			int pastasRemovidas = 0;

			// Limpa arquivos
			foreach (var file in dirInfo.GetFiles())
			{
				try
				{
					file.Delete();
					arquivosRemovidos++;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"      Não foi possível excluir {file.Name}: {ex.Message}");
					sucessoTotal = false;
				}
			}

			// Limpa subdiretórios
			foreach (var dir in dirInfo.GetDirectories())
			{
				try
				{
					dir.Delete(true);
					pastasRemovidas++;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"      Não foi possível excluir {dir.Name}: {ex.Message}");
					sucessoTotal = false;
				}
			}

			// Log detalhado (opcional)
			if (arquivosRemovidos > 0 || pastasRemovidas > 0)
			{
				Console.WriteLine($"      Removidos: {arquivosRemovidos} arquivos, {pastasRemovidas} pastas");
			}

			return sucessoTotal;
		}

		// Método para limpar cache de um aplicativo específico - Retorna true se tudo OK
		public static bool LimparCacheAplicativo(string nomeApp)
		{
			Console.WriteLine($"\nLimpando cache do {nomeApp}...");
			bool sucessoTotal = true;
			int pastasLimpas = 0;

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
						bool sucesso = LimparDiretorio(caminho);
						if (sucesso)
						{
							Console.WriteLine($"    ✓ {nomeApp} limpo: {Path.GetFileName(caminho)}");
							pastasLimpas++;
						}
						else
						{
							Console.WriteLine($"    ⚠ {nomeApp} parcialmente limpo: {Path.GetFileName(caminho)}");
							sucessoTotal = false;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"    ✗ Erro em {nomeApp}: {ex.Message}");
						sucessoTotal = false;
					}
				}
			}

			Console.WriteLine($"  → Resultado: {pastasLimpas} pastas do {nomeApp} limpas");
			return sucessoTotal;
		}

		// Métodos públicos para testar funções individuais com retorno
		public static bool TestarLimpezaNavegadores()
		{
			Console.WriteLine("\n=== TESTE: LIMPEZA DE NAVEGADORES ===");
			return LimparCacheNavegadores();
		}

		public static bool TestarLimpezaCookies()
		{
			Console.WriteLine("\n=== TESTE: LIMPEZA DE COOKIES ===");
			return LimparCookiesNavegadores();
		}

		public static bool TestarLimpezaSistema()
		{
			Console.WriteLine("\n=== TESTE: LIMPEZA DO SISTEMA ===");
			return LimparCacheSistema();
		}

		public static bool TestarLimpezaDNS()
		{
			Console.WriteLine("\n=== TESTE: LIMPEZA DNS ===");
			return LimparCacheDNS();
		}
	}
}