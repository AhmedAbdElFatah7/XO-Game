using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XO
{
    public partial class Form1 : Form
    {
        private const int GRID_SIZE = 3;
        private Button[,] buttons;
        private char[,] board;
        private char currentPlayer;
        private int depthLimit = 3; // Depth limit for the Minimax algorithm

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            buttons = new Button[GRID_SIZE, GRID_SIZE];
            board = new char[GRID_SIZE, GRID_SIZE];
            currentPlayer = 'X';

            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    buttons[row, col] = new Button
                    {
                        Width = 60,
                        Height = 60,
                        Top = row * 60,
                        Left = col * 60,
                        Tag = new Tuple<int, int>(row, col)
                    };
                    buttons[row, col].Click += Button_Click;

                    Controls.Add(buttons[row, col]);

                    board[row, col] = ' ';
                }
            }

            Button newGameButton = new Button
            {
                Text = "New Game",
                Width = 100,
                Height = 30,
                Top = GRID_SIZE * 60 + 10,
                Left = 10
            };

            newGameButton.Click += NewGameButton_Click;
            Controls.Add(newGameButton);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Tuple<int, int> position = (Tuple<int, int>)button.Tag;
            int row = position.Item1;
            int col = position.Item2;

            if (board[row, col] == ' ' && !IsGameEnded())
            {
                board[row, col] = currentPlayer;
                button.Text = currentPlayer.ToString();

                if (CheckForWin(currentPlayer))
                {
                    MessageBox.Show($"{currentPlayer} wins!");
                    InitializeGame();
                }
                else if (IsBoardFull())
                {
                    MessageBox.Show("Draw!");
                    InitializeGame();
                }
                else // Change the current player
                {
                    currentPlayer = (currentPlayer == 'X') ? 'O' : 'X'; // Switch player
                    if (currentPlayer == 'O')
                    {
                        Tuple<int, int> computerMove = MiniMax(board, currentPlayer, depthLimit);
                        MakeMove(computerMove.Item1, computerMove.Item2);
                    }
                }
            }
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            foreach (Button button in buttons)
            {
                Controls.Remove(button); // Remove all buttons from the form
            }
            InitializeGame(); // Reinitialize the game
        }

        private bool IsGameEnded()
        {
            return CheckForWin('X') || CheckForWin('O') || IsBoardFull();
        }

        private bool CheckForWin(char player)
        {
            // Check rows and columns
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if ((board[i, 0] == player && board[i, 1] == player && board[i, 2] == player) ||
                    (board[0, i] == player && board[1, i] == player && board[2, i] == player))
                {
                    return true;
                }
            }

            // Check diagonals
            if ((board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) ||
                (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player))
            {
                return true;
            }

            return false;
        }

        private bool IsBoardFull()
        {
            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (board[row, col] == ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void MakeMove(int row, int col)
        {
            board[row, col] = 'O'; // Assuming computer is always 'O'
            buttons[row, col].Text = "O";

            if (CheckForWin('O'))
            {
                MessageBox.Show("Computer wins!");
                InitializeGame();
            }
            else if (IsBoardFull())
            {
                MessageBox.Show("Draw!");
                InitializeGame();
            }
            else
            {
                currentPlayer = 'X'; // Switch back to human player
            }
        }

        private Tuple<int, int> MiniMax(char[,] currentBoard, char player, int depth)
        {
            List<Tuple<int, int>> possibleMoves = GetPossibleMoves(currentBoard);
            Tuple<int, int> bestMove = null;
            int bestScore = int.MinValue;

            foreach (Tuple<int, int> move in possibleMoves)
            {
                char[,] newBoard = CloneBoard(currentBoard);
                newBoard[move.Item1, move.Item2] = player;

                int score = MiniMaxScore(newBoard, player, depth - 1, false);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int MiniMaxScore(char[,] currentBoard, char currentPlayer, int depth, bool isMaximizingPlayer)
        {
            if (depth == 0 || IsGameEnded())
            {
                return EvaluateBoard(currentBoard);
            }

            List<Tuple<int, int>> possibleMoves = GetPossibleMoves(currentBoard);

            if (isMaximizingPlayer)
            {
                int bestScore = int.MinValue;

                foreach (Tuple<int, int> move in possibleMoves)
                {
                    char[,] newBoard = CloneBoard(currentBoard);
                    newBoard[move.Item1, move.Item2] = currentPlayer;

                    int score = MiniMaxScore(newBoard, currentPlayer == 'X' ? 'O' : 'X', depth - 1, false);

                    bestScore = Math.Max(bestScore, score);
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                foreach (Tuple<int, int> move in possibleMoves)
                {
                    char[,] newBoard = CloneBoard(currentBoard);
                    newBoard[move.Item1, move.Item2] = currentPlayer;

                    int score = MiniMaxScore(newBoard, currentPlayer == 'X' ? 'O' : 'X', depth - 1, true);

                    bestScore = Math.Min(bestScore, score);
                }

                return bestScore;
            }
        }

        private List<Tuple<int, int>> GetPossibleMoves(char[,] currentBoard)
        {
            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();

            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (currentBoard[row, col] == ' ')
                    {
                        moves.Add(new Tuple<int, int>(row, col));
                    }
                }
            }

            return moves;
        }

        private int EvaluateBoard(char[,] currentBoard)
        {
            if (CheckForWin('O'))
            {
                return 1;
            }
            else if (CheckForWin('X'))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private char[,] CloneBoard(char[,] currentBoard)
        {
            char[,] clone = new char[GRID_SIZE, GRID_SIZE];

            for (int row = 0; row < GRID_SIZE; row++)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    clone[row, col] = currentBoard[row, col];
                }
            }

            return clone;
        }
    }
}