"import all modules"
import tkinter
import copy
from tkinter import messagebox

class GameError(Exception):
    pass


"class to load and play a game of four in a row!"
class FourInARow:
    def __init__(self):
        self._board = [[' ' for _ in range(7)] for _ in range(6)]
        self._player = 'X'
        self._winner = None
        self._draw = False

    def play(self, col):
        if self._winner is not None:
            raise GameError("Game is over")
        if self._board[0][col] != ' ':
            raise GameError("Invalid move")
        row = 5
        while self._board[row][col] != ' ':
            row -= 1
        self._board[row][col] = self._player
        if self._winner is None:
            self._player = 'O' if self._player == 'X' else 'X'
        self._check_for_winner()

    def at(self, row, col):
        return self._board[row][col]
    
    def winner(self):
        return self._winner
    
    def draw(self):
        return self._draw
    
    def _check_for_winner(self):
        for player in ['X', 'O']:
            for row in range(6):
                if all(self._board[row][col] == player for col in range(7)):
                    self._winner = player
            for col in range(7):
                if all(self._board[row][col] == player for row in range(6)):
                    self._winner = player
            for row in range(3):
                for col in range(4):
                    if all(self._board[row + i][col + i] == player for i in range(4)):
                        self._winner = player
            for row in range(3):
                for col in range(4):
                    if all(self._board[row + 3 - i][col + i] == player for i in range(4)):
                        self._winner = player
        if self._winner is None:
            self._draw = all(self._board[row][col] != ' ' for row in range(6) for col in range(7))

    def __str__(self):
        return '\n'.join(['|'.join(self._board[row]) for row in range(6)])
    
    def __repr__(self):
        return f"FourInARow({self._board},{self._player},{self._winner},{self._draw})"
    
    def __eq__(self,other):
        return self._board == other._board and self._player == other._player and self._winner == other._winner and self._draw == other._draw
    

"class for tkinter GUI that loads and plays the game of four in a row!"
class FourInARowGUI:
    def __init__(self, game):
        self._game = game
        self._root = tkinter.Tk()
        self._root.title("Four in a Row")
        self._canvas = tkinter.Canvas(master = self._root, width = 700, height = 600, background = '#00ff00')
        self._canvas.grid(row = 0, column = 0, padx = 10, pady = 10, sticky = tkinter.N + tkinter.S + tkinter.W + tkinter.E)
        self._canvas.bind('<Configure>', self._on_canvas_resized)
        self._canvas.bind('<Button-1>', self._on_canvas_clicked)
        self._root.rowconfigure(0, weight = 1)
        self._root.columnconfigure(0, weight = 1)
        self._root.update()
        self._draw_board()

    def start(self):
        self._root.mainloop()
        
    def _on_canvas_resized(self, event: tkinter.Event):
        self._draw_board()

    def _on_canvas_clicked(self, event: tkinter.Event):
        if self._game.winner() is None:
            col = int(event.x / (self._canvas.winfo_width() / 7))
            try:
                self._game.play(col)
            except:
                pass
            self._draw_board()

    def _draw_board(self):
        self._canvas.delete(tkinter.ALL)
        canvas_width = self._canvas.winfo_width()
        canvas_height = self._canvas.winfo_height()
        for col in range(1, 7):
            self._canvas.create_line(col * canvas_width / 7, 0, col * canvas_width / 7, canvas_height)
        for row in range(1, 6):
            self._canvas.create_line(0, row * canvas_height / 6, canvas_width, row * canvas_height / 6)
        for row in range(6):
            for col in range(7):
                if self._game.at(row, col) == 'X':
                    self._canvas.create_oval(col * canvas_width / 7 + 10, row * canvas_height / 6 + 10, (col + 1) * canvas_width / 7 - 10, (row + 1) * canvas_height / 6 - 10, fill = '#ff0000')
                elif self._game.at(row, col) == 'O':
                    self._canvas.create_oval(col * canvas_width / 7 + 10, row * canvas_height / 6 + 10, (col + 1) * canvas_width / 7 - 10, (row + 1) * canvas_height / 6 - 10, fill = '#ffff00')
        if self._game.winner() is not None:
            self._canvas.create_text(canvas_width / 2, canvas_height / 2, text = f"{self._game.winner()} wins!", font = ('Helvetica', 32))
        elif self._game.draw():
            self._canvas.create_text(canvas_width / 2, canvas_height / 2, text = f"Draw!", font = ('Helvetica', 32))
        self._root.update()

    
if __name__ == "__main__":
    game = FourInARow()
    gui = FourInARowGUI(game)
    gui.start()