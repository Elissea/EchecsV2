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
using System.Drawing;
//using ChessLibrary;
namespace ChessLibrary
{
	/// <summary>
	/// Summary description for BoardSquare.
	/// </summary>
	public class BoardSquare
	{
    public Bitmap Background
    {
      get{ return coBackground; }
      set{ coBackground = value; }
    }
    private Bitmap coBackground;
    
    public Bitmap PieceImage
    {
      get{ return coPieceImage; }
      set{ coPieceImage = value; }
    }
    private Bitmap coPieceImage;
    
    public Rectangle Square
    {
      get{ return coSquare;}
      set{ coSquare = value;}
    }
    private Rectangle coSquare;

    public Chess.Pieces  Piece
    {
      get{ return coPiece;}
      set{ coPiece=value;}
    }
    private Chess.Pieces coPiece;
    public string Name
    {
      get{ return coName; }
      set{ coName = value; }
    }
    private string coName;

    public BoardSquare()
    {
      coBackground = null;      
      coPieceImage = null;
    }

		public BoardSquare(Bitmap oBackground, Rectangle oRectangle)
		{
      coBackground = oBackground;
      coSquare = oRectangle;
    }
	}
}
