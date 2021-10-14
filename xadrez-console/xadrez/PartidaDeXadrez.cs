using System;
using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; } // private set para o programador nao alterar o tabuleiro pelo programa principal
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas; // este conjunto ira guardar todas as pecas da partida
        private HashSet<Peca> capturadas; // este conjunto ira guardar todas as pecas capturadas
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; } // a partida ira armazenar a peca que for movida tornando-a vuneravel a jogada En passant

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino) // metodo para executar um movimento de uma posicao inicial para outra posicao (final)
        {
            Peca p = tab.retirarPeca(origem); // tirar a peca de onde ela esta
            p.incrementarQtdeMovimentos(); // incrementar movimentos
            Peca pecaCapturada = tab.retirarPeca(destino); // captura e retira a peca, caso tenha no indice
            tab.colocarPeca(p, destino); // coloca peca na posicao de destino
            if (pecaCapturada != null) // testar se tinha uma peca na posicao de destino
            {
                capturadas.Add(pecaCapturada); // insere essa peca no conjunto das pecas capturadas 
            }

            // #Jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) //se a peca e rei e se o destino da coluna for igual a origem na coluna
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT); // tira a torre de onde estava
                T.incrementarQtdeMovimentos();
                tab.colocarPeca(T, destinoT); // coloca na posicao indicada
            }

            // #Jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT); // tira a torre de onde estava
                T.incrementarQtdeMovimentos();
                tab.colocarPeca(T, destinoT); // coloca na posicao indicada
            }

            // #Jogadaespecial En Passant
            if (p is Peao) // se a peca p for um Peao
            {
                if (origem.coluna != destino.coluna && pecaCapturada == null) // se mexeu na diagonal e a mecanica geral de peca capturada nao for usada, foi realizado o en passant
                {
                    Posicao posP; // posicao do peao
                    if (p.cor == Cor.Branca)
                    {
                        posP = new Posicao(destino.linha + 1, destino.coluna); // logica para capturar o peao Preto
                    }
                    else
                    {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retirarPeca(posP); // guardar a peca capturada 
                    capturadas.Add(pecaCapturada); // adcionar no conjunto de pecas capturadas
                }
            }

            return pecaCapturada;
        }
        

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQtdeMovimentos();
            if (pecaCapturada != null) // se houve uma peca capturada 
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem); //recoloca a peca p no local de origem

            // #Jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) //se a peca e rei e se o destino da coluna for igual a origem na coluna
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT); // tira a torre de onde estava
                T.decrementarQtdeMovimentos();
                tab.colocarPeca(T, origemT); // coloca na posicao indicada
            }

            // #Jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT); // tira a torre de onde estava
                T.decrementarQtdeMovimentos();
                tab.colocarPeca(T, origemT); // coloca na posicao indicada
            }

            // #Jogadaespecial En Passant
            if (p is Peao)
            {
                if (origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posP;
                    if (p.cor == Cor.Branca) // testar se foi um peao branco que mexeu
                    {
                        posP = new Posicao(3, destino.coluna); // volta para a posicao original
                    }
                    else // caso tenha sido um peao das Pretas
                    {
                        posP = new Posicao(4, destino.coluna); // volta para a posicao original
                    }
                    tab.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino); // executa o movimento

            if (estaEmXeque(jogadorAtual)) // se o movimento te faz entrar em xeque
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }
            Peca p = tab.peca(destino); // qual peca foi movida

            // #Jogadaespeical Promocao
            if(p is Peao)
            {
                if((p.cor == Cor.Branca && destino.linha == 0) || (p.cor == Cor.Preta && destino.linha == 7)) // testa se o peao branco ou o peao das pretas chegarem no limite de linhas respectivas
                {
                    p = tab.retirarPeca(destino); // retira a peca
                    pecas.Remove(p); // remove a peca do tabuleiro
                    Peca dama = new Dama(tab, p.cor); // instancia uma nova dama, automaticamente
                    tab.colocarPeca(dama, destino); // coloca a peca na posicao de destino
                    pecas.Add(dama); // adciona a dama ao tabuleiro
                }
            }

            if (estaEmXeque(adversaria(jogadorAtual))) // se o adversario estiver em xeque
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }

            if (testeXequeMate(adversaria(jogadorAtual))) // se meu adversario esta em xeque mate
            {
                terminada = true;
            }
            else
            {
                turno++;
                mudaJogador();
            }

            // #Jogadaespecial En Passant
            if (p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2)) //testando se a peca que foi movida e um peao e ela andou 2 linhas a mais ou a menos
            {
                vulneravelEnPassant = p; // essa peca esta vuneravel a tomar um en passant no segundo turno
            }
            else
            {
                vulneravelEnPassant = null; // caso contrario, ninguem esta vuneravel para en passant
            }

        }

        public void validarPosicaoDeOrigem(Posicao pos) // verificar se na origem existe peca para movimentar
        {
            if (tab.peca(pos) == null) // significa que não tem peca nessa posicao
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (jogadorAtual != tab.peca(pos).cor) // verificar se a peca e da cor do jogador atual
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis()) // verificar se nao existe movimentos possiveis para a peca escolhida
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino) // verifica se o destino e valido
        {
            if (!tab.peca(origem).movimentoPossivel(destino)) // testa se a peca de origem nao pode mover para a posicao destino
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        private void mudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) // metodo para informar quais as pecas capturadas brancas
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas) // percorrer todo o conjunto de pecas capturadas 
            {
                if (x.cor == cor) // se a peca for da cor informada faca
                {
                    aux.Add(x); // adcionar a peca no conjunto aux
                }
            }
            return aux; // retorna todas as pecas capturadas apenas da cor que o usuario informar
        }

        public HashSet<Peca> pecasEmJogo(Cor cor) // informar pecas em jogo de uma determinada cor
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas) // percorrer todo o conjunto de pecas em jogo 
            {
                if (x.cor == cor) // se a peca for da cor informada faca
                {
                    aux.Add(x); // adcionar a peca no conjunto aux
                }
            }
            aux.ExceptWith(pecasCapturadas(cor)); // retirar  todas as pecas capturadas dessa cor, para saber quais ainda estao em jogo
            return aux;
        }

        private Cor adversaria(Cor cor) // metodo para descobrir quem e o adversario de uma cor dada pelo usuario
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca rei(Cor cor) // devolver o rei de uma dada cor
        {
            foreach (Peca x in pecasEmJogo(cor))
            {
                if (x is Rei) // se o x for uma instancia de Rei
                {
                    return x; // retornar o Rei daquela cor, se for true
                }
            }
            return null; // se esgotar o for e nao tiver Rei
        }

        public bool estaEmXeque(Cor cor) // metodo para testar se o Rei de uma determinada cor esta em xeque
        {
            Peca R = rei(cor);
            if (R == null)
            {
                throw new TabuleiroException("Não tem Rei da cor " + cor + " no tabuleiro!");
            }

            foreach (Peca x in pecasEmJogo(adversaria(cor))) // para cada peca em jogo da cor adversaria
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna]) // se na posicao onde estiver o rei for verdeira, significa que estou em xeque
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequeMate(Cor cor) // testar se o rei de uma certa cor esta em xequemate
        {
            if (!estaEmXeque(cor)) // se nao esta em xeque
            {
                return false; // nao esta em xequemate
            }
            foreach (Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis(); // pegar a matriz de movimentos possiveis da peca x
                for (int i = 0; i < tab.linhas; i++)
                {
                    for (int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino); // faz o movimento
                            bool testeXeque = estaEmXeque(cor); // testa se ainda esta em xeque
                            desfazMovimento(origem, destino, pecaCapturada); // desfaz o movimento
                            if (!testeXeque) // se nao estiver mais em xeque, existe um movimento que tira do xeque
                            {
                                return false; // nao esta em xeque mate
                            }
                        }
                    }
                }
            }
            return true; // se mesmo com todos os movimentos o rei ainda esta em xeque, o jogador atual perdeu
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) // colocar no tabuleiro a peca em uma posicao determinada
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca); // adciona a peca no conjunto pra dizer que ela faz parte da partida
        }

        private void colocarPecas()
        {
            colocarNovaPeca('a', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branca, this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('a', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.Branca, this));

            colocarNovaPeca('a', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preta, this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('a', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.Preta, this));

        }
    }
}
