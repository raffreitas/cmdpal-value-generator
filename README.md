# Command Palette - Value Generator

Extensão C# do PowerToys Command Palette baseada no plugin Value Generator do
PowerToys Run. A origem e a licença dos fontes portados estão em
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).

## Compilar e testar

Requisitos: Windows, .NET SDK 10, Visual Studio com os componentes de `.vsconfig`,
Windows SDK 10.0.26100.0 e ferramentas de empacotamento MSIX.

```powershell
dotnet restore ValueGenerator.sln -p:Platform=x64
dotnet build ValueGenerator.sln -p:Platform=x64 --no-restore
dotnet test ValueGenerator.UnitTests/ValueGenerator.UnitTests.csproj -p:Platform=x64 --no-restore
```

Os testes compilam diretamente os fontes de cálculo da extensão, sem iniciar o
servidor COM e sem acessar o clipboard real. Incluem os testes originais portados,
vetores conhecidos de hashes e GUID, round-trips, ULID e leitura simulada do clipboard.

O projeto mantém os targets e as versões de pacotes do template. Em Release,
o trimming das dependências `Microsoft.Windows.SDK.NET` e `WinRT.Runtime` pode
emitir `IL2104`, agrupando avisos `IL2081` do código de fallback dessas bibliotecas.
Esses avisos não foram ocultados. O build Debug não apresentou warnings.
O projeto apenas exclui o WinMD de interfaces do CmdPal da geração automática
de registros de ativação, evitando `APPX1707`; os manifestos fonte permanecem iguais.

## Deploy pelo Visual Studio

1. Abra `ValueGenerator.sln` e defina `ValueGenerator` como projeto de inicialização.
2. Selecione `Debug`, plataforma `x64` (ou `ARM64` em um computador ARM64) e o perfil
   **ValueGenerator (Package)**.
3. Ative o Modo de Desenvolvedor do Windows, se o Visual Studio solicitar.
4. Use **Build > Deploy Solution**, ou clique com o botão direito no projeto
   `ValueGenerator` e selecione **Deploy**. Somente Build não registra o MSIX.
5. No Command Palette, procure `Reload` e execute **Reload Command Palette extensions**.
6. Abra **Value Generator**. Para depurar o processo da extensão, use F5 com o
   perfil de pacote selecionado.

Se aparecer `APPX3217` por falta de `UAP.props`, instale ou repare o Windows SDK
10.0.26100 pelo Visual Studio Installer; não é necessário alterar o target framework.

## Alias `#`

1. Abra as configurações do **Command Palette** e vá a **Extensions**.
2. Localize **Value Generator** e expanda o comando **Value Generator**.
3. Defina **Alias** como `#` e habilite **Direct** / ativação direta do alias.
4. Volte à busca principal e digite `#` para entrar na página; em seguida digite
   `guid7`, `sha256 abc` ou outro comando. O `#` é tratado pelo CmdPal e não faz
   parte da sintaxe do parser da extensão.

O fallback também reconhece os comandos diretamente na busca principal, sem `#`,
quando habilitado nas configurações do CmdPal.

## Comandos

| Comando | Exemplo ou comportamento |
| --- | --- |
| `guid`, `uuid` | GUID v4 |
| `guid1`, `guid4`, `guid7` | GUID v1, v4 e v7 nativo do .NET |
| `guid3`, `guid5` | `guid5 ns:DNS abc`, ou namespace GUID explícito e nome |
| `md5`, `sha1`, `sha256`, `sha384`, `sha512` | `sha256 abc`; sem conteúdo, usa o texto do clipboard |
| `base64`, `base64d` | `base64 Olá mundo`, `base64d YWJj` |
| `url`, `urld` | `url a b+c`, `urld a+b%2bc` |
| `esc:data`, `uesc:data` | Escape/unescape de dados URI |
| `esc:hex`, `uesc:hex` | `esc:hex z`, `uesc:hex %7A` |
| `ulid` | ULID com timestamp e aleatoriedade criptográfica, em Crockford Base32 |

As versões de GUID também aceitam os aliases originais `guidv1`, `uuid1`, `uuidv1`
e os equivalentes para v3/v4/v5/v7. Namespaces predefinidos: `ns:DNS`, `ns:URL`,
`ns:OID` e `ns:X500`. GUID v3/v5 mantém a codificação ASCII e a sintaxe de nome
sem espaços do plugin original.

A busca vazia mostra sugestões; Enter em uma sugestão preenche o comando.
Enter no resultado copia o valor. O menu de ações do resultado de GUID também
oferece cópia sem hífens, em maiúsculas ou com chaves, usando o mesmo GUID exibido.

O parser ignora maiúsculas/minúsculas dos comandos e remove espaços nas bordas
do conteúdo digitado, preservando os espaços internos. O texto lido do clipboard
é usado integralmente. Clipboard vazio ou sem texto e entradas inválidas mostram
um erro sem ação de cópia. ULIDs são ordenáveis pelo timestamp; a ordem dentro do
mesmo milissegundo não é garantida.

## Verificação no CmdPal após o deploy

- Abrir a página vazia, selecionar uma sugestão e conferir o preenchimento da busca.
- Copiar `sha256 abc` e conferir o hash; copiar um GUID nos três formatos adicionais.
- Executar `guid7`, `guid5 ns:DNS abc`, `ulid`, Base64 e URL pela página e pelo fallback.
- Conferir que `guidance` e `sha256sum abc` não exibem o fallback do Value Generator.
- Conferir erros em `base64d %%%`, `guid2` e `esc:hex ab` sem fechar a extensão.
- Copiar texto e executar somente `sha256`; repetir com clipboard sem texto.
- Conferir a ativação direta por `#`.

Referências: [configurações do CmdPal](https://learn.microsoft.com/windows/powertoys/command-palette/settings),
[desenvolvimento de extensões](https://learn.microsoft.com/windows/powertoys/command-palette/creating-an-extension),
[especificação ULID](https://github.com/ulid/spec).
