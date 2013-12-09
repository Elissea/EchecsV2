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
using System.Text;
using System.Collections;

namespace ChessLibrary
{
  using Bitboard = System.UInt64;

  /// <summary>
  /// Contains the logic to handle bitboard manipulations and move validation.  Initial implementation is discarding
  /// crafties approach of pre-calculated move boards for sliding pieces to get an idea of speed issues that
  /// may be related to CPU cache size and the large amount of data needed to hold all of our tables.
  /// </summary>
  public class StandardValidation: IPositionEvents, IValidation
	{
    #region Constants
    public const string WHITEPIECES = "W";
    public const string BLACKPIECES = "B";
    public const string SQUARE = "S";

    AttackingPieces Attacking = new AttackingPieces();

    static string [,] coCastleSq = new string[,]
                         {
                           {"f1","g1","h1"},
                           {"d1","c1","a1"},
                           {"f8","g8","h8"},
                           {"d8","c8","a8"},
    };
    #endregion Constants

    #region Attributes
    /// <summary>
    /// algebraic notation.
    /// </summary>
    ChessMove coMove;

    /// <summary>
    /// For FEN notation parsing and generation.
    /// </summary>
    //FenNotation coFen;

    /// <summary>
    /// Holds all of our bitboards for our pieces.
    /// </summary>
    Hashtable coPieceTable;

    // Source squares
    BitboardSquare coFromSquare;

    // Destination squares
    BitboardSquare coToSquare;

    Bitboard Enpasant;

    public class InCheckException: ApplicationException
    {
      public InCheckException()
      {
      }
    }
    #endregion Attributes;

    #region Properties
    private Bitboard MovePieceBoard
    {
      get{ return coMovePieceBoard; }
      set{ coMovePieceBoard=value;  }
    }
    Bitboard coMovePieceBoard;

    public ChessMove Move
    {
      get{ return coMove;}
    }

    private int coSquare;
    public int Square
    {
      get{ return coSquare; }
      set{ coSquare = value; }
    }

    private int coBitsToPrint;
    #endregion Properties

    #region Constructors
    /// <summary>
    /// Construction of our bitboard to a stable state
    /// </summary>
    public StandardValidation()
    {
      coMove = new ChessMove();
      coFromSquare = new BitboardSquare(0);
      coToSquare = new BitboardSquare(0);
      coPieceTable = new Hashtable();
      coPieceTable[WHITEPIECES] = new ChessPosition(ChessMove.WHITE);
      coPieceTable[BLACKPIECES] = new ChessPosition(ChessMove.BLACK);      
      coBitsToPrint=64;
      Enpasant = 0;
    }

    #endregion Constructors
    #region Methods
    /// <summary>
    /// Get the specified sides position.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public ChessPosition getPosition(bool color)
    {
      // Select the correct hash tables based on side to move.
      return (ChessPosition) coPieceTable[color?WHITEPIECES:BLACKPIECES];
    }
    #region IPositionEvents
    /// <summary>
    /// Adds the requested piece to our bitboards.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="square"></param>
    public void placePiece(Chess.Pieces piece, int square)
    {
      // Select the correct hash tables based on side to move.
      ChessPosition position = (ChessPosition) coPieceTable[piece < Chess.Pieces.BKING?WHITEPIECES:BLACKPIECES];
      //coToSquare = getBitSquare(square);
      position.addPiece(piece, new BitboardSquare(square));
    }

    public void setCastling(bool WK, bool WQ, bool BK, bool BQ)
    {
      ChessPosition WhiteSide = (ChessPosition) coPieceTable[WHITEPIECES];
      ChessPosition BlackSide = (ChessPosition) coPieceTable[BLACKPIECES];
      WhiteSide.canCastleKingside = WK;
      WhiteSide.canCastleQueenside = WQ;
      BlackSide.canCastleKingside = BK;
      BlackSide.canCastleQueenside = BQ;
    }
    public void setColor(bool bColor)
    {
      coMove.Color = bColor;
    }
    public void finished()
    {
      // Select the correct hash tables based on side to move.
      ChessPosition Attacker = (ChessPosition) coPieceTable[coMove.Color?WHITEPIECES:BLACKPIECES];
      ChessPosition Attacked = (ChessPosition) coPieceTable[coMove.Color?BLACKPIECES:WHITEPIECES];
      isKingCheckedMatedStaled(Attacked , Attacker);
    }
    #endregion
    
    /// <summary>
    /// Simply generates the starting positions of all
    /// the pieces.
    /// </summary>
    public void newBoard()
    {
      ChessPosition Pieces = (ChessPosition) coPieceTable[WHITEPIECES];
      // Init Whites starting position.
      Pieces.newBoard();
      // Init Black starting position.
      Pieces = (ChessPosition) coPieceTable[BLACKPIECES];
      Pieces.newBoard();
      Move.Color = ChessMove.WHITE;
    }

    /// <summary>
    /// Clears out all bitboards, setting them to zero.
    /// </summary>
    public void clearBoard()
    {
      ChessPosition Pieces = (ChessPosition) coPieceTable[WHITEPIECES];
      Pieces.clearBoard();
      Pieces = (ChessPosition) coPieceTable[BLACKPIECES];
      Pieces.clearBoard();
      Move.Color = ChessMove.WHITE;
    }

    /// <summary>
    /// Parses out our Algebraic notation and then makes the move.
    /// This is a slower process than calling the function
    /// move("e2","e4") which does not have to compute the source
    /// piece that is actually moving.
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public bool moveAlgebraic(string notation)
    {
      coMove.parseMove(notation);

      //FromSquare = getPiece(coMove.Piece, ToSquare);
      return true; //move(coMove.Piece,coMove.Move,"e4");
    }

    /// <summary>
    /// Returns the valid moves that a chess piece may make.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="AttackerTable"></param>
    /// <param name="AttackedTable"></param>
    /// <returns></returns>
    Bitboard getLegalMoves(Chess.Pieces piece, ChessPosition Attacker, ChessPosition Attacked)
    {
      Bitboard legalMoves = 0;
      switch(piece)
      {
        case Chess.Pieces.WKING:
        case Chess.Pieces.BKING:
          legalMoves = getKingMoves( Attacker.Color, Attacker.canCastleKingside, Attacker.canCastleQueenside,
            coFromSquare.getSquareMask(), Attacker.Board, Attacked.Board);
          break;
        case Chess.Pieces.WQUEEN:
        case Chess.Pieces.BQUEEN:
          legalMoves  = getQueenMoves(coFromSquare.getSquareMask(),Attacker.Board,Attacked.Board);
          break;
        case Chess.Pieces.WROOK:
        case Chess.Pieces.BROOK:
          legalMoves  = getRookMoves(coFromSquare.getSquareMask(),Attacker.Board,Attacked.Board);
          break;
        case Chess.Pieces.WBISHOP:
        case Chess.Pieces.BBISHOP:
          legalMoves = getBishopMoves(coFromSquare.getSquareMask(),Attacker.Board,Attacked.Board);
          break;
        case Chess.Pieces.WKNIGHT:
        case Chess.Pieces.BKNIGHT:
          legalMoves = getKnightMoves(coFromSquare.getSquareMask(),Attacker.Board,Attacked.Board);
          break;
        case Chess.Pieces.WPAWN:
        case Chess.Pieces.BPAWN:
          // Get basic pawn moves.
          legalMoves = getPawnMoves(coFromSquare.getSquareMask(),coMove.Color,Attacker.Board,Attacked.Board);
          // Only a legal move if a pawn is attacking a piece.
          legalMoves |= (Attacked.Board & getPawnAttacks(coFromSquare.getSquareMask(),coMove.Color));
          // Get enpasant move
          legalMoves |= getPawnEnpasant(coFromSquare.getSquareMask(), Attacker, Attacked);
          break;
      }
      return legalMoves;
    }

