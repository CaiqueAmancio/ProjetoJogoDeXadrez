
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
        public abstract bool[,] movimentosPossiveis();

    }

}
