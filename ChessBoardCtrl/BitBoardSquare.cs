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
  /// This class represents the chess square number based on 0 through 63.
  /// Where 0 - 7 = a1 to g1 and 56 - 63 = a8 to h8.  It then translates
  /// the square number to the appropriate rotated bitboard square number.
  /// The idea here is to rid ourselves of the arrays needed to perform
  /// lookups, thus the amount of memory stored in the L2/L1 CPU cache.
  /// Which is based on internet discussions.
  /// Supported are L90, R90, L45, and R45.
  ///                    Normal                     A8H1                     A1H8
  ///   56 57 58 59 60 61 62 63  56 49 42 35 28 21 14  7  56  1 10 19 28 37 46 55
  ///   48 49 50 51 52 53 54 55  48 41 34 27 20 13  6 63  48 57  2 11 20 29 38 47
  ///   40 41 42 43 44 45 46 47  40 33 26 19 12  5 62 55  40 49 58  3 12 21 30 39
  ///   32 33 34 35 36 37 38 39  32 25 18 11  4 61 54 47  32 41 50 59  4 13 22 31
  ///   24 25 26 27 28 29 30 31  24 17 10  3 60 53 46 39  24 33 42 51 60  5 14 23
  ///   16 17 18 19 20 21 22 23  16  9  2 59 52 45 38 31  16 25 34 43 52 61  6 15
  ///    8  9 10 11 12 13 14 15   8  1 58 51 44 37 30 23   8 17 26 35 44 53 62  7
  ///    0  1  2  3  4  5  6  7   0 57 50 43 36 29 22 15   0  9 18 27 36 45 54 63
  ///
  ///   a8 b8 c8 d8 e8 f8 g8 h8  a8 b7 c6 d5 e4 f3 g2 h1  a8 b1 c2 d3 e4 f5 g6 h7
  ///   a7 b7 c7 d7 e7 f7 g7 h7  a7 b6 c5 d4 e3 f2 g1 h8  a7 b8 c1 d2 e3 f4 g5 h6
  ///   a6 b6 c6 d6 e6 f6 g6 h6  a6 b5 c4 d3 e2 f1 g8 h7  a6 b7 c8 d1 e2 f3 g4 h5
  ///   a5 b5 c5 d5 e5 f5 g5 h5  a5 b4 c3 d2 e1 f8 g7 h6  a5 b6 c7 d8 e1 f2 g3 h4
  ///   a4 b4 c4 d4 e4 f4 g4 h4  a4 b3 c2 d1 e8 f7 g6 h5  a4 b5 c6 d7 e8 f1 g2 h3
  ///   a3 b3 c3 d3 e3 f3 g3 h3  a3 b2 c1 d8 e7 f6 g5 h4  a3 b4 c5 d6 e7 f8 g1 h2
  ///   a2 b2 c2 d2 e2 f2 g2 h2  a2 b1 c8 d7 e6 f5 g4 h3  a2 b3 c4 d5 e6 f7 g8 h1
  ///   a1 b1 c1 d1 e1 f1 g1 h1  a1 b8 c7 d6 e5 f4 g3 h2  a1 b2 c3 d4 e5 f6 g7 h8
  /// </summary>
  public class BitboardSquare
  {
    /// <summary>
    /// Needed to simply allow us to obtain a lock 
    /// while we initialize our static objects that live
    /// for the life of the program.
    /// </summary>
    static string coLock="";
    /// <summary>
    /// Static array of our bitboard masks.
    /// </summary>
    static Bitboard[] coSquareMask;
    /// <summary>
    /// Mapping of algebraic square to numbers.
    /// </summary>
    static Hashtable coAlphaToSquare;
    static Hashtable coSquareToAlpha;

    /// <summary>
    /// Initialize our square to the first one.
    /// </summary>
    public BitboardSquare()
    {
      Square = 0;
      initMasks();
    }

    /// <summary>
    /// Initialize our square to a specific location.
    /// </summary>
    /// <param name="square"></param>
    public BitboardSquare(int square)
    {
      Square = square;
      initMasks();
    }
    /// <summary>
    /// Create the bitboard for single bit look up based on a square index.
    /// </summary>
    static private void initMasks()
    {
      lock(coLock)
      {
        if( coAlphaToSquare == null )
        {
          coSquareMask = new Bitboard[64];
          coAlphaToSquare = new Hashtable();
          coSquareToAlpha = new Hashtable();
          // Setup our mapping of square number to an associated bitboard
          //  where only the squares bit is set.
          int ndx=0,row,colm;
          Bitboard mask;
          for(row=0; row<8; row++)
          {
            mask= ((Bitboard)0x0000000000000080) << (row*8);
            for(colm=0; colm<8; colm++)
            {
              coSquareMask[ndx++]=mask>>colm;
            }
          }

          StringBuilder builder=new StringBuilder();
          char [] mapcol = { 'a','b','c','d','e','f','g','h' };
          char [] maprow = { '1','2','3','4','5','6','7','8' };  
          // Initialize our algebraic chess notation to square mapping.
          //  Example a8 = 56, a1 = 0, squares numbered 0-63
          ndx=0;
          for(row=0; row<8; row++)
          {
            for(colm=0; colm<8; colm++)
            {
              builder.Append(mapcol[colm]);
              builder.Append(maprow[row]);
              coSquareToAlpha[ndx] = builder.ToString();
              coAlphaToSquare[builder.ToString()]=ndx++;
              builder.Length=0;
            }
          }
        }
      }
    }

    static public string SquareToAlpha(int square)
    {
      return (string)coSquareToAlpha[square];
    }
    static public int AlphaToSquare(string square)
    {
      return (int)coAlphaToSquare[square];
    }
    
    public int getSquare(string square){ return (int)coAlphaToSquare[square]; }
    public Bitboard getMask(string square){ return coSquareMask[(int)coAlphaToSquare[square]];}
    public Bitboard getMask(int square){ return coSquareMask[square]; }
    public Bitboard getSquareMask(){ return coSquareMask[coSquare]; }
    public Bitboard getA8H1Mask(){ return coSquareMask[coA8H1Square]; }
    public Bitboard getA1H8Mask(){ return coSquareMask[coA1H8Square]; }
    public Bitboard getR90Mask(){ return coSquareMask[coR90Square]; }
    public Bitboard getL90Mask(){ return coSquareMask[coL90Square]; }

    /// <summary>
    /// Represents a L45 bitboard.
    /// </summary>
    public int A8H1Square
    {get{ return coA8H1Square;}}
    private int coA8H1Square;
    /// <summary>
    /// Represents a R45 bitboard.
    /// </summary>
    public int A1H8Square
    {get{ return coA1H8Square;}}
    private int coA1H8Square;
    /// <summary>
    /// Represents a L90 bitboard.
    /// </summary>
    public int L90Square
    {get{ return coL90Square;}}
    private int coL90Square;
    /// <summary>
    /// Represents a R90 bitboard.
    /// </summary>
    public int R90Square
    {
      get{ return coR90Square;}
    }
    private int coR90Square;

    public void setSquare(string square)
    {
      Square = (int)coAlphaToSquare[square];
    }

    public int Square
    {
      set
      {
        coSquare = value;
        coA8H1Square = (coSquare + (coSquare << 3)) & 63;
        coA1H8Square = (coSquare - (coSquare << 3)) & 63;
        coL90Square  = ((coSquare & 7) << 3) + (coSquare >> 3);
        coR90Square  = ((coSquare & 7) << 3) + (coSquare >> 3) ^ 7;
      }
      get{ return coSquare; }
    }
    private int coSquare;
  }
}