    #region chessmoves
    Bitboard getPawnEnpasant(Bitboard pawn, ChessPosition Attacker, ChessPosition Attacked)
    {
      Bitboard mask = 0;
      // Potential Empasant move available.
      if( Attacked.Enpasant > 0 )
      {
        int rowAttacker = getBitRow(pawn);
        int rowAttacked = getBitRow(Attacked.Enpasant);
        if( rowAttacker == rowAttacked )
        {
          int colmAttacker = getBitColm(pawn);
          int colmAttacked = getBitColm(Attacked.Enpasant);
          if( (colmAttacker + 1) == colmAttacked ||
            (colmAttacker - 1) == colmAttacked )
          {
            if( Attacker.Color )
            {
              mask = Attacked.Enpasant << 8;
            }
            else
            {
              mask = Attacked.Enpasant >> 8;
            }
            Enpasant = mask;
          }
        }
      }
      return mask;
    }
    /// <summary>
    /// Returns all of the valid moves for the specified pawn.
    /// Note if you wish to simply get all squares a piece can move
    /// to then simply pass zero for attBucket and defBucket.  If you
    /// do not want to include protected pieces then xor piece board
    /// then AND with function returned value.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="bit">One bit set for location of biship</param>
    /// <param name="dir">If true then white pawn move</param>
    /// <param name="attBucket">The piece board associated with pawn move</param>
    /// <param name="defBucket">The piece board associated with the opposite color</param>
    /// <returns></returns>
    Bitboard getPawnMoves(Bitboard bit, bool dir, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket=0,pBit=bit;
      int row=this.getBitRow(bit),colm=getBitColm(bit);      
      if( dir )
      {
        bit = bit << 8;
        // Generate first pawn move.
        if( (bit & attBucket) == 0 && (bit & defBucket) == 0 )
          mBucket |= bit;
        // Generate second pawn move only if first was valid.
        if( mBucket > 0 && row == 2 )
        {
          bit = bit << 8;
          if( (bit & attBucket) == 0 && (bit & defBucket) == 0 )
            mBucket |= bit;
        }
      }
      else
      {
        bit = bit >> 8;
        // Generate first pawn move
        if( (bit & attBucket) == 0 && (bit & defBucket) == 0 )
          mBucket |= bit;
        // Generate second pawn move only if first was valid.
        if( mBucket > 0 && row == 7 )
        {
          bit = bit >> 8;
          if( (bit & attBucket) == 0 && (bit & defBucket) == 0 )
            mBucket |= bit;
        }
      }
      return mBucket;
    }
    /// <summary>
    /// Generates only the square a pawn could attack.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="dir"></param>
    /// <param name="attBucket"></param>
    /// <param name="defBucket"></param>
    /// <returns></returns>
    Bitboard getPawnAttacks(Bitboard pawn, bool dir )
    {
      Bitboard mBucket=0;//,pBit=pawn;
      int row=this.getBitRow(pawn),colm=getBitColm(pawn);
      if( dir )
      {
        pawn = pawn << 8;
        if( colm != 8 )
          mBucket |= (pawn >> 1) ;
        if( colm != 1 )
          mBucket |= (pawn << 1);
      }
      else
      {
        pawn = pawn >> 8;
        if( colm != 8 )
          mBucket |= (pawn >> 1);
        if( colm != 1 )
          mBucket |= (pawn << 1);
      }
      // mBucket &= defBucket;
      return mBucket;
    }

    /// <summary>
    /// Determines all of the given square a knight may move to, includes it's captures
    /// and piece protection.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="attBucket"></param>
    /// <param name="defBucket"></param>
    /// <returns></returns>
    Bitboard getKnightMoves(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket,night1=0,night2=0;
      int row=this.getBitRow(bit),colm=getBitColm(bit);

      switch(colm)
      {
        case 1:
          night1 = (Bitboard)0xFF &  (Bitboard)(0x05 << 6);
          night2 = (Bitboard)0xFF &  (Bitboard)(0x11 << 5);
          break;
        case 2:
          night1 = 0xFF & (0x05 << 5);
          night2 = 0xFF & (0x11 << 4);
          break;
        case 3:
          night1 = 0xFF & (0x05 << 4);
          night2 = 0xFF & (0x11 << 3);
          break;
        case 4:
          night1 = 0xFF & (0x05 << 3);
          night2 = 0xFF & (0x11 << 2);
          break;
        case 5:
          night1 = 0xFF & (0x05 << 2);
          night2 = 0xFF & (0x11 << 1);
          break;
        case 6:
          night1 = 0xFF & (0x05 << 1);
          night2 = 0x11;
          break;
        case 7:
          night1 = 0x05;
          night2 = 0xFF & (0x11 >> 1);
          break;
        case 8:
          night1 = 0xFF & (0x05 >> 1);
          night2 = 0xFF & (0x11 >> 2);
          break;
      }

      mBucket = 0;
      if( row < 8 )
        mBucket |= night2 << (8*row);
      if( row < 7 )
        mBucket |= night1 << (8*(row+1));
      if( row > 1 )
        mBucket |= night2 << (8*(row-2));
      if( row > 2 )
        mBucket |= night1 << (8*(row-3));

      return mBucket;
    }

    /// <summary>
    /// Determines all of the given square a queen may move to, includes it's captures
    /// and piece protection.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="attBucket"></param>
    /// <param name="defBucket"></param>
    /// <returns></returns>
    Bitboard getQueenMoves(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket;
      mBucket  = getRankMoves(bit,attBucket,defBucket);
      mBucket |= getFileMoves(bit,attBucket,defBucket);
      mBucket |= getBishopMoves(bit,attBucket,defBucket);
      return mBucket;
    }
 
    /// <summary>
    /// Determines all of the given squares a rook may move to includes it's captures
    /// and piece protection.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="attBucket"></param>
    /// <param name="defBucket"></param>
    /// <returns></returns>
    Bitboard getRookMoves(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket;
      mBucket  = getRankMoves(bit,attBucket,defBucket);
      mBucket |= getFileMoves(bit,attBucket,defBucket);
      return mBucket;
    }

