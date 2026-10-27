# Gerar e enviar o pacote privado

Preparação local implementada em 27/10/2026. O cadastro e a primeira submissão
continuam manuais. O [registro do plano](plano-e-contexto.md) preserva as decisões.

## Identidade do produto

Após reservar **Value Generator** no Partner Center, abra **Product management >
Product identity**. Em **GitHub > Settings > Secrets and variables > Actions >
Variables**, configure estas variáveis de repositório com os valores exatos:

| Variável | Campo do Partner Center |
| --- | --- |
| `STORE_IDENTITY_NAME` | Package/Identity/Name |
| `STORE_PUBLISHER` | Package/Identity/Publisher, incluindo `CN=` |
| `STORE_PUBLISHER_DISPLAY_NAME` | Package/Properties/PublisherDisplayName |

São identificadores públicos. Não é necessário colocar senha, certificado, PFX,
PAT ou credencial do Partner Center no workflow. O build recusa identidade ausente
ou o publisher Microsoft do template. Um manifesto temporário em `obj/Store/`
recebe os valores, preservando o manifesto fonte para deploy local, o framework
e o CLSID. Não configure uma identidade inventada para a submissão.

## GitHub Actions

1. Suba os arquivos e o workflow para a branch padrão do GitHub.
2. Abra **Actions > Build Microsoft Store package > Run workflow**.
3. Aguarde restore, build Debug, 110 testes e empacotamento Release x64.
4. Baixe **ValueGenerator-Store-x64** e **test-results**. Retenção: 30 dias.
5. Envie o arquivo `.msixupload` ao Partner Center. O MSIX correspondente está
   incluído no artefato para inspeção.

O workflow é manual, usa permissões de leitura e não publica uma release nem envia
pacotes à Store. Foi validado por execução local dos comandos; ainda precisa ser
executado no GitHub. Sem as variáveis de identidade ele falha com uma orientação.

O pacote é self-contained e não exige .NET SDK nem Visual Studio no PC de destino.
O `.msixupload` é **sem assinatura**, destinado à submissão: a Store assina a edição
distribuída. Ele não substitui o instalador local assinado utilizado antes da Store.

Para gerar localmente, em Windows com .NET 10 SDK, PowerShell 7 (`pwsh`), Windows
SDK 26100 e ferramentas MSIX instalados:

```powershell
dotnet restore ValueGenerator.slnx -p:Platform=x64
dotnet build ValueGenerator.slnx -p:Platform=x64 --no-restore -warnaserror
dotnet test ValueGenerator.UnitTests/ValueGenerator.UnitTests.csproj -c Release -p:Platform=x64 --no-restore
# Configure as três variáveis STORE_* com seus valores públicos do Partner Center.
./build/Build-StorePackage.ps1
```

O destino padrão é `ValueGenerator/AppPackages/Store/` e deve estar vazio. Para
outro build, use `-OutputDirectory` com uma nova pasta. A versão vem exclusivamente
do manifesto fonte; a versão inicial continua `0.0.1.0`. Para a atualização seguinte,
incremente ali, por exemplo para `0.0.2.0`. A versão do pacote de teste local é
independente e não determina a versão inicial de uma identidade nova da Store.

O script confere identidade, versão, arquitetura, CLSID, capabilities, arquivos do
runtime e documentos de licença. O pacote não leva testes, scripts, certificados
nem a imagem fonte de alta resolução. Em Release permanece o aviso `IL2104` da
projeção Windows SDK; o Debug passa com warnings tratados como erros.

## Completar a submissão

- Escolha **Private audience**, adicione suas contas Microsoft pessoais e confira
  os mercados/contas habilitados. Visibilidade oculta com link não equivale a uma
  audiência privada; a escolha inicial merece atenção, pois um produto público
  não pode depois ser convertido para privado.
- Defina gratuito, idioma en-US e categoria Developer tools, se disponível.
- Use os textos de [store-listing.en-US.md](store-listing.en-US.md), complete
  classificação etária e revise a URL pública da política de privacidade.
- Capture screenshots reais da extensão no CmdPal: sugestões com busca vazia,
  `guid7` com menu de formatos e `sha256 abc` pelo fallback. Use apenas dados de
  exemplo, confira as dimensões exigidas pelo formulário atual e não apresente
  mockups como se fossem a interface. As screenshots ainda estão pendentes.
- Rode o Windows App Certification Kit no pacote e revise os resultados aplicáveis.
  A validação do script não substitui o WACK nem a certificação da Store.
- Revise identidade, audiência, pacote, ícones e textos antes de enviar manualmente.

## Instalação e validação após aprovação

Remova manualmente a edição de desenvolvimento/local que registra o mesmo CLSID
antes de instalar a edição da Store. Instale PowerToys com Command Palette habilitado
nos dois PCs, entre na Store com uma conta autorizada e adquira pelo link privado.
Não há necessidade de importar o certificado local para a edição da Store.

Recarregue as extensões no CmdPal, configure o alias `#` conforme o README e teste
as sugestões, clipboard, formatos de GUID, fallback e erros. Confira também a
restrição de aquisição com uma conta fora da audiência e, na atualização seguinte,
o recebimento da versão nova pela Store.

Referências: [publicação CmdPal](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension-store),
[visibilidade](https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msix/visibility-options),
[assinatura MSIX](https://learn.microsoft.com/en-us/windows/msix/package/sign-msix-package-guide).
