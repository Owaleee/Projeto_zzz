# 🖥️ PC Optimizer (nome provisório)

> Aplicação de otimização de computadores desenvolvida em **C# com .NET**, focada em manutenção básica do sistema, desempenho e organização de arquivos.

⚠️ **Nome provisório:** este projeto ainda não possui um nome definitivo.

---

## 📌 Sobre o Projeto

Este projeto tem como objetivo o desenvolvimento de uma aplicação de **otimização de PC**, inspirada em ferramentas utilitárias populares, mas construída do zero com foco em **aprendizado**, **boas práticas** e **arquitetura limpa**.

A aplicação oferece recursos básicos e avançados de otimização, como:
- Limpeza de arquivos temporários
- Execução de comandos de verificação do Windows
- Monitoramento de desempenho
- Organização de arquivos do usuário

O projeto está sendo desenvolvido como parte de um processo de aprendizado em **C#**, **.NET** e **.NET MAUI**, partindo inicialmente de aplicações de console e evoluindo para interface gráfica.

---

## 🎯 Objetivos

- Aprender C# e o ecossistema .NET na prática  
- Aplicar conceitos de:
  - Programação orientada a objetos
  - Arquitetura em camadas
  - MVVM (no MAUI)
- Criar uma aplicação funcional e documentada

---

## ⚙️ Funcionalidades Planejadas

### 🔹 Otimização do Sistema
- Limpeza de arquivos temporários
- Limpeza de cache e cookies
- Limpeza da lixeira
- Execução de:
  - `sfc /scannow`
  - `DISM /Online /Cleanup-Image /RestoreHealth`

### 🔹 Desempenho
- Monitoramento de CPU, RAM e Disco
- Identificação de processos pesados
- Otimização básica de memória

### 🔹 Organização de Arquivos
- Organização automática por tipo de arquivo
- Organização manual com regras personalizadas
- Detecção e remoção de arquivos duplicados

### 🔹 Relatórios
- Registro de ações realizadas
- Histórico de otimizações
- Logs de execução

---

## 🧱 Arquitetura do Projeto

O projeto segue uma arquitetura baseada em **camadas**, utilizando o padrão **MVVM** para a interface gráfica:

```text
PCOptimizer/
│
├── Views/            # Telas (MAUI)
├── ViewModels/       # Lógica da UI
├── Services/         # Regras de negócio
├── Models/           # Modelos de domínio
├── Infrastructure/   # Acesso ao sistema operacional
│
└── Program.cs / App.xaml

## 🛠️ Tecnologias Utilizadas

- **C#**
- **.NET**
- **.NET MAUI** (interface gráfica)
- **Windows API / CMD / PowerShell**
- **MVVM**

---

## 🚧 Status do Projeto

🚧 **Em desenvolvimento / fase inicial**

Atualmente:
- [x] Planejamento
- [x] Diagramas UML e Fluxogramas
- [ ] Aplicação de console (primeiro MVP)
- [ ] Interface gráfica com MAUI
- [ ] Funcionalidades avançadas

---

## 📚 Aprendizados Envolvidos

Este projeto envolve o estudo e aplicação de:
- C# básico e intermediário
- Manipulação de arquivos e pastas
- Execução de comandos do sistema
- Programação assíncrona (`async/await`)
- Arquitetura de software
- UI com XAML

---

## 🧠 Próximos Passos

- Criar o primeiro MVP em **Console Application**
- Implementar limpeza da pasta TEMP
- Criar sistema de logs
- Migrar funcionalidades para **.NET MAUI**
- Definir nome oficial do projeto

---

## 🤝 Contribuições

Este é um projeto de estudo, mas sugestões, ideias e melhorias são sempre bem-vindas.

---

## 📄 Licença

Este projeto está sob a licença **MIT**.  
Sinta-se livre para estudar e modificar.