    /// <summary>
    /// Return all valid moves for the king, including pieces it protects.
    /// Note if you wish to simply get all squares a piece can move
    /// to then simply pass zero for attBucket and defBucket.  If you
    /// do not want to include protected pieces then xor piece board
    /// then AND with function returned value.
    /// </summary>
    /// <param name="bit">One bit set for location of king</param>
    /// <param name="attBucket">The piece board associated with king move</param>
    /// <param name="defBucket">The piece board associated with the opposite color</param>
    /// <returns></returns>
    Bitboard getKingMoves(bool bColor, bool canCastleKingside, bool canCastleQueenside,
      Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket=0;
      Bitboard pBit=bit;
      int row=this.getBitRow(bit);
      int colm=getBitColm(bit);

      if( colm != 1 )
        mBucket |= bit << 1;
      if( colm != 8 )
        mBucket |= bit >> 1;

      if( row != 8 )
      {
        mBucket |= bit << 8;
        if( colm != 1  )
          mBucket |= bit << 9;
        if( colm != 8 )
          mBucket |= bit << 7;
      }

      if( row != 1 )
      {
        mBucket |= bit >> 8;
        if( colm != 1 )
          mBucket |= bit >> 7;
        if( colm != 8 )
          mBucket |= bit >> 9;
      }

      //Check for castling privileges
      if (canCastleKingside)
      {
        if( bColor )
        {
          // Check white kingside castling
          if( ((attBucket | defBucket) & 0x07) == 0x01 )
            mBucket |= bit >> 2;
        }
        else
        {
          // Check black kingside castling
          // Check white kingside castling
          if( ((attBucket | defBucket) & 0x0700000000000000) == 0x0100000000000000 )
            mBucket |= bit >> 2;
        }
      }
      if (canCastleQueenside)
      {
        if( bColor )
        {
          // Check white kingside castling
          if( ((attBucket | defBucket) & 0xF0) == 0x80 )
            mBucket |= bit << 2;
        }
        else
        {
          // Check black kingside castling
          // Check white kingside castling
          if( ((attBucket | defBucket) & 0xF000000000000000) == 0x8000000000000000 )
            mBucket |= bit << 2;
        }
      }

      return mBucket;
    }

    /// <summary>
    /// This routine is used to calculate all of the squares between an attacking
    /// piece and the attacked piecs.  It requires that only one bit be set in each
    /// of the bitboards.  Only calculates sliding piece moves such as rooks and
    /// bishops.
    /// </summary>
    /// <param name="attacker">bit set of attacking piece</param>
    /// <param name="attacked">bit set of defending piece</param>
    /// <returns></returns>
    Bitboard getDirectAttack(Bitboard attacker, Bitboard attacked)
    {
      int arow=getBitRow(attacker),acolm=getBitColm(attacker);
      int drow=getBitRow(attacked),dcolm=getBitColm(attacked);
      Bitboard mask = attacker | attacked;
      if( arow == drow )
      {
        if( acolm > dcolm )
        {
          while( attacker != attacked )
          {
            attacker <<= 1;
            mask |= attacker;
          }
        }
        else
        {
          while( attacker != attacked )
          {
            attacker >>= 1;
            mask |= attacker;
          }
        }

      }
      else if( acolm == dcolm )
      {
        if( arow > drow )
        {
          while( attacker != attacked )
          {
            attacker = attacker >> 8;
            mask |= attacker;
          }

        }
        else
        {
          while( attacker != attacked )
          {
            attacker = attacker << 8;
            mask |= attacker;
          }
        }
      }
      else if( arow < drow )
      {
        if( acolm > dcolm )
        {
          while( attacker != attacked )
          {
            attacker = attacker << 9;
            mask |= attacker;
          }
        }
        else
        {
          while( attacker != attacked )
          {
            attacker = attacker << 7;
            mask |= attacker;
          }
        }
      }
      else if( arow > drow )
      {
        if( acolm > dcolm )
        {
          while( attacker != attacked )
          {
            attacker = attacker >> 7;
            mask |= attacker;
          }
        }
        else
        {
          while( attacker != attacked )
          {
            attacker = attacker >> 9;
            mask |= attacker;
          }
        }
      }
      return mask;
    }

    /// <summary>
    /// Returns all bits set for valid moves of specified bishop, including
    /// square in which the bishop protects one of it's own pieces.
    /// Note if you wish to simply get all squares a piece can move
    /// to then simply pass zero for attBucket and defBucket.  If you
    /// do not want to include protected pieces then xor piece board
    /// then AND with function returned value.
    /// </summary>
    /// <param name="bit">One bit set for location of biship</param>
    /// <param name="attBucket">The piece board associated with bishop move</param>
    /// <param name="defBucket">The piece board associated with the opposite color</param>
    /// <returns></returns>
    Bitboard getBishopMoves(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket=0;
      Bitboard pBit=bit;
      int row = this.getBitRow(bit);
      int colm = getBitColm(bit);

      if ((row==0) || (colm == 0)) // This can happen if a king is off the board
        return mBucket;

      for(int r=row, c=colm;r!=8 && c != 1;r++,c--)
      {
        bit = bit << 9;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }

      bit=pBit;
      for(int r=row, c=colm;r!=8 && c!=8;r++,c++)
      {
        bit = bit << 7;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }

      bit=pBit;
      for(int r=row, c=colm;r!=1 && c != 8;r--,c++)
      {
        bit = bit >> 9;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }

      bit=pBit;
      for(int r=row, c=colm;r!=1 && c!=1;r--,c--)
      {
        bit = bit >> 7;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }
      return mBucket;
    }

    public Bitboard getDiagonalLR(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket=0;
      Bitboard pBit=bit;
      int row = this.getBitRow(bit);
      int colm = getBitColm(bit);

      if ((row==0) || (colm == 0)) // This can happen if a king is off the board
        return mBucket;

      for(int r=row, c=colm;r!=8 && c != 1;r++,c--)
      {
        bit = bit << 9;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }

      bit=pBit;
      for(int r=row, c=colm;r!=1 && c != 8;r--,c++)
      {
        bit = bit >> 9;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }
      return mBucket;
    }
    public Bitboard getDiagonalRL(Bitboard bit, Bitboard attBucket, Bitboard defBucket)
    {
      Bitboard mBucket=0;
      Bitboard pBit=bit;
      int row = this.getBitRow(bit);
      int colm = getBitColm(bit);

      if ((row==0) || (colm == 0)) // This can happen if a king is off the board
        return mBucket;

      bit=pBit;
      for(int r=row, c=colm;r!=8 && c!=8;r++,c++)
      {
        bit = bit << 7;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }

      bit=pBit;
      for(int r=row, c=colm;r!=1 && c!=1;r--,c--)
      {
        bit = bit >> 7;
        mBucket |= bit;
        if( (bit & attBucket) != 0 || (bit & defBucket) != 0 )
          break;
      }
      return mBucket;
    }
    /// <summary>
    /// Determines what horizontal squares a sliding piece may successfully move to, this
    /// would include the capture of an enemy piece.
    /// Note if you wish to simply get all squares a piece can move
    /// to then simply pass zero for attBucket and defBucket.  If you
    /// do not want to include protected pieces then xor piece board
    /// then AND with function returned value.
    /// </summary>
    /// <param name="piece">single bit of piece loation in row or column</param>
    /// <param name="attacker">the bits of all pieces in row or column of the side to move</param>
    /// <param name="defender">the bits of all pieces in row or column of the side not to move</param>
    /// <returns></returns>
    Bitboard getRankMoves(Bitboard piece, Bitboard attacker, Bitboard defender)
    {
      Bitboard pBit = 0x08;
      Bitboard moveBucket = 0x00;
      int colm = getBitColm(piece);
      int workcolm=colm+1;
      pBit = piece;

      //Check all squares to the right
      while (workcolm < 9)
      {
        pBit = (Bitboard)(pBit >> 1);
        moveBucket = (Bitboard)( moveBucket | pBit );
        if( (Bitboard)(pBit & attacker) == 0 )
        {
          if( (Bitboard)(pBit & defender) != 0 )
            break;
        }
        else
          break;
        workcolm++;
      }

      workcolm=colm-1;
      pBit = piece;

      //Check all squares to the left.
      while (workcolm > 0)
      {
        pBit = (Bitboard)(pBit << 1);
        moveBucket = (Bitboard)( moveBucket | pBit );
        if( (Bitboard)(pBit & attacker) == 0 )
        {
          if( (Bitboard)(pBit & defender) != 0 )
            break;
        }
        else
          break;
        workcolm--;
      }
      return moveBucket;
    }

