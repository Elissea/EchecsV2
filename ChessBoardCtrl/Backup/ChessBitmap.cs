/*
                 \\|//
                 (o o)
--------------ooO-(_)-Ooo--------------
 Copyright 2004 By Gregory A. Prentice
                      Ooo.
--------------.ooO----(  )-------------
              (  )    (_/
               \_)
If you wish to use this code in any part
I request that you simply let me know where
and give the author credit for his work.
gregoryprentice@comcast.net
www.cafechess.org
*/
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace ChessLibrary
{
  /// <summary>
  /// Object handles the drawing and manipulations of a chess board.
  /// </summary>
  public class ChessBitmap : IPositionEvents
  {
    //public System.Windows.Forms.ImageList 
    private ImageList imageSquares;
    private ImageList imagePieces;
    public Hashtable coChessBoard;
    public Bitmap coBmpBoard;
    public bool isFlipped;
    public bool isRedraw;

    public Rectangle ClientRectangle
    {
      set
      {
        coClient = value;
        coSquare = new Rectangle();
        if( coClient.Height > coClient.Width )
          coSquare.Height = coClient.Width;
        else
          coSquare.Height = coClient.Height;
        coSquare.Width = coSquare.Height / 8;
        coSquare.Height = coSquare.Width;
      }
      get{return coClient;}
    }
    Rectangle coClient;

    public Rectangle Square
    {
      //set{coSquare = value;}
      get{return coSquare;}
    }
    Rectangle coSquare;

    char [] mapcol = { 'a','b','c','d','e','f','g','h' };
    char [] maprow = { '1','2','3','4','5','6','7','8' };

    public ChessBitmap(ImageList Squares,ImageList Pieces)
    {
      imageSquares = Squares;
      imagePieces = Pieces;
      coChessBoard = new Hashtable();
    }

    /// <summary>
    /// Initialize our visual chess board.
    /// </summary>
    public void clearBoard()
    {
      //coBitBoard.newBoard()
      foreach(DictionaryEntry pb in coChessBoard)
      {
        BoardSquare sq = (BoardSquare) pb.Value;
        sq.Piece = Chess.Pieces.NONE;
        sq.PieceImage = null;
      }
    }

    public string getFEN()
    {
      System.Text.StringBuilder fen = new StringBuilder();
      int empty=0;
      for(int row=7;row>=0;row--)
      {
        if( empty > 0 )
        {
          fen.Append(empty.ToString());
          empty = 0;
        }
        if(row != 7)
          fen.Append('/');
        for(int col=0; col<8; col++)
        {
          string notation = mapcol[col].ToString()+maprow[row].ToString();
          BoardSquare sq = (BoardSquare) coChessBoard[notation];
          switch(sq.Piece)
          {
            case Chess.Pieces.BKING:
            case Chess.Pieces.BQUEEN:
            case Chess.Pieces.BROOK:
            case Chess.Pieces.BBISHOP:
            case Chess.Pieces.BKNIGHT:
            case Chess.Pieces.BPAWN:
            case Chess.Pieces.WKING:
            case Chess.Pieces.WQUEEN:
            case Chess.Pieces.WROOK:
            case Chess.Pieces.WBISHOP:
            case Chess.Pieces.WKNIGHT:
            case Chess.Pieces.WPAWN:
              if( empty > 0 )
              {
                fen.Append(empty.ToString());
                empty = 0;
              }
              switch(sq.Piece)
              {
                case Chess.Pieces.BKING:
                  fen.Append('k');
                  break;
                case Chess.Pieces.BQUEEN:
                  fen.Append('q');
                  break;
                case Chess.Pieces.BROOK:
                  fen.Append('r');
                  break;
                case Chess.Pieces.BBISHOP:
                  fen.Append('b');
                  break;
                case Chess.Pieces.BKNIGHT:
                  fen.Append('n');
                  break;
                case Chess.Pieces.BPAWN:
                  fen.Append('p');
                  break;
                case Chess.Pieces.WKING:
                  fen.Append('K');
                  break;
                case Chess.Pieces.WQUEEN:
                  fen.Append('Q');
                  break;
                case Chess.Pieces.WROOK:
                  fen.Append('R');
                  break;
                case Chess.Pieces.WBISHOP:
                  fen.Append('B');
                  break;
                case Chess.Pieces.WKNIGHT:
                  fen.Append('N');
                  break;
                case Chess.Pieces.WPAWN:
                  fen.Append('P');
                  break;
              }
              break;
            case Chess.Pieces.NONE:
              empty++;
              break;
          }
        }
      }
      if( empty > 0 )
      {
        fen.Append(empty.ToString());
        empty = 0;
      }
      return fen.ToString();
    }

    public void placePiece(Chess.Pieces piece,int square)
    {
      int col = square % 8;
      int row = square / 8;
      string notation = mapcol[col].ToString()+maprow[row].ToString();
      placePiece(notation, piece);
    }
    
    /// <summary>
    /// Given a x and y coordinate, locate the chess square which
    /// contains the given point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public BoardSquare findSquare(int x,int y)
    {
      BoardSquare sqReturn = null;
      foreach(DictionaryEntry pb in coChessBoard)
      {
        BoardSquare sq = (BoardSquare) pb.Value;
        if( sq.Square.Contains(x,y) )
        {
          sqReturn = sq;
          break;
        }
      }
      return sqReturn;
    }

		/// <summary>
		/// Given an algebraic coordinate, locate the corresponding chess square.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public BoardSquare findSquare(string square)
		{
			BoardSquare sqReturn = null;
			foreach(DictionaryEntry pb in coChessBoard)
			{
				BoardSquare sq = (BoardSquare) pb.Value;
				if( sq.Name == square )
				{
					sqReturn = sq;
					break;
				}
			}
			return sqReturn;
		}

    /// <summary>
    /// Initializes our chess board squares for proper color and algebraic
    /// notation based on orientation, i.e. white or black on bottom.
    /// </summary>
    public void initializeBoard()
    {
      int row,colm;

      Bitmap ws = (Bitmap) imageSquares.Images[0]; 
      Bitmap bs = (Bitmap) imageSquares.Images[1]; 
      bool toggle = true;
      Rectangle square = coSquare;

      BoardSquare brdsqu;
      int rowflip = isFlipped ? 0:7;
      try
      {
        string notation;
        for(row=0;row<8;row++)
        {
          int colflip = isFlipped ? 7:0;
          for(colm=0; colm<8; colm++)
          {
            notation = mapcol[colflip].ToString()+maprow[rowflip].ToString();
            if( coChessBoard.ContainsKey(notation )== false )
            {
              brdsqu = new BoardSquare();
              coChessBoard[notation] = brdsqu;
              brdsqu.Name = notation;
            }
            else
            {
              brdsqu = (BoardSquare) coChessBoard[notation];
            }

            brdsqu.Square = new Rectangle(square.X,square.Y,square.Width,square.Height);
            brdsqu.Background = toggle ? ws : bs; 
            square.X += square.Width;                
            toggle = !toggle;
            colflip = isFlipped ? colflip-1:colflip+1;
          }

          rowflip = isFlipped ? rowflip+1:rowflip-1;
          square.Y += square.Height;
          square.X = 0;
          toggle = !toggle;
        }
      }
      catch(Exception ex)
      {
        String msg = ex.ToString();
        msg.Trim();
      }
    }

    public void draw(System.Drawing.Graphics graphics)
    {      
      try
      {
        if( coBmpBoard == null || isRedraw == true )
        {
          if(coBmpBoard != null )
          {
            coBmpBoard.Dispose();
            coBmpBoard = null;
          }

          coBmpBoard = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, graphics);// this.CreateGraphics()); 
          Graphics offScreenDC = Graphics.FromImage(coBmpBoard);

          foreach(DictionaryEntry pb in coChessBoard)
          {
            BoardSquare sq = (BoardSquare) pb.Value;
            //graphics.DrawImage(sq.Background,sq.Square);
            offScreenDC.DrawImage(sq.Background,sq.Square);
            if( sq.Piece != Chess.Pieces.NONE && sq.PieceImage != null )
              //graphics.DrawImage(sq.PieceImage,sq.Square);
              offScreenDC.DrawImage(sq.PieceImage,sq.Square);
          }

          offScreenDC.Dispose();
          offScreenDC=null;
          isRedraw = false;
        }
        graphics.DrawImage(coBmpBoard, ClientRectangle);
      }
      catch(Exception ex)
      {
        ex.Message.ToLower();
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected void Dispose()
    {
      if(coBmpBoard != null )
      {
        coBmpBoard.Dispose();
        coBmpBoard = null;
      }
    }
    #region IPositionEvents Members
    
    public void placePiece(string notation, Chess.Pieces piece)
    {
      BoardSquare sq = (BoardSquare) coChessBoard[notation];
      sq.PieceImage = (Bitmap)imagePieces.Images[((int)piece - 1)];
      sq.Piece = piece;
    }
    public void setColor(bool bColor)
    {
      // Does nothing currently, just to satisfy the compiler.
    }

    public void setCastling(bool WK, bool WQ, bool BK, bool BQ)
    {
      // Does nothing currently, just to satisfy the compiler.
    }
    public void finished()
    {
      // Does nothing currently, just to satisfy the compiler.
    }
 

    #endregion
  }
}
