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

namespace ChessBoardCtrl
{
  public interface INavigationEvents
  {
    void moveFirst();
    void moveLast();
    void moveNext();
    void movePrev();
    void OK();
    void delete();
    void add();
    void cancel();
  }

  public interface INavigation
  { 
    /// <summary>
    /// Used to register event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void addEvents(INavigationEvents ievents);
    /// <summary>
    /// Used to remove event handlers.
    /// </summary>
    /// <param name="ievents"></param>
    void removeEvents(INavigationEvents ievents);
  }


  /// <summary>
  /// 
  /// </summary>
  public class GameNavigation : System.Windows.Forms.UserControl, INavigation
  {
    private System.Windows.Forms.ToolBarButton _moveFirstButton;
    private System.Windows.Forms.ToolBarButton _movePreviousButton;
    private System.Windows.Forms.ToolBarButton _newButton;
    private System.Windows.Forms.ToolBarButton _deleteButton;
    private System.Windows.Forms.ToolBarButton _okButton;
    private System.Windows.Forms.ToolBarButton _cancelButton;
    private System.Windows.Forms.ToolBarButton _moveNextButton;
    private System.Windows.Forms.ToolBarButton _moveLastButton;
    private System.Windows.Forms.Label _recordLabel;
    private System.Windows.Forms.ToolBar _leftToolBar;
    private System.Windows.Forms.ToolBar _rightToolBar;
    private System.Windows.Forms.ImageList _navigatorImageList;
    private System.ComponentModel.IContainer components;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbNavigator"/> class.
    /// </summary>
    public GameNavigation ()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GameNavigation));
      this._leftToolBar = new System.Windows.Forms.ToolBar();
      this._moveFirstButton = new System.Windows.Forms.ToolBarButton();
      this._movePreviousButton = new System.Windows.Forms.ToolBarButton();
      this._newButton = new System.Windows.Forms.ToolBarButton();
      this._deleteButton = new System.Windows.Forms.ToolBarButton();
      this._rightToolBar = new System.Windows.Forms.ToolBar();
      this._okButton = new System.Windows.Forms.ToolBarButton();
      this._cancelButton = new System.Windows.Forms.ToolBarButton();
      this._moveNextButton = new System.Windows.Forms.ToolBarButton();
      this._moveLastButton = new System.Windows.Forms.ToolBarButton();
      this._recordLabel = new System.Windows.Forms.Label();
      this._navigatorImageList = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // _leftToolBar
      // 
      this._leftToolBar.AutoSize = false;
      this._leftToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                    this._moveFirstButton,
                                                                                    this._movePreviousButton,
                                                                                    this._newButton,
                                                                                    this._deleteButton});
      this._leftToolBar.Divider = false;
      this._leftToolBar.Dock = System.Windows.Forms.DockStyle.Left;
      this._leftToolBar.DropDownArrows = true;
      this._leftToolBar.ImageList = this._navigatorImageList;
      this._leftToolBar.Location = new System.Drawing.Point(0, 0);
      this._leftToolBar.Name = "_leftToolBar";
      this._leftToolBar.ShowToolTips = true;
      this._leftToolBar.Size = new System.Drawing.Size(96, 24);
      this._leftToolBar.TabIndex = 0;
      this._leftToolBar.Wrappable = false;
      this._leftToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.OnButtonClick);
      // 
      // _moveFirstButton
      // 
      this._moveFirstButton.ImageIndex = 0;
      this._moveFirstButton.ToolTipText = "Move to First";
      // 
      // _movePreviousButton
      // 
      this._movePreviousButton.ImageIndex = 1;
      this._movePreviousButton.ToolTipText = "Move to Previous";
      // 
      // _newButton
      // 
      this._newButton.ImageIndex = 2;
      this._newButton.ToolTipText = "New Record";
      // 
      // _deleteButton
      // 
      this._deleteButton.ImageIndex = 3;
      this._deleteButton.ToolTipText = "Delete Current Record";
      // 
      // _rightToolBar
      // 
      this._rightToolBar.AutoSize = false;
      this._rightToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                                     this._okButton,
                                                                                     this._cancelButton,
                                                                                     this._moveNextButton,
                                                                                     this._moveLastButton});
      this._rightToolBar.Divider = false;
      this._rightToolBar.Dock = System.Windows.Forms.DockStyle.Right;
      this._rightToolBar.DropDownArrows = true;
      this._rightToolBar.ImageList = this._navigatorImageList;
      this._rightToolBar.Location = new System.Drawing.Point(216, 0);
      this._rightToolBar.Name = "_rightToolBar";
      this._rightToolBar.ShowToolTips = true;
      this._rightToolBar.Size = new System.Drawing.Size(96, 24);
      this._rightToolBar.TabIndex = 1;
      this._rightToolBar.Wrappable = false;
      this._rightToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.OnButtonClick);
      // 
      // _okButton
      // 
      this._okButton.ImageIndex = 4;
      this._okButton.ToolTipText = "End Current Edit";
      // 
      // _cancelButton
      // 
      this._cancelButton.ImageIndex = 5;
      this._cancelButton.ToolTipText = "Cancel Current Edit";
      // 
      // _moveNextButton
      // 
      this._moveNextButton.ImageIndex = 6;
      this._moveNextButton.ToolTipText = "Move to Next";
      // 
      // _moveLastButton
      // 
      this._moveLastButton.ImageIndex = 7;
      this._moveLastButton.ToolTipText = "Move to Last";
      // 
      // _recordLabel
      // 
      this._recordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this._recordLabel.Location = new System.Drawing.Point(96, 0);
      this._recordLabel.Name = "_recordLabel";
      this._recordLabel.Size = new System.Drawing.Size(120, 24);
      this._recordLabel.TabIndex = 2;
      this._recordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // _navigatorImageList
      // 
      this._navigatorImageList.ImageSize = new System.Drawing.Size(16, 16);
      this._navigatorImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_navigatorImageList.ImageStream")));
      this._navigatorImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // GameNavigation
      // 
      this.Controls.Add(this._recordLabel);
      this.Controls.Add(this._rightToolBar);
      this.Controls.Add(this._leftToolBar);
      this.Name = "GameNavigation";
      this.Size = new System.Drawing.Size(312, 24);
      this.ResumeLayout(false);

    }
    #endregion

    /// <summary>
    /// Gets or sets a value indicating whether the navigator 
    /// displays a ToolTip for each button.
    /// </summary>
    /// <value>true if the navigator displays a ToolTip for each button;
    /// otherwise, false. The default is true.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether tool tips will be shown.")]
    public bool ShowToolTips
    {
      get { return _leftToolBar.ShowToolTips; }
      set { _leftToolBar.ShowToolTips = _rightToolBar.ShowToolTips = value; }
    }

    /// <summary>
    /// Indicates whether the New button is visible.
    /// </summary>
    /// <value>true New button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the New button is visible.")]
    public bool enableNew
    {
      get{ return _newButton.Visible; }
      set{ _newButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the OK button is visible.
    /// </summary>
    /// <value>true OK button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the OK button is visible.")]
    public bool enableOk
    {
      get{ return _okButton.Visible; }
      set{ _okButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the Delete button is visible.
    /// </summary>
    /// <value>true Delete button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the Delete button is visible.")]
    public bool enableDelete
    {
      get{ return _deleteButton.Visible; }
      set{ _deleteButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the Cancel button is visible.
    /// </summary>
    /// <value>true Cancel button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the Cancel button is visible.")]
    public bool enableCancel
    {
      get{ return _cancelButton.Visible; }
      set{ _cancelButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the First button is visible.
    /// </summary>
    /// <value>true First button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the First button is visible.")]
    public bool enableFirst
    {
      get{ return _moveFirstButton.Visible; }
      set{ _moveFirstButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the Last button is visible.
    /// </summary>
    /// <value>true Last button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the Last button is visible.")]
    public bool enableLast
    {
      get{ return _moveLastButton.Visible; }
      set{ _moveLastButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the Previous button is visible.
    /// </summary>
    /// <value>true Previous button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the Previous button is visible.")]
    public bool enablePrevious
    {
      get{ return _movePreviousButton.Visible; }
      set{ _movePreviousButton.Visible = value; }
    }
    /// <summary>
    /// Indicates whether the Next button is visible.
    /// </summary>
    /// <value>true Next button is visible.</value>
    [DefaultValue(true), Category("Behavior"), 
    Description("Indicates whether the Next button is visible.")]
    public bool enableNext
    {
      get{ return _moveNextButton.Visible; }
      set{ _moveNextButton.Visible = value; }
    }

    private void OnButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
    {
      if(_newButton == e.Button && EventAdd != null)
        EventAdd();
      else if(_deleteButton == e.Button && EventDelete!=null)
        EventDelete();
      else if(_moveFirstButton == e.Button && EventFirst != null)
        EventFirst();
      else if(_movePreviousButton == e.Button && EventPrev!=null)
        EventPrev();
      else if(_moveNextButton == e.Button && EventNext!=null)
        EventNext();
      else if(_moveLastButton == e.Button && EventLast!=null)
        EventLast();
      else if(_okButton == e.Button && EventOK!=null)
        EventOK();
      else if(_cancelButton == e.Button && EventCancel!=null)
        EventCancel();     
    }

    delegate void First();
    event First EventFirst;
    delegate void Last();
    event Last EventLast;
    delegate void Next();
    event Next EventNext;
    delegate void Prev();
    event Prev EventPrev;
    delegate void OK();
    event OK EventOK;
    delegate void Delete();
    event Delete EventDelete;
    delegate void Add();
    event Add EventAdd;
    delegate void Cancel();
    event Cancel EventCancel;

    public void addEvents(INavigationEvents ievents)
    {
      EventFirst += new First(ievents.moveFirst);
      EventLast += new Last(ievents.moveLast);
      EventNext += new Next(ievents.moveNext);
      EventPrev += new Prev(ievents.movePrev);
      EventOK += new OK(ievents.OK);
      EventDelete += new Delete(ievents.delete);
      EventAdd += new Add(ievents.add);        
      EventCancel += new Cancel(ievents.cancel);        
    }

    public void removeEvents(INavigationEvents ievents)
    {
      EventFirst -= new First(ievents.moveFirst);
      EventLast -= new Last(ievents.moveLast);
      EventNext -= new Next(ievents.moveNext);
      EventPrev -= new Prev(ievents.movePrev);
      EventOK -= new OK(ievents.OK);
      EventDelete -= new Delete(ievents.delete);
      EventAdd -= new Add(ievents.add);        
      EventCancel -= new Cancel(ievents.cancel);        
    }
  }
}