    /// <summary>
    /// Determines what vertical squares a sliding piece may successfully move to, this
    /// would include the capture of an enemy piece.
    /// Note if you wish to simply get all squares a piece can move
    /// to then simply pass zero for attBucket and defBucket.  If you
    /// do not want to include protected pieces then xor piece board
    /// then AND with function returned value.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    Bitboard getFileMoves(Bitboard piece, Bitboard attacker, Bitboard defender)
    {
      Bitboard pBit    = 0x08;
      Bitboard moveBucket = 0x00;
      int row = getBitRow(piece);
      int workrow=row;
      pBit = piece;
      if( workrow == 8 )
        workrow++;

      while( workrow < 9 )
      {
        pBit = (Bitboard)(pBit << 8);
        moveBucket = (Bitboard)( moveBucket | pBit );
        if( (Bitboard)(pBit & attacker) == 0 )
        {
          if( (Bitboard)(pBit & defender) != 0 )
            break;
        }
        else
          break;
        workrow++;
      }

      workrow=row;
      if( workrow == 1 )
        workrow--;
      pBit = piece;

      while( workrow > 0 )
      {
        pBit = (Bitboard)(pBit >> 8);
        moveBucket = (Bitboard)( moveBucket | pBit );
        if( (Bitboard)(pBit & attacker) == 0 )
        {
          if( (Bitboard)(pBit & defender) != 0 )
            break;
        }
        else
          break;
        workrow--;
      }
      return moveBucket;
    }
    #endregion chessmoves

    /// <summary>
    /// Test to see if the king is in check or the game has ended 
    /// in a Checkmate or Stalemate.
    /// Tests for Stale mate:
    ///   5k2/5P2/5PK1/8/8/8/8/ b  ok
    ///   7k/R7/8/8/8/8/8/6R1 b  ok
    ///   2Q1k3/2p1brpp/8/2q5/8/4R3/PP1N2PP/4R2K b  ok
    ///   1k5r/6p1/4qpp1/5p1P/8/P7/KPP5/3q4 w ok
    ///   8/5Q2/7k/4R2P/1KP5/p5P1/P7/8 b ok
    ///   rnbq1bnr/ppp1pQp1/3kB3/3P4/N4B1p/8/PPP2KPP/R5NR b pawn move two squares not suppressed with piece in the way.
    ///   rnbqkbnr/pppppp1p/6p1/3PP3/3PKP2/3PPP2/7P/RNBQ1BNR b weird enpasant capture to prevent mate
    ///   rnbqkbnr/pppppp1p/6p1/3PB3/3PKP2/3PPP2/7P/RNBQ2NR b weird enpasant can't capture to prevent mate
    /// </summary>
    /// <param name="Attacker"></param>
    /// <param name="Attacked"></param>
    /// <returns></returns>
    bool isKingCheckedMatedStaled(ChessPosition Attacker, ChessPosition Attacked)
    {
      Bitboard pawnMoves;
      Attacked.isKingMated = false;
      Attacked.isKingStaleMated = false;
      // Only test for mate if our opponents king is currently in check.
      if( isKingInCheck(Attacked.Color, Attacked, Attacker) )
        // Set the flag for use later.
        Attacked.isKingInCheck = true;
      else
        Attacked.isKingInCheck = false;

      // Generate the Attacker's moves as if the Attacked king is not there.
      //  This allows us to get rid of any potential king moves that would run past a
      //  king; such as the attack of a rook or bishop.
      // Remove the king
      Attacked.Board = Attacked.Board & ~Attacked.King;
      // Generate all attackes.
      Bitboard attacks = getAttacks(Attacker,Attacked, suppresswho.GETALL,out pawnMoves);
      // Put king back on board.
      Attacked.Board = Attacked.Board | Attacked.King;
      // Get all of the king moves for the king being attacked and
      //  subtract off the pieces the king protects.  These are the valid moves.
      Bitboard king = (~Attacked.Board & getKingMoves(Attacked.Color,false,false,
        Attacked.King,
        Attacked.Board,
        Attacker.Board));
      
      // Lets see if the king can move by complementing the attacks and
      //  anding the result to our king moves.  If we get a value of zero
      //  then the king has no moves available.
      bool KingCantMove =( 0 == (~attacks & king));

      // Is more than one piece attacking the king? If so our only option
      //  was to move the king to safety.
      if( KingCantMove && Attacking.Count > 1 )
      {
        Attacked.isKingInCheck = true;
        Attacked.isKingMated = true;
      }
      else if( KingCantMove )
      {        
        // If the king can't move and atleast one attacking piece we need to see if any other piece
        // can attack another attacking piece or 
        // Get all of the leagal moves for the attacked except the king.
        //  We determined above the king could not move out of check or
        //  capture the attacking piece.
        Bitboard retaliate = ~king & getAttacks(Attacked,Attacker, suppresswho.KINGS|suppresswho.PAWNILLEGALATTACKS, out pawnMoves);
        Bitboard attackLine=0;
        // The king is in check so now we're checking for check mate.
        if( Attacked.isKingInCheck )
        {
          if( Attacking.Queen > 0 )
          {
            // First see if we can capture the Queen.
            if( (retaliate & Attacking.Queen) == 0 )
            {
              attackLine = getDirectAttack(Attacking.Queen,Attacked.King);
            }
          }
          else if( Attacking.Rook > 0 )
          {
            // First see if we can capture the Rook.
            if( (retaliate & Attacking.Rook) == 0 )
            {
              attackLine = getDirectAttack(Attacking.Rook,Attacked.King);
            }

          }
          else if( Attacking.Bishop > 0 )
          {
            // First see if we can capture the Bishop.
            if( (retaliate & Attacking.Bishop) == 0 )
            {
              attackLine = getDirectAttack(Attacking.Bishop,Attacked.King);
            }
          }
          else if( Attacking.Knight > 0 )
          {
            // First see if we can capture the Knight.
            if( (retaliate & Attacking.Knight) == 0 )
            {
              // Calculate attacking piece and defending kings common attacks.
              attackLine = Attacking.Knight | Attacked.King;
            }
          }
          else if( Attacking.Pawn > 0 )
          {
            // First see if we can capture the pawn.
            if( (retaliate & Attacking.Pawn) == 0 )
            {
              if((Attacker.Enpasant & Attacking.Pawn) == 0  || Enpasant == 0 )
              {
                // Calculate attacking piece and defending kings common attacks.
                attackLine = getDirectAttack(Attacking.Pawn,Attacked.King);
              }
            }
          }          
          // See if we have a direct line of attack as we didn't capture the attacking
          //  piece.
          if( attackLine != 0 )
          {           
            // Generate pinned pieces as we can't use them to interpose.
            getPins(Attacker, Attacked);
            // Suppress pawn attackes from the defender here as they can't interpose
            //  and we've already tested for king moves out of checks above.
            retaliate = getAttacks(Attacked,Attacker,suppresswho.PAWNATTACKS|suppresswho.KINGS, out pawnMoves);
            // Remove the king itself incase it is protected by another piece, add in any pawn moves
            retaliate = pawnMoves | (retaliate & ~Attacked.King);
            // See if a piece can interpose.
            if( (attackLine & ~retaliate) == attackLine )
              Attacked.isKingMated = true;
            // Clear for now but in future use to optimize move generation.
            Attacked.Pinned = 0x00;
            Attacked.Pinning = 0x00;
          }
        }
        else if( retaliate == 0 && pawnMoves == 0)
        {
          // We're in stalemate.
          Attacked.isKingStaleMated = true;
        }
      }

      if( Attacked.isKingMated )
      {
        if( EventKingIsMated != null )
          EventKingIsMated();
      }
      else if( Attacked.isKingInCheck )
      {
        if( EventKingIsInCheck != null )
          EventKingIsInCheck();
      }
      else if( Attacked.isKingStaleMated )
      {
        if( EventKingIsStaleMated != null )
          EventKingIsStaleMated();
      }
      else
        EventKingIsFree();
      return Attacked.isKingMated;
    }

