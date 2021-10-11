using System;
using tabuleiro;
using xadrez;

namespace xadrez_console
{
    class Tela
    {
        public static void imprimirTabuleiro(Tabuleiro tab)
        {
            for (int i = 0; i < tab.linhas; i++)
            {
                Console.Write(8 - i + " ");
                for (int j = 0; j < tab.colunas; j++)
                {
                    imprimirPeca(tab.peca(i, j)); // imprimir a peca
                }
            Console.WriteLine();
        }
            Console.WriteLine("  a b c d e f g h");
        }

        public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoesPossiveis)
        {
            ConsoleColor fundoOriginal = Console.BackgroundColor; // pegar a cor do fundo
            ConsoleColor fundoAlterado = ConsoleColor.DarkGray; // cor cinza escuro quando a posicao estiver marcada

            for (int i = 0; i < tab.linhas; i++)
            {
                Console.Write(8 - i + " ");
                for (int j = 0; j < tab.colunas; j++)
                {
                    if(posicoesPossiveis[i, j])
                    {
                        Console.BackgroundColor = fundoAlterado;
                    }
                    else
                    {
                        Console.BackgroundColor = fundoOriginal;
                    }
                    imprimirPeca(tab.peca(i, j)); // imprimir a peca
                    Console.BackgroundColor = fundoOriginal;
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
            Console.BackgroundColor = fundoOriginal;
        }

        public static PosicaoXadrez lerPosicaoXadrez() // ler do teclado o que o usuario digitar
    {
        string s = Console.ReadLine();
        char coluna = s[0];
        int linha = int.Parse(s[1] + "");
        return new PosicaoXadrez(coluna, linha);
    }

    public static void imprimirPeca(Peca peca)
    {
        if (peca == null)
        {
            Console.Write("- ");
        }
        else
        {

            if (peca.cor == Cor.Branca)
            {
                Console.Write(peca);
            }
            else
            {
                ConsoleColor aux = Console.ForegroundColor; // salvar a cor atual da peca em 'aux'
                Console.ForegroundColor = ConsoleColor.Yellow; // muda para amarelo
                Console.Write(peca); // imprime a cor
                Console.ForegroundColor = aux; // volta para a cor original
            }
            Console.Write(" ");
        }
    }
}
}
