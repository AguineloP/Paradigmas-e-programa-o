Public Class Form1
    ' DECLARAÇÃO DAS VARIÁVEIS
    Dim tabuleiro(7, 7) As Peca ' Cria uma matriz 8x8
    Dim turnoAtual As CorPeca = CorPeca.Branca ' Define de quem é a vez de jogar
    Dim pecaSelecionada As Peca = Nothing ' Guarda a peça que o player clicou
    Dim origemLinha As Integer ' Armazena onde essa peça foi selecionada
    Dim origemColuna As Integer ' Armazena onde essa peça foi selecionada

    ' Variáveis para o tamanho do tabuleiro e área do xadrez com borda
    Dim larguraTabuleiroTotal As Integer = 1000       ' tamanho total da imagem do tabuleiro (com borda)
    Dim alturaTabuleiroTotal As Integer = 1000

    Dim larguraAreaTabuleiro As Double = 776.5         ' área do xadrez dentro do tabuleiro (sem borda)
    Dim alturaAreaTabuleiro As Double = 776.5

    Dim bordaX As Integer                              ' deslocamento da área do xadrez dentro da imagem (horizontal)
    Dim bordaY As Integer                              ' deslocamento da área do xadrez dentro da imagem (vertical)

    ' Variáveis para controlar o tamanho das casas e das peças
    Dim larguraTabuleiro As Integer = 825
    Dim alturaTabuleiro As Integer = 825
    Dim tamanhoCasa As Integer 'Tamanho da casa/quadrado
    Dim fatorTamanhoPeca As Double = 0.6 ' Fator para diminuir o tamanho das peças (60% do tamanho da casa)

    ' Imagens que serão usadas
    Dim imagemFundo As Image
    Dim imagemTabuleiro As Image
    Dim imagemPecaBranca As Image
    Dim imagemPecaPreta As Image

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'Evento que carrega tudo quando o programa é executado
        ' Definir a janela para abrir centralizada e maximizada
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.DoubleBuffered = True

        ' Carregar as imagens
        Try
            imagemFundo = Image.FromFile("ImagensDama\FundoPreto.png") ' Substitua pelo caminho da sua imagem de fundo
            imagemTabuleiro = Image.FromFile("ImagensDama\Tabuleiro.png")
            imagemPecaBranca = Image.FromFile("ImagensDama\PecaAntivirus.png")
            imagemPecaPreta = Image.FromFile("ImagensDama\PecaVirus.png")
        Catch ex As Exception
            MessageBox.Show("Erro ao carregar imagens: " & ex.Message)
            Me.Close()
            Return
        End Try

        bordaX = (larguraTabuleiroTotal - larguraAreaTabuleiro) \ 2
        bordaY = (alturaTabuleiroTotal - alturaAreaTabuleiro) \ 2

        InicializarTabuleiro()
    End Sub

    Private Sub InicializarTabuleiro()
        For linha As Integer = 0 To 7
            For coluna As Integer = 0 To 7
                tabuleiro(linha, coluna) = Nothing
            Next
        Next

        ' Peças pretas
        For i As Integer = 0 To 2
            For j As Integer = 0 To 7
                If (i + j) Mod 2 = 1 Then
                    tabuleiro(i, j) = New Peca(i, j, CorPeca.Preta)
                End If
            Next
        Next

        ' Peças brancas
        For i As Integer = 5 To 7
            For j As Integer = 0 To 7
                If (i + j) Mod 2 = 1 Then
                    tabuleiro(i, j) = New Peca(i, j, CorPeca.Branca)
                End If
            Next
        Next
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics

        ' 1. Desenha imagem de fundo na tela toda
        If imagemFundo IsNot Nothing Then
            g.DrawImage(imagemFundo, 0, 0, Me.ClientSize.Width, Me.ClientSize.Height)
        Else
            g.Clear(Color.DarkGreen) ' fallback se não tiver imagem
        End If

        ' 2. Cálculo do tamanho das casas e posição centralizada
        Dim margemX As Integer = (Me.ClientSize.Width - larguraTabuleiroTotal) \ 2  'Consigo ajustar a imagem horizontalmente
        Dim margemY As Integer = (Me.ClientSize.Height - alturaTabuleiroTotal) \ 2 + 1 'Consigo ajustar a imagem Verticalmente

        ' 3. Calcula o tamanho de cada casa baseado na área interna do xadrez (825x825)
        tamanhoCasa = larguraAreaTabuleiro \ 8

        ' 4. Desenha a imagem do tabuleiro completo (com borda)
        If imagemTabuleiro IsNot Nothing Then
            g.DrawImage(imagemTabuleiro, margemX, margemY, larguraTabuleiroTotal, alturaTabuleiroTotal)
        Else
            ' fallback: desenha tabuleiro quadriculado dentro da área do xadrez
            For linha As Integer = 0 To 7
                For coluna As Integer = 0 To 7
                    Dim x As Integer = margemX + coluna * tamanhoCasa
                    Dim y As Integer = margemY + linha * tamanhoCasa
                    If (linha + coluna) Mod 2 = 0 Then
                        g.FillRectangle(Brushes.Beige, x, y, tamanhoCasa, tamanhoCasa)
                    Else
                        g.FillRectangle(Brushes.Sienna, x, y, tamanhoCasa, tamanhoCasa)
                    End If
                Next
            Next
        End If

        ' 5. Desenha as peças centralizadas nas casas (área do xadrez), menores que as casas
        For linha As Integer = 0 To 7
            For coluna As Integer = 0 To 7
                Dim peca = tabuleiro(linha, coluna)
                If peca IsNot Nothing Then
                    Dim imagemPeca As Image = If(peca.Cor = CorPeca.Branca, imagemPecaBranca, imagemPecaPreta)
                    If imagemPeca IsNot Nothing Then
                        Dim xCasa As Integer = margemX + bordaX + coluna * tamanhoCasa
                        Dim yCasa As Integer = margemY + bordaY + linha * tamanhoCasa
                        Dim larguraPeca As Integer = CInt(tamanhoCasa * 0.85)  ' peça com 50% do tamanho da casa
                        Dim alturaPeca As Integer = CInt(tamanhoCasa * 0.85)
                        g.DrawImage(imagemPeca,
                                xCasa + (tamanhoCasa - larguraPeca) \ 2,
                                yCasa + (tamanhoCasa - alturaPeca) \ 2,
                                larguraPeca,
                                alturaPeca)
                    End If
                End If
            Next
        Next
    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        ' Calcula as margens para centralizar o tabuleiro (total, com borda)
        Dim margemX As Integer = (Me.ClientSize.Width - larguraTabuleiroTotal) \ 2  'Consigo ajustar o click horizontalmente
        Dim margemY As Integer = (Me.ClientSize.Height - alturaTabuleiroTotal) \ 2 + 1  'Consigo ajustar o click verticalmente

        ' Ajusta as coordenadas do clique para a área interna do tabuleiro (área do xadrez)
        Dim posX As Integer = e.X - margemX - bordaX
        Dim posY As Integer = e.Y - margemY - bordaY

        ' Calcula linha e coluna baseado na área do xadrez (825x825)
        Dim coluna As Integer = posX \ tamanhoCasa
        Dim linha As Integer = posY \ tamanhoCasa

        ' Ignora clique fora da área do tabuleiro
        If linha < 0 OrElse linha > 7 OrElse coluna < 0 OrElse coluna > 7 Then Return

        If pecaSelecionada Is Nothing Then
            ' Seleciona a peça somente se for do turno atual
            Dim peca = tabuleiro(linha, coluna)
            If peca IsNot Nothing AndAlso peca.Cor = turnoAtual Then
                pecaSelecionada = peca
                origemLinha = linha
                origemColuna = coluna
            End If
        Else
            ' Tenta mover a peça selecionada para a posição clicada
            Dim movimentos = pecaSelecionada.MovimentosPossiveis(tabuleiro)
            Dim movimentoValido = movimentos.Any(Function(m) m.Item1 = linha AndAlso m.Item2 = coluna)

            If movimentoValido Then
                pecaSelecionada.MoverPara(linha, coluna, tabuleiro)
                turnoAtual = If(turnoAtual = CorPeca.Branca, CorPeca.Preta, CorPeca.Branca)
            End If

            pecaSelecionada = Nothing
        End If

        ' Força o repaint da tela (redesenhar tudo)
        Me.Invalidate()
    End Sub
End Class
