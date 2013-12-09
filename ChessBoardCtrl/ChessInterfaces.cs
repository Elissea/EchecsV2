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

  #region ValidationEngine
  /// <summary>
  /// Defines the interfaces needed to determine the
  /// rules for a game of chess or chess variant.
  /// </summary>
  public interface IValidation
  {
    /// <summary>
    /// Request that a piece be moved on the board.
    /// </summary>
    /// <param name="piece">Piece to move</param>
    /// <param name="fromSquare">Source square in algerbraic notation</param>
    /// <param name="toSquare">Destination square in algerbraic notation</param>
    /// <returns></returns>
    bool move(Chess.Pieces piece, string fromSquare, string toSquare);

    /// <summary>
    /// Used to register event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void addEvents(IValidationEvents ievents);
    /// <summary>
    /// Used to remove event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void removeEvents(IValidationEvents ievents);
  }

  /// <summary>
  /// Interface used to define the events that must be handled by the class
  /// using the IValidation interface to ensure all interaction between the
  /// two are communicated correctly.
  /// </summary>
  public interface IValidationEvents
  {
    void Promotion(bool color, string square, ref Chess.Pieces piece);
    void KingIsMated();
    void KingIsStaleMated();
    void KingIsInCheck();
		void KingIsFree();
    void finishedMove();
    void updateBoard(Chess.Operation op, string FromSquare, string ToSquare);
  }
  #endregion

  #region PositionParser
  /// <summary>
  /// An interface that defines the minimal functionality that must
  ///   be implemented to allow the setup of a chess position. 
  ///   Normally used for a FEN or EDP parser that needs to setup
  ///   a position in multiple places such as a validation engine
  ///   and the bitmap dislay of the pieces.
  /// </summary>
  public interface IPosition
  {
    /// <summary>
    /// Used to parse out a position calling the events as needed
    ///   to inform the using class of what's happening.
    /// </summary>
    /// <param name="str"></param>
    void parse(string str);
    /// <summary>
    /// Used to register event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void addEvents(IPositionEvents ievents);
    /// <summary>
    /// Used to remove event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void removeEvents(IPositionEvents ievents);
  }
  /// <summary>
  /// Defines the events that the IPosition class will need to fire to allow
  ///   the proper setup of a chess position.
  /// </summary>
  public interface IPositionEvents
  {
    /// <summary>
    /// Used to inform a subscriber that a new piece needs to be
    ///  placed onto a square.
    ///  </summary>
    /// <param name="piece"></param>
    /// <param name="square"></param>
    void placePiece(Chess.Pieces piece,int square);
    /// <summary>
    /// Used to inform a subscriber who's move it is.    
    /// </summary>
    /// <param name="bColor">True for white else false for black</param>
    void setColor(bool bColor);
    /// <summary>
    /// Used to inform a subscriber the state of castling.
    /// </summary>
    /// <param name="WK"></param>
    /// <param name="WQ"></param>
    /// <param name="BK"></param>
    /// <param name="BQ"></param>
    void setCastling(bool WK, bool WQ, bool BK, bool BQ);
    /// <summary>
    /// Called when we are done parsing/setting up the position.
    /// </summary>
    void finished();
  }
  #endregion

  #region GameParser

  /// <summary>
  /// This interface defines the minimum functionality that a game 
  /// parser must implement.  It does not have to have all the functionality
  /// present, but the interface must be delcared.
  /// <seealso cref="IGameParserEvents"/>
  /// </summary>
  public interface IGameParser
  {
    /// <summary>
    /// Name of the fileto parse.
    /// </summary>
    string Filename{ get;set;}
    /// <summary>
    /// Show what STATE with in the parser 
    /// that is currently active.
    /// </summary>
    GameParserState.State State{get;set;}
    /// <summary>
    /// Holds any header/game information that describes attributes
    /// of the game such as: Date, Players, ELO, ECO, Ratings, and others.
    /// Will contain valid data during the IGameParserEvents.tagParsed event.
    /// </summary>
    string Tag{get;}
    /// <summary>
    /// Will contain valid data during the IGameParserEvents.tagParsed event which
    /// represents the data associated with the game tag.  Also during the following
    /// events:
    ///   IGameParserEvents.nagParsed
    ///   IGameParserEvents.moveParsed
    ///   IGameParserEvents.commentParsed
    /// </summary>
    string Value{ get; }
    /// <summary>
    /// Executes the main loop for parsing out the games.
    /// </summary>
    void parse();
    /// <summary>
    /// Used to register event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void addEvents(IGameParserEvents ievents);
    /// <summary>
    /// Used to remove event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void removeEvents(IGameParserEvents ievents);
  }
  /// <summary>
  /// Defines all of the events that a game parser may call to
  /// listeners so that they may build a chess game.
  /// </summary>
  public interface IGameParserEvents
  {
    /// <summary>
    /// Called when a new game in the parsed data file has been located.
    /// </summary>
    /// <param name="iParser"></param>
    void newGame(IGameParser iParser);
    /// <summary>
    /// Called when the parser is entering a variation.
    /// </summary>
    /// <param name="iParser"></param>
    void enterVariation(IGameParser iParser);
    /// <summary>
    /// Calle when the parser is leaving a variation.
    /// </summary>
    /// <param name="iParser"></param>
    void exitVariation(IGameParser iParser);
    /// <summary>
    /// Called when the parser is completely finished parsing a file.
    /// </summary>
    /// <param name="iParser"></param>
    void finished(IGameParser iParser);
    /// <summary>
    /// Called when the parser has determined game header information
    /// such as Player names, ELO, ECO, Dates, and others.
    /// <seealso cref="PGN standards"/>
    /// </summary>
    /// <param name="iParser"></param>
    void tagParsed(IGameParser iParser);
    /// <summary>
    /// Called when the parser has determined a NAG comments based on PGN
    /// standards.
    /// </summary>
    /// <param name="iParser"></param>
    void nagParsed(IGameParser iParser);
    /// <summary>
    /// Called when the parser has determined a move is present.
    /// </summary>
    /// <param name="iParser"></param>
    void moveParsed(IGameParser iParser);
    /// <summary>
    /// Called when the parser has determined a comment is present.
    /// </summary>
    /// <param name="iParser"></param>
    void commentParsed(IGameParser iParser);
  }
  /// <summary>
  /// Used for a game parser state machine.
  /// </summary>
  public class GameParserState
  {
    public enum State{ HEADER, NUMBER, COLOR, WHITE, BLACK, COMMENT, NAGS };  
  }
  #endregion

  #region GameNavigation
  /// <summary>
  /// Defines the minimal functionality that is required to navigate
  ///   through a complete chess game.  This includes handling
  ///   variations and retrieving informations such as nags, comments,
  ///   tags, and any other information non game related.
  /// </summary>
  public interface IGameNavigation
  {


    

    /// <summary>
    /// Used to register event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void addEvents(IGameNavigationEvents ievents);
    /// <summary>
    /// Used to remove event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void removeEvents(IGameNavigationEvents ievents);
  }
  public interface IGameNavigationEvents
  {

  }

  #endregion
}
