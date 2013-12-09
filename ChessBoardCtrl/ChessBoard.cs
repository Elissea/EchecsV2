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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
// Additional packages
using System.Text;

using ChessLibrary;

namespace ChessBoardCtrl
{
	/// <summary>
	/// Summary description for ChessBoard.
	/// </summary>
  public class ChessBoard : System.Windows.Forms.UserControl, IValidationEvents
  {
    private System.ComponentModel.IContainer components;
    /// <summary>
    /// True while we are dragging a chess piece around.
    /// </summary>
    bool isDragging;
    /// <summary>
    /// The chess board is flopped around so black is on the bottom.
    /// </summary>
    public bool isFlipped
    {
      set
			{ 
				coChessBitmap.isFlipped = value; 
				coChessBitmap.initializeBoard(); 
				coChessBitmap.isRedraw=true;
				Invalidate();
			}      
			get{ return coChessBitmap.isFlipped; }
    }
    public bool isValidating
    {
      set
      { 
        coValidateMoves = value;
        // Makesure bitboards get set.
        // Need to set castling privilages.
        FENnotation = coChessBitmap.getFEN() + " " + 
                      (coBitBoard.Move.Color?"w":"b") + " " +
                      "KQkq";

      }
      get{ return coValidateMoves; }
    }
    bool coValidateMoves;
    /// <summary>
    /// If true then left click picks up a piece an holds onto it.
    /// When left click again the piece is put down.  Property defined
    /// so as to expose it to the outside world.
    /// </summary>
    public bool isHoldingPiece
    {
      get{ return coHoldingPiece;}
      set{ coHoldingPiece = value;}
    }
    bool coHoldingPiece;

    /// <summary>
    /// Used to set the chess position in the graphical and bitboards.
    /// </summary>
    public string FENnotation
    {
      set
      {
        try
        {  
          coChessBitmap.clearBoard();
          coBitBoard.clearBoard();
          coFen.parse(value);
          coChessBitmap.isRedraw = true;
          Invalidate();
        }
        catch (Exception ex)
        {
					MessageBox.Show(ex.Message, "Error in FEN string");
        }
      }
    }
    /// <summary>
    /// Image list used for chess piece display.  You must specify the transparency color
    /// When you add your images to the list so that the piece drag and drop properly draws
    /// your display.  The order of the pieces in the list must be as follows:
    /// White: King (0) Queen (1) Rook (2) Bishop (3) Knight (4) Pawn (5)
    /// Black: King (6) Queen (7) Rook (8) Bishop (9) Knight (10) Pawn (11)
    /// </summary>
    public System.Windows.Forms.ImageList ChessImages
    {
      set{imagePieces = value;}
      get{return imagePieces;}
    }
    // Holds the images of the pieces.
    private System.Windows.Forms.ImageList imagePieces;
    /// <summary>
    /// Image list used for chess board display.  
    /// The order of the squares in the list must be as follows:
    /// White square (0) Black square (1) 
    /// </summary>
    public System.Windows.Forms.ImageList BoardImages
    {
      set{imageBoard = value;}
      get{return imageBoard;}
    }
    // Holds the images of the board squares.
    private System.Windows.Forms.ImageList imageBoard;
    /// <summary>
    /// Holds our cursors for piece placement.
    /// </summary>
    Hashtable coChessCursors;
    /// <summary>
    /// Source square for the piece that is going to move.
    /// </summary>
    BoardSquare coSourceSquare;
    /// <summary>
    /// Destination square for the piece that is going to move.
    /// </summary>
    BoardSquare coTargetSquare;
		Rectangle coMovingSquare;
    FenNotation coFen;
    Point coPoint;
    Chess.Pieces coPiece;

    public delegate bool PieceMoved(Chess.Pieces piece, string from, string to);
    public event PieceMoved EventPieceMoved;
    /// <summary>
    /// Used to generate our chessboard graphics.
    /// </summary>
    ChessBitmap coChessBitmap;
    /// <summary>
    /// Used to generate our move validation.
    /// </summary>
    StandardValidation coBitBoard;
    bool promotion;


