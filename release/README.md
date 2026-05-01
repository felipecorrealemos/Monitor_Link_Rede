# Pasta de Distribuicao (APK)

Esta pasta e destinada a armazenar manualmente o APK final pronto para instalacao no Android.

Recomendacoes:
# Monitor Link Rede

Aplicativo desenvolvido em **C# com .NET MAUI Blazor Hybrid** para **monitoramento de conectividade com a internet**, registrando localmente eventos de **queda** e **retorno da conexão**.

O sistema foi projetado para funcionar inicialmente em **desktop**, com estrutura preparada para testes e execução também em **dispositivos móveis**, permitindo acompanhar a estabilidade da rede de forma simples, visual e prática.

## Funcionalidades

* Monitoramento contínuo da conectividade
* Registro local de eventos de queda e retorno
* Histórico de ocorrências
* Exibição visual de informações em gráfico
* Interface responsiva para desktop e mobile
* Armazenamento local sem uso de SQLite

## Tecnologias utilizadas

* C#
* .NET MAUI
* Blazor Hybrid
* HTML / CSS / Razor Components
* Armazenamento local em arquivo JSON / Preferences

## Objetivo do projeto

Este projeto foi criado para testar e validar o comportamento da conexão de internet em diferentes cenários, como uso em notebook e celular conectados por Wi-Fi, permitindo analisar períodos de instabilidade e retorno do link.

## APK para instalação

A versão compilada para Android pode ser disponibilizada na pasta:

```text
/release
```

Quando disponível, basta baixar o arquivo APK e instalar manualmente no dispositivo Android.

## Estrutura do projeto

```text
/Components
/Pages
/Services
/Models
/wwwroot
/Platforms
/release
README.md
.gitignore
```

## Como executar

1. Abra o projeto no Visual Studio
2. Restaure os pacotes do projeto
3. Execute em Windows ou compile para Android
4. Para distribuição no Android, gere o APK e coloque a versão final na pasta `release`, se desejar disponibilizar diretamente no repositório
