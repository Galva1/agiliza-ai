export type PerfilUsuario = "Administrador" | "Tecnico" | "Solicitante";

export type StatusChamado =
  | "Aberto"
  | "EmAndamento"
  | "Aguardando"
  | "Resolvido"
  | "Fechado";

export type PrioridadeChamado = "Baixa" | "Media" | "Alta" | "Urgente";

export type AcaoHistorico =
  | "status"
  | "prioridade"
  | "categoria"
  | "tecnico"
  | "comentario"
  | "resolucao_proposta"
  | "resolucao_aprovada"
  | "resolucao_recusada"
  | "pendencia"
  | "pendencia_concluida"
  | "solicitacao_aprovacao"
  | "solicitacao_aprovada"
  | "solicitacao_recusada";

export type TipoComentario = "Comentario" | "Pendencia" | "SolicitacaoAprovacao";

export type ResponsavelPendencia = "Tecnico" | "Solicitante";

export type SituacaoComentario = "Aberta" | "Concluida" | "Aprovada" | "Recusada";

export type SituacaoResolucao = "Pendente" | "Aprovada" | "Recusada";

export interface Usuario {
  id: string;
  nome: string;
  email: string;
  perfil: PerfilUsuario;
  ativo: boolean;
  criadoEm: string;
}

export interface LoginResponse {
  token: string;
  expiraEm: string;
  usuario: Usuario;
}

export interface Categoria {
  id: string;
  nome: string;
  descricao: string | null;
  ativa: boolean;
  criadoEm: string;
}

export interface Chamado {
  id: string;
  titulo: string;
  descricao: string;
  status: StatusChamado;
  prioridade: PrioridadeChamado;
  categoriaId: string | null;
  categoriaNome: string | null;
  solicitanteId: string;
  solicitanteNome: string;
  tecnicoId: string | null;
  tecnicoNome: string | null;
  resolucao: string | null;
  resolucaoAprovada: boolean;
  resolucaoPropostaEm: string | null;
  concluidoEm: string | null;
  criadoEm: string;
  atualizadoEm: string | null;
  fechadoEm: string | null;
}

export interface ComentarioChamado {
  id: string;
  usuarioId: string;
  usuarioNome: string;
  mensagem: string;
  criadoEm: string;
  tipo: TipoComentario;
  responsavel: ResponsavelPendencia | null;
  situacao: SituacaoComentario | null;
  resposta: string | null;
  respondidoPorNome: string | null;
  respondidoEm: string | null;
}

export interface HistoricoChamado {
  id: string;
  usuarioId: string;
  usuarioNome: string;
  acao: AcaoHistorico;
  valorAnterior: string | null;
  valorNovo: string | null;
  criadoEm: string;
}

export interface ResolucaoChamado {
  id: string;
  usuarioId: string;
  usuarioNome: string;
  descricao: string;
  situacao: SituacaoResolucao;
  motivoRecusa: string | null;
  criadoEm: string;
  avaliadoPorNome: string | null;
  avaliadoEm: string | null;
}

export interface ChamadoDetalhe extends Chamado {
  comentarios: ComentarioChamado[];
  historico: HistoricoChamado[];
  resolucoes: ResolucaoChamado[];
}

export interface ModeloTexto {
  id: string;
  titulo: string;
  conteudo: string;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm: string | null;
}

export interface Pagina<T> {
  itens: T[];
  paginaAtual: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}

export interface PainelChamados {
  abertos: Pagina<Chamado>;
  atribuidos: Pagina<Chamado>;
  concluidos: Pagina<Chamado>;
}

export interface ApiErrorBody {
  status: number;
  message: string;
}