    public ChessBoard()
    {
      promotion = false;

      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      isDragging = false;
      isHoldingPiece = true;
      coValidateMoves = true;
      coBitBoard = new StandardValidation();
      coBitBoard.addEvents(this);

      coChessCursors = new Hashtable();    
      try
      {
/*
        coChessCursors[Chess.Pieces.WKING] = new Cursor(GetType(), "cursors.wking.cur");
        coChessCursors[Chess.Pieces.WQUEEN] = new Cursor(GetType(), "cursors.wqueen.cur");
        coChessCursors[Chess.Pieces.WROOK] = new Cursor(GetType(), "cursors.wrook.cur");
        coChessCursors[Chess.Pieces.WBISHOP] = new Cursor(GetType(), "cursors.wbishop.cur");
        coChessCursors[Chess.Pieces.WKNIGHT] = new Cursor(GetType(), "cursors.wknight.cur");
        coChessCursors[Chess.Pieces.WPAWN] = new Cursor(GetType(), "cursors.wpawn.cur");
        coChessCursors[Chess.Pieces.BKING] = new Cursor(GetType(), "cursors.bking.cur");
        coChessCursors[Chess.Pieces.BQUEEN] = new Cursor(GetType(), "cursors.bqueen.cur");
        coChessCursors[Chess.Pieces.BROOK] = new Cursor(GetType(), "cursors.brook.cur");
        coChessCursors[Chess.Pieces.BBISHOP] = new Cursor(GetType(), "cursors.bbishop.cur");
        coChessCursors[Chess.Pieces.BKNIGHT] = new Cursor(GetType(), "cursors.bknight.cur");
        coChessCursors[Chess.Pieces.BPAWN] = new Cursor(GetType(), "cursors.bpawn.cur");
*/
        coChessCursors[Chess.Pieces.OPENHAND] = new Cursor(GetType(), "cursors.OpenHand.cur");
        coChessCursors[Chess.Pieces.CLOSEDHAND] = new Cursor(GetType(), "cursors.ClosedHand.cur");
      }
      catch(Exception ex)
      {
        ex.Message.ToUpper();
      }
     
      // Create our graphical chessboard
      coChessBitmap = new ChessBitmap(imageBoard,imagePieces);
      // Assign the size of the board.
      coChessBitmap.ClientRectangle = ClientRectangle;
      // Setup the defaults.
      coChessBitmap.initializeBoard();      
      // Instantiate and set the event handler so a FEN string 
      //  may be used to setup a position on the graphics and
      //  bit board.
      coFen = new FenNotation();
      // Assign our graphical and bit board to the FEN class so that we can
      //  capture the events.
			coFen.addEvents(coChessBitmap);
      coFen.addEvents(coBitBoard);
      // A new starting position.
      FENnotation = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq";
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public Cursor getCursor(Chess.Pieces piece)
    {
      return (Cursor)coChessCursors[piece];
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if( components != null )
          components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChessBoard));
      this.imageBoard = new System.Windows.Forms.ImageList(this.components);
      this.imagePieces = new System.Windows.Forms.ImageList(this.components);
      // 
      // imageBoard
      // 
      this.imageBoard.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
      this.imageBoard.ImageSize = new System.Drawing.Size(160, 160);
      this.imageBoard.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageBoard.ImageStream")));
      this.imageBoard.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // imagePieces
      // 
      this.imagePieces.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
      this.imagePieces.ImageSize = new System.Drawing.Size(46, 46);
      this.imagePieces.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagePieces.ImageStream")));
      this.imagePieces.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // ChessBoard
      // 
      this.Name = "ChessBoard";
      this.Load += new System.EventHandler(this.ChessBoard_Load);

    }
    #endregion

    public bool Color
    {
      get{ return coBitBoard.Move.Color; }
    }

    /// <summary>
    /// Responsible for drawing our chess board onto our screen.
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaint( PaintEventArgs pe )
    {
      base.OnPaint(pe);
      // Draw the chess board in it's current state.
      coChessBitmap.draw(pe.Graphics);
      // If we're moving a piece then we need to draw it's place on the
      //  board as well.
      if( !coMovingSquare.IsEmpty )
      {
        pe.Graphics.DrawImage(coSourceSquare.PieceImage, coMovingSquare);
      }
    }

