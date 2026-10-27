# Publicação privada do Value Generator na Microsoft Store

Registro da conversa e plano de continuidade, atualizado em **27/10/2026**.

## Situação para retomada

O usuário aprovou preparar a publicação inicialmente privada na Microsoft Store.
Solicitou as instruções da etapa de cadastro para executá-la pessoalmente e
autorizou o assistente a implementar o restante do plano. Em seguida, pediu para
salvar o contexto no repositório e continuar em outra sessão.

Na etapa inicial foi salvo apenas este documento. A preparação foi retomada e
implementada em 27/10/2026 conforme o status abaixo. Não houve submissão nem foram
criadas credenciais de publicação.

### Status da implementação — 27/10/2026

- Windows SDK estável `10.0.26100.57`; framework e CLSID preservados.
- Removida a capability sem uso `internetClient`, mantida `runFullTrust`.
- Licenças, avisos do runtime, inventário das versões e política de privacidade
  incluídos no MSIX; licenças de dependências revisadas nas versões restauradas.
- Nova arte original inspirada em Fluent, com `#` azul/ciano, volume e transparência.
  Substitui a proposta inicial branca sobre azul, rejeitada pelo usuário.
- Correção após teste visual: o CmdPal também usa a nova arte; `AppListEntry="none"`
  oculta o atalho do aplicativo, evitando duplicidade com o comando da extensão.
- Workflow manual e script de StoreUpload criados. Identidade fornecida por três
  variáveis públicas do GitHub; manifesto fonte não recebe identidade inventada.
- Debug sem warnings; 110 testes Release passando. StoreUpload local gerado e
  inspecionado com identidade temporária de validação, que não serve para submissão.
  Release mantém um aviso `IL2104` da dependência Windows SDK.
- Fluxo operacional e listagem en-US em [publicacao.md](publicacao.md) e
  [store-listing.en-US.md](store-listing.en-US.md).
- Pendente: cadastro/identidade real, execução do workflow no GitHub, screenshots
  reais, WACK, revisão do Partner Center, envio e teste em dois PCs após aprovação.
- As alterações que já estavam staged pelo usuário foram preservadas e incluídas
  na separação de commits solicitada depois da preparação. Os pacotes locais
  assinados foram gerados até a versão `0.0.4.0` e permanecem fora do Git.
  Não houve push, instalação automática ou submissão à Store.

As seções de planejamento abaixo registram o estado e as pendências da conversa
original; consulte este status e o guia operacional para o estado atual.

Próxima ação do usuário: criar a conta e reservar o produto no Partner Center,
conforme a seção de cadastro abaixo. O assistente pode adiantar os ajustes que não
dependem da identidade; os valores reais do Partner Center são necessários para
concluir a configuração do pacote e gerar o artefato definitivo.

## Decisões aprovadas

| Item | Decisão |
| --- | --- |
| Repositório | `https://github.com/raffreitas/cmdpal-value-generator` |
| Nome a reservar na Store | **Value Generator** |
| Canal inicial | Microsoft Store, produto MSIX |
| Distribuição | **Private audience**, inicialmente suas contas Microsoft pessoais |
| Preço | Gratuito |
| Arquitetura inicial | Somente **x64** |
| Geração dos artefatos | **GitHub Actions** |
| Envio inicial | Manual pelo Partner Center, após revisão |
| Versão inicial | `0.0.1.0` |
| Ícone do pacote | Símbolo `#` próprio, azul/ciano dimensional inspirado em Fluent |
| Idioma da interface e listagem inicial | Inglês / en-US |
| Framework | Preservar `net10.0-windows10.0.26100.0` |
| Hospedagem | Preservar MSIX e servidor COM em EXE |
| CLSID | Preservar `cdce3db6-ffd6-46e9-864a-ff25ade45013` |
| Fora desta etapa | WinGet, Gallery, ARM64 e publicação automática |

A audiência privada restringe a aquisição do produto na Store; ela não torna o
código do GitHub privado.

## Contexto técnico e estado do repositório

