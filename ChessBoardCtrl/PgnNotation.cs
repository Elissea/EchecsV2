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
using System.Text.RegularExpressions;

namespace ChessLibrary
{

  /// <summary>
  /// NOTHING / MAY DELETE LATER??
  /// </summary>
  public class PgnMove
  {
    public string whitemove;
    public string blackmove;
    public PgnMove variation;
  }

  /// <summary>
  /// Summary description for PgnNotation.
  /// NOTHING / MAY DELETE LATER??
  /// </summary>
  public class PgnNotation
  {
    string [] coTags = {
                         "Event","Site","Date","Round","White","Black","Result",
                         "WhiteNA","BlackNA","WhiteType","BlackType","EventDate",
                         "EventSponsor","Section","Board","Opening","Variation",
                         "SubVariation","ECO","NIC","Time","UTCTime","UTCDate",
                         "TimeControl","Annotator","Mode","PlyCount",
                         "WhiteElo","BlackElo"
                       };
    public PgnNotation()
    {
    }
  }

 


  /// <summary>
  /// A state machine parser which handles the disection of a 
  /// PGN standard game file
  /// </summary>
  public class PgnData : IGameParser
  {
    Regex coRegex;
    public delegate void newGame(IGameParser iParser);
    public event newGame EventNewGame;
    public delegate void enterVariation(IGameParser iParser);
    public event enterVariation EventEnterVariation;
    public delegate void exitVariation(IGameParser iParser);
    public event exitVariation EventExitVariation;
    public delegate void finished(IGameParser iParser);
    public event finished EventFinished;
    public delegate void tagParsed(IGameParser iParser);
    public event tagParsed EventTagParsed;
    public delegate void nagParsed(IGameParser iParser);
    public event nagParsed EventNagParsed;
    public delegate void moveParsed(IGameParser iParser);
    public event moveParsed EventMoveParsed;
    public delegate void commentParsed(IGameParser iParser);
    public event commentParsed EventCommentParsed;

    string coData;

//    System.Text.StringBuilder coComment;
    System.Text.StringBuilder coValue;
//    System.Text.StringBuilder coNag;
    bool coNextGame;
    int coPeriodCount;
    
    public GameParserState.State State
    {
      get{ return coState; }
      set{ coState = value;}
    }
    GameParserState.State coState;
    GameParserState.State coPrevState;
    public string Tag
    {
      get{ return coTag; }
    }
    string coTag;
    public string Value
    { 
      get{ return coValue.ToString(); }    
    }
    
    public string Filename
    {
      get{ return coFilename;}
      set{ coFilename=value;}
    }
    string coFilename;

    public PgnData()
    {
      coRegex = new Regex("^\\[([A-Za-z]*) \"(.*)\"",RegexOptions.Compiled);
      coValue = new System.Text.StringBuilder();
      coState = GameParserState.State.HEADER;
      coPeriodCount = 0;
    }

    public void addEvents(IGameParserEvents ievents)
    {
      EventNewGame += new newGame(ievents.newGame);
      EventEnterVariation += new enterVariation(ievents.enterVariation);
      EventExitVariation += new exitVariation(ievents.exitVariation);
      EventFinished +=new finished(ievents.finished);
      EventTagParsed +=new tagParsed(ievents.tagParsed);
      EventNagParsed +=new nagParsed(ievents.nagParsed);
      EventMoveParsed +=new moveParsed(ievents.moveParsed);
      EventCommentParsed += new commentParsed(ievents.commentParsed);
    }

    public void removeEvents(IGameParserEvents ievents)
    {
      EventNewGame -= new newGame(ievents.newGame);
      EventEnterVariation -= new enterVariation(ievents.enterVariation);
      EventExitVariation -= new exitVariation(ievents.exitVariation);
      EventFinished -=new finished(ievents.finished);
      EventTagParsed -=new tagParsed(ievents.tagParsed);
      EventNagParsed -=new nagParsed(ievents.nagParsed);
      EventMoveParsed -=new moveParsed(ievents.moveParsed);
      EventCommentParsed += new commentParsed(ievents.commentParsed);
    }
    