    /// <summary>
    /// Handles the rescaling of our chess board based on the current clients size.
    /// </summary>
    /// <param name="se"></param>
    protected override void OnSizeChanged(System.EventArgs se)
    {
      base.OnSizeChanged(se);
      coChessBitmap.ClientRectangle = this.ClientRectangle;// Square = getSquareSize();
      coChessBitmap.initializeBoard();
      coChessBitmap.isRedraw = true;
      Invalidate();
    }
    
    /// <summary>
    /// Just makes sure that when we enter the chess board with the mouse we change
    /// the cursor to our hand that grabs stuff.
    /// </summary>
    /// <param name="mouse"></param>
    protected override void OnMouseEnter(EventArgs mouse )
    {
      Cursor.Current = (Cursor) coChessCursors[Chess.Pieces.OPENHAND]; //Cursors.Hand;
    }

    protected override void OnMouseLeave(EventArgs mouse )
    {
      //Cursor.Current = Cursors.Default;
    }

    /// <summary>
    /// User has clicked the mouse so lets found out if he is dragging
    /// a piece/placing a piece/or simply droping a new piece.
    /// </summary>
    /// <param name="mouse"></param>
    protected override void OnMouseDown(MouseEventArgs mouse )
    {
      switch (mouse.Button) 
      {
        case MouseButtons.Left:
          // Ok, user is moving or dropping a piece onto the board.
          if( isHoldingPiece && isDragging )
            dropPiece(mouse);
          else if( !isDragging )
            pickupPiece(mouse);
          break;
        case MouseButtons.Right:
          // Put code here for placing pieces.
          break;
        case MouseButtons.Middle:
          break;
      }
    }
    /// <summary>
    /// User released the mouse so lets either drop the piece or lock the piece
    /// into drag mode.
    /// </summary>
    /// <param name="mouse"></param>
    protected override void OnMouseUp(MouseEventArgs mouse )
    {
      if( !isHoldingPiece )
        dropPiece(mouse);
    }

    /// <summary>
    /// Handles setting the offsets used to paint the moving chess piece.
    /// </summary>
    /// <param name="mouse"></param>
    protected override void OnMouseMove(MouseEventArgs mouse )
    {
      if( isDragging )
      {
        // Needed to keep cursor set to closed hand.  Need to figure out why
        //  this is needed as if you set the flag isHoldingPiece to false the
        //  cursor remains a closed hand.  Which means your holding down the
        //  left mouse button.
        if( isHoldingPiece )
          Cursor.Current = (Cursor) coChessCursors[Chess.Pieces.CLOSEDHAND];
        // We have a valid point
        if( !coPoint.IsEmpty )
        {
          // We have a valid square
          if( !coMovingSquare.IsEmpty )
          {
            coMovingSquare.Offset(mouse.X - coPoint.X, mouse.Y - coPoint.Y);
            coPoint.X = mouse.X;
            coPoint.Y = mouse.Y;
            Invalidate();
          }
        }
      }
    }

    /// <summary>
    /// This routine will grab ahold of a piece and allow us to move it around
    /// the chess board.
    /// </summary>
    /// <param name="mouse"></param>
    public void pickupPiece(MouseEventArgs mouse)
    {
      if( !isDragging )
      {
        coSourceSquare =  coChessBitmap.findSquare(mouse.X,mouse.Y);
        // Found a source square.
        if( coSourceSquare != null )
        {
          // Do we have a hold of one of the little dudes?
          if( coSourceSquare.Piece != Chess.Pieces.NONE)
          {
            // Let everyone know that we are dragging the little dude around.
            isDragging = true;
            coMovingSquare = coSourceSquare.Square;
            coPoint = new Point(coMovingSquare.X + coMovingSquare.Width / 2, coMovingSquare.Y + coMovingSquare.Height / 2);
            coPiece = coSourceSquare.Piece;
            coSourceSquare.Piece = Chess.Pieces.NONE;
            Graphics offScreenDC = Graphics.FromImage(coChessBitmap.coBmpBoard);
            offScreenDC.DrawImage(coSourceSquare.Background,coSourceSquare.Square);
            Cursor.Current = (Cursor) coChessCursors[Chess.Pieces.CLOSEDHAND];
            // If we don't do this then resources do not get released.  Memory leaks...
            offScreenDC.Dispose();
          }
        }
      }
    }

