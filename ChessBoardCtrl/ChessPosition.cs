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
  using Bitboard = System.UInt64;
  /// <summary>
  /// Defines the bitboard positions and special move availability for one color.
  /// Normally used in pairs, one for white and black.
  /// </summary>
  public class ChessPosition
  {
    public Bitboard Board;
    public Bitboard King;
    public Bitboard Queens;
    public Bitboard Rooks;
    public Bitboard Bishops;
    public Bitboard Knights;
    public Bitboard Pawns;
    public Bitboard RotatedR90;
    public Bitboard RotatedL90;
    public Bitboard RotatedA1H8;
    public Bitboard RotatedA8H1;
    public Bitboard Pinning;
    public Bitboard Pinned;
    public Bitboard Enpasant;
    public bool canCastleKingside;
    public bool canCastleQueenside;
    public bool Color;
    public bool isKingMated;
    public bool isKingInCheck;
    public bool isKingStaleMated;
    /// <summary>
    /// Standard constructor that requires you to specify the color the position represents.
    /// </summary>
    /// <param name="bColor"></param>
    public ChessPosition(bool bColor)
    {
      Color = bColor;
      newBoard();
    }
    /// <summary>
    /// A copy constructor used for cloning a position.
    /// </summary>
    /// <param name="position"></param>
    public ChessPosition(ChessPosition position)
    {
      copy(position);
    }
    /// <summary>
    /// Used to set one position equal to another.
    /// </summary>
    /// <param name="position"></param>
    public void copy(ChessPosition position)
    {
      Board=position.Board;
      King=position.King;
      Queens=position.Queens;
      Rooks=position.Rooks;
      Bishops=position.Bishops;
      Knights=position.Knights;
      Pawns=position.Pawns;
      RotatedR90=position.RotatedR90;
      RotatedL90=position.RotatedL90;
      RotatedA1H8=position.RotatedA1H8;
      RotatedA8H1=position.RotatedA8H1;
      Pinning=position.Pinning;
      Pinned=position.Pinned;
      Enpasant=position.Enpasant;
      canCastleKingside=position.canCastleKingside;
      canCastleQueenside=position.canCastleQueenside;      
      Color=position.Color;
      isKingInCheck=position.isKingInCheck;
      isKingMated=position.isKingMated;
      isKingStaleMated=position.isKingStaleMated;
    }

    /// <summary>
    /// Completely clears a position to an empty state.
    /// </summary>
    public void clearBoard()
    {
      Board=0;
      King=0;
      Queens=0;
      Rooks=0;
      Bishops=0;
      Knights=0;
      Pawns=0;
      RotatedR90=0;
      RotatedL90=0;
      RotatedA1H8=0;
      RotatedA8H1=0;
      Pinning=0;
      Pinned=0;
      Enpasant=0;
      canCastleKingside=true;
      canCastleQueenside=true;
      isKingMated=false;
      isKingInCheck=false;
      isKingStaleMated=false;

    }
    /// <summary>
    /// Sets up the default bitboards based on the color of the pieces.
    /// </summary>
    public void newBoard()
    {
      if( Color )
      {
        // White starting bitboards.
        Pawns=(Bitboard)0x000000000000FF00;
        Rooks=(Bitboard)0x0000000000000081;
        Knights=(Bitboard)0x0000000000000042;
        Bishops=(Bitboard)0x0000000000000024;
        Queens=(Bitboard)0x0000000000000010;
        King=(Bitboard) 0x0000000000000008;
        Board=(Bitboard)0x000000000000FFFF;
        RotatedR90=(Bitboard)0xC0C0C0C0C0C0C0C0;
        // Not correct.
        RotatedL90=(Bitboard)0xC0C0C0C0C0C0C0C0;;
        RotatedA1H8=(Bitboard)0x6030180C060381C0;
        RotatedA8H1=(Bitboard)0x03060C183060C081;
      }
      else
      {
        // Blacks starting bitboards.
        Pawns=(Bitboard)0x00FF000000000000;
        Rooks=(Bitboard)0x8100000000000000;
        Knights=(Bitboard)0x4200000000000000;
        Bishops=(Bitboard)0x2400000000000000;
        Queens=(Bitboard)0x1000000000000000;
        King=(Bitboard) 0x0800000000000000;
        Board=(Bitboard)0xFFFF000000000000;
        RotatedR90=(Bitboard)0x0303030303030303;
        // Not correct
        RotatedL90=(Bitboard)0x0303030303030303;
        RotatedA1H8=(Bitboard)0x81C06030180C0603;
        RotatedA8H1=(Bitboard)0xC08103060C183060;
      }
      Pinning=0x00;
      Pinned=0x00;
      Enpasant=0x00;
      canCastleKingside=true;
      canCastleQueenside=true;
      isKingMated=false;
      isKingInCheck=false;
      isKingStaleMated=false;
    }

    /// <summary>
    /// Adds the requested piece into the proper bitboards.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="mask"></param>
    public void addPiece(Chess.Pieces piece, Bitboard mask)
    {
      switch(piece)
      {
        case Chess.Pieces.WKING:
        case Chess.Pieces.BKING:
          this.King |= mask;
          break;
        case Chess.Pieces.WQUEEN:
        case Chess.Pieces.BQUEEN:
          this.Queens |= mask;
          break;
        case Chess.Pieces.WROOK:
        case Chess.Pieces.BROOK:
          this.Rooks |= mask;
          break;
        case Chess.Pieces.WBISHOP:
        case Chess.Pieces.BBISHOP:
          this.Bishops |= mask;
          break;
        case Chess.Pieces.WKNIGHT:
        case Chess.Pieces.BKNIGHT:
          this.Knights |= mask;
          break;
        case Chess.Pieces.WPAWN:
        case Chess.Pieces.BPAWN:
          this.Pawns |= mask;
          break;
      }
    }

    /// <summary>
    /// Removes the requested piece from the bitboards.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="mask"></param>
    public void removePiece(Chess.Pieces piece, Bitboard mask)
    {
      switch(piece)
      {
        case Chess.Pieces.WKING:
        case Chess.Pieces.BKING:
          this.King &= ~mask;
          break;
        case Chess.Pieces.WQUEEN:
        case Chess.Pieces.BQUEEN:
          this.Queens &= ~mask;
          break;
        case Chess.Pieces.WROOK:
        case Chess.Pieces.BROOK:
          this.Rooks &= ~mask;
          break;
        case Chess.Pieces.WBISHOP:
        case Chess.Pieces.BBISHOP:
          this.Bishops &= ~mask;
          break;
        case Chess.Pieces.WKNIGHT:
        case Chess.Pieces.BKNIGHT:
          this.Knights &= ~mask;
          break;
        case Chess.Pieces.WPAWN:
        case Chess.Pieces.BPAWN:
          this.Pawns &= ~mask;
          break;
      }
    }


    /// <summary>
    /// Moves the the given piece in the bitboards.  May want to consider moving all of
    /// the movePiece logic into the ChessPosition.removePiece function.  This routine
    /// basically mimics the ChessBitboard.removePiece and ChessBitboard.addPiece so maybe
    /// simply call those routines to to simplify the logic.  Currenty only being done
    /// for a small optimization gain.
    /// </summary>
    /// <param name="piece">Piece we are deleting, note that the ChessPostion
    /// class ignores the color of the piece as basically it is color blind.</param>
    /// <param name="square"></param>
    /// <param name="Pieces">Current bitboards to update.</param>
    public void movePiece(Chess.Pieces piece, BitboardSquare from, BitboardSquare to)
    {
      // Move the piece.
      removePiece(piece,from.getSquareMask());
      addPiece(piece,to.getSquareMask());
      // Remove any prevoious Enpasant.
      Enpasant = 0x00;
      // See if this move puts a pawn in danger of Enpasant      
      if( piece == Chess.Pieces.BPAWN || piece == Chess.Pieces.WPAWN )
      {
        if( isPotentialEnpasant(from.getSquareMask(), to.getSquareMask()) )
          // Set square of pawn susceptible to enpasant attack.
          Enpasant = to.getSquareMask();
      }
      Board = (Board & (~from.getSquareMask()))|to.getSquareMask();
      // Update attacker rotated boards:
      Bitboard maskFrom = from.getA8H1Mask();// getBitSquare(coFrom.A8H1Square);
      Bitboard maskTo   = to.getA8H1Mask();//getBitSquare(coTo.A8H1Square);
      RotatedA8H1 = (RotatedA8H1 & (~maskFrom)) | maskTo;
      maskFrom = from.getA1H8Mask();//getBitSquare(coFrom.A1H8Square);
      maskTo   = to.getA1H8Mask();//getBitSquare(coTo.A1H8Square);
      RotatedA1H8 = (RotatedA1H8& (~maskFrom)) | maskTo;
      maskFrom = from.getR90Mask();//getBitSquare(coFrom.R90Square);
      maskTo   = to.getR90Mask();//getBitSquare(coTo.R90Square);
      RotatedR90 = (RotatedR90 & (~maskFrom)) | maskTo;
    }
    public bool isPotentialEnpasant(Bitboard from, Bitboard to)
    {
      bool bOk = false;
      // See if a black Enpasant move.
      if( ((0x00FF000000000000 & from) != 0 ) &&
        ((0x000000FF00000000 & to) !=0 ) )
        bOk = true;
      // See if a white Enpasant move.
      else if( ((0x000000000000FF00 & from) != 0 ) &&
        ((0x00000000FF000000 & to) !=0 ) )
        bOk = true;
      return bOk;
    }

    /// <summary>
    /// Adds the given piece to the bitboards.  May want to consider moving all of
    /// the addPiece logic into the ChessPosition.removePiece function.
    /// </summary>
    /// <param name="piece">Piece we are deleting, note that the ChessPostion
    /// class ignores the color of the piece as basically it is color blind.</param>
    /// <param name="Pieces">Current bitboards to update.</param>
    public void addPiece(Chess.Pieces piece, BitboardSquare to)
    {
      // Add the piece to the piece board.
      addPiece(piece, to.getSquareMask());
      // Update main board.
      Board  = Board |to.getSquareMask();
      // Update rotated boards:
      Bitboard maskTo   = to.getA8H1Mask(); // getBitSquare(coTo.A8H1Square);
      RotatedA8H1 = RotatedA8H1 | maskTo;
      maskTo   = to.getA1H8Mask(); // getBitSquare(coTo.A1H8Square);
      RotatedA1H8 = RotatedA1H8 | maskTo;
      maskTo   = to.getR90Mask(); // getBitSquare(coTo.R90Square);
      RotatedR90 = RotatedR90 | maskTo;
    }

    /// <summary>
    /// Removes the given piece from the bitboards.  May want to consider moving all of
    /// the removePiece logic into the ChessPosition.removePiece function.
    /// </summary>
    /// <param name="piece">Piece we are deleting, note that the ChessPostion
    /// class ignores the color of the piece as basically it is color blind.</param>
    /// <param name="square"></param>
    /// <param name="Pieces"></param>
    public void removePiece(Chess.Pieces piece, BitboardSquare square )
    {
      // Get the squares.
      // BitboardSquare bbSquare = new BitboardSquare(square);
      Bitboard mask = square.getSquareMask(); // getBitSquare(bbSquare.Square);
      removePiece(piece,mask);
      Board = Board & ~mask;

      // Update rotated boards:
      mask   = square.getA8H1Mask(); // getBitSquare(bbSquare.A8H1Square);
      RotatedA8H1 = RotatedA8H1 & ~mask;
      mask   = square.getA1H8Mask(); // getBitSquare(bbSquare.A1H8Square);
      RotatedA1H8 = RotatedA1H8 & ~mask;
      mask   = square.getR90Mask(); // getBitSquare(bbSquare.R90Square);
      RotatedR90 = RotatedR90 & ~ mask;
    }

    /// <summary>
    /// Removes the requested piece bitboard.  Note you may have multiple bits set for all but
    /// the kings bitboard.
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public Bitboard getPiece(Chess.Pieces piece)
    {
      Bitboard mask = 0;
      switch(piece)
      {
        case Chess.Pieces.WKING:
        case Chess.Pieces.BKING:
          mask = this.King;
          break;
        case Chess.Pieces.WQUEEN:
        case Chess.Pieces.BQUEEN:
          mask = this.Queens;
          break;
        case Chess.Pieces.WROOK:
        case Chess.Pieces.BROOK:
          mask = this.Rooks;
          break;
        case Chess.Pieces.WBISHOP:
        case Chess.Pieces.BBISHOP:
          mask = this.Bishops;
          break;
        case Chess.Pieces.WKNIGHT:
        case Chess.Pieces.BKNIGHT:
          mask = this.Knights;
          break;
        case Chess.Pieces.WPAWN:
        case Chess.Pieces.BPAWN:
          mask = this.Pawns;
          break;
      }
      return mask;
    }
  }
}
