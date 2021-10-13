﻿using System;
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

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
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
            if(pecaCapturada != null) // testar se tinha uma peca na posicao de destino
            {
                capturadas.Add(pecaCapturada); // insere essa peca no conjunto das pecas capturadas 
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQtdeMovimentos();
            if(pecaCapturada != null) // se houve uma peca capturada 
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem); //recoloca a peca p no local de origem
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino); // executa o movimento

            if (estaEmXeque(jogadorAtual)) // se o movimento te faz entrar em xeque
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
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
        }

        public void validarPosicaoDeOrigem(Posicao pos) // verificar se na origem existe peca para movimentar
        {
            if(tab.peca(pos) == null) // significa que não tem peca nessa posicao
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if(jogadorAtual != tab.peca(pos).cor) // verificar se a peca e da cor do jogador atual
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
            if(jogadorAtual == Cor.Branca)
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
            foreach(Peca x in capturadas) // percorrer todo o conjunto de pecas capturadas 
            {
                if(x.cor == cor) // se a peca for da cor informada faca
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
            if(cor == Cor.Branca)
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
            foreach(Peca x in pecasEmJogo(cor)){
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

            foreach(Peca x in pecasEmJogo(adversaria(cor))) // para cada peca em jogo da cor adversaria
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
            foreach(Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis(); // pegar a matriz de movimentos possiveis da peca x
                for(int i=0; i<tab.linhas; i++)
                {
                    for(int j=0; j<tab.colunas; j++)
                    {
                        if(mat[i, j])
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
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branca));
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
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preta));
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
