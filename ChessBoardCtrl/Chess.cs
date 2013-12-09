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

namespace ChessLibrary
{
  /// <summary>
  /// Defines some basic constants and conversion routines.
  /// </summary>
  public class Chess
  {
    public enum Pieces
    {
      NONE, WKING, WQUEEN, WROOK, WBISHOP, WKNIGHT, WPAWN,
      BKING, BQUEEN, BROOK, BBISHOP, BKNIGHT, BPAWN, OPENHAND,CLOSEDHAND 
    };
    public enum Operation
    {
      NONE, MOVE, ADD, DELETE
    };

    public static Pieces pieceFromFEN(char piece)
    {
      Pieces aPiece=Chess.Pieces.NONE;

      switch(piece)
      {
        case 'K':
          aPiece = Chess.Pieces.WKING;
          break;
        case 'Q':
          aPiece = Chess.Pieces.WQUEEN;
          break;
        case 'R':
          aPiece = Chess.Pieces.WROOK;
          break;
        case 'B':
          aPiece = Chess.Pieces.WBISHOP;
          break;
        case 'N':
          aPiece = Chess.Pieces.WKNIGHT;
          break;
        case 'P':
          aPiece = Chess.Pieces.WPAWN;
          break;
        case 'k':
          aPiece = Chess.Pieces.BKING;
          break;
        case 'q':
          aPiece = Chess.Pieces.BQUEEN;
          break;
        case 'r':
          aPiece = Chess.Pieces.BROOK;
          break;
        case 'b':
          aPiece = Chess.Pieces.BBISHOP;
          break;
        case 'n':
          aPiece = Chess.Pieces.BKNIGHT;
          break;
        case 'p':
          aPiece = Chess.Pieces.BPAWN;
          break;
      }    
      return aPiece;
    }  
    public static char pieceToNotation(Chess.Pieces piece)
    {
      char aPiece=' ';
      switch(piece)
      {
        case Chess.Pieces.WKING:
        case Chess.Pieces.BKING:                                 
          aPiece = 'K';
          break;
        case Chess.Pieces.WQUEEN:
        case Chess.Pieces.BQUEEN:          
          aPiece = 'Q';
          break;
        case Chess.Pieces.WROOK:
        case Chess.Pieces.BROOK:
          aPiece = 'R';
          break;
        case Chess.Pieces.WBISHOP:
        case Chess.Pieces.BBISHOP:
          aPiece = 'B';
          break;
        case Chess.Pieces.WKNIGHT:
        case Chess.Pieces.BKNIGHT:          
          aPiece = 'N' ;
          break;
        case Chess.Pieces.WPAWN:
        case Chess.Pieces.BPAWN:
          aPiece = 'P';
          break;
      }    
      return aPiece;
    }  
  }
}
