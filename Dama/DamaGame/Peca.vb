Public Enum CorPeca
    Nenhuma
    Branca
    Preta
End Enum

Public Class Peca
    Public Property Linha As Integer
    Public Property Coluna As Integer
    Public Property Cor As CorPeca

    Public Sub New(linha As Integer, coluna As Integer, cor As CorPeca)
        Me.Linha = linha
        Me.Coluna = coluna
        Me.Cor = cor
    End Sub

    Public Sub MoverPara(novaLinha As Integer, novaColuna As Integer, tabuleiro(,) As Peca)
        Dim deltaLinha = novaLinha - Me.Linha
        Dim deltaColuna = novaColuna - Me.Coluna

        ' Verifica se é captura
        If Math.Abs(deltaLinha) = 2 AndAlso Math.Abs(deltaColuna) = 2 Then
            Dim linhaCaptura = Me.Linha + deltaLinha \ 2
            Dim colunaCaptura = Me.Coluna + deltaColuna \ 2

            Dim pecaCapturada = tabuleiro(linhaCaptura, colunaCaptura)
            If pecaCapturada IsNot Nothing AndAlso pecaCapturada.Cor <> Me.Cor Then
                tabuleiro(linhaCaptura, colunaCaptura) = Nothing
            Else
                MessageBox.Show("Sem peça inimiga na diagonal!")
                Return
            End If
        End If

        tabuleiro(Me.Linha, Me.Coluna) = Nothing
        Me.Linha = novaLinha
        Me.Coluna = novaColuna
        tabuleiro(novaLinha, novaColuna) = Me
    End Sub

    Public Function MovimentosPossiveis(tabuleiro(,) As Peca) As List(Of Tuple(Of Integer, Integer))
        Dim movimentos As New List(Of Tuple(Of Integer, Integer))()

        Dim direcao As Integer = If(Me.Cor = CorPeca.Branca, -1, 1)
        Dim linhas = tabuleiro.GetLength(0)
        Dim colunas = tabuleiro.GetLength(1)

        Dim deltaLinha = direcao
        For Each deltaColuna In {-1, 1}
            Dim novaLinha = Me.Linha + deltaLinha
            Dim novaColuna = Me.Coluna + deltaColuna

            If novaLinha >= 0 AndAlso novaLinha < linhas AndAlso
               novaColuna >= 0 AndAlso novaColuna < colunas AndAlso
               tabuleiro(novaLinha, novaColuna) Is Nothing Then

                movimentos.Add(Tuple.Create(novaLinha, novaColuna))

            ElseIf novaLinha + direcao >= 0 AndAlso novaLinha + direcao < linhas AndAlso
                   novaColuna + deltaColuna >= 0 AndAlso novaColuna + deltaColuna < colunas Then

                Dim pecaInimiga = tabuleiro(novaLinha, novaColuna)
                If pecaInimiga IsNot Nothing AndAlso pecaInimiga.Cor <> Me.Cor AndAlso
                   tabuleiro(novaLinha + direcao, novaColuna + deltaColuna) Is Nothing Then

                    movimentos.Add(Tuple.Create(novaLinha + direcao, novaColuna + deltaColuna))
                End If
            End If
        Next

        Return movimentos
    End Function
End Class