    /// <summary>
    /// This routine expects the kings bit location and the attacking and
    /// defending bitboards.  Then imposing the king as each of the attacking pieces:
    /// queen, rook, bishop, knight, and pawn; generates the attack bitboards for
    /// the respective piece.  As each pieces moves are calculated we can then and
    /// the attack piece bit boards to see if we find a hit.  So generate bishop moves
    /// from the kings current posistion then and with opposite bishop bitboard, if
    /// we have a hit then the king is in check.
    /// </summary>
    /// <param name="bit"></param>
    /// <param name="attacker">Side to see if they are in check</param>
    /// <param name="defender">Side doing the checking</param>
    /// <returns>
    /// A bit board containing all squares that harbor
    /// pieces that are attacking the king
    /// </returns>
    /// <summary>
    /// This routine checks to see if a king is in check by impersonating each
    /// opposite color's pieces and generating legals moves for each piece.
    /// The legal moves are then compared with the kings attackers to determine
    /// whom is attacking the king.  This is an optimization that only requires
    /// us to generate the moves of each of our chess piece types vs the all the
    /// moves of our attackers.
    /// </summary>
    /// <param name="bColor"></param>
    /// <param name="inCheck"></param>
    /// <param name="Attacker"></param>
    /// <returns>True if the king is in check.  Also set a </returns>
    bool isKingInCheck(bool bColor, ChessPosition inCheck, ChessPosition Attacker)
    {
      Bitboard mBucket=0;
      Bitboard moves;
      // Used so queen is only counted once by Attacking class.
      Bitboard qmoves;
      Attacking.clear();
      //-- Used for pin detection. --
      Attacking.DiagonalLR = inCheck.King | this.getDiagonalLR(inCheck.King, 0,0);
      Attacking.DiagonalRL = inCheck.King | this.getDiagonalRL(inCheck.King, 0,0);
      Attacking.Files = inCheck.King | this.getFileMoves(inCheck.King,0,0);
      Attacking.Ranks = inCheck.King | this.getRankMoves(inCheck.King,0,0);
      //---------------------------


      moves = getBishopMoves(inCheck.King, inCheck.Board, Attacker.Board);
      Attacking.Bishop = (moves & ~inCheck.Board) & (Bitboard)Attacker.Bishops;
      mBucket |= Attacking.Bishop;

      qmoves = (moves & ~inCheck.Board) & (Bitboard)Attacker.Queens;
      mBucket |= qmoves;

      moves = getRookMoves(inCheck.King, inCheck.Board, Attacker.Board);
      Attacking.Rook = (moves & ~inCheck.Board) & Attacker.Rooks;
      mBucket |= Attacking.Rook;
      Attacking.Queen = qmoves | ((moves & ~inCheck.Board) & (Bitboard)Attacker.Queens);
      mBucket |= Attacking.Queen;

      moves = getKnightMoves(inCheck.King, inCheck.Board, Attacker.Board);
      Attacking.Knight = (moves & ~inCheck.Board) & Attacker.Knights;
      mBucket |= Attacking.Knight;
      // Only want the attacks of a pawn if there is an opponents piece there.
      moves = (Attacker.Board & getPawnAttacks(inCheck.King, bColor /*, inCheck.Board, Attacker.Board*/));
      Attacking.Pawn = (moves & ~inCheck.Board) & Attacker.Pawns;
      mBucket |= Attacking.Pawn;

      moves = getKingMoves(inCheck.Color, inCheck.canCastleKingside, inCheck.canCastleQueenside,
        inCheck.King,inCheck.Board, Attacker.Board);
      mBucket |= (moves & ~inCheck.Board) & (Bitboard)Attacker.King;
      return (mBucket != 0);
    }
    public enum suppresswho{ GETALL = 0x00, KINGS= 0x01, QUEENS = 0x02, ROOKS = 0x04,BISHOPS=0x08, KNIGHTS= 0x10, PAWNS=0x20, PAWNATTACKS=0x40, PAWNILLEGALATTACKS=0x80,PAWNMOVES=0x100, ALL=0xFF };

    /// <summary>
    /// Used to retrieve the square and piece attacks of the attacking army.
    /// You can supress a piece types moves from being generated by passing a
    /// set of enum values in suppresswho.
    /// </summary>
    /// <param name="Attacker"></param>
    /// <param name="Attacked"></param>
    /// <param name="suppress">OR'd values that specify which pieces to ignore.</param>
    /// <param name="pawnMoves">An optional parameter that will be set to legal pawn moves for the position</param>
    /// <returns></returns>
    public Bitboard getAttacks(ChessPosition Attacker,ChessPosition Attacked, suppresswho suppress, out Bitboard pawnMoves)
    {
      Bitboard attacked=0;
      Bitboard mask=0x8000000000000000;
      pawnMoves = 0;

      while(mask != 0)
      {
        // Only if this piece is not pinned.
        if( (Attacker.Pinned & mask) == 0 )
        {
          if((mask & Attacker.Queens) > 0)
          {
            if( (suppress & suppresswho.QUEENS) == 0 )
              attacked |= getQueenMoves(mask,Attacker.Board,Attacked.Board);
          }
          else if((mask & Attacker.Rooks) > 0)
          {
            if( (suppress & suppresswho.ROOKS) == 0 )
              attacked |= getRookMoves(mask,Attacker.Board,Attacked.Board);
          }
          else if((mask & Attacker.Bishops) > 0)
          {
            if( (suppress & suppresswho.BISHOPS) == 0 )
              attacked |= getBishopMoves(mask,Attacker.Board,Attacked.Board);
          }
          else if((mask & Attacker.Knights) > 0)
          {
            if( (suppress & suppresswho.KNIGHTS) == 0 )
              attacked |= getKnightMoves(mask,Attacker.Board,Attacked.Board);
          }
          else if((mask & Attacker.Pawns) > 0)
          {
            if( (suppress & suppresswho.PAWNS) == 0 )
            {
              if( (suppress & suppresswho.PAWNATTACKS) == 0 )
              {
                if( (suppress & suppresswho.PAWNILLEGALATTACKS) == 0 )
                  attacked |= getPawnAttacks(mask,Attacker.Color/*,Attacker.Board,Attacked.Board*/);
                else
                  attacked |= (Attacked.Board & getPawnAttacks(mask,Attacker.Color/*,Attacker.Board,Attacked.Board*/));
                // Get enpasant move
                attacked |= getPawnEnpasant(mask, Attacker, Attacked);
              }
              if( (suppress & suppresswho.PAWNMOVES) == 0 )
              {
                pawnMoves |= getPawnMoves(mask,Attacker.Color,Attacker.Board,Attacked.Board);
              }
            }
          }
          else if((mask & Attacker.King) > 0)
          {
            if( (suppress & suppresswho.KINGS) == 0 )
              attacked |= getKingMoves(Attacker.Color,false,false, mask,Attacker.Board,Attacked.Board);
          }
        }
        mask = mask >> 1;
      }
      //attacked = attacked & ~(Attacker.Board);
      return attacked;
    }

