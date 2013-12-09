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
namespace ChessLibrary
{
  /// <summary>
  /// Represents a standard Algebraic notation chess move.
  /// </summary>
  public class ChessMove
  {
    public const bool  WHITE=true;
    public const bool  BLACK=false;

    public bool Color;
    public bool Mate;
    public bool Check;
    public bool Capture;
    public int  Number;
    public int  Question;
    public int  Exclamation;
    public string Move;
    public char Promotion;
    public char Piece;
    public char Qualify;

    public ChessMove()
    {
      clear();
    }
    public void switchColor()
    {
      Color = !Color;
    }
    /// <summary>
    /// Set the move object to it's initial stabel state.
    /// </summary>
    public void clear()
    {
      Color=WHITE;
      Mate=false;
      Check=false;
      Capture=false;
      Question=0;
      Exclamation=0;
      Move="";
      Promotion=' ';
      Piece=' ';
      Qualify=' ';
      Number=0;
    }
    /// <summary>
    /// Breaks appart the chess move into our object
    /// for easier manipulation.
    /// </summary>
    /// <param name="chessmove"></param>
    public void parseMove(string chessmove)
    {
      char achar;
      StringBuilder build = new StringBuilder();
      chessmove = chessmove.ToUpper();
      clear();
      Piece = 'P';
      for(int ndx=chessmove.Length-1; ndx >=0; ndx-- )
      {
        achar = chessmove[ndx];
        switch( achar )
        {
          case '+':
            Check=true;
            break;
          case '!':
            Exclamation++;
            break;
          case '#':
            Mate=true;
            break;
          case '?':
            Question++;
            break;
          case '=':
            Promotion = build.ToString()[0];
            build.Length = 0;
            break;
          case 'X':
            Capture = true;
            break;
          case 'K':
            if(ndx == 0)
            {
              Piece = achar;
            }
            else
            {
              build.Append(achar);
            }
            break;
          case 'Q':
            if(ndx == 0)
            {
              Piece = achar;
            }
            else
            {
              build.Append(achar);
            }
            break;
          case 'B':
            if(ndx == 0)
            {
              Piece = achar;
            }
            else
            {
              build.Append(achar);
            }
            break;
          case 'N':
            if(ndx == 0)
            {
              Piece = achar;
            }
            else
            {
              build.Append(achar);
            }
            break;
          case 'R':
            if(ndx == 0)
            {
              Piece = achar;
            }
            else
            {
              build.Append(achar);
            }
            break;
          default:
            build.Append(achar);
            break;
        }
      }
      // Get remaining command and reverse it.
      chessmove = build.ToString();
      build.Length=0;
      for(int ndx=chessmove.Length-1; ndx >=0; ndx-- )
      {
        achar = chessmove[ndx];
        build.Append(achar);
      }
      chessmove = build.ToString().ToLower();
      if(chessmove.Length == 3)
      {
        Qualify = chessmove[0];
        chessmove = chessmove.Substring(1,2);
      }

      Move = chessmove;
    }
    /// <summary>
    /// Converts our chess object into standard algebraic notation.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      StringBuilder build = new StringBuilder();
      if(Piece!='P')
        build.Append(Piece);
      if(Qualify!=' ')
        build.Append(Qualify);
      if(Capture)
        build.Append("x");
      build.Append(Move);

      if(Promotion!=' ')
      {
        build.Append('=');
        build.Append(Promotion);
      }
      if(Check)
        build.Append('+');

      if(Mate)
        build.Append('#');
      return build.ToString();
    }

  }

}