    /// <summary>
    /// Used to signal that we are ready to drop a chess piece on it's respective
    /// target square.
    /// </summary>
    /// <param name="mouse"></param>
    public void dropPiece(MouseEventArgs mouse)
    {
      // Only if we were moving a piece.
      if( isDragging )
      {
        // Clear out coordinates used during the drag.
        coMovingSquare.X = 0;
        coMovingSquare.Y = 0;
        coMovingSquare.Width = 0;
        coMovingSquare.Height = 0;
        coPoint.X = 0;
        coPoint.Y = 0;   
     
        // Reset our source square to the original piece.
        coSourceSquare.Piece = coPiece;
        // Clear our temporary variable.
        coPiece = Chess.Pieces.NONE;
        // Locate where we want to drop the piece.
        coTargetSquare = coChessBitmap.findSquare(mouse.X, mouse.Y);
				Graphics offScreenDC = Graphics.FromImage(coChessBitmap.coBmpBoard);				
				// Found a target square
				if( coTargetSquare != null )
				{
					// Make sure we had a chess piece in hand.
					if( coSourceSquare.PieceImage != null )
					{
						// Used to see if hosting window says we can move the piece.
						bool okMove = true;           
						if( EventPieceMoved != null )
							okMove = EventPieceMoved(coSourceSquare.Piece,coSourceSquare.Name,coTargetSquare.Name);
						if( okMove )
						{
              bool bValidMove = true;
							// Make sure we are not moving to the same square the piece originally occupied.
							//  Then validate the move with the bitboard class.
							if( coTargetSquare != coSourceSquare )
							{
                if( coValidateMoves )
                  bValidMove = coBitBoard.move(coSourceSquare.Piece, coSourceSquare.Name, coTargetSquare.Name);

                if(bValidMove)
                {
                  if( !promotion )
                  {
                    // Everything ok so now move the piece into the new square.
                    coTargetSquare.PieceImage = coSourceSquare.PieceImage;
                    coTargetSquare.Piece = coSourceSquare.Piece;
                  }
                  else
                    promotion =false;

                  // Clear chess image and piece.
                  coSourceSquare.PieceImage = null;
                  coSourceSquare.Piece = Chess.Pieces.NONE;

                  // Just draw the new background and image.
                  offScreenDC.DrawImage(coTargetSquare.Background,coTargetSquare.Square);
                  offScreenDC.DrawImage(coTargetSquare.PieceImage ,coTargetSquare.Square);
                }
                else
                {
                  // Redraw the original pieces.
                  offScreenDC.DrawImage(coSourceSquare.Background,coSourceSquare.Square);
                  offScreenDC.DrawImage(coSourceSquare.PieceImage ,coSourceSquare.Square);
                }
							}
							else
							{
								// Redraw the original pieces.
								offScreenDC.DrawImage(coSourceSquare.Background,coSourceSquare.Square);
								offScreenDC.DrawImage(coSourceSquare.PieceImage ,coSourceSquare.Square);
							}
						}
					}
				}
				else //Dragging a piece off board
				{
					// Redraw the original pieces.
					offScreenDC.DrawImage(coSourceSquare.Background,coSourceSquare.Square);
					offScreenDC.DrawImage(coSourceSquare.PieceImage ,coSourceSquare.Square);
				}
        offScreenDC.Dispose();
        Invalidate();
        isDragging = false;
        Cursor.Current = (Cursor) coChessCursors[Chess.Pieces.OPENHAND];
      }
    }

    public string printBitboards()
    {
      return coBitBoard.ToString();
    }

    public string getFEN()
    {      
      string str = coChessBitmap.getFEN();
      
      return str;
    }

    private void ChessBoard_Load(object sender, System.EventArgs e)
    {
      Cursor = getCursor(ChessLibrary.Chess.Pieces.OPENHAND);
    }
    #region IValidationEvents Members

