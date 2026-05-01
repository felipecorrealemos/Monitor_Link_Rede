# Monitor_Link_Rede

## Nome do projeto
Monitor_Link_Rede

## Descricao do sistema
Aplicativo de monitoramento de conectividade de internet desenvolvido em .NET MAUI Blazor Hybrid.
O sistema verifica o estado da conexao, registra eventos de queda e retorno localmente e apresenta os dados em historico e grafico para facilitar analise de instabilidade.

## Funcionalidades principais
- Monitoramento de conectividade em tempo real.
- Registro local de eventos de `QUEDA` e `RETORNO`.
- Historico de ocorrencias com filtros por periodo (Hoje, 7 dias, 30 dias e Tudo).
- Grafico de quedas para analise visual.
- Execucao multiplataforma: desktop e Android.

## Tecnologias utilizadas
- .NET MAUI
- Blazor Hybrid
- C#
- HTML/CSS
- JavaScript (Chart.js para graficos)

## Como executar o projeto
1. Abra a solucao no Visual Studio.
2. Restaure os pacotes NuGet (se necessario).
3. Selecione o alvo desejado (Windows para desktop ou Android).
4. Execute o projeto.

## Como instalar o APK no Android
1. Gere o APK em modo Release no Visual Studio.
2. Copie manualmente o arquivo APK final para a pasta [`release/`](./release).
3. Transfira o APK para o celular Android.
4. No aparelho, habilite instalacao de apps de fontes confiaveis (quando solicitado).
5. Instale o APK.

## Estrutura de pastas
```text
/Monitor_Link_Rede
  /Monitor_Link_Rede
    /Components
    /Helpers
    /Models
    /Services
    /ViewModels
    /wwwroot
    /Platforms
  /release
  README.md
  .gitignore
```

## Observacao sobre a pasta `release`
A pasta `release` existe para apoiar a distribuicao manual do APK final de instalacao.
Ela nao substitui um fluxo de publicacao formal, mas facilita disponibilizar uma versao instalavel diretamente no repositorio.

## Observacao sobre `bin` e `obj`
As pastas `bin` e `obj` contem artefatos temporarios/intermediarios de compilacao.
Esses arquivos nao devem ser usados para distribuicao e nao devem ser enviados ao GitHub.

## Codigo-fonte x APK de instalacao
- Codigo-fonte: arquivos de desenvolvimento, mantidos normalmente no repositorio.
- APK de instalacao: arquivo final para usuario, opcionalmente colocado em [`release/`](./release).

## Distribuicao futura (recomendado)
Quando o projeto evoluir, o ideal e publicar versoes instalaveis na area de **GitHub Releases**.
A pasta `release/` pode continuar como apoio para distribuicao rapida.
