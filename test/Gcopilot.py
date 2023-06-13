"import all necessary libraries"
from tkinter import *

"A class that plays a game of tic tac toe"
class GameError(Exception):
    pass

class TicTacToe:
    def __init__(self):
        self._board = [[None]*3 for _ in range(3)]
        self._player = 'X'
        self._winner = None
        self._moves = 0
        self._game_over = False

    def play(self, row, col):
        if self._game_over:
            raise GameError("Game is over")
        if not (0 <= row < 3):
            raise GameError(f"Invalid row {row}")
        if not (0 <= col < 3):
            raise GameError(f"Invalid column {col}")
        if self._board[row][col] is not None:
            raise GameError(f"Position ({row},{col}) already played")
        self._board[row][col] = self._player
        self._moves += 1
        self._check_for_winner()
        if not self._winner:
            self._player = 'X' if self._player == 'O' else 'O'
        if self._moves == 9 or self._winner:
            self._game_over = True

    def _check_for_winner(self):
        for i in range(3):
            if self._board[i][0] == self._board[i][1] == self._board[i][2]:
                self._winner = self._board[i][0]
                return
            if self._board[0][i] == self._board[1][i] == self._board[2][i]:
                self._winner = self._board[0][i]
                return
        if self._board[0][0] == self._board[1][1] == self._board[2][2]:
            self._winner = self._board[0][0]
            return
        if self._board[0][2] == self._board[1][1] == self._board[2][0]:
            self._winner = self._board[0][2]
            return

    @property
    def game_over(self):
        return self._game_over

    @property
    def winner(self):
        return self._winner

    @property
    def player(self):
        return self._player

    def __str__(self):
        return f"{self._board[0]}\n{self._board[1]}\n{self._board[2]}"
    

"create a class to run a tkinter GUI to lpay the game"

class GUI:
    def __init__(self):
        self._root = Tk()
        self._root.title("Tic Tac Toe")
        self._game = TicTacToe()
        self._buttons = [[None]*3 for _ in range(3)]
        for row in range(3):
            for col in range(3):
                handler = lambda r=row, c=col: self._play(r,c)
                button = Button(self._root, command=handler, font=("Arial", 60), width=2, height=1)
                button.grid(row=row, column=col)
                self._buttons[row][col] = button
        handler = lambda: self._reset()
        button = Button(self._root, text="New Game", command=handler)
        button.grid(row=3, column=0, columnspan=3, sticky="WE")
        self._message = Label(self._root, text="X's turn to play")
        self._message.grid(row=4, column=0, columnspan=3)
        self._root.mainloop()

    def _play(self, row, col):
        try:
            self._game.play(row, col)
            self._buttons[row][col]["text"] = self._game.player
            if self._game.game_over:
                if self._game.winner:
                    self._message["text"] = f"{self._game.winner} wins!"
                else:
                    self._message["text"] = "Draw"
            else:
                self._message["text"] = f"{self._game.player}'s turn to play"
        except GameError as e:
            self._message["text"] = e

    def _reset(self):
        self._game = TicTacToe()
        for row in range(3):
            for col in range(3):
                self._buttons[row][col]["text"] = ""
        self._message["text"] = "X's turn to play"

    

if __name__ == "__main__":
    GUI()