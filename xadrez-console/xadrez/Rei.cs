using tabuleiro;

namespace xadrez
{
    class Rei : Peca
    {
        public Rei(Tabuleiro tab, Cor cor) : base (tab, cor)
        {

        }
        public override string ToString()
        {
            return "R"; // representacao da peca REI
        }

        private bool podeMover(Posicao pos)
        {
            Peca p = tab.peca(pos);
            return p == null || p.cor != cor; // retornar se a posicao esta vazia ou se ha uma peca adversaria
        }
        public override bool[,] movimentosPossiveis()
        {
            bool[,] mat = new bool[tab.linhas, tab.colunas];

            Posicao pos = new Posicao(0, 0);
            // acima
            pos.definirValores(posicao.linha - 1, posicao.coluna); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // nordeste
            pos.definirValores(posicao.linha - 1, posicao.coluna + 1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // direta
            pos.definirValores(posicao.linha, posicao.coluna + 1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // sudeste
            pos.definirValores(posicao.linha + 1, posicao.coluna + 1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // abaixo
            pos.definirValores(posicao.linha + 1, posicao.coluna); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // sudoeste
            pos.definirValores(posicao.linha + 1, posicao.coluna - 1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // esquerda
            pos.definirValores(posicao.linha, posicao.coluna - 1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            // noroeste
            pos.definirValores(posicao.linha - 1, posicao.coluna -1); //posicao acima da posicao do rei
            if (tab.posicaoValida(pos) && podeMover(pos))
            {
                mat[pos.linha, pos.coluna] = true;
            }
            return mat;
        }
    }
}