    public enum Castling { CASTLING_NO, CASTLING_YES, CASTLING_THROUGH_CHECK, CASTLING_ERR }; 


    //If castling, then update related items
    /// <summary>
    /// Determines if castling is legal, then performs 
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="fromSquare"></param>
    /// <param name="Attacker"></param>
    /// <param name="Attacked"></param>
    /// <returns></returns>
    public Castling castle(Chess.Pieces piece, string fromSquare, string toSquare, ChessPosition Attacker, ChessPosition Attacked)
    {
      Bitboard from = coFromSquare.getMask(fromSquare);
      Bitboard to =   coFromSquare.getMask(toSquare);
      Chess.Pieces rook=Chess.Pieces.NONE;
      Castling Castled = Castling.CASTLING_NO;

      int nIndex = -1;

      //If castling, then also move the rook
      if (piece == Chess.Pieces.WKING)
      {
        rook=Chess.Pieces.WROOK;
        if (to<<2 == from) //Castling kingside
          nIndex = 0;
        else if (to>>2 == from) //Castling queenside
          nIndex = 1;
      }
      else if (piece == Chess.Pieces.BKING)
      {
        rook=Chess.Pieces.WROOK;
        if (to<<2 == from) //Castling kingside
          nIndex = 2;
        else if (to>>2 == from) //Castling queenside
          nIndex = 3;
      }
      else if (piece == Chess.Pieces.WROOK)
      {
        if (fromSquare == coCastleSq[1,2])
          Attacker.canCastleQueenside = false;
        else if (fromSquare == coCastleSq[0,2])
          Attacker.canCastleKingside = false;
      }
      else if (piece == Chess.Pieces.BROOK)
      {
        if (fromSquare == coCastleSq[3,2])
          Attacker.canCastleQueenside = false;
        else if (fromSquare == coCastleSq[2,2])
          Attacker.canCastleKingside = false;
      }

      if( nIndex != -1 )
      {
        //Save original positions for restoring on errors.
        ChessPosition AttackerS = new ChessPosition(Attacker);
        BitboardSquare fromSq = new BitboardSquare(coFromSquare.Square);
        BitboardSquare toSq   = new BitboardSquare(coToSquare.Square);

        //Is our king in check before castle/can't castle if true?
        if( false == Attacker.isKingInCheck )
        {
          toSq.setSquare(coCastleSq[nIndex,0]);
          Attacker.movePiece(piece,fromSq, toSq);

          //Is our king in check?
          if( ! isKingInCheck(coMove.Color, Attacker, Attacked) )
          {
            // Now move the king to 2nd square and test for check.
            fromSq.setSquare(coCastleSq[nIndex,0]);
            toSq.setSquare(coCastleSq[nIndex,1]);
            Attacker.movePiece(piece,fromSq,toSq);
            //Is our king is in check?
            if( ! isKingInCheck(coMove.Color, Attacker, Attacked)  )
            {
              //Now move the rook
              fromSq.setSquare(coCastleSq[nIndex,2]);
              toSq.setSquare(coCastleSq[nIndex,0]);
              Attacker.movePiece(rook,fromSq,toSq);
              if( EventUpdateBoard != null )
                EventUpdateBoard(Chess.Operation.MOVE, coCastleSq[nIndex,2], coCastleSq[nIndex,0]);
              Castled = Castling.CASTLING_YES;
              Attacker.canCastleKingside = false;
              Attacker.canCastleQueenside = false;
            }
            else
              Castled = Castling.CASTLING_THROUGH_CHECK;
          }
          else
            Castled = Castling.CASTLING_THROUGH_CHECK;
        }
        else
          Castled = Castling.CASTLING_ERR;
        
        if( Castled == Castling.CASTLING_THROUGH_CHECK )
        {
          Attacker.copy(AttackerS);
        }
      }
      return Castled;
    }

    public void promotePawn(Chess.Pieces piece, string square)
    {
      Chess.Pieces promotePiece = Chess.Pieces.NONE;

      if( coMove.Color )
      {
        if( Chess.Pieces.WPAWN == piece &&
          getBitRow(coToSquare.getSquareMask()) == 8 )
        {
          if( EventPromotion != null )
          {
            EventPromotion(coMove.Color, square, ref promotePiece);
            if(promotePiece != Chess.Pieces.NONE )
              placePiece(promotePiece, coToSquare.getSquare(square));
          }
        }
      }
      else
      {
        if( Chess.Pieces.BPAWN == piece &&
          getBitRow(coToSquare.getSquareMask()) == 1 )
        {
          if( EventPromotion != null )
          {
            EventPromotion(coMove.Color, square, ref promotePiece );
            if(promotePiece != Chess.Pieces.NONE )
              placePiece(promotePiece, coToSquare.getSquare(square));
          }            
        }
      }
    }

    /// <summary>
    /// Returns the bitboard for the actual piece moved.  This is used to resolve
    /// the issue presented with Algebraic notation where in most cases only
    /// a destination move is given.  Leaving it up to the computer to figure
    /// which piece moved.
    ///
    /// NOTE: Not currently working..
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="square"></param>
    /// <returns></returns>
    public Bitboard getPiece(Bitboard square)
    {
      Bitboard usquare=0;
      return usquare;
    }

    /// <summary>
    /// Translate a bit into a row coordinate.
    /// </summary>
    /// <param name="bit">Single bit set representing piece on board</param>
    /// <returns>row of piece</returns>
    int getBitRow(Bitboard bit)
    {
      if( (0xFF00000000000000 & bit) != 0 )
        return 8;
      if( (0x00FF000000000000 & bit) != 0 )
        return 7;
      if( (0x0000FF0000000000 & bit) != 0 )
        return 6;
      if( (0x000000FF00000000 & bit) != 0 )
        return 5;
      if( (0x00000000FF000000 & bit) != 0 )
        return 4;
      if( (0x0000000000FF0000 & bit) != 0 )
        return 3;
      if( (0x000000000000FF00 & bit) != 0 )
        return 2;
      if( (0x00000000000000FF & bit) != 0 )
        return 1;
      return 0;
    }

