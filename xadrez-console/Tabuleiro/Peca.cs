
namespace tabuleiro
{
    abstract class Peca
    {
        public Posicao posicao { get; set; }
        public Cor cor { get; protected set; }
        public int qtdeMovimentos { get; set; }
        public Tabuleiro tab { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor)
        {
            this.posicao = null; // ainda nao tem posicao
            this.tab = tab;
            this.cor = cor;
            this.qtdeMovimentos = 0; // inicia sem movimentos
        }
        public void incrementarQtdeMovimentos()
        {
            qtdeMovimentos++;
        }

        public void decrementarQtdeMovimentos()
        {
            qtdeMovimentos--;
        }

        public bool existeMovimentosPossiveis()
        {
            bool[,] mat = movimentosPossiveis();
            for(int i=0; i<tab.linhas; i++)
            {
                for(int j=0; j<tab.colunas; j++)
                {
                    if(mat[i, j])
                    {
                        return true;
                    }
                }
            }
            return false; // ira retornar o movimento se a peca puder se mover no tabuleiro (linha, coluna)
        }

        public bool podeMoverPara(Posicao pos) // verifica se a peça pode se mover para uma posicao determinada
        {
            return movimentosPossiveis()[pos.linha, pos.coluna]; // testar se na linha e na coluna essa posicao e verdadeira
        }

        public abstract bool[,] movimentosPossiveis();

    }

}