    public void Promotion(bool color, string square, ref ChessLibrary.Chess.Pieces piece)
    {
      // See if the user of this control to do the promotion.
      if( EventPromote != null )
      {        
        EventPromote(color,square,ref piece);
      }
      // No piece selected by user so we force the piece to a queen.
      if( piece == Chess.Pieces.NONE )
      {
        if( color )
          piece = Chess.Pieces.WQUEEN;
        else
          piece = Chess.Pieces.BQUEEN;
      }
      coChessBitmap.placePiece(square, piece);
      promotion = true;
    }

    public void KingIsMated()
    {
      if( EventIsMated != null )
        EventIsMated();
    }

    public void KingIsStaleMated()
    {
      if( EventIsStaleMated != null )
        EventIsStaleMated();
    }

    public void KingIsInCheck()
    {
      if( EventIsInCheck!= null )
        EventIsInCheck();
    }

		public void KingIsFree()
		{
			if( EventIsFree!= null )
				EventIsFree();
		}
    public void finishedMove()
    {
      if(EventIsDone != null )
        EventIsDone();
    }
    /// <summary>
    /// Moves a piece simply based on a source and destination
    ///   square.  Normally when the validation engine has
    ///   updated it's internal position and now the graphics
    ///   need to be updated as well.
    /// </summary>
    /// <param name="FromSquare"></param>
    /// <param name="ToSquare"></param>
    public void updateBoard(Chess.Operation op, string FromSquare, string ToSquare)
    {
      BoardSquare from=null;
      BoardSquare to=null;
      if( FromSquare != null )
        from = coChessBitmap.findSquare( FromSquare );
      if( ToSquare != null )
        to = coChessBitmap.findSquare( ToSquare );
      // Draw the new background and images.
      Graphics offScreenDC = Graphics.FromImage(coChessBitmap.coBmpBoard);

      switch(op)
      {
        case Chess.Operation.MOVE:
          to.PieceImage = from.PieceImage;
          to.Piece = from.Piece;
          from.PieceImage = null;
          from.Piece = Chess.Pieces.NONE;
          offScreenDC.DrawImage(from.Background, from.Square);
          offScreenDC.DrawImage(to.Background, to.Square);
          offScreenDC.DrawImage(to.PieceImage, to.Square);
          if( EventMovePiece != null )
            EventMovePiece(Chess.Operation.MOVE, FromSquare,ToSquare);
          break;
        case Chess.Operation.DELETE:
          from.PieceImage = null;
          from.Piece = Chess.Pieces.NONE;
          offScreenDC.DrawImage(from.Background, from.Square);
          break;
      }
      offScreenDC.Dispose();
    }
    #endregion

    #region Events
    delegate void Promote(bool color, string square, ref Chess.Pieces piece);
    event Promote EventPromote;
    delegate void IsMated();
    event IsMated EventIsMated;
    delegate void IsStaleMated();
    event IsStaleMated EventIsStaleMated;
    delegate void IsInCheck();
    event IsInCheck EventIsInCheck;
		delegate void IsFree();
		event IsFree EventIsFree;
    delegate void IsDone();
    event IsDone EventIsDone;
		delegate void movePiece(Chess.Operation op, string FromSquare, string ToSquare);
    event movePiece EventMovePiece;        
    #endregion Events

    public void addEvents(IValidationEvents ievents)
    {
      EventPromote += new Promote(ievents.Promotion);
      EventIsMated += new IsMated(ievents.KingIsMated);
      EventIsStaleMated += new IsStaleMated(ievents.KingIsStaleMated);
      EventIsInCheck += new IsInCheck(ievents.KingIsInCheck);
			EventIsFree += new IsFree(ievents.KingIsFree);
      EventIsDone += new IsDone(ievents.finishedMove);
			EventMovePiece += new movePiece(ievents.updateBoard);        
    }

    public void removeEvents(IValidationEvents ievents)
    {
      EventPromote -= new Promote(ievents.Promotion);
      EventIsMated -= new IsMated(ievents.KingIsMated);
      EventIsStaleMated -= new IsStaleMated(ievents.KingIsStaleMated);
      EventIsInCheck -= new IsInCheck(ievents.KingIsInCheck);
			EventIsFree -= new IsFree(ievents.KingIsFree);
			EventMovePiece -= new movePiece(ievents.updateBoard);        
    }
  }
}