    /// <summary>
    /// Translate a single bit into a column coordinate.
    /// </summary>
    /// <param name="bit">Single bit set representing piece on board</param>
    /// <returns>column of piece</returns>
    int getBitColm(Bitboard bit)
    {
      if( (0x8080808080808080 & bit) != 0 )
        return 1;
      if( (0x4040404040404040 & bit) != 0 )
        return 2;
      if( (0x2020202020202020 & bit) != 0 )
        return 3;
      if( (0x1010101010101010 & bit) != 0 )
        return 4;
      if( (0x0808080808080808 & bit) != 0 )
        return 5;
      if( (0x0404040404040404 & bit) != 0 )
        return 6;
      if( (0x0202020202020202 & bit) != 0 )
        return 7;
      if( (0x0101010101010101 & bit) != 0 )
        return 8;
      return 0;
    }

    /// <summary>
    /// Junk routine just delete later when you don't need...
    /// </summary>
    public void print()
    {
      printBoard();
    }

    /// <summary>
    /// Routine used to print any bitboard that we want to.
    /// </summary>
    /// <param name="uboard">bitboard to print</param>
    public void printBoard(Bitboard uboard)
    {
      Console.Write(buildString(uboard));
      Console.WriteLine("");
    }

    /// <summary>
    /// Simply used for debug purposes, displays the complete
    /// chess bitboard
    /// </summary>
    /// <returns></returns>
    public void printBoard()
    {
      Console.Write(ToString());
      Console.WriteLine("");
    }

    /// <summary>
    /// Converts the current position into a string representing the bit positions
    /// on the board.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      ChessPosition Pieces = (ChessPosition) coPieceTable[WHITEPIECES];
      Bitboard board = Pieces.Board;
      Pieces = (ChessPosition) coPieceTable[BLACKPIECES];
      board = board | Pieces.Board;
      return buildString(board);
    }
    /// <summary>
    /// Generates the bitboards that contain the pinned and pinning pieces.  Used to correctly
    /// generate legal moves for mate/stalemate detection.
    /// </summary>
    /// <param name="Attacker"></param>
    /// <param name="Attacked"></param>
    /// <returns></returns>
    public void getPins(ChessPosition Attacker, ChessPosition Attacked)
    {
      //-- Used for pin detection. --
      Attacking.DiagonalLR = getDiagonalLR(Attacked.King,0,0);
      Attacking.DiagonalRL = getDiagonalRL(Attacked.King,0,0);
      Attacking.Files = getFileMoves(Attacked.King,0,0);
      Attacking.Ranks = getRankMoves(Attacked.King,0,0);
      Bitboard DiagonalLR = getDiagonalLR(Attacked.King,Attacked.Board,Attacker.Board);
      Bitboard DiagonalRL = getDiagonalRL(Attacked.King,Attacked.Board,Attacker.Board);
      Bitboard Files = getFileMoves(Attacked.King,Attacked.Board,Attacker.Board);
      Bitboard Ranks = getRankMoves(Attacked.King,Attacked.Board,Attacker.Board);
      //---------------------------
      Bitboard attacked=0;
      Attacked.Pinned = 0;
      Attacked.Pinning = 0;
      Bitboard mask=0x8000000000000000;
      while(mask != 0)
      {
        attacked = 0;
        if((mask & Attacker.Queens) > 0)
        {
          if((mask & Attacking.Files) > 0)
          {
            attacked = getFileMoves(mask, Attacker.Board, Attacked.Board);
            attacked = attacked & Files & Attacked.Board;
          }
          else if((mask & Attacking.Ranks) > 0 )
          {
            attacked = getRankMoves(mask, Attacker.Board, Attacked.Board);
            attacked = attacked & Ranks & Attacked.Board;
          }
          else if((mask & Attacking.DiagonalLR) > 0)
          {
            attacked = getDiagonalLR(mask,Attacker.Board,Attacked.Board);
            attacked = attacked & DiagonalLR & Attacked.Board;
          }
          else if((mask & Attacking.DiagonalRL) > 0)
          {
            attacked = getDiagonalRL(mask,Attacker.Board,Attacked.Board);
            attacked = attacked & DiagonalRL & Attacked.Board;
          }
          if(attacked != 0)
          {
            Attacked.Pinned |= attacked;
            Attacked.Pinning |= mask;
          }

        }
        else if((mask & Attacker.Rooks) > 0)
        {
          if((mask & Attacking.Files) > 0)
          {
            attacked = getFileMoves(mask, Attacker.Board, Attacked.Board);
            attacked = attacked & Files & Attacked.Board;
          }
          else if((mask & Attacking.Ranks) > 0)
          {
            attacked = getRankMoves(mask, Attacker.Board, Attacked.Board);
            attacked = attacked & Ranks & Attacked.Board;
          }
          if(attacked != 0)
          {
            Attacked.Pinned |= attacked;
            Attacked.Pinning |= mask;
          }
        }
        else if((mask & Attacker.Bishops) > 0)
        {
          if((mask & Attacking.DiagonalLR) > 0)
          {
            attacked = getDiagonalLR(mask,Attacker.Board,Attacked.Board);
            attacked = attacked & DiagonalLR & Attacked.Board;
          }
          else if((mask & Attacking.DiagonalRL) > 0)
          {
            attacked = getDiagonalRL(mask,Attacker.Board,Attacked.Board);
            attacked = attacked & DiagonalRL & Attacked.Board;
          }
          if(attacked != 0)
          {
            Attacked.Pinned |= attacked;
            Attacked.Pinning |= mask;
          }
        }
        mask = mask >> 1;
      }
    }




    /// <summary>
    /// Performs the work of building our printable string representations of
    /// our bitboard.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public string buildString(Bitboard board)
    {
      int ndx=0;
      StringBuilder build = new StringBuilder(100);
      Bitboard mask=0x8000000000000000 >> (64 - coBitsToPrint);
      while(ndx!=coBitsToPrint)
      {
        Bitboard nboard = (mask&board);
        if( nboard > 0)
          build.Append('1');
        else
          build.Append('0');
        mask = mask >> 1;
        ndx++;
        if( ndx%8 == 0)
        {
          build.Append("\r\n");
        }
      }
      return build.ToString();
    }

    public string buildFEN()
    {      
      StringBuilder fen = new StringBuilder(100);
      int empty=0;
      int square=64;
      char piece = ' ';
      Bitboard mask=0x8000000000000000;
      ChessPosition white = getPosition(true);
      ChessPosition black = getPosition(false);      
      while(mask != 0)
      {        
        if((mask & white.Queens) > 0)
        {
          piece = 'Q';
        }
        else if((mask & white.Rooks) > 0)
        {
          piece = 'R';
        }
        else if((mask & white.Bishops) > 0)
        {
          piece = 'B';
        }
        else if((mask & white.Knights) > 0)
        {
          piece = 'N';
        }
        else if((mask & white.Pawns) > 0)
        {
          piece = 'P';
        }
        else if((mask & white.King) > 0)
        {
          piece = 'K';
        }
        else if((mask & black.Queens) > 0)
        {
          piece = 'q';
        }
        else if((mask & black.Rooks) > 0)
        {
          piece = 'r';
        }
        else if((mask & black.Bishops) > 0)
        {
          piece = 'b';
        }
        else if((mask & black.Knights) > 0)
        {
          piece = 'n';
        }
        else if((mask & black.Pawns) > 0)
        {
          piece = 'p';
        }
        else if((mask & black.King) > 0)
        {
          piece = 'k';
        }
        else
        {
          empty++;
          piece = ' ';
        }
        if( piece != ' ' )
        {
          if( empty > 0 )
          {
            fen.Append(empty.ToString());
            empty = 0;
          }
          fen.Append(piece);
        }
        mask = mask >> 1;
        square--;
        if(square % 8 == 0)
        {
          if( empty > 0 )
          {
            fen.Append(empty.ToString());
            empty = 0;
          }
          if( square != 0 )
            fen.Append('/');
        }
      }
      fen.Append(' ');
      fen.Append(Move.Color?'w':'b');
      fen.Append(' ');
      fen.Append(white.canCastleKingside?'K':'-');
      fen.Append(white.canCastleQueenside?'Q':'-');
      fen.Append(black.canCastleKingside?'k':'-');
      fen.Append(black.canCastleQueenside?'q':'-');
      return fen.ToString();
    }

