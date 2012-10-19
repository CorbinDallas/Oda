/**
* Style for the Oda.UI.Dialog
* @constructor
* @name Dialog
* @version 0.1.0 beta release
* @author Tony Germaneri (TonyGermaneri@gmail.com)
* @augments Oda.UI.WidgetStyle
* @requires Oda
* @requires Oda.UI
* @memberOf Oda.UI.Style
* @param {Native.Object} [args] Parameters for the style.
* @param {Oda.UI.Rect} [args.rect] The Oda.UI.Rect used by the Oda.UI.Dialog during initialization.
* @param {Oda.UI.Rect} [args.minRect] The minimum size Oda.UI.Rect used by the Oda.UI.Dialog.
* @param {Oda.UI.Rect} [args.contentRect] The Oda.UI.Rect used for the content area.
* @param {Oda.UI.Rect} [args.titleRect] The Oda.UI.Rect used for the content area. Offset x, y, w and actual h.
* @param {Oda.UI.Rect} [args.nRect] The Oda.UI.Rect used north edge.
* @param {Oda.UI.Rect} [args.neRect] The Oda.UI.Rect used north east corner.
* @param {Oda.UI.Rect} [args.eRect] The Oda.UI.Rect used east edge.
* @param {Oda.UI.Rect} [args.seRect] The Oda.UI.Rect used south east corner.
* @param {Oda.UI.Rect} [args.sRect] The Oda.UI.Rect used south edge.
* @param {Oda.UI.Rect} [args.swRect] The Oda.UI.Rect used south west corner.
* @param {Oda.UI.Rect} [args.wRect] The Oda.UI.Rect used wRect edge.
* @param {Oda.UI.Rect} [args.nwRect] The Oda.UI.Rect used north west corner.
* @param {Oda.UI.Rect} [args.minimizeRect] The Oda.UI.Rect for the minimize button.
* @param {Oda.UI.Rect} [args.maximizeRect] The Oda.UI.Rect for the maximize button.
* @param {Oda.UI.Rect} [args.closeRect] The Oda.UI.Rect for the close button.
* @param {Oda.UI.Border} [args.Inactive.buttonBorder] The Oda.UI.Border used for the inactive button borders.
* @param {Oda.UI.Border} [args.Active.buttonBorder] The Oda.UI.Border used for the active button borders.
* @param {Oda.UI.Border} [args.Active.titleBorder] The Oda.UI.Border used for the active title borders.
* @param {Oda.UI.Border} [args.Inactive.titleBorder] The Oda.UI.Border used for the inactive title borders.
* @param {Oda.UI.Border} [args.previewOutline] The Oda.UI.Border used for the move/resize preview outline.
* @param {Oda.UI.Padding} [args.titlePadding] The Oda.UI.Padding used for the title padding.
* @param {Native.String} [args.Active.titleColor] The HTML color code used for the title text color.
* @param {Native.String} [args.Inactive.titleColor] The HTML color code used for the inactive title text color.
* @param {Native.String} [args.Inactive.dialogColor] The HTML color code used for the dialog text color.
* @param {Native.String} [args.Inactive.contentColor] The HTML color code used for the content text color.
* @param {Native.String} [args.Active.dialogColor] The HTML color code used for the dialog text color.
* @param {Native.String} [args.Active.contentColor] The HTML color code used for the content text color.
* @param {Native.String} [args.Active.Hover.maximizeButtonBackground] The background used for the active maximize button when it is hovered over.
* @param {Native.String} [args.Active.Hover.minimizeButtonBackground] The background used for the active minimize button when it is hovered over.
* @param {Native.String} [args.Active.Hover.closeButtonBackground] The background used for the active close button when it is hovered over.
* @param {Native.String} [args.Active.Hover.restoreButtonBackground] The background used for the active restore button when it is hovered over.
* @param {Native.String} [args.Inactive.Hover.maximizeButtonBackground] The background used for the inactive maximize button when it is hovered over.
* @param {Native.String} [args.Inactive.Hover.minimizeButtonBackground] The background used for the inactive minimize button when it is hovered over.
* @param {Native.String} [args.Inactive.Hover.closeButtonBackground] The background used for the inactive close button when it is hovered over.
* @param {Native.String} [args.Inactive.Hover.restoreButtonBackground] The background used for the inactive restore button when it is hovered over.
* @param {Native.String} [args.Inactive.maximizeButtonBackground] The background used for the inactive maximize button.
* @param {Native.String} [args.Inactive.minimizeButtonBackground] The background used for the inactive minimize button.
* @param {Native.String} [args.Inactive.closeButtonBackground] The background used for the inactive close button.
* @param {Native.String} [args.Inactive.restoreButtonBackground] The background used for the inactive restore button.
* @param {Native.String} [args.Active.maximizeButtonBackground] The background used for the active maximize button.
* @param {Native.String} [args.Active.minimizeButtonBackground] The background used for the active minimize button.
* @param {Native.String} [args.Active.closeButtonBackground] The background used for the active close button.
* @param {Native.String} [args.Active.restoreButtonBackground] The background used for the active restore button.
* @param {Native.String} [args.Active.contentBackground] The background used for the active content.
* @param {Native.String} [args.Active.dialogBackground] The background used for the active dialog.
* @param {Native.String} [args.Active.titleBackground] The background used for the active title.
* @param {Native.String} [args.Active.nBackground] The background used for the active north edge.
* @param {Native.String} [args.Active.neBackground] The background used for the active north east corner.
* @param {Native.String} [args.Active.eBackground] The background used for the active east edge.
* @param {Native.String} [args.Active.seBackground] The background used for the active south east corner.
* @param {Native.String} [args.Active.sBackground] The background used for the active south edge.
* @param {Native.String} [args.Active.swBackground] The background used for the active south west corner.
* @param {Native.String} [args.Active.wBackground] The background used for the active west edge.
* @param {Native.String} [args.Active.nwBackground] The background used for the active north west corner.
* @param {Native.String} [args.Inactive.contentBackground] The background used for the inactive content.
* @param {Native.String} [args.Inactive.dialogBackground] The background used for the inactive dialog.
* @param {Native.String} [args.Inactive.titleBackground] The background used for the inactive title.
* @param {Native.String} [args.Inactive.nBackground] The background used for the inactive north edge.
* @param {Native.String} [args.Inactive.neBackground] The background used for the inactive north east corner.
* @param {Native.String} [args.Inactive.eBackground] The background used for the inactive east edge.
* @param {Native.String} [args.Inactive.seBackground] The background used for the inactive south east corner.
* @param {Native.String} [args.Inactive.sBackground] The background used for the inactive south edge.
* @param {Native.String} [args.Inactive.swBackground] The background used for the inactive south west corner.
* @param {Native.String} [args.Inactive.wBackground] The background used for the inactive west edge.
* @param {Native.String} [args.Inactive.nwBackground] The background used for the inactive north west corner.
* @param {Native.String} [args.titleFont] The font used by the title bar.
* @param {Native.String} [args.modalBackground] The background for the modal overlay.
* @param {Native.String} [args.previewBackground] The background for the move/resize preview overlay.
*/
Oda.UI.Style.Dialog = function (args) {
    var self = Oda.beget(Oda.UI.WidgetStyle);
    // make sure objects exist
    args = args || {};
    args.Active = args.Active || {};
    args.Inactive = args.Inactive || {};
    args.Active.Hover = args.Active.Hover || {};
    args.Inactive.Hover = args.Inactive.Hover || {};
    /**
    * Collection of active element styles.
    * @Class
    * @name Active
    * @type Native.Object
    * @memberOf Oda.UI.Style.Dialog
    */
    self.Active = {};
    /**
    * Collection of inactive element styles.
    * @Class
    * @name Inactive
    * @type Native.Object
    * @memberOf Oda.UI.Style.Dialog
    */
    self.Inactive = {};
    /**
    * Collection of active hovered element styles.
    * @Class
    * @name Hover
    * @type Native.Object
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.Hover = {};
    /**
    * Collection of inactive hovered element styles.
    * @Class
    * @name Hover
    * @type Native.Object
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.Hover = {};
    /**
    * The Oda.UI.Rect used by the Oda.UI.Dialog during initialization.
    * @field
    * @name rect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.rect = args.rect || { x: parseInt(Oda.UI.Widget.client().w * .5, 10) - 250, y: 100, h: 350, w: 500 };
    /**
    * The minimum size Oda.UI.Rect used by the Oda.UI.Dialog.
    * @field
    * @name minRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.minRect = args.minRect || { x: -5000, y: -5000, w: 165, h: 100 };
    // content
    /**
    * The Oda.UI.Rect used for the content area.
    * @field
    * @name contentRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.contentRect = args.contentRect || { x: 0, y: 0, w: 0, h: 0 };
    // title bar offset x,y,w and actual h
    /**
    * The Oda.UI.Rect used for the content area. Offset x, y, w and actual h.
    * @field
    * @name titleRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.titleRect = args.titleRect || { x: 0, y: 0, w: 0, h: 25 };
    /**
    * The Oda.UI.Rect used north edge.
    * @field
    * @name nRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.nRect = args.nRect || { x: 0, y: 0, w: 0, h: 6 };
    /**
    * The Oda.UI.Rect used north east corner.
    * @field
    * @name neRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.neRect = args.neRect || { x: 0, y: 0, w: 6, h: 6 };
    /**
    * The Oda.UI.Rect used east edge.
    * @field
    * @name eRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.eRect = args.eRect || { x: 0, y: 0, w: 6, h: 0 };
    /**
    * The Oda.UI.Rect used south east corner.
    * @field
    * @name seRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.seRect = args.seRect || { x: 0, y: 0, w: 6, h: 6 };
    /**
    * The Oda.UI.Rect used south edge.
    * @field
    * @name sRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.sRect = args.sRect || { x: 0, y: 0, w: 0, h: 6 };
    /**
    * The Oda.UI.Rect used south west corner.
    * @field
    * @name swRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.swRect = args.swRect || { x: 0, y: 0, w: 6, h: 6 };
    /**
    * The Oda.UI.Rect used wRect edge.
    * @field
    * @name wRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.wRect = args.wRect || { x: 0, y: 0, w: 6, h: 0 };
    /**
    * The Oda.UI.Rect used north west corner.
    * @field
    * @name nwRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.nwRect = args.nwRect || { x: 0, y: 0, w: 6, h: 6 };
    // control buttons
    /**
    * The Oda.UI.Rect for the minimize button.
    * @field
    * @name minimizeRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.minimizeRect = args.minimizeRect || { x: 0, y: 0, w: 34, h: 26 };
    /**
    * The Oda.UI.Rect for the maximize button.
    * @field
    * @name maximizeRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.maximizeRect = args.maximizeRect || { x: 0, y: 0, w: 34, h: 26 };
    /**
    * The Oda.UI.Rect for the close button.
    * @field
    * @name closeRect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog
    */
    self.closeRect = args.closeRect || { x: 0, y: 0, w: 34, h: 26 };
    // inactive button borders
    /**
    * The Oda.UI.Border used for the inactive button borders.
    * @field
    * @name buttonBorder
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.buttonBorder = args.Inactive.buttonBorder || { size: 0, color: 'transparent', style: 'solid' };
    // active button borders
    /**
    * The Oda.UI.Border used for the active button borders.
    * @field
    * @name buttonBorder
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.buttonBorder = args.Active.buttonBorder || { size: 0, color: 'transparent', style: 'solid' };
    // active hover button backgrounds
    /**
    * The background used for the active maximize button when it is hovered over.
    * @field
    * @name maximizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active.Hover
    */
    self.Active.Hover.maximizeButtonBackground = args.Active.Hover.maximizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///z8/QSH5BAAAAAAALAAAAAAiABgAAAItjI+py+0Po5y02ouz3g34/3EBSIokaC5Aqqycm8CafNCYPZqnJ/b+DwwKh5oCADs=)';
    /**
    * The background used for the active minimize button when it is hovered over.
    * @field
    * @name minimizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active.Hover
    */
    self.Active.Hover.minimizeButtonBackground = args.Active.Hover.minimizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///z8/QSH5BAAAAAAALAAAAAAiABgAAAIhjI+py+0Po5y02ouz3rz7D4YYQJZlZ6ZoeoruC8fyTE8FADs=)';
    /**
    * The background used for the active close button when it is hovered over.
    * @field
    * @name closeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active.Hover
    */
    self.Active.Hover.closeButtonBackground = args.Active.Hover.closeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8T8/QSH5BAAAAAAALAAAAAAiABgAAAIpjI+py+0Po5y02ouz3vwC9XVBaJBiaIpjqgJsh6ol8mJsLef6zve+WAAAOw==)';
    /**
    * The background used for the active restore button when it is hovered over.
    * @field
    * @name restoreButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active.Hover
    */
    self.Active.Hover.restoreButtonBackground = args.Active.Hover.restoreButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8T8/QSH5BAAAAAAALAAAAAAiABgAAAIvjI+py+0Po5y02ouzBrwDnXgdiHwlKXKGuaUrGr4gu45zTMoHPbv5DwwKh8QiqQAAOw==)';
    // inactive hover button backgrounds
    /**
    * The background used for the inactive maximize button when it is hovered over.
    * @field
    * @name maximizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive.Hover
    */
    self.Inactive.Hover.maximizeButtonBackground = args.Inactive.Hover.maximizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///5qamiH5BAAAAAAALAAAAAAiABgAAAItjI+py+0Po5y02ouz3g34/3EBSIokaC5Aqqycm8CafNCYPZqnJ/b+DwwKh5oCADs=)';
    /**
    * The background used for the inactive minimize button when it is hovered over.
    * @field
    * @name minimizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive.Hover
    */
    self.Inactive.Hover.minimizeButtonBackground = args.Inactive.Hover.minimizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///5qamiH5BAAAAAAALAAAAAAiABgAAAIhjI+py+0Po5y02ouz3rz7D4YYQJZlZ6ZoeoruC8fyTE8FADs=)';
    /**
    * The background used for the inactive close button when it is hovered over.
    * @field
    * @name closeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive.Hover
    */
    self.Inactive.Hover.closeButtonBackground = args.Inactive.Hover.closeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8ZqamiH5BAAAAAAALAAAAAAiABgAAAIpjI+py+0Po5y02ouz3vwC9XVBaJBiaIpjqgJsh6ol8mJsLef6zve+WAAAOw==)';
    /**
    * The background used for the inactive restore button when it is hovered over.
    * @field
    * @name restoreButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive.Hover
    */
    self.Inactive.Hover.restoreButtonBackground = args.Inactive.Hover.restoreButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8ZqamiH5BAAAAAAALAAAAAAiABgAAAIvjI+py+0Po5y02ouzBrwDnXgdiHwlKXKGuaUrGr4gu45zTMoHPbv5DwwKh8QiqQAAOw==)';
    // active button backgrounds
    /**
    * The background used for the active maximize button.
    * @field
    * @name maximizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.maximizeButtonBackground = args.Active.maximizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///y0tMCH5BAAAAAAALAAAAAAiABgAAAItjI+py+0Po5y02ouz3g34/3EBSIokaC5Aqqycm8CafNCYPZqnJ/b+DwwKh5oCADs=)';
    /**
    * The background used for the active minimize button.
    * @field
    * @name minimizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.minimizeButtonBackground = args.Active.minimizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///y0tMCH5BAAAAAAALAAAAAAiABgAAAIhjI+py+0Po5y02ouz3rz7D4YYQJZlZ6ZoeoruC8fyTE8FADs=)';
    /**
    * The background used for the active close button.
    * @field
    * @name closeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.closeButtonBackground = args.Active.closeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8S0tMCH5BAAAAAAALAAAAAAiABgAAAIpjI+py+0Po5y02ouz3vwC9XVBaJBiaIpjqgJsh6ol8mJsLef6zve+WAAAOw==)';
    /**
    * The background used for the active restore button.
    * @field
    * @name restoreButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.restoreButtonBackground = args.Active.restoreButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8S0tMCH5BAAAAAAALAAAAAAiABgAAAIvjI+py+0Po5y02ouzBrwDnXgdiHwlKXKGuaUrGr4gu45zTMoHPbv5DwwKh8QiqQAAOw==)';
    // inactive button backgrounds
    /**
    * The background used for the inactive maximize button.
    * @field
    * @name maximizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.maximizeButtonBackground = args.Inactive.maximizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///3d3dyH5BAAAAAAALAAAAAAiABgAAAItjI+py+0Po5y02ouz3g34/3EBSIokaC5Aqqycm8CafNCYPZqnJ/b+DwwKh5oCADs=)';
    /**
    * The background used for the inactive minimize button.
    * @field
    * @name minimizeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.minimizeButtonBackground = args.Inactive.minimizeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAP///3d3dyH5BAAAAAAALAAAAAAiABgAAAIhjI+py+0Po5y02ouz3rz7D4YYQJZlZ6ZoeoruC8fyTE8FADs=)';
    /**
    * The background used for the inactive close button.
    * @field
    * @name closeButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.closeButtonBackground = args.Inactive.closeButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8Xd3dyH5BAAAAAAALAAAAAAiABgAAAIpjI+py+0Po5y02ouz3vwC9XVBaJBiaIpjqgJsh6ol8mJsLef6zve+WAAAOw==)';
    /**
    * The background used for the inactive restore button.
    * @field
    * @name restoreButtonBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.restoreButtonBackground = args.Inactive.restoreButtonBackground || 'url(data:image/gif;base64,R0lGODlhIgAYAIAAAPHx8XZ2diH5BAAAAAAALAAAAAAiABgAAAIvjI+py+0Po5y02ouzBrwDnXgdiHwlKXKGuaUrGr4gu45zTMoHPbv5DwwKh8QiqQAAOw==)';
    // inactive backgrounds
    /**
    * The background used for the inactive content.
    * @field
    * @name contentBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.contentBackground = args.Inactive.contentBackground || '#EEE';
    /**
    * The background used for the inactive dialog.
    * @field
    * @name dialogBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.dialogBackground = args.Inactive.dialogBackground || 'transparent';
    /**
    * The background used for the inactive title.
    * @field
    * @name titleBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.titleBackground = args.Inactive.titleBackground || '#777';
    /**
    * The background used for the inactive north edge.
    * @field
    * @name nBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.nBackground = args.Inactive.nBackground || 'url(data:image/gif;base64,R0lGODlhAQAGAIABAI+PjwAAACH5BAEAAAEALAAAAAABAAYAAAIDjA0FADs=)';
    /**
    * The background used for the inactive north east corner.
    * @field
    * @name neBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.neBackground = args.Inactive.neBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAYAAAIGjI+pa5AFADs=)';
    /**
    * The background used for the inactive east edge.
    * @field
    * @name eBackground
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.eBackground = args.Inactive.eBackground || 'url(data:image/gif;base64,R0lGODlhBgABAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAEAAAIDRH4FADs=)';
    /**
    * The background used for the inactive south east corner.
    * @field
    * @name seBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.seBackground = args.Inactive.seBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAYAAAIFRI6py1wAOw==)';
    /**
    * The background used for the inactive south edge.
    * @field
    * @name sBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.sBackground = args.Inactive.sBackground || 'url(data:image/gif;base64,R0lGODlhAQAGAIABAI+PjwAAACH5BAEAAAEALAAAAAABAAYAAAIDRH4FADs=)';
    /**
    * The background used for the inactive south west corner.
    * @field
    * @name swBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.swBackground = args.Inactive.swBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAYAAAIGjA2ny70FADs=)';
    /**
    * The background used for the inactive west edge.
    * @field
    * @name wBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.wBackground = args.Inactive.wBackground || 'url(data:image/gif;base64,R0lGODlhBgABAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAEAAAIDjA0FADs=)';
    /**
    * The background used for the inactive north west corner.
    * @field
    * @name nwBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.nwBackground = args.Inactive.nwBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAI+PjwAAACH5BAEAAAEALAAAAAAGAAYAAAIFjI+pu1AAOw==)';
    // active backgrounds
    /**
    * The background used for the active content.
    * @field
    * @name contentBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.contentBackground = args.Active.contentBackground || '#EEE';
    /**
    * The background used for the active dialog.
    * @field
    * @name dialogBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.dialogBackground = args.Active.dialogBackground || 'transparent';
    /**
    * The background used for the active title.
    * @field
    * @name titleBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.titleBackground = args.Active.titleBackground || '#2d2d30';
    /**
    * The background used for the active north edge.
    * @field
    * @name nBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.nBackground = args.Active.nBackground || 'url(data:image/gif;base64,R0lGODlhAQAGAIABAABWjwAAACH5BAEAAAEALAAAAAABAAYAAAIDjA0FADs=)';
    /**
    * The background used for the active north east corner.
    * @field
    * @name neBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.neBackground = args.Active.neBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAYAAAIGjI+pa5AFADs=)';
    /**
    * The background used for the active east edge.
    * @field
    * @name eBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.eBackground = args.Active.eBackground || 'url(data:image/gif;base64,R0lGODlhBgABAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAEAAAIDRH4FADs=)';
    /**
    * The background used for the active south east corner.
    * @field
    * @name seBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.seBackground = args.Active.seBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAYAAAIFRI6py1wAOw==)';
    /**
    * The background used for the active south edge.
    * @field
    * @name sBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.sBackground = args.Active.sBackground || 'url(data:image/gif;base64,R0lGODlhAQAGAIABAABWjwAAACH5BAEAAAEALAAAAAABAAYAAAIDRH4FADs=)';
    /**
    * The background used for the active south west corner.
    * @field
    * @name swBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.swBackground = args.Active.swBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAYAAAIGjA2ny70FADs=)';
    /**
    * The background used for the active west edge.
    * @field
    * @name wBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.wBackground = args.Active.wBackground || 'url(data:image/gif;base64,R0lGODlhBgABAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAEAAAIDjA0FADs=)';
    /**
    * The background used for the active north west corner.
    * @field
    * @name nwBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.nwBackground = args.Active.nwBackground || 'url(data:image/gif;base64,R0lGODlhBgAGAIABAABWjwAAACH5BAEAAAEALAAAAAAGAAYAAAIFjI+pu1AAOw==)';
    /**
    * The HTML color code used for the inactive title text color.
    * @field
    * @name titleColor
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.titleColor = args.Inactive.titleColor || '#DDD';
    /**
    * The HTML color code used for the active title text color.
    * @field
    * @name titleColor
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.titleColor = args.Active.titleColor || '#FFF';
    // neutral colors
    /**
    * The HTML color code used for the dialog text color.
    * @field
    * @name dialogColor
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.dialogColor = args.Active.dialogColor || '#000';
    /**
    * The HTML color code used for the content text color.
    * @field
    * @name contentColor
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.contentColor = args.Active.contentColor || '#000';
    // padding
    /**
    * The Oda.UI.Padding used for the title padding.
    * @field
    * @name titlePadding
    * @type Oda.UI.Padding
    * @memberOf Oda.UI.Style.Dialog
    */
    self.titlePadding = args.titlePadding || { t: 1, r: 0, b: 0, l: 10 };
    // font 
    /**
    * The font used by the title bar.
    * @field
    * @name titleFont
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog
    */
    self.titleFont = args.titleFont || self.defaultFontFamily;
    // title border
    /**
    * The Oda.UI.Border used for the active title borders.
    * @field
    * @name titleBorder
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.Dialog.Active
    */
    self.Active.titleBorder = args.Active.titleBorder || { size: 1, style: 'solid', color: '#00568f' };
    /**
    * The Oda.UI.Border used for the inactive title borders.
    * @field
    * @name titleBorder
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.Dialog.Inactive
    */
    self.Inactive.titleBorder = args.Inactive.titleBorder || { size: 1, style: 'solid', color: '#777' };
    // modal background
    /**
    * The background for the modal overlay.
    * @field
    * @name modalBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog
    */
    self.modalBackground = args.modalBackground || 'url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAA9JREFUeNpiYGBgaAAIMAAAhQCB69VMmQAAAABJRU5ErkJggg==)';
    // preview background
    /**
    * The background for the move/resize preview overlay.
    * @field
    * @name previewBackground
    * @type Native.String
    * @memberOf Oda.UI.Style.Dialog
    */
    self.previewBackground = args.previewBackground || 'url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAA9JREFUeNpiYGBgaAAIMAAAhQCB69VMmQAAAABJRU5ErkJggg==)';
    // preview outline
    /**
    * The Oda.UI.Border used for the move/resize preview outline.
    * @field
    * @name previewOutline
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.Dialog
    */
    self.previewOutline = args.previewOutline || { size: 1, style: 'solid', color: '#00568f' };
    return self;
}