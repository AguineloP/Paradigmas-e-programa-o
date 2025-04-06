Imports System
Imports System.Collections.Generic
Imports System.Linq

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

        If Math.Abs(deltaLinha) = 2 AndAlso Math.Abs(deltaColuna) = 2 Then
            Dim linhaCaptura = Me.Linha + deltaLinha \ 2
            Dim colunaCaptura = Me.Coluna + deltaColuna \ 2

            Dim pecaCapturada = tabuleiro(linhaCaptura, colunaCaptura)

            If pecaCapturada IsNot Nothing AndAlso pecaCapturada.Cor <> Me.Cor Then
                tabuleiro(linhaCaptura, colunaCaptura) = Nothing
            Else
                Console.WriteLine("sem peça inimiga na diagonal!")
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

Module Program
    Dim tabuleiro(7, 7) As Peca
    Dim turnoAtual As CorPeca = CorPeca.Branca

    Sub Main()
        InicializarTabuleiro()

        While True
			Console.WriteLine($"turno: {turnoAtual}")
			Console.WriteLine("digite a posição da peça e o destino (L,C->L,C):")
            Dim entrada As String = Console.ReadLine()?.Trim()

            If String.IsNullOrWhiteSpace(entrada) Then
							Console.WriteLine("fechando jogo")
                Exit While
            End If

            Dim partes() As String = entrada.Split(New String() {"->"}, StringSplitOptions.None)

            If partes.Length <> 2 Then
								Console.WriteLine("entrada inválida")
                Continue While
            End If

            Dim origemCoords() As String = partes(0).Split(","c)
            Dim destinoCoords() As String = partes(1).Split(","c)

            If origemCoords.Length <> 2 OrElse destinoCoords.Length <> 2 Then
                Console.WriteLine("coordenadas inválidas")
                Continue While
            End If

            Dim origemLinha As Integer = Integer.Parse(origemCoords(0))
            Dim origemColuna As Integer = Integer.Parse(origemCoords(1))
            Dim destinoLinha As Integer = Integer.Parse(destinoCoords(0))
            Dim destinoColuna As Integer = Integer.Parse(destinoCoords(1))

            If origemLinha < 0 OrElse origemLinha > 7 OrElse origemColuna < 0 OrElse origemColuna > 7 Then
				Console.WriteLine("posição fora do tabuleiro")
                Continue While
            End If

            Dim peca As Peca = tabuleiro(origemLinha, origemColuna)

            If peca Is Nothing Then
				Console.WriteLine("não há peça nessa posição")
                Continue While
            End If

            If peca.Cor <> turnoAtual Then
                Console.WriteLine($"não é o turno das peças {peca.Cor}")
                Continue While
            End If

            Dim movimentos = peca.MovimentosPossiveis(tabuleiro)

            Dim movimentoValido As Boolean = movimentos.Any(Function(m) m.Item1 = destinoLinha AndAlso m.Item2 = destinoColuna)

            If movimentoValido Then
                peca.MoverPara(destinoLinha, destinoColuna, tabuleiro)
                Console.WriteLine("movimento realizado")

                turnoAtual = If(turnoAtual = CorPeca.Branca, CorPeca.Preta, CorPeca.Branca)
            Else
                Console.WriteLine("movimento inválido")
            End If
        End While
    End Sub

    Sub InicializarTabuleiro()
        For linha As Integer = 0 To 7
            For coluna As Integer = 0 To 7
                tabuleiro(linha, coluna) = Nothing
            Next
        Next

        For i As Integer = 0 To 2
            For j As Integer = 0 To 7
                If (i + j) Mod 2 = 1 Then
                    tabuleiro(i, j) = New Peca(i, j, CorPeca.Preta)
                End If
            Next
        Next

        For i As Integer = 5 To 7
            For j As Integer = 0 To 7
                If (i + j) Mod 2 = 1 Then
                    tabuleiro(i, j) = New Peca(i, j, CorPeca.Branca)
                End If
            Next
        Next
    End Sub
End Module