    #endregion Methods

    #region IValidation

    /// <summary>
    /// This routine currently moves a piece by:
    ///  1. Ensures that there is a piece to move
    ///  2. Validates that the piece can move to where it is going.
    ///  3. Updates all associated bitboards when a valid move is made.
    ///  4. Checks for Checks / Mates / Stale Mates.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="fromSquare"></param>
    /// <param name="toSquare"></param>
    /// <returns></returns>
    public bool move(Chess.Pieces piece, string fromSquare, string toSquare)
    {
      bool bOk = false;
      // Select the correct hash tables based on side to move.
      ChessPosition Attacker = (ChessPosition) coPieceTable[coMove.Color?WHITEPIECES:BLACKPIECES];
      ChessPosition Attacked = (ChessPosition) coPieceTable[coMove.Color?BLACKPIECES:WHITEPIECES];
      // Make sure we're not mated already.
      if( ! Attacker.isKingMated & ! Attacked.isKingMated )
      {
        coFromSquare.setSquare(fromSquare);
        coToSquare.setSquare(toSquare);

        ChessPosition AttackerS = new ChessPosition(Attacker); //Save original positions for restoring on errors.
        ChessPosition AttackedS = new ChessPosition(Attacked);

        // First see if there is a piece to move, and if we are moving in our turn.
        if( (Attacker.Board & coFromSquare.getSquareMask()) != (Bitboard)0)
        {
          // Validate if you can move to the destination square.
          //  1. Test to see if we are moving to a square that already
          //  holds one of our own pieces.
          if( (Attacker.Board & coToSquare.getSquareMask()) == (Bitboard)0 )
          {
            // Ok we can move there but is it a legal move:
            // Check move available for this piece.
            Bitboard legalMoves = getLegalMoves(piece,Attacker,Attacked);
            if( (legalMoves & coToSquare.getSquareMask()) != (Bitboard)0)
            {
              if( (Attacked.Board & coToSquare.getSquareMask()) != (Bitboard)0 )
              {
                if((Attacked.Queens & coToSquare.getSquareMask()) != (Bitboard)0 )
                  Attacked.removePiece(Chess.Pieces.WQUEEN, coToSquare);
                else if((Attacked.Rooks & coToSquare.getSquareMask()) != (Bitboard)0 )
                  Attacked.removePiece(Chess.Pieces.WROOK, coToSquare);
                else if((Attacked.Bishops & coToSquare.getSquareMask()) != (Bitboard)0 )
                  Attacked.removePiece(Chess.Pieces.WBISHOP, coToSquare);
                else if((Attacked.Knights & coToSquare.getSquareMask()) != (Bitboard)0 )
                  Attacked.removePiece(Chess.Pieces.WKNIGHT, coToSquare);
                else if((Attacked.Pawns & coToSquare.getSquareMask()) != (Bitboard)0 )
                  Attacked.removePiece(Chess.Pieces.WPAWN, coToSquare);
              }
              else if( Enpasant == coToSquare.getSquareMask() )
              {
                if( piece == Chess.Pieces.WPAWN || piece == Chess.Pieces.BPAWN )
                {
                  int square = ((getBitColm(Attacked.Enpasant)-1) + ((getBitRow(Attacked.Enpasant)- 1) * 8));
                  Attacked.removePiece(Chess.Pieces.WPAWN, new BitboardSquare(square) );
                  EventUpdateBoard(Chess.Operation.DELETE, BitboardSquare.SquareToAlpha(square), null);

                }
              }
              // Clear as we are done with this value.
              Enpasant = 0;
              //Are we castling? The castle function moves the pieces and tests for incheck.
              switch( castle(piece, fromSquare, toSquare, Attacker, Attacked))
              {
                case Castling.CASTLING_NO:
                  // Not castling so now just make our normal moves and
                  // test for 
                  Attacker.movePiece(piece, coFromSquare, coToSquare);
                  //Is side to move's king in check?
                  if (isKingInCheck(coMove.Color, Attacker, Attacked) )
                  {
                    Attacker.copy(AttackerS);
                    Attacked.copy(AttackedS);
                  }
                  else
                  {
                    // Check to see if we want to promote a pawn.
                    promotePawn(piece, toSquare);
                    // Switch color to move.
                    coMove.Color = !coMove.Color;
                    bOk = true;
                  }
                  isKingCheckedMatedStaled(Attacker,Attacked);
                  break;
                case Castling.CASTLING_YES:
                  // Switch color to move.
                  coMove.Color = !coMove.Color;
                  bOk = true;
                  // Check to see if we have attacked the opponents king.
                  isKingCheckedMatedStaled(Attacker,Attacked);
                  break;
              }
            }
          }
        }
      }
      if( EventFinishedMove != null )
        EventFinishedMove();
      return bOk;
    }
    #region Events
    delegate void Promotion(bool color, string square, ref Chess.Pieces piece);
    event Promotion EventPromotion;
    delegate void KingIsMated();
    event KingIsMated EventKingIsMated;
    delegate void KingIsStaleMated();
    event KingIsStaleMated EventKingIsStaleMated;
    delegate void KingIsInCheck();
    event KingIsInCheck EventKingIsInCheck;
    delegate void KingIsFree();
    event KingIsFree EventKingIsFree;
    delegate void updateBoard(Chess.Operation op, string FromSquare, string ToSquare);
    event updateBoard EventUpdateBoard;
    delegate void finishedMove();
    event finishedMove EventFinishedMove;
    #endregion Events

    public void addEvents(IValidationEvents ievents)
    {
      EventPromotion += new Promotion(ievents.Promotion);
      EventKingIsMated += new KingIsMated(ievents.KingIsMated);
      EventKingIsStaleMated += new KingIsStaleMated(ievents.KingIsStaleMated);
      EventKingIsInCheck += new KingIsInCheck(ievents.KingIsInCheck);
      EventKingIsFree += new KingIsFree(ievents.KingIsFree);
      EventUpdateBoard += new updateBoard(ievents.updateBoard);
      EventFinishedMove += new finishedMove(ievents.finishedMove);
    }

    public void removeEvents(IValidationEvents ievents)
    {
      EventPromotion -= new Promotion(ievents.Promotion);
      EventKingIsMated -= new KingIsMated(ievents.KingIsMated);
      EventKingIsStaleMated -= new KingIsStaleMated(ievents.KingIsStaleMated);
      EventKingIsInCheck -= new KingIsInCheck(ievents.KingIsInCheck);
      EventKingIsFree -= new KingIsFree(ievents.KingIsFree);
      EventUpdateBoard -= new updateBoard(ievents.updateBoard);        
      EventFinishedMove -= new finishedMove(ievents.finishedMove);
    }
    #endregion IValidation

  }
}
