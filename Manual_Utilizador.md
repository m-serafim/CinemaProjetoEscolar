# Manual de Utilizador - Nexor Cinema

## Sistema de Gestão de Cinema

---

## Índice

1. [Introdução](#1-introdução)
2. [Requisitos do Sistema](#2-requisitos-do-sistema)
3. [Como Registar-se no Sistema](#3-como-registar-se-no-sistema)
4. [Como Efetuar Login](#4-como-efetuar-login)
5. [Funcionalidades para Clientes](#5-funcionalidades-para-clientes)
   - 5.1 [Visualizar Catálogo de Filmes](#51-visualizar-catálogo-de-filmes)
   - 5.2 [Consultar Sessões Disponíveis](#52-consultar-sessões-disponíveis)
   - 5.3 [Fazer Reservas de Bilhetes](#53-fazer-reservas-de-bilhetes)
   - 5.4 [Ver Histórico de Reservas](#54-ver-histórico-de-reservas)
   - 5.5 [Notificações de Reembolso](#55-notificações-de-reembolso)
   - 5.6 [Gerir Perfil](#56-gerir-perfil)
6. [Funcionalidades para Administradores](#6-funcionalidades-para-administradores)
   - 6.1 [Gerir Filmes (CRUD)](#61-gerir-filmes-crud)
   - 6.2 [Gerir Sessões (CRUD)](#62-gerir-sessões-crud)
   - 6.3 [Gerir Reservas](#63-gerir-reservas)
   - 6.4 [Gerir Utilizadores](#64-gerir-utilizadores)
7. [Pesquisa de Filmes](#7-pesquisa-de-filmes)
8. [Tabela de Verificação de Requisitos](#8-tabela-de-verificação-de-requisitos)

---

## 1. Introdução

Bem-vindo ao **Nexor Cinema**, um sistema de gestão de cinema desenvolvido em ASP.NET MVC. Esta aplicação permite aos clientes visualizar filmes, consultar sessões e efetuar reservas de bilhetes. Os administradores têm acesso completo para gerir todos os aspetos do cinema, incluindo filmes, sessões, reservas e utilizadores.

### Características Principais:
- Interface moderna e intuitiva com tema escuro
- Sistema de autenticação seguro com ASP.NET Core Identity
- Gestão completa de filmes com integração à API OMDB/TMDB
- Sistema de reservas com seleção visual de lugares
- Painel de administração completo

---

## 2. Requisitos do Sistema

Para utilizar o Nexor Cinema, necessita de:

- **Navegador Web**: Google Chrome, Mozilla Firefox, Microsoft Edge ou Safari (versões recentes)
- **Ligação à Internet**: Para aceder à aplicação web
- **Conta de Utilizador**: Para efetuar reservas (pode registar-se gratuitamente)

---

## 3. Como Registar-se no Sistema

Para criar uma conta no Nexor Cinema, siga os seguintes passos:

### Passo 1: Aceder à Página de Registo
1. Abra o navegador e aceda ao Nexor Cinema
2. Clique no botão **"Login"** no canto superior direito da página
3. Na página de login, clique em **"Registar"** ou **"Criar conta"**

### Passo 2: Preencher o Formulário de Registo
1. **Email**: Introduza um endereço de email válido (será usado para login)
2. **Palavra-passe**: Crie uma palavra-passe segura com:
   - Mínimo de 6 caracteres
   - Pelo menos uma letra maiúscula
   - Pelo menos uma letra minúscula
   - Pelo menos um número
   - Pelo menos um caractere especial
3. **Confirmar Palavra-passe**: Repita a palavra-passe

### Passo 3: Concluir o Registo
1. Clique no botão **"Registar"**
2. Será automaticamente atribuído o papel de **"Cliente"**
3. Será redirecionado para a página inicial, já com sessão iniciada

> **Nota**: O papel de "Cliente" permite visualizar filmes, consultar sessões e efetuar reservas. Para acesso de administrador, contacte o gestor do sistema.

---

## 4. Como Efetuar Login

### Para iniciar sessão na sua conta:

1. Clique no botão **"Login"** no canto superior direito
2. Introduza o seu **Email** registado
3. Introduza a sua **Palavra-passe**
4. Clique no botão **"Entrar"**

### Após o login bem-sucedido:
- Verá "Olá [seu email]!" no canto superior direito
- Terá acesso às funcionalidades correspondentes ao seu papel (Cliente ou Administrador)

### Terminar Sessão:
- Clique em **"Sair"** no menu superior para terminar a sessão

---

## 5. Funcionalidades para Clientes

Os utilizadores com papel de **Cliente** têm acesso às seguintes funcionalidades:

### 5.1 Visualizar Catálogo de Filmes

#### Página Inicial
- A página inicial apresenta os filmes em destaque num carrossel rotativo
- Secção **"Filmes em Cartaz"**: Filmes atualmente disponíveis nas salas
- Secção **"Brevemente"**: Filmes que serão lançados em breve

#### Catálogo Completo
1. Clique em **"Catálogo de Filmes"** no menu de navegação
2. Visualize a página do catálogo com:
   - Cabeçalho com estatísticas (total de filmes, em cartaz, brevemente)
   - Sistema de filtros integrado
3. Cada filme é apresentado em cartões modernos com:
   - Capa/Poster do filme com efeito de hover
   - Badge de estado (Em Cartaz / Em Breve)
   - Título
   - Género e Duração
   - Classificação etária
   - Descrição resumida
   - Botão para ver trailer (se disponível)

#### Filtros de Pesquisa
- **Pesquisa por texto**: Digite o nome do filme
- **Filtro por género**: Selecione um género específico
- **Filtro por estado**: Escolha entre "Em Cartaz" ou "Brevemente"
- **Limpar filtros**: Clique em "Limpar" para remover todos os filtros

#### Detalhes do Filme
1. Clique em **"Ver Detalhes"** no cartão do filme
2. Visualize informações completas:
   - Sinopse completa
   - Elenco principal
   - Realizador
   - Data de estreia
   - Trailer (quando disponível)
   - Sessões disponíveis

### 5.2 Consultar Sessões Disponíveis

Para ver as sessões de um filme:

1. Aceda à página de detalhes do filme
2. Clique em **"Comprar bilhete"** ou **"Ver Sessões"**
3. As sessões são organizadas por data
4. Para cada sessão é apresentado:
   - **Hora**: Horário da sessão
   - **Sala**: Número da sala
   - **Preço**: Valor do bilhete
   - **Lugares Disponíveis**: Quantidade de lugares ainda disponíveis
     - 🟢 Verde: Muitos lugares disponíveis
     - 🟡 Amarelo: Últimos lugares
     - 🔴 Vermelho: Esgotado

### 5.3 Fazer Reservas de Bilhetes

> **Importante**: É necessário ter sessão iniciada como Cliente para efetuar reservas.

#### Processo de Reserva:

1. **Escolher o Filme**: Navegue até ao filme desejado
2. **Selecionar a Sessão**: Escolha a data e hora pretendidas
3. **Clicar em "Reservar Bilhete"**: Acederá à página de reserva
4. **Selecionar Lugares**: 
   - Visualize o mapa da sala
   - Lugares disponíveis aparecem a azul
   - Lugares ocupados aparecem a vermelho
   - Clique nos lugares que deseja reservar
5. **Confirmar Quantidade**: Indique o número de bilhetes
6. **Finalizar Reserva**: Clique em **"Confirmar Reserva"**
7. **Confirmação**: Receberá uma confirmação com os detalhes da reserva

### 5.4 Ver Histórico de Reservas

Para consultar as suas reservas:

1. Certifique-se de que tem sessão iniciada
2. Clique em **"As Minhas Reservas"** no menu de navegação
3. Visualize a lista de todas as suas reservas com:
   - Nome do filme
   - Data e hora da sessão
   - Sala
   - Lugares reservados
   - Estado da reserva
   - Preço total

### 5.5 Notificações de Reembolso

Quando uma sessão é cancelada pelo administrador, o sistema trata automaticamente do reembolso:

#### Como Funciona:
1. Se o administrador cancelar uma sessão ou eliminar um filme para o qual tem bilhetes comprados
2. Ao fazer **login** na sua conta, receberá imediatamente um **aviso grande em ecrã cheio**
3. O aviso mostra:
   - Detalhes da reserva cancelada
   - Motivo do cancelamento (com o nome do filme e data da sessão)
   - Valor a reembolsar
   - Estado do reembolso (processado ou pendente)
   - Últimos 4 dígitos do cartão de pagamento
4. Clique em **"Entendido"** para fechar o aviso

#### Garantia de Reembolso:
- O reembolso é automático para o cartão utilizado na compra
- O valor total pago é devolvido
- Processamento em 5-10 dias úteis
- Receberá o aviso sempre que fizer login até confirmar que viu
- Os dados do filme e sessão são preservados mesmo após eliminação

### 5.6 Gerir Perfil

Para gerir a sua conta:

1. Clique no seu nome/email no canto superior direito
2. Acederá à página de **Perfil**
3. Pode:
   - Ver o seu nome de utilizador
   - Atualizar o número de telefone
   - Alterar a palavra-passe
   - Alterar o email
   - Gerir dados pessoais
   - **Ver Transações** (histórico de compras e reembolsos)

> **Nota**: A opção "Transações" só está disponível para clientes, não para administradores.

---

## 6. Funcionalidades para Administradores

Os utilizadores com papel de **Administrador** têm acesso ao painel de administração com funcionalidades CRUD completas.

> **Nota**: O menu **"Administração"** só aparece para utilizadores com papel de Administrador.

### 6.1 Gerir Filmes (CRUD)

#### Aceder à Gestão de Filmes:
1. Clique em **"Administração"** no menu
2. Selecione **"Gerir Filmes"**
3. Visualize todos os filmes num layout de cartões moderno com:
   - Capa do filme
   - Título e género
   - Duração
   - Estado (Em Cartaz / Brevemente / Arquivado)
   - Descrição resumida
4. Utilize a **barra de pesquisa** para filtrar filmes rapidamente

#### Criar Novo Filme:
1. Clique no botão **"Adicionar Novo Filme"**
2. Preencha os campos:
   - **Título** (obrigatório)
   - **Género** (obrigatório)
   - **Duração** em minutos
   - **Descrição/Sinopse**
   - **Classificação Etária**
   - **Data de Estreia**
   - **URL da Capa**
   - **URL do Banner**
   - **URL do Trailer**
   - **Elenco**
   - **Realizador**
   - **Status** (Em Cartaz / Brevemente / Arquivado)
   - **Destaque na Home** (checkbox)
3. **Funcionalidade OMDB**: Pode pesquisar automaticamente informações do filme:
   - Digite o nome do filme no campo de pesquisa
   - Selecione o filme da lista de resultados
   - Os campos serão preenchidos automaticamente
4. Clique em **"Criar"**

#### Editar Filme:
1. No cartão do filme, clique no botão **"Editar"**
2. Modifique os campos necessários
3. Clique em **"Guardar"**

#### Eliminar Filme:
1. Na lista de filmes, clique no ícone de **eliminar** (lixo)
2. Confirme a eliminação
> **Atenção**: Eliminar um filme também elimina todas as sessões e reservas associadas.

#### Ver Detalhes:
- Clique no ícone de **detalhes** (olho) para ver todas as informações do filme

### 6.2 Gerir Sessões (CRUD)

#### Aceder à Gestão de Sessões:
1. Clique em **"Administração"** no menu
2. Selecione **"Gerir Sessões"**

#### Criar Nova Sessão:
1. Clique no botão **"Nova Sessão"**
2. Preencha os campos:
   - **Filme**: Selecione da lista de filmes
   - **Data e Hora**: Escolha quando será a sessão
   - **Sala**: Indique o número da sala
   - **Preço**: Defina o valor do bilhete
   - **Lugares Totais**: Capacidade da sala
3. Clique em **"Criar"**

#### Gerar Sessões Automaticamente:
1. Clique em **"Criar Automaticamente"**
2. Selecione o filme
3. Defina o intervalo de datas
4. Configure os horários
5. O sistema criará múltiplas sessões automaticamente

#### Selecionar e Eliminar Múltiplas Sessões:
1. Na página de gestão de sessões, clique em **"Selecionar Várias"**
2. As sessões entram em modo de seleção
3. Clique nas sessões que deseja eliminar (ou nas checkboxes)
4. O contador mostrará quantas sessões estão selecionadas
5. Clique em **"Eliminar Selecionadas"**
6. Confirme a eliminação no modal
> **Nota**: As reservas associadas serão automaticamente canceladas e os clientes serão notificados para reembolso.

#### Editar Sessão:
1. Clique no ícone de **edição** da sessão
2. Modifique os campos necessários
3. Clique em **"Guardar"**

#### Eliminar Sessão Individual:
1. Clique no ícone de **eliminar**
2. Confirme a eliminação
> **Nota**: As reservas associadas serão canceladas e os clientes serão notificados.

#### Reset Total do Sistema:
1. Clique no botão **"Reset Total"** (vermelho)
2. Leia atentamente o aviso sobre o que será eliminado
3. Marque a checkbox de confirmação
4. Clique em **"Eliminar Tudo"**
> **ATENÇÃO**: Esta ação elimina permanentemente TODOS os filmes, sessões e reservas. É irreversível.

### 6.3 Gerir Reservas

Os administradores podem visualizar e gerir todas as reservas do sistema.

#### Ver Todas as Reservas:
1. Aceda à secção de reservas através do painel de administração
2. Visualize a lista completa de reservas com:
   - Cliente
   - Filme
   - Sessão (data/hora)
   - Lugares reservados
   - Estado
   - Valor

#### Ações Disponíveis:
- Ver detalhes completos da reserva
- Cancelar reservas (quando necessário)

### 6.4 Gerir Utilizadores

#### Aceder à Gestão de Utilizadores:
1. Clique em **"Administração"** no menu
2. Selecione **"Gerir Utilizadores"**

#### Funcionalidades:
- Visualizar lista de todos os utilizadores registados
- Ver detalhes de cada utilizador:
  - Email
  - Nome de utilizador
  - Papel (Cliente/Administrador)
  - Data de registo

---

## 7. Pesquisa de Filmes

O Nexor Cinema dispõe de uma funcionalidade de pesquisa rápida:

### Utilizar a Pesquisa:
1. Localize a **barra de pesquisa** no cabeçalho da página
2. Digite o nome do filme (parcial ou completo)
3. Os resultados aparecem automaticamente à medida que escreve
4. Clique no filme desejado para aceder aos seus detalhes

### Características da Pesquisa:
- Pesquisa em tempo real (sem necessidade de pressionar Enter)
- Não é sensível a maiúsculas/minúsculas
- Mostra até 5 resultados
- Apresenta miniatura, título e género de cada filme

---

## 8. Tabela de Verificação de Requisitos

| Requisitos | Implementado (Sim/Não) |
|------------|:----------------------:|
| Sistema de registo de utilizadores | Sim |
| Atribuição automática do papel "Cliente" no registo | Sim |
| Sistema de login com email e password | Sim |
| Controlo de acessos baseado em roles | Sim |
| CRUD de filmes (apenas administradores) | Sim |
| CRUD de sessões (apenas administradores) | Sim |
| Eliminação múltipla de sessões | Sim |
| Reset total do sistema (filmes, sessões, reservas) | Sim |
| Gestão de reservas (administradores) | Sim |
| Visualização de catálogo de filmes (clientes) | Sim |
| Consulta de sessões disponíveis (clientes) | Sim |
| Sistema de reservas de bilhetes (clientes) | Sim |
| Visualização de histórico de reservas pessoais (clientes) | Sim |
| Notificação automática de reembolso no login | Sim |
| Reembolso automático quando sessão é cancelada | Sim |

---

## Tecnologias Utilizadas

- **ASP.NET MVC** - Framework principal
- **Entity Framework Core** - ORM e migrations
- **SQL Server / SQLite** - Base de dados
- **ASP.NET Core Identity** - Autenticação e autorização
- **Bootstrap 5** - Interface responsiva
- **OMDB/TMDB API** - Informações de filmes

---

## Suporte

Para questões ou suporte técnico, contacte a equipa de desenvolvimento.

---

**Nexor Cinema** © 2026 - Sistema de Gestão de Cinema  
*Projeto desenvolvido no âmbito do Módulo 5 - TGPSI*
