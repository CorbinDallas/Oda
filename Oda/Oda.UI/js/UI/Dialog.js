/**
* Creates a DHTML based dialog.  The dialog contains control boxes, a title bar, an entry in the Oda.UI.TaskBar if it's visible, 
* resizable, control boxes etc. dozens of events to attach to.
* @constructor
* @name Dialog
* @version 0.1.0 beta release
* @author Tony Germaneri (TonyGermaneri@gmail.com)
* @augments Oda.UI.Widget
* @requires Oda
* @requires Oda.UI
* @requires Oda.UI.Style.Dialog
* @memberOf Oda.UI
* @param {Native.Object} [args] Parameters for the dialog.
* @param {Native.Number} [args.state=0] The state of the Oda.UI.Dialog.  0: Normal, 1: minimized, 2: maximized, 3: hidden.
* @param {Oda.UI.Rect} [args.rect] The Oda.UI.Rect of the Oda.UI.Dialog.
* @param {Native.Boolean} [args.resizable=true] When set true the Oda.UI.Dialog will be resizable. When set false the Oda.UI.Dialog will be resizable.
* @param {Native.Boolean} [args.showContentWhileMoving=false] When set true content will be shown while moving and resizing.  When set false a preview will be shown while moving and resizing.
* @param {Native.Boolean} [args.centerVertically=false] When true this Oda.UI.Dialog will be centered vertically when the position is next updated or moveToCenter is called.
* @param {Native.Boolean} [args.centerHorizontally=false] When true this Oda.UI.Dialog will be centered horizontally when the position is next updated or moveToCenter is called.
* @param {Oda.UI.Style} [args.style=Oda.UI.Style] The Oda.UI.Style of this Oda.UI.Dialog.
* @param {Native.Boolean} [args.modal=false] When set true the Oda.UI.Dialog will be in modal mode, appearing on top of everything and preventing access to background elements.
* @param {Native.Boolean} [args.modalCanClose=false] When set true, the close control box will be enabled even when in modal mode.
* @param {Native.Boolean} [args.moveable=true] When set false the Oda.UI.Dialog cannot be moved.
* @param {Native.Boolean} [args.alwaysOnTop=false] When set true the dialog will appear above all other Oda.UI.Dialog instances.
* @param {Native.Boolean} [args.dontInit=false] When set true the dialog will not initialize until you call the Oda.UI.Dialog.Init method.
* @param {Native.Boolean} [args.hidden=false] When set true the dialog will be hidden.
* @param {Native.Boolean} [args.hiddenFromTaskBar=false] When set true the dialog will be hidden from the task bar.
* @param {Native.String} [args.title] The title of the Oda.UI.Dialog.
* @example ///Create a simple reference to a new dialog, set the title and make it modal.///
*var myDialog = Oda.UI.Dialog({
*	title:'My Dialog',
*	modal: true
*});
* @example ///Attach to an event when you create the dialog.///
*var myDialog = Oda.UI.Dialog({
*	title:'My Dialog',
*	close:function(e,dialog){
*	    dialog.title('Can\'t close me.');
*		dialog.preventDefault();
*		return;
*	}
*});
* @example ///Attach to an event after you create the dialog///
*var myDialog = Oda.UI.Dialog({
*	title:'My Dialog'
*});
*myDialog.addEventListener('close',function(e,dialog){
*   dialog.title('Can\'t close me.');
*	dialog.preventDefault();
*	return;
*},false);
*/
Oda.UI.Dialog = function (args) {
    args = args || {};
    var self = Oda.beget(Oda.UI.Widget);
    self.publicMembers = {};
    self.style = args.style || Oda.UI.Style.Dialog();
    self.type = 'Dialog';
    // setup events, set events from args
    self.events = {
        /**
        * Occurs after the Oda.UI.Dialog is initialized.
        * @event
        * @name init
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        init: self.addInitalEvents(args.init),
        /**
        * Occurs before the Oda.UI.Dialog is being disposed.
        * @event
        * @name dispose
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        dispose: self.addInitalEvents(args.dispose),
        /**
        * Occurs before the Oda.UI.Dialog is being shown.
        * @event
        * @name show
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        show: self.addInitalEvents(args.show),
        /**
        * Occurs before the Oda.UI.Dialog is hidden.
        * @event
        * @name hide
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        hide: self.addInitalEvents(args.hide),
        /**
        * Occurs before the Oda.UI.Dialog is closed.
        * @event
        * @name close
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        close: self.addInitalEvents(args.close),
        /**
        * Occurs before the Oda.UI.Dialog is minimized.
        * @event
        * @name minimize
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        minimize: self.addInitalEvents(args.minimize),
        /**
        * Occurs before the Oda.UI.Dialog is maximized.
        * @event
        * @name maximize
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        maximize: self.addInitalEvents(args.maximize),
        /**
        * Occurs before the Oda.UI.Dialog is restored.
        * @event
        * @name restore
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        restore: self.addInitalEvents(args.restore),
        /**
        * Occurs while the Oda.UI.Dialog is moved.
        * @event
        * @name move
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        move: self.addInitalEvents(args.move),
        /**
        * Occurs while the Oda.UI.Dialog is resized.
        * @event
        * @name resize
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        resize: self.addInitalEvents(args.resize),
        /**
        * Occurs when the Oda.UI.Dialog is stylized.
        * @event
        * @name stylize
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        stylize: self.addInitalEvents(args.stylize),
        /**
        * Occurs when the Oda.UI.Dialog title is changed.
        * @event
        * @name titleChanged
        * @memberOf Oda.UI.Dialog
        * @public
        * @param {Native.Object} e Browser event object.
        * @param {Native.Object} Oda.UI.Dialog instance.
        */
        titleChanged: self.addInitalEvents(args.titleChanged)
    };
    self.createPublicMembers = function () {
        // API Interface
        
        /**
        * When used in an event listener, prevents the default event.
        * @function
        * @name preventDefault
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.preventDefault = function () {
            self.cancelDefault = true;
            return self.publicMembers;
        };
        /**
        * Closes the Oda.UI.Dialog.
        * @function
        * @name close
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.close = function () {
            self.close();
            return self.publicMembers;
        };
        /**
        * Initializes the Oda.UI.Dialog.  Should only be run once and only if dontInit argument was used during instantiation.
        * @function
        * @name init
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.init = function () {
            self.init();
            return self.publicMembers;
        };
        /**
        * Minimizes the Oda.UI.Dialog if it isn't already minimized.
        * @function
        * @name minimize
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.minimize = function () {
            self.minimize();
            return self.publicMembers;
        };
        /**
        * Maximizes the Oda.UI.Dialog if it is not already maximized.
        * @function
        * @name maximize
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.maximize = function () {
            self.maximize();
            return self.publicMembers;
        };
        /**
        * Restores the the Oda.UI.Dialog to the original size if it is maximized.
        * @function
        * @name restore
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.restore = function () {
            self.restore();
            return self.publicMembers;
        };
        /**
        * When returns true the Oda.UI.Dialog is the active Oda.UI.Dialog, otherwise false.
        * @function
        * @name isActive
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Native.Boolean} True the Oda.UI.Dialog is the active Oda.UI.Dialog, otherwise false.
        */
        self.publicMembers.isActive = function () {
            return self.isActive();
        };
        /**
        * Sets any number of objects as the title of the Oda.UI.Dialog.
        * @function
        * @name title
        * @param {Native.Object} [obj] Sets any number of objects as the title of the Oda.UI.Dialog.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.title = function (obj) {
            return self.title(obj);
        };
        /**
        * Moves the Oda.UI.Dialog to the center of the browser. If centerHorizontally and/or centerVertically are set.
        * @function
        * @name moveToCenter
        * @param {Oda.UI.Rect} [rect] The new Oda.UI.Rect to set.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.moveToCenter = function () {
            self.moveToCenter();
            return self.publicMembers;
        };
        /**
        * Places the Oda.UI.Dialog in a new position.
        * @function
        * @name setPosition
        * @param {Oda.UI.Point} [point] The new Oda.UI.Point to set.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.setPosition = function (point) {
            var rect = { x: point.x, y: point.y, w: self.rect.w, h: self.rect.h };
            self.setRect(rect);
            return self.publicMembers;
        };
        /**
        * Sets the Oda.UI.Dialog to a new size and position.
        * @function
        * @name setRect
        * @param {Oda.UI.Rect} [rect] The new Oda.UI.Rect to set.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.setRect = function (rect) {
            self.setRect(rect);
            return self.publicMembers;
        };
        /**
        * Shows the Oda.UI.Dialog if hidden.
        * @function
        * @name show
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.show = function () {
            self.show();
            return self.publicMembers;
        };
        /**
        * Hides the Oda.UI.Dialog if visible.
        * @function
        * @name hide
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.hide = function () {
            self.hide();
            return self.publicMembers;
        };
        /**
        * Applies the current Oda.UI.Style.Dialog to the Oda.UI.Dialog.
        * @function
        * @name redraw
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.redraw = function () {
            self.moveToCenter();
            self.stylize();
            return self.publicMembers;
        };
        /**
        * Adds a function to an event for this widget.   When the event occurs the function will execute in the context of this widget. 
        * Calling dialog.preventDefault() will cancel the default event.
        * @function
        * @name addEventListener
        * @param {Oda.UI.String} [eventName] The name of the event to subscribe to.
        * @param {Oda.UI.Function} [procedure] The function to execute when the event is raised.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.addEventListener = function (eventName, procedure, capturePhase) {
            self.addEventListener(eventName, procedure, capturePhase);
            return self.publicMembers;
        };
        /**
        * Removes a function from an event for this widget.  This function must match exactly the function to remove.
        * @function
        * @name removeEventListener
        * @param {Oda.UI.String} [eventName] The name of the event to unsubscribe from.
        * @param {Oda.UI.Function} [procedure] The function to execute when the event is raised.
        * @memberOf Oda.UI.Dialog
        * @public
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.removeEventListener = function (eventName, procedure, capturePhase) {
            self.removeEventListener(eventName, procedure, capturePhase);
            return self.publicMembers;
        };
        /**
        * The Oda.UI.Style.Dialog of this Oda.UI.Dialog.
        * @field
        * @name style
        * @type Oda.UI.Style.Dialog
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.style = self.style;
        /**
        * When set true, the dialog will be centered horizontally when moveToCenter.
        * @field
        * @name centerHorizontally
        * @type Native.Boolean
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.centerHorizontally = self.centerHorizontally;
        /**
        * When set true, the dialog will be centered vertically when moveToCenter.
        * @field
        * @name centerVertically
        * @type Native.Boolean
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.centerVertically = self.centerVertically;
        /**
        * The session unique id of the dialog.
        * @field
        * @name id
        * @type Native.String
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.id = self.id;
        /**
        * The type of widget. Returns Dialog.
        * @field
        * @name type
        * @type Native.String
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.type = self.type;
        /**
        * If the Oda.UI.Dialog is modal, then true otherwise false.
        * @function
        * @name isModal
        * @memberOf Oda.UI.Dialog
        * @returns {Native.Boolean} If the Oda.UI.Dialog is modal, then true otherwise false.
        */
        self.publicMembers.isModal = function () {
            return self.modal;
        };
        /**
        * When true, the content of the Oda.UI.Dialog will show the content while moving.
        * @field
        * @name showContentWhileMoving
        * @type Native.Boolean
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.showContentWhileMoving = self.showContentWhileMoving;
        /**
        * When true, the Oda.UI.Dialog can be resized.
        * @field
        * @name resizable
        * @type Native.Boolean
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.resizable = self.resizable;
        /**
        * Sets any number of objects as the content of the Oda.UI.Dialog. Can be a string, function or array of strings or functions.
        * @function
        * @name content
        * @param {Oda.UI.Object} [obj] The new content.
        * @memberOf Oda.UI.Dialog
        * @returns {Oda.UI.Dialog} Oda.UI.Dialog instance.
        */
        self.publicMembers.content = function (obj) {
            self.appendObj(self.content, obj);
            return self.publicMembers;
        };
        /**
        * The current Oda.UI.Rect of this Oda.UI.Dialog.
        * @field
        * @name rect
        * @type Oda.UI.Rect
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.rect = self.rect;
        /**
        * The current state of the Oda.UI.Dialog.  0: Normal, 1: minimized, 2: maximized, 3: hidden.
        * @field
        * @name state
        * @type Native.Number
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.state = self.state;
        /**
        * When true the dialog will be drawn on top of other non modal dialogs.
        * @field
        * @name alwaysOnTop
        * @type Native.Number
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.alwaysOnTop = self.alwaysOnTop;
        /**
        * When true the dialog will be moveable if false, the dialog cannot move.
        * @field
        * @name moveable
        * @type Native.Number
        * @memberOf Oda.UI.Dialog
        */
        self.publicMembers.moveable = self.moveable;
        return self;
    };
    // start the dialog
    self.init = function init() {
        // raise init event
        if (self.raiseEvent('init', undefined, undefined, undefined)) { return self; };
        // create an id for this widget
        self.id = Oda.UI.widgetIdCount++;
        // set (most) Boolean arguments  ---> for Booleans not default ? not default : default. 
        self.modal = args.modal === true ? true : false;
        self.modalCanClose = args.modalCanClose === true ? true : false;
        self.resizable = args.resizable === false ? false : true;
        self.moveable = args.moveable === false ? false : true;
        self.alwaysOnTop = args.alwaysOnTop === true ? true : false;
        self.dontInit = args.dontInit === true ? true : false;
        self.hidden = args.hidden === true ? true : false;
        self.hiddenFromTaskBar = args.hiddenFromTaskBar === true ? true : false;
        self.showContentWhileMoving = args.showContentWhileMoving === true ? true : false;
        // everything holder
        self.dialog = this.createElement('div');
        self.dialog.style.position = 'absolute';
        self.dialog.style.overflow = 'hidden';
        window.document.body.appendChild(self.dialog);
        // content
        self.content = this.createElement('div');
        self.content.style.position = 'absolute';
        self.dialog.appendChild(self.content);
        // control borders
        // title
        self.titleBar = this.createElement('div');
        self.titleBar.style.position = 'absolute';
        self.titleBar.style.overflow = 'hidden';
        self.dialog.appendChild(self.titleBar);
        // title text
        self.titleBarText = this.createElement('div');
        self.titleBarText.style.display = 'inline-block';
        self.titleBar.appendChild(self.titleBarText);
        // minimize button
        self.minimizeButton = this.createElement('button');
        self.minimizeButton.style.position = 'absolute';
        if (!self.modal) {
            self.titleBar.appendChild(self.minimizeButton);
        }
        // maximize button
        self.maximizeButton = this.createElement('button');
        self.maximizeButton.style.position = 'absolute';
        if (!self.modal) {
            self.titleBar.appendChild(self.maximizeButton);
        }
        // close
        self.closeButon = this.createElement('button');
        self.closeButon.style.position = 'absolute';
        if ((!self.modal) || self.modalCanClose) {
            self.titleBar.appendChild(self.closeButon);
        }
        // n
        self.n = this.createElement('div');
        self.n.style.position = 'absolute';
        self.dialog.appendChild(self.n);
        // s
        self.s = this.createElement('div');
        self.s.style.position = 'absolute';
        self.dialog.appendChild(self.s);
        // e
        self.e = this.createElement('div');
        self.e.style.position = 'absolute';
        self.dialog.appendChild(self.e);
        // w
        self.w = this.createElement('div');
        self.w.style.position = 'absolute';
        self.dialog.appendChild(self.w);
        // ne
        self.ne = this.createElement('div');
        self.ne.style.position = 'absolute';
        self.dialog.appendChild(self.ne);
        // se
        self.se = this.createElement('div');
        self.se.style.position = 'absolute';
        self.dialog.appendChild(self.se);
        // sw
        self.sw = this.createElement('div');
        self.sw.style.position = 'absolute';
        self.dialog.appendChild(self.sw);
        // nw
        self.nw = this.createElement('div');
        self.nw.style.position = 'absolute';
        self.dialog.appendChild(self.nw);
        // preview
        self.preview = this.createElement('div');
        if (!self.publicMembers.showContentWhileMoving) {
            self.preview.style.position = 'absolute';
            self.preview.style.visibility = 'hidden';
            self.preview.style.background = self.style.previewBackground;
            self.preview.style.outline = self.border(self.style.previewOutline);
            self.preview.style.zIndex = '99999';
            window.document.body.appendChild(self.preview);
        }
        // modal background
        self.modalBackground = this.createElement('div');
        if (self.modal) {
            self.modalBackground.style.position = 'absolute';
            self.modalBackground.style.top = '0px';
            self.modalBackground.style.left = '0px';
            self.modalBackground.style.width = self.client().w + 'px';
            self.modalBackground.style.height = self.client().h + 'px';
            self.modalBackground.style.background = self.style.modalBackground;
            window.document.body.appendChild(self.modalBackground);
        }
        // apply no select
        self.noSelect([self.titleBar, self.titleBarText, self.n, self.s, self.e,
        self.w, self.ne, self.se, self.sw, self.nw, self.closeButon,
        self.maximizeButton, self.minimizeButton, self.preview]);
        // setup initial rect
        args.rect = args.rect || {};
        self.publicMembers.rect = self.rect = {
            x: args.rect.x || self.style.rect.x,
            y: args.rect.y || self.style.rect.y,
            w: args.rect.w || self.style.rect.w,
            h: args.rect.h || self.style.rect.h
        };
        // resize centering
        self.centerVertically = args.centerVertically === undefined ? false : args.centerVertically;
        self.centerHorizontally = args.centerHorizontally === undefined ? false : args.centerHorizontally;
        // add dialog to set of dialogs
        Oda.UI.Widgets.Dialogs[self.id] = self;
        // set zIndex
        self.dialog.style.zIndex = ++Oda.UI.topZIndex;
        // make the dialog the active dialog
        Oda.UI.activeDialog = self;
        // apply stuff
        self.title(args.title);
        self.setRect(self.rect);
        self.moveToCenter();
        // stylize all dialogs to update active status
        Oda.UI.stylizeDialogs();
        // attach events
        self.titleBar.addEventListener('dblclick', self.maxRestoreButtonEvent, true);
        self.dialog.addEventListener('mousedown', self.startMoving, false);
        window.document.documentElement.addEventListener('mousemove', self.mouseMove, true);
        window.document.documentElement.addEventListener('mouseup', self.stopMoving, true);
        window.addEventListener('resize', self.browserResize, true);
        self.closeButon.addEventListener('click', self.closeButtonEvent, true);
        self.maximizeButton.addEventListener('click', self.maxRestoreButtonEvent, true);
        self.minimizeButton.addEventListener('click', self.minButtonEvent, true);
        self.closeButon.addEventListener('mousedown', self.buttonStartEvent, true);
        self.maximizeButton.addEventListener('mousedown', self.buttonStartEvent, true);
        self.minimizeButton.addEventListener('mousedown', self.buttonStartEvent, true);
        self.dialog.addEventListener('mouseout', self.mouseoffButtons, false);
        self.closeButon.addEventListener('mouseover', function () { self.mouseoverButtons(0); }, true);
        self.maximizeButton.addEventListener('mouseover', function () { self.mouseoverButtons(1); }, true);
        self.minimizeButton.addEventListener('mouseover', function () { self.mouseoverButtons(2); }, true);
        // create publicMembers
        self.createPublicMembers();
        return self;
    };
    self.mouseoffButtons = function () {
        var a = Oda.UI.activeDialog === self ? self.style.Active : self.style.Inactive;
        self.styleElement(self.closeButon, '', a.closeButtonBackground); // close
        self.styleElement(self.minimizeButton, '', a.minimizeButtonBackground); // min
        if (self.state === 2) {
            self.styleElement(self.maximizeButton, '', a.restoreButtonBackground); // restore
        } else {
            self.styleElement(self.maximizeButton, '', a.maximizeButtonBackground); // max
        }
    };
    self.mouseoverButtons = function (buttonId) {
        var a = Oda.UI.activeDialog === self ? self.style.Active : self.style.Inactive;
        // 0 = close, 1 = max, 2 = min
        if (buttonId === 0) {
            self.styleElement(self.closeButon, '', a.Hover.closeButtonBackground); // close
        }
        if (buttonId === 2) {
            self.styleElement(self.minimizeButton, '', a.Hover.minimizeButtonBackground); // min
        }
        if (buttonId === 1) {
            if (self.state === 2) {
                self.styleElement(self.maximizeButton, '', a.Hover.restoreButtonBackground); // restore
            } else {
                self.styleElement(self.maximizeButton, '', a.Hover.maximizeButtonBackground); // max
            }
        }
    };
    // when a control button is pressed this turns true
    // why not cheer it up?
    self.controlButtonDepressed = false;
    // move states 0: none, 1: titlebar move, 2: nw resize, 3: n  resize, 4: ne resize
    // 5: e  resize, 6: se resize, 7: s  resize, 8: sw resize, 9: w  resize
    // 10: min button, 11: max button, 12: close button
    self.moveState = 0;
    // states 0: Normal, 1: minimized, 2: maximized, 3: hidden
    self.state = 0;
    self.originalBodyOverflowStyle = '';
    // set the move state of the dialog
    self.getMoveState = function (e) {
        if (self.attached) { return self.moveState; };
        var pos = self.mouse(e, self.dialog);
        var s = self.style;
        if (pos.y <= s.neRect.h + s.neRect.y &&
        pos.x >= (self.rect.w - (s.eRect.x + s.neRect.w)) && self.publicMembers.resizable) {
            self.moveState = 4; //ne
            self.dialog.style.cursor = 'ne-resize';
        } else if (pos.y >= (self.rect.h - (s.seRect.y + s.seRect.h)) &&
        pos.x >= (self.rect.w - (s.seRect.x + s.seRect.w)) && self.publicMembers.resizable) {
            self.moveState = 6; //se
            self.dialog.style.cursor = 'se-resize';
        } else if (pos.y >= (self.rect.h - (s.swRect.y + s.swRect.h)) &&
        pos.x <= s.swRect.w && self.publicMembers.resizable) {
            self.moveState = 8; //sw
            self.dialog.style.cursor = 'sw-resize';
        } else if (pos.y <= s.nwRect.h + s.nwRect.y &&
        pos.x <= s.nwRect.w && self.publicMembers.resizable) {
            self.moveState = 2; //nw
            self.dialog.style.cursor = 'nw-resize';
        } else if (pos.y <= s.nRect.h + s.nRect.y && self.publicMembers.resizable) {
            self.moveState = 3; //n
            self.dialog.style.cursor = 'n-resize';
        } else if (pos.y >= (self.rect.h - (s.sRect.y + s.sRect.h)) && self.publicMembers.resizable) {
            self.moveState = 7; //s
            self.dialog.style.cursor = 's-resize';
        } else if (pos.x >= (self.rect.w - (s.eRect.x + s.eRect.w)) && self.publicMembers.resizable) {
            self.moveState = 5; //e
            self.dialog.style.cursor = 'e-resize';
        } else if (pos.x <= s.wRect.w && self.publicMembers.resizable) {
            self.moveState = 9; //w
            self.dialog.style.cursor = 'w-resize';
        } else if (pos.y <= (s.titleRect.h + s.titleRect.y) && self.publicMembers.moveable) {
            self.moveState = 1; //titlebar
            self.dialog.style.cursor = 'move';
        } else {
            self.moveState = 0; //none
            self.dialog.style.cursor = 'default';
        }
        return self;
    };
    self.isActive = function isActive() {
        return Oda.UI.activeDialog === self;
    };
    self.title = function title(obj) {
        if (self.raiseEvent('titleChanged', self.titleBarText, obj, undefined)) { return self; };
        if (obj === undefined) { return self; }
        self.titleBarText.originalText = undefined;
        self.appendObj(self.titleBarText, obj);
        return self.titleBarText.textContent || self.titleBarText.innerText;
    };
    self.attached = false;
    self.mouseOffset = { x: 0, y: 0 };
    self.showPosition = { x: 0, y: 0 };
    self.mouseMove = function (e) {
        if (!self.publicMembers.showContentWhileMoving && self.attached && self.moveState !== 0) {
            self.preview.style.visibility = 'visible';
        }
        self.getMoveState(e);
        var o = self.mouseLiteral(e);
        var newRect = { x: self.rect.x, y: self.rect.y, h: self.rect.h, w: self.rect.w };
        if (self.moveState === 1 && self.attached) {
            //move
            newRect.x = o.x - self.mouseOffset.x;
            newRect.y = o.y - self.mouseOffset.y;
        } else if (self.moveState === 3 && self.attached) {
            //n
            newRect.y = o.y - self.mouseOffset.y;
            newRect.h = self.resizeOffset.y - newRect.y + self.resizeOffset.h;
        } else if (self.moveState === 9 && self.attached) {
            //w
            newRect.x = o.x - self.mouseOffset.x;
            newRect.w = self.resizeOffset.x - newRect.x + self.resizeOffset.w;
        } else if (self.moveState === 5 && self.attached) {
            //e
            newRect.w = self.resizeOffset.w + (o.x - self.mouseOffset.x) - self.resizeOffset.x;
        } else if (self.moveState === 7 && self.attached) {
            //s
            newRect.h = self.resizeOffset.h + (o.y - self.mouseOffset.y) - self.resizeOffset.y;
        } else if (self.moveState === 4 && self.attached) {
            //ne
            newRect.y = o.y - self.mouseOffset.y;
            newRect.h = self.resizeOffset.y - newRect.y + self.resizeOffset.h;
            newRect.w = self.resizeOffset.w + (o.x - self.mouseOffset.x) - self.resizeOffset.x;
        } else if (self.moveState === 6 && self.attached) {
            //se
            newRect.w = self.resizeOffset.w + (o.x - self.mouseOffset.x) - self.resizeOffset.x;
            newRect.h = self.resizeOffset.h + (o.y - self.mouseOffset.y) - self.resizeOffset.y;
        } else if (self.moveState === 8 && self.attached) {
            //sw
            newRect.x = o.x - self.mouseOffset.x;
            newRect.w = self.resizeOffset.x - newRect.x + self.resizeOffset.w;
            newRect.h = self.resizeOffset.h + (o.y - self.mouseOffset.y) - self.resizeOffset.y;
        } else if (self.moveState === 2 && self.attached) {
            //nw
            newRect.x = o.x - self.mouseOffset.x;
            newRect.w = self.resizeOffset.x - newRect.x + self.resizeOffset.w;
            newRect.y = o.y - self.mouseOffset.y;
            newRect.h = self.resizeOffset.y - newRect.y + self.resizeOffset.h;
        }
        if (self.attached) {
            if (self.moveState === 1) {
                if (self.raiseEvent('move', undefined, undefined, undefined)) { return self; };
            } else if (self.moveState !== 0) {
                if (self.raiseEvent('resize', undefined, undefined, undefined)) { return self; };
            }
            self.setRect(newRect);
        }
        return self;
    };
    self.startMoving = function startMoving(e) {
        if (self.controlButtonDepressed) { return; };
        Oda.UI.activeDialog = self;
        Oda.UI.stylizeDialogs();
        self.publicMembers.centerHorizontally = false;
        self.publicMembers.centerVertically = false;
        self.attached = true;
        self.mouseOffset = self.mouse(e, self.titleBar);
        self.mouseOffset.x += self.style.wRect.x + self.style.wRect.w;
        self.mouseOffset.y += +self.style.nRect.y + self.style.nRect.h;
        self.resizeOffset = self.rect;
    };
    self.stopMoving = function stopMoving() {
        self.controlButtonDepressed = false;
        self.attached = false;
        if (!self.publicMembers.showContentWhileMoving) {
            self.preview.style.visibility = 'hidden';
            self.setRect(self.rect);
        }
    };
    self.moveToCenter = function moveToCenter() {
        if (self.publicMembers.centerHorizontally) {
            self.rect.x = parseInt(self.client().w * .5, 10) -
                parseInt(self.rect.w * .5, 10);
        }
        if (self.publicMembers.centerVertically) {
            self.rect.y = parseInt(self.client().h * .5, 10) -
                parseInt(self.rect.h * .5, 10);
        }
        return self.setRect(self.rect);
    };
    self.browserResize = function browserResize(e) {
        self.resizeMaximized();
        self.moveToCenter(e);
    };
    self.resizeMaximized = function () {
        if (self.state === 2) {
            self.rect.x = 0 + self.style.maximizeOffsetRect.x;
            self.rect.y = 0 + self.style.maximizeOffsetRect.y;
            self.rect.w = self.client().w + self.style.maximizeOffsetRect.w;
            self.rect.h = self.client().h + self.style.maximizeOffsetRect.h;
            self.setRect(self.rect).stylize();
            if (self.raiseEvent('resize', undefined, undefined, undefined)) { return self; };
        }
        return self;
    };
    self.minimize = function () {
        if (self.raiseEvent('minimize', undefined, undefined, undefined)) { return self; };
        self.originalBodyOverflowStyle = document.body.style.overflow;
        self.originalScroll = { top: window.document.documentElement.scrollTop, left: window.document.documentElement.scrollLeft };
        self.state = 1;
        self.dialog.style.visibility = 'hidden';
        return self;
    };
    self.maximize = function () {
        if (self.raiseEvent('maximize', undefined, undefined, undefined)) { return self; };
        self.state = 2;
        self.publicMembers.resizable = false;
        self.publicMembers.moveable = false;
        self.restoreRect = { x: self.rect.x, y: self.rect.y, h: self.rect.h, w: self.rect.w };
        self.originalBodyOverflowStyle = document.body.style.overflow;
        self.originalScroll = { top: window.document.documentElement.scrollTop, left: window.document.documentElement.scrollLeft };
        window.document.documentElement.scrollTop = 0;
        window.document.documentElement.scrollLeft = 0;
        document.body.style.overflow = 'hidden';
        self.resizeMaximized();
        return self;
    };
    self.restore = function () {
        if (self.raiseEvent('restore', undefined, undefined, undefined)) { return self; };
        document.body.style.overflow = self.originalBodyOverflowStyle;
        window.document.documentElement.scrollTop = self.originalScroll.top;
        window.document.documentElement.scrollLeft = self.originalScroll.left;
        self.state = 0;
        self.publicMembers.resizable = true;
        self.publicMembers.moveable = true;
        self.rect = { x: self.restoreRect.x, y: self.restoreRect.y, h: self.restoreRect.h, w: self.restoreRect.w };
        self.dialog.style.visibility = 'visible';
        self.setRect(self.rect).stylize();
        return self;
    };
    self.minButtonEvent = function () {
        self.controlButtonDepressed = false;
        self.minimize();
    };
    self.maxRestoreButtonEvent = function () {
        self.controlButtonDepressed = false;
        if (self.state === 2) {//maximized
            self.restore();
        } else {
            self.maximize();
        }
    };
    // when a button is pressed, stop moving and resizing
    // and listen for when it gets clicked
    self.buttonStartEvent = function () {
        Oda.UI.activeDialog = self;
        Oda.UI.stylizeDialogs();
        self.controlButtonDepressed = true;
    };
    self.closeButtonEvent = function () {
        self.controlButtonDepressed = false;
        self.close();
    };
    self.close = function close() {
        if (self.raiseEvent('close', undefined, undefined, undefined)) { return self; }
        self.dispose();
        return self;
    };
    self.updateElementRect = function (e, w, h, x, y) {
        e.style.width = w + 'px';
        e.style.height = h + 'px';
        e.style.left = x + 'px';
        e.style.top = y + 'px';
        return self;
    };
    self.show = function () {
        if (!self.hidden) { return self; }
        if (self.raiseEvent('show', undefined, undefined, undefined)) { return self; };
        self.hidden = false;
        self.rect.x = self.showPosition.x;
        self.rect.y = self.showPosition.y;
        self.showPosition = undefined;
        return self.setRect(self.rect);
    };
    self.hide = function () {
        if (self.hidden) { return self; }
        if (self.raiseEvent('hide', undefined, undefined, undefined)) { return self; };
        self.hidden = true;
        self.showPosition = undefined;
        return self.setRect(self.rect);
    };
    self.setRect = function (rect) {
        var s = self.style;
        rect.x = rect.x <= s.minRect.x ? s.minRect.x : rect.x;
        rect.y = rect.y <= s.minRect.y ? s.minRect.y : rect.y;
        rect.w = rect.w <= s.minRect.w ? s.minRect.w : rect.w;
        rect.h = rect.h <= s.minRect.h ? s.minRect.h : rect.h;
        self.publicMembers.rect = self.rect = rect;
        if (self.hidden && self.showPosition === undefined) {
            self.showPosition = { x:self.rect.x,y:self.rect.y };
            rect.x = -10000;
            rect.y = -10000;
        }
        if (self.attached && !self.publicMembers.showContentWhileMoving) {
            self.updateElementRect(self.preview, rect.w, rect.h, rect.x, rect.y);
            return self;
        }
        // updateElementRect( w, h, x, y )
        // dialog
        self.updateElementRect(self.dialog, rect.w, rect.h, rect.x, rect.y);
        // title
        self.updateElementRect(self.titleBar,
        rect.w + s.titleRect.w - s.wRect.w - s.eRect.w - s.titlePadding.r,
        s.titleRect.h - s.titlePadding.t - s.titlePadding.b, s.titleRect.x + s.wRect.w + s.wRect.x, s.titleRect.y + s.nRect.h + s.nRect.y);
        // title text
        self.titleBarText.style.height = s.titleRect.h - s.titlePadding.t - s.titlePadding.b + 'px';
        //content
        self.updateElementRect(self.content,
        rect.w + s.contentRect.w - s.wRect.x - s.wRect.x - s.wRect.w - s.eRect.w, // w
        rect.h + s.contentRect.h - s.titleRect.h - s.titleRect.y - s.nRect.y - s.nRect.h + s.sRect.y - s.sRect.h, // h
        s.wRect.w + s.wRect.x + s.contentRect.x, // x
        s.contentRect.y + s.titleRect.h + s.nRect.h + s.nRect.y + s.titleRect.y); // y
        // n
        self.updateElementRect(self.n, rect.w - s.wRect.x - s.wRect.w - s.eRect.w,
        s.nRect.h, s.nRect.x + s.wRect.w + s.wRect.x, s.nRect.y);
        // ne
        self.updateElementRect(self.ne, s.neRect.w,
        s.neRect.h, rect.w + s.neRect.x - s.neRect.w,
        s.neRect.y);
        // e
        self.updateElementRect(self.e, s.eRect.w,
        rect.h + s.eRect.h - s.nRect.y - s.nRect.h - s.sRect.h,
        rect.w + s.eRect.x - s.eRect.w,
        s.eRect.y + s.nRect.y + s.nRect.h);
        // se
        self.updateElementRect(self.se, s.seRect.w,
        s.seRect.h, rect.w + s.seRect.x - s.seRect.w,
        rect.h + s.seRect.y - s.seRect.h);
        // s
        self.updateElementRect(self.s, rect.w - s.wRect.x - s.wRect.w - s.eRect.w,
        s.sRect.h, s.sRect.x + s.wRect.w + s.wRect.x, rect.h + s.sRect.y - s.sRect.h);
        // sw
        self.updateElementRect(self.sw, s.swRect.w,
        s.swRect.h, s.swRect.x, rect.h + s.swRect.y - s.swRect.h);
        // w
        self.updateElementRect(self.w, s.wRect.w,
        rect.h + s.wRect.h - s.nRect.y - s.nRect.h - s.sRect.h,
        s.wRect.x, s.wRect.y + s.nRect.y + s.nRect.h);
        // nw
        self.updateElementRect(self.nw, s.nwRect.w,
        s.nwRect.h, s.nwRect.x, s.nwRect.y);
        // min
        self.updateElementRect(self.minimizeButton, s.minimizeRect.w,
        s.minimizeRect.h, rect.w + s.closeRect.x + s.maximizeRect.x +
        s.minimizeRect.x - s.titlePadding.l - s.minimizeRect.w - s.maximizeRect.w -
        s.closeRect.w - (s.Active.buttonBorder.size * 5),
        s.minimizeRect.y - s.Active.buttonBorder.size);
        // max
        self.updateElementRect(self.maximizeButton, s.maximizeRect.w,
        s.maximizeRect.h, rect.w + s.closeRect.x + s.maximizeRect.x - s.titlePadding.l -
        s.maximizeRect.w - s.closeRect.w - (s.Active.buttonBorder.size * 3),
        s.maximizeRect.y - s.Active.buttonBorder.size);
        // close
        self.updateElementRect(self.closeButon, s.closeRect.w,
        s.closeRect.h, rect.w + s.closeRect.x - s.titlePadding.l -
        s.closeRect.w,
        s.closeRect.y - s.Active.buttonBorder.size);
        // clip text
        self.clipText(self.titleBarText,
            rect.w + s.titleRect.w - s.wRect.w - s.eRect.w - s.titlePadding.l - s.titlePadding.r -
            s.closeRect.w - s.maximizeRect.w - s.minimizeRect.w);
        return self;
    };
    self.styleElement = function styleElement(e, c, b) {
        e.style.color = c;
        e.style.background = b;
        return self;
    };
    self.stylize = function applyStyle() {// apply the style
        if (self.raiseEvent('stylize', undefined, undefined, undefined)) { return self; };
        var s = self.style;
        var a = Oda.UI.activeDialog === self ? self.style.Active : self.style.Inactive;
        if (Oda.UI.activeDialog === self) {
            if (self.modal || self.publicMembers.alwaysOnTop) {
                self.modalBackground.style.zIndex = ++Oda.UI.topModalZIndex;
                self.dialog.style.zIndex = ++Oda.UI.topModalZIndex;
            } else {
                self.dialog.style.zIndex = ++Oda.UI.topZIndex;
            }
        }
        self.titleBar.style.borderBottom = self.border(a.titleBorder);
        self.titleBarText.style.padding = self.pad(s.titlePadding);
        self.titleBarText.style.fontFamily = s.titleFont;
        self.styleElement(self.dialog, a.dialogColor, a.dialogBackground);// dialog
        self.styleElement(self.titleBar, a.titleColor, a.titleBackground);// title
        self.styleElement(self.content, a.contentColor, a.contentBackground);//content
        self.styleElement(self.n, '', a.nBackground);   // n
        self.styleElement(self.ne, '', a.neBackground); // ne
        self.styleElement(self.e, '', a.eBackground);   // e
        self.styleElement(self.se, '', a.seBackground); // se
        self.styleElement(self.s, '', a.sBackground);   // s
        self.styleElement(self.sw, '', a.swBackground); // sw
        self.styleElement(self.w, '', a.wBackground);   // w
        self.styleElement(self.nw, '', a.nwBackground); // nw
        self.styleElement(self.closeButon, '', a.closeButtonBackground); // close
        self.styleElement(self.minimizeButton, '', a.minimizeButtonBackground); // min
        if (self.state === 2) {
            self.styleElement(self.maximizeButton, '', a.restoreButtonBackground); // restore
        } else {
            self.styleElement(self.maximizeButton, '', a.maximizeButtonBackground); // max
        }
        var b = [self.closeButon, self.maximizeButton, self.minimizeButton];
        for (var x = 0; b.length > x; x++) {
            b[x].style.border = self.border(a.buttonBorder);
            b[x].style.padding = '0';
            b[x].style.margin = '0';
        }
        return self;
    };
    self.dispose = function dispose() {
        if (self.raiseEvent('dispose', undefined, undefined, undefined)) { return self; }
        // remove events
        self.titleBar.removeEventListener('dblclick', self.maxRestoreButtonEvent, true);
        self.dialog.removeEventListener('mousedown', self.startMoving, false);
        document.documentElement.removeEventListener('mouseup', self.stopMoving, true);
        document.documentElement.removeEventListener('mousemove', self.getMoveState, true);
        self.closeButon.removeEventListener('mousedown', self.buttonStartEvent, true);
        self.maximizeButton.removeEventListener('mousedown', self.buttonStartEvent, true);
        self.minimizeButton.removeEventListener('mousedown', self.buttonStartEvent, true);
        self.closeButon.removeEventListener('click', self.closeButtonEvent, true);
        self.maximizeButton.removeEventListener('click', self.maxRestoreButtonEvent, true);
        self.minimizeButton.removeEventListener('click', self.minButtonEvent, true);
        self.dialog.removeEventListener('mouseout', self.mouseoffButtons, false);
        self.closeButon.removeEventListener('mouseover', function () { self.mouseoverButtons(0); }, true);
        self.maximizeButton.removeEventListener('mouseover', function () { self.mouseoverButtons(1); }, true);
        self.minimizeButton.removeEventListener('mouseover', function () { self.mouseoverButtons(2); }, true);
        // remove global ref
        delete Oda.UI.Widgets.Dialogs[self.id];
        // remove from DOM
        document.body.removeChild(self.dialog);
        if (self.modal) {
            document.body.removeChild(self.modalBackground);
        }
        return null;
    };
    if (!args.dontInit) {
        self.init();
    }
    // return publicMembers
    return self.publicMembers;
};