A extensão já foi implementada e o usuário confirmou que funcionou no CmdPal.
Inclui generators do PowerToys Run, parser sem Wox, sugestões, página dinâmica,
fallback, formatos extras de GUID, ULID e hashes do clipboard. Os testes originais
foram portados, e a suíte totalizou 110 testes passando nas verificações anteriores.

A solução principal foi migrada para `ValueGenerator.slnx`. Foram criados estes
commits locais durante a conversa:

| Commit | Assunto |
| --- | --- |
| `8e8e402` | Add CmdPal extension template |
| `640510b` | Port Value Generator generators and input parser |
| `d499d5c` | Add dynamic Value Generator page |
| `77db1ec` | Add Value Generator fallback commands |
| `be0956d` | Add ULID, GUID formats and clipboard hashing |
| `f3454cf` | Add generator and input parser tests |
| `77ea526` | Document deployment and Command Palette alias |
| `3757f63` | Migrate solution to slnx format |

Na inspeção ao salvar este documento, havia alterações do usuário **já no stage**:

- `Directory.Packages.props`: atualização de dependências, incluindo CmdPal SDK
  `0.12.260812002`, CsWinRT `2.3.1`, Shmuelie.WinRTServer `2.2.1`, MSIX BuildTools
  `1.7.260930100`, Microsoft.NET.Test.Sdk `18.11.0` e MSTest `4.5.1`.
- `ValueGenerator.sln`: recriado localmente e adicionado ao stage.

Preservar essas alterações. Não reverter dependências nem remover o `.sln` por
conta própria. O pipeline planejado usa explicitamente `.slnx`.

Na inspeção inicial, o projeto fixava o Windows SDK em `10.0.26100.68-preview`; o manifesto ainda
usa o publisher Microsoft do template e `PublisherDisplayName="A Lone Developer"`.
Esses valores precisam ser preparados para distribuição.

Durante o planejamento, este comando passou com zero warnings e erros, sem alterar
os fontes:

```powershell
dotnet build ValueGenerator.slnx -p:Platform=x64 -p:WindowsSdkPackageVersion=10.0.26100.57 --no-restore -warnaserror
```

Isso valida o build Debug com a versão estável candidata; não substitui restore
limpo, testes, build Release e validação do MSIX no ambiente de publicação.

## Licença: conclusões e pendências

- O código portado do PowerToys é MIT. A licença permite modificar, publicar,
  redistribuir e comercializar, preservando copyright e o texto da licença.
- `LICENSE`, cabeçalhos originais e `THIRD-PARTY-NOTICES.md` já existem no
  repositório. Falta garantir sua inclusão no pacote distribuído.
- A revisão dos pacotes usados à época encontrou MIT no Toolkit, CsWinRT e
  Shmuelie.WinRTServer. A atualização local de dependências exige revisar as
  versões efetivamente restauradas antes de fechar o inventário.
- O perfil é self-contained e distribui o runtime .NET, que possui licença e
  avisos de terceiros próprios. Não basta incluir apenas a licença do PowerToys.
- Ferramentas de build e componentes distribuídos têm papéis distintos. Examinar
  o conteúdo final do MSIX e os termos aplicáveis a cada componente que acompanha
  o produto; não rotular automaticamente todas as dependências como MIT.
- A versão `10.0.26100.68-preview` de `Microsoft.Windows.SDK.NET.Ref` aponta para
  `https://aka.ms/WinSDKPreviewLicenseURL`. Na pesquisa, esse endereço redirecionou
  para termos antigos de SDK pré-release, com restrições de uso e redistribuição.
  Isso foi tratado como pendência, não como prova de que qualquer SDK preview
  proíbe qualquer aplicação. A decisão é substituir pelo estável `10.0.26100.57`
  e revisar os termos correspondentes, preservando o target framework.
- A MIT não concede automaticamente direitos de marca. Usar identidade própria,
  explicar a integração com PowerToys/Command Palette e preservar a atribuição
  sem apresentar o produto como uma publicação oficial da Microsoft.

## 1. Cadastro no Partner Center — responsabilidade do usuário

1. Acessar [Microsoft Store Developer](https://storedeveloper.microsoft.com/) e
   escolher começar gratuitamente.
2. Entrar com uma conta Microsoft pessoal, selecionar desenvolvedor individual
   e concluir a verificação de identidade na plataforma.
3. No [Partner Center](https://partner.microsoft.com/dashboard/home), acessar
   **Apps and games → New product → MSIX or PWA app**.
4. Tentar reservar **Value Generator**. Se estiver indisponível, definir outro
   nome com o usuário antes de configurar o pacote.
5. Em **Product management → Product identity**, obter os valores abaixo e
   fornecê-los ao assistente. São identificadores públicos, não credenciais:

   ```text
   Nome reservado:
   Package/Identity/Name:
   Package/Identity/Publisher:
   Package/Properties/PublisherDisplayName:
   Product ID:
   ```

6. Na submissão, em **Pricing and availability → Visibility → Audience**,
   selecionar **Private audience** e criar um grupo com as contas Microsoft
   pessoais usadas na Store dos PCs autorizados.
7. Definir preço **Free**, não habilitar **Make this product public on** e salvar
   o rascunho. Não enviar para certificação antes de preparar e revisar o pacote.

Contas corporativas ou escolares Entra ID não atendem à aquisição por audiência
privada. Selecionar apenas “não descobrível” não restringe da mesma forma: usuários
com o link ainda podem adquirir o produto. A audiência privada permite abrir ao
público depois; o caminho de público para privado não é permitido.

Não solicitar ao usuário senhas, documentos de identidade, tokens ou PFX pela
conversa. A verificação de identidade deve acontecer diretamente na plataforma.

## 2. Preparação do pacote — responsabilidade do assistente

- Aplicar a identidade exata do Partner Center ao manifesto e às propriedades de
  empacotamento, preservando CLSID e registros de extensão/COM.
- Fixar `WindowsSdkPackageVersion` em `10.0.26100.57`, com validação de restore,
  compilação e testes sobre as dependências atuais do usuário.
- Manter self-contained e empacotar licenças e avisos do produto e dos componentes
  realmente distribuídos, incluindo o runtime .NET.
- Gerar o ícone próprio e os tamanhos exigidos pela Store, substituindo os
  placeholders. Preservar o glyph atual dentro do CmdPal.
- Remover a capability `internetClient`, que não é usada pela extensão; manter
  `runFullTrust`, justificando o servidor COM em EXE na submissão.
- Não alterar generators, sintaxe, comportamento dos comandos ou framework como
  parte da preparação para publicação.

## 3. Privacidade, listagem e documentação

- Criar `PRIVACY.md` em inglês com URL pública no repositório. A política atual
  da Store exige política de privacidade para produtos Win32.
- Descrever processamento local de texto e clipboard, leitura para hashes sem
  conteúdo explícito, escrita nas ações de cópia e configurações locais.
- Descrever a ausência de telemetria e transmissão pela extensão conforme o código
  efetivamente auditado, sem prometer o comportamento de Windows ou CmdPal.
- Preparar listagem en-US, categoria de ferramentas de desenvolvimento,
  screenshots, suporte pelo GitHub e atribuição ao PowerToys.
- Explicitar que PowerToys com Command Palette habilitado é pré-requisito e
  fornecer instruções de teste para a certificação.
- Atualizar o README com geração dos artefatos, submissão privada, instalação e
  atualização da versão da Store.

## 4. GitHub Actions

Criar workflow acionado manualmente por **`workflow_dispatch`**, sem publicação
automática, criação de releases ou envio ao Partner Center.

- Usar runner `windows-2025`, instalar .NET SDK 10 e conferir Windows SDK 26100.
- Restaurar `.slnx`, compilar e executar os testes em x64.
- Gerar Release self-contained x64 em modo **StoreUpload**, com assinatura
  desabilitada. A Microsoft Store assina o pacote durante a submissão.
- Obter a versão do manifesto; não manter um número independente no workflow.
- Disponibilizar `.msixupload`, MSIX correspondente e resultados dos testes como
  artefatos por 30 dias.
- Falhar se faltar pacote, ou se identidade, arquitetura, versão ou documentos
  de licença do pacote estiverem incorretos.
- Usar permissões `contents: read`. Não é necessário certificado, PFX, PAT ou
  credencial do Partner Center para gerar esses artefatos.

## 5. Validação e primeira submissão

- Confirmar Debug sem warnings, testes passando e Release sem novos avisos do
  código da extensão. Investigar separadamente avisos das dependências; builds
  anteriores exibiram IL2104/IL2081 nos fallbacks WinRT/Windows SDK.
- Executar Windows App Certification Kit e tratar resultados aplicáveis.
- Conferir o conteúdo do pacote e testar página, sugestões, fallback, formatos
  de GUID, ULID, encodings, clipboard e entradas inválidas.
- Orientar a remoção do pacote de desenvolvimento anterior antes de instalar a
  edição da Store, evitando duplicidade com o mesmo CLSID. Não desinstalar
  automaticamente durante a preparação.
- No Partner Center, completar privacidade, classificação etária, screenshots,
  justificativa de `runFullTrust` e instruções para certificação.
- Revisar pacote, metadados e **Private audience** antes do envio manual.
- Após aprovação, instalar pelo link privado em dois PCs x64 com contas
  autorizadas; confirmar que uma conta fora do grupo não consegue adquirir.
- Na atualização seguinte, incrementar para `0.0.2.0`, repetir o workflow e
  validar atualização pela Store preservando funcionamento e alias `#`.

Aceite final: instalação privada pela Store nos PCs do usuário. Geração do
artefato ou aprovação do build, isoladamente, não concluem a publicação.

## Organização da implementação

Trabalhar por etapas revisáveis: pacote/SDK, licenças e privacidade, assets,
workflow e documentação. Se forem criados commits, manter esses assuntos
separados, sem incluir alterações já staged pelo usuário por acidente.

Não acrescentar `Co-Authored-By` ou links de sessão às mensagens de commit.
Não fazer push, criar releases ou enviar a submissão como efeito do workflow.

## Avaliação da skill local de publicação

A skill `.github/skills/publish-extension/SKILL.md` foi consultada como referência,
mas seus exemplos não devem ser executados literalmente:

- O guia local de WinGet afirma que é necessário EXE/Inno Setup, mas o WinGet
  também aceita MSIX.
- O exemplo local usa `InprocServer32` apontando para DLL. Esta extensão utiliza
  um servidor COM fora de processo, em EXE.
- A limitação descrita sobre descoberta de produtos somente da Store está
  desatualizada: a Gallery atual pode apontar para Store ou WinGet.
- Não ampliar esta etapa para corrigir toda a skill ou implementar WinGet.

## Fontes oficiais consultadas

Informações verificadas em 27/10/2026. Revisar políticas e ferramentas ao retomar
caso tenha transcorrido tempo relevante.

- [Licença MIT do PowerToys](https://github.com/microsoft/PowerToys/blob/main/LICENSE)
- [Publicação de extensões do CmdPal](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension)
- [Publicação do CmdPal na Microsoft Store](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension-store)
- [Cadastro individual gratuito](https://learn.microsoft.com/pt-br/windows/apps/publish/whats-new-individual-developer)
- [Audiência privada e visibilidade](https://learn.microsoft.com/en-us/windows/apps/publish/publish-your-app/msix/visibility-options)
- [Políticas da Store, incluindo privacidade — seção 10.5](https://learn.microsoft.com/en-us/windows/apps/publish/store-policies#105-personal-information)
- [Assinatura de MSIX e assinatura pela Store](https://learn.microsoft.com/en-us/windows/msix/package/sign-msix-package-guide)
- [Marcas da Microsoft](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks)
- [Pacote Windows SDK preview em uso no template](https://www.nuget.org/packages/Microsoft.Windows.SDK.NET.Ref/10.0.26100.68-preview)
- [Licença vinculada pelo pacote preview](https://aka.ms/WinSDKPreviewLicenseURL)
- [Formatos de instalador aceitos pelo WinGet](https://learn.microsoft.com/en-us/windows/package-manager/package/manifest)
- [Gallery do Command Palette](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/extension-gallery)
- [Imagem Windows 2025 do GitHub Actions](https://github.com/actions/runner-images/blob/main/images/windows/Windows2025-Readme.md)
