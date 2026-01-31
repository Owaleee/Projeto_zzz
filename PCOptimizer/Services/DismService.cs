using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Versioning;

namespace PCOptimizer.Services
{
	[SupportedOSPlatform("windows")]
	public static class DismService
	{
		public static void Run()
		{
			if (!OperatingSystem.IsWindows()) return;

			Logger.Log(">>> INICIANDO REPARO DE IMAGEM (DISM /RestoreHealth)");

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Iniciando Reparo da Imagem do Sistema (DISM)...");
			Console.WriteLine("-----------------------------------------------------------");
			Console.WriteLine("AVISO: Este processo � demorado (5 a 20 minutos).");
			Console.WriteLine("� normal o progresso parecer 'parado' em certas etapas.");
			Console.WriteLine("-----------------------------------------------------------\n");
			Console.ResetColor();

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "cmd.exe",
					Arguments = "/c DISM /Online /Cleanup-Image /RestoreHealth",
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

					ParseOutput(line);
				}

				process.WaitForExit();
				Logger.Log($"DISM: Processo finalizado. C�digo de sa�da: {process.ExitCode}");
			}
			catch (Exception ex)
			{
				string erroMsg = $"ERRO CR�TICO NO DISM: {ex.Message}";
				Console.WriteLine($"\n{erroMsg}");
				Logger.Log(erroMsg);
			}
		}

		private static void ParseOutput(string line)
		{
			// 1. Filtra progresso num�rico (ex: 10.0%) para exibi��o din�mica no console
			var progressMatch = Regex.Match(line, @"(\d+\.\d+)%");
			if (progressMatch.Success)
			{
				Console.Write($"\rProgresso do Reparo: [{progressMatch.Groups[1].Value}%] Aguarde...");
				return;
			}

			// 2. Filtra barras de progresso visuais (ex: [==== 100.0% ====])
			if (line.Contains("[") && line.Contains("=") && line.Contains("]"))
			{
				return;
			}

			// 3. Captura informa��es textuais relevantes
			string trimmedLine = line.Trim();
			if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.Length > 5)
			{
				// Se for a mensagem de conclus�o com �xito, destacamos em verde
				if (trimmedLine.Contains("successfully") || trimmedLine.Contains("�xito"))
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"\n\n[RESULTADO]: {trimmedLine}");
					Console.ResetColor();
				}
				else
				{
					// Outras informa��es informativas do DISM
					Console.WriteLine($"\n[DISM INFO]: {trimmedLine}");
				}

				// Grava absolutamente todo o texto informativo no log
				Logger.Log($"DISM Detalhe: {trimmedLine}");
			}
		}
	}
}