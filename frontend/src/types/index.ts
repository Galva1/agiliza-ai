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
  | "resolucao_aprovada";

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

export interface ChamadoDetalhe extends Chamado {
  comentarios: ComentarioChamado[];
  historico: HistoricoChamado[];
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