    public void parse()
    {
      System.IO.StreamReader reader = new System.IO.StreamReader(coFilename);

      long position = 0;
      for( coData   = reader.ReadLine(); 
           coData  != null;            
           coData   = reader.ReadLine())
      {
        if( coData.Length > 0 )
        {
          if( Regex.IsMatch(coData,"^\\[") )
          {
            if( coNextGame == false )
            {

              callEvent(coState);

              coNextGame = true;
              if( EventNewGame != null )
              {
                EventNewGame(this);
              }
            }
            coState = GameParserState.State.HEADER;
            parseTag(coData);
            coValue.Length = 0;
          }
          else
          {
            if( coNextGame )
            {
              coNextGame = false;
            }            
            parseDetail(coData);
          }
          position += coData.Length + 2;
        }
      }
      callEvent(coState);
      reader.Close();
      if( EventFinished != null )
        EventFinished(this);
    }

    public void parseDetail(string line)
    {
      foreach(char aChar in line)
      {
        // Handle any special processing of our text.
        switch(coState)
        {
          case GameParserState.State.COMMENT:
            if( aChar == '}' )
            {
              callEvent(coState);
              coState = coPrevState;
            }
            else
              coValue.Append(aChar);
            break;

          case GameParserState.State.NAGS:            
            if( aChar >= '0' && aChar <= '9' )
              coValue.Append(aChar);                              
            else
            {
              callEvent(coState);
              coState = coPrevState;
              handleChar(aChar);
            }            
            break;
          case GameParserState.State.COLOR:
            if( aChar == '.' )
            {
              coPeriodCount++;
            }
            else
            {
              coValue.Length = 0;
              if( coPeriodCount == 1 )
                coState = GameParserState.State.WHITE;
              else if( coPeriodCount > 1 )
                coState = GameParserState.State.BLACK;
              handleChar(aChar);
              coPeriodCount=0;
            }
            break;

          default:
            handleChar(aChar);
            break;
        }
      }
      // Ensure we add a space between comment lines that are broken appart.
      if( coState == GameParserState.State.COMMENT )
        coValue.Append( ' ' );
      else
        callEvent(coState);
    }

    void handleChar(char aChar)
    {
      switch( aChar )
      {
        case '{':
          callEvent(coState);
          coPrevState = coState;
          coState = GameParserState.State.COMMENT;
          break;
        case '(':
          if( EventEnterVariation != null )
            EventEnterVariation(this);
          break;
        case ')':
          if( EventExitVariation != null )
            EventExitVariation(this);
          break;
        case ' ':
          // Only if we have some data do we want to fire an event.
          callEvent(coState);
          break;
        case '.':
          // We may have come across 6. e4 6... d5 as in our example data.
          coState = GameParserState.State.NUMBER;
          callEvent(coState);
          coPeriodCount=1;
          break;
        case '$':
          callEvent(coState);
          coPrevState = coState;
          coState = GameParserState.State.NAGS;
          break;
        default:
          coValue.Append(aChar);
          break;
      }
    }

    /// <summary>
    /// Calls the correct event based on the parsers state.
    /// </summary>
    /// <param name="state"></param>
    void callEvent(GameParserState.State state)
    {
      if( coValue.Length > 0 )
      {
        switch(state)
        {
          case GameParserState.State.COMMENT:
            if( EventCommentParsed != null )
              EventCommentParsed(this);
            break;
          case GameParserState.State.NAGS:
            if( EventNagParsed != null )
              EventNagParsed(this);
            break;
          case GameParserState.State.NUMBER:
            if( EventMoveParsed != null )
              EventMoveParsed(this);
            coState = GameParserState.State.COLOR;
            break;
          case GameParserState.State.WHITE:
            if( EventMoveParsed != null )
              EventMoveParsed(this);
            coState = GameParserState.State.BLACK;
            break;
          case GameParserState.State.BLACK:
            if( EventMoveParsed != null )
              EventMoveParsed(this);         
            coState = GameParserState.State.NUMBER;
            break;
        }
      }

      // Always clear out our data as the handler should have used this value during the event.
      coValue.Length = 0;
    }

    /// <summary>
    /// Parses out the PGN tag and value from the game header.
    /// </summary>
    /// <param name="line"></param>
    public void parseTag(string line)
    {
      int nleft = line.IndexOf('"');
      int nright = line.LastIndexOf('"');

      System.Text.RegularExpressions.Match regMatch = coRegex.Match(line);
      if( regMatch.Groups.Count == 3 )
      {
        // Call the events with the tag and tag value.
        if( EventTagParsed != null )
        {
          coTag = regMatch.Groups[1].Value;
          coValue.Length = 0;
          coValue.Append(regMatch.Groups[2].Value);
          EventTagParsed(this);
        }
      }
    }
  }

}
