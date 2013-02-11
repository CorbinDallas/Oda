/**
* Holds all Oda classes and stuff related to these classes
* such as widgets, styles for widgets, static fields that allow widgets to
* talk back and forth and AJAX stuff for communicating with the Oda server
* components.
* @namespace 
* @name Oda
* @version 0.1.0 beta release
* @author Tony Germaneri (TonyGermaneri@gmail.com)
*/
var Oda = {
    /**
    * Causes one object to inherit another as a prototype.
    * @function 
    * @name Oda.beget
    * @param {Native.String} [o] The object to inherit.
    * @returns {Native.Object} The new object.
    */
    beget: function (o) {
        var f = function () { };
        f.prototype = o;
        return new f();
    },
    /**
    * Tests a value to see if it's a number.
    * @function 
    * @name Oda.isNumber
    * @param {Native.Object} [value] The object to test.
    * @returns {Native.Boolean} When the value is a number then returns true otherwise false.
    */
    isNumber: function (value) {
        return typeof value === 'number' && isFinite(value);
    },
    /**
    * Tests a value to see if it's an array.
    * @function 
    * @name Oda.isArray
    * @param {Native.Object} [value] The object to test.
    * @returns {Native.Boolean} When the value is an array then returns true otherwise false.
    */
    isArray: function (value) {
        return value && typeof value === 'object' &&
            typeof value.length === 'number' &&
            !(value.propertyIsEnumerable('length'));
    },
    /**
    * Holds all widgets and supporting methods.  
    * @namespace 
    * @memberOf Oda
    */
    UI: {
        /**
        * Global counter of widgets that have ever been instantiated in this session.  Used to create ids for widgets.
        * @property
        * @type Native.Integer
        * @name widgetIdCount
        * @memberOf Oda.UI
        * @public
        */
        widgetIdCount: 0,
        /**
        * Updates the style of stacked dialogs, giving some the focused style and others the unfocused style.
        * @function
        * @type Native.Function
        * @name stylizeDialogs
        * @memberOf Oda.UI
        * @public
        */
        stylizeDialogs: function () {
            for (d in this.Widgets.Dialogs) {
                if (this.Widgets.Dialogs.hasOwnProperty(d)) {
                    this.Widgets.Dialogs[d].stylize();
                }
            }
        },
        /**
        * Point class for Oda.UI.Widget objects.
        * @class
        * @type Native.Object
        * @name Point
        * @memberOf Oda.UI
        * @public
        */
        Point: {
            /**
            * The horizontal coordinate.
            * @field
            * @name x
            * @type Native.Number
            * @memberOf Oda.UI.Point
            */
            x: 0,
            /**
            * The vertical coordinate.
            * @field
            * @name y
            * @type Native.Number
            * @memberOf Oda.UI.Point
            */
            y: 0
        },
        /**
        * Rectangle class for Oda.UI.Widget objects.
        * @class
        * @type Native.Object
        * @name Rect
        * @memberOf Oda.UI
        * @public
        */
        Rect: {
            /**
            * The horizontal coordinate.
            * @field
            * @name x
            * @type Native.Number
            * @memberOf Oda.UI.Rect
            */
            x: 0,
            /**
            * The vertical coordinate.
            * @field
            * @name y
            * @type Native.Number
            * @memberOf Oda.UI.Rect
            */
            y: 0,
            /**
            * The height of the rectangle.
            * @field
            * @name h
            * @type Native.Number
            * @memberOf Oda.UI.Rect
            */
            h: 0,
            /**
            * The width of the rectangle.
            * @field
            * @name w
            * @type Native.Number
            * @memberOf Oda.UI.Rect
            */
            w: 0
        },
        /**
        * Padding class for Oda.UI.Widget objects.
        * @class
        * @type Native.Object
        * @name Padding
        * @memberOf Oda.UI
        * @public
        */
        Padding: {
            /**
            * The top amount of padding in pixels.
            * @field
            * @name t
            * @type Native.Number
            * @memberOf Oda.UI.Padding
            */
            t: 0,
            /**
            * The right amount of padding in pixels.
            * @field
            * @name r
            * @type Native.Number
            * @memberOf Oda.UI.Padding
            */
            r: 0,
            /**
            * The bottom amount of padding in pixels.
            * @field
            * @name b
            * @type Native.Number
            * @memberOf Oda.UI.Padding
            */
            b: 0,
            /**
            * The left amount of padding in pixels.
            * @field
            * @name l
            * @type Native.Number
            * @memberOf Oda.UI.Padding
            */
            l: 0
        },
        /**
        * Border class for Oda.UI.Widget objects.
        * @class
        * @type Native.Object
        * @name Border
        * @memberOf Oda.UI
        * @public
        */
        Border: {
            /**
            * The size of the border in pixels.
            * @field
            * @name size
            * @type Native.Number
            * @memberOf Oda.UI.Border
            */
            size: 0,
            /**
            * The style of the border.  E.g.: 'solid', 'dashed', etc..
            * @field
            * @name style
            * @type Native.String
            * @memberOf Oda.UI.Border
            */
            style: '',
            /**
            * The color of the border in standard CSS color code.
            * @field
            * @name color
            * @type Native.String
            * @memberOf Oda.UI.Border
            */
            color: ''
        },
        /**
        * The active dialog.
        * @property
        * @type Oda.UI.Dialog
        * @name activeDialog
        * @memberOf Oda.UI
        * @public
        */
        activeDialog: undefined,
        /**
        * The top CSS z-index for Oda.UI.Dialog objects.
        * @property
        * @type Oda.UI.Number
        * @name topZIndex
        * @memberOf Oda.UI
        * @public
        */
        topZIndex: 0,
        /**
        * The top modal CSS z-index for Oda.UI.Dialog objects.
        * @property
        * @type Oda.UI.Number
        * @name topModalZIndex
        * @memberOf Oda.UI
        * @public
        */
        topModalZIndex: 100,
        /**
        * Prototype for Oda.UI.WidgetStyle implementations.
        * @class
        * @type Native.Object
        * @name WidgetStyle
        * @memberOf Oda.UI
        * @public
        */
        WidgetStyle: {
            /**
            * The unique id of this Oda.UI.WidgetStyle.
            * @field
            * @name id
            * @type Native.String
            * @memberOf Oda.UI.WidgetStyle
            */
            id: '',
            /**
            * The name of this widget Oda.UI.WidgetStyle.
            * @field
            * @name type
            * @type Native.String
            * @memberOf Oda.UI.WidgetStyle
            */
            type: 'WidgetStyle',
            /**
            * The default font family for all widgets.
            * @field
            * @name defaultFontFamily
            * @type Native.String
            * @memberOf Oda.UI.WidgetStyle
            */
            defaultFontFamily: 'Segoe UI Light, Segoe UI, Lucida Grande, Verdana, Arial, Helvetica, sans-serif'
        },
        /**
        * Collection of widgets implementing Oda.UI.Widget.
        * @class
        * @type Native.Object
        * @name Widgets
        * @memberOf Oda.UI
        * @public
        */
        Widgets: {
            /**
            * Collection of Oda.UI.Dialog instances.
            * @field
            * @name Dialogs
            * @type Native.String
            * @memberOf Oda.UI
            */
            Dialogs: {}
        },
        /**
        * Set of styles for objects implementing Oda.UI.Widget.
        * @namespace
        * @type Native.Object
        * @name Style
        * @memberOf Oda.UI
        * @public
        */
        Style: {},
        /**
        * Prototype for objects implementing Oda.UI.Widget.
        * @class
        * @type Native.Object
        * @name Widget
        * @memberOf Oda.UI
        * @public
        */
        Widget: {
            /**
            * The unique id of this Oda.UI.Widget.
            * @field
            * @name id
            * @type Native.String
            * @memberOf Oda.UI.Widget
            */
            id: '',
            /**
            * The type of this Oda.UI.Widget.
            * @field
            * @name type
            * @type Native.String
            * @memberOf Oda.UI.Widget
            */
            type: 'Widget',
            /**
            * When set true in an event handler the default event will not occur.
            * @field
            * @name cancelDefault
            * @type Native.Boolean
            * @memberOf Oda.UI.Widget
            */
            cancelDefault: false,
            /**
            * Converts a Oda.UI.Rect object into a string for debugging.
            * @function
            * @type Native.Function
            * @name rectToString
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Rect} [rect] The Oda.UI.Rect to convert to a string.
            * @returns {Native.String} String representation of the Oda.UI.Rect object.
            * @public
            */
            rectToString: function (rect) {
                return "x:" + rect.x + ",y:" + rect.y + ",w:" + rect.w + ",h:" + rect.h;
            },
            /**
            * Clips the text to a specified with adding "..." to text that exceeds the width parameter.
            * @function
            * @type Native.Function
            * @name clipText
            * @memberOf Oda.UI.Widget
            * @param {Native.HTMLElement} [obj] The element to clip.
            * @param {Native.Number} [width] The maximum width of the element before clipping occurs.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            clipText: function (obj, width) {
                var usesInnerText = obj.innerText !== undefined;
                if (obj.originalText === undefined) {
                    obj.originalText = obj.innerText || obj.textContent;
                }
                this.setTextContent(obj, obj.originalText);
                var i = obj.originalText.length;
                while (width < obj.offsetWidth) {
                    i--;
                    if (i < 0) { return this; }
                    this.setTextContent(obj, obj.originalText.substring(0, i) + "...");
                }
                return this;
            },
            /**
            * Sets the text content of an HTML Element.
            * @function
            * @type Native.Function
            * @name setTextContent
            * @memberOf Oda.UI.Widget
            * @param {Native.HTMLElement} [obj] The object to set text.
            * @param {Native.String} [text] The text to set.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            setTextContent: function (obj, text) {
                if (obj.innerText) {
                    obj.innerText = text;
                } else {
                    obj.textContent = text;
                }
                return this;
            },
            /**
            * Prevents selection of the elements passed to this function.
            * @function
            * @type Native.Function
            * @name noSelect
            * @memberOf Oda.UI.Widget
            * @param {Native.Array} [eles] The array list of elements to prevent selection.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            noSelect: function (eles) {
                try {
                    for (var x = 0; eles.length > x; x++) {
                        eles[x].onselectstart = function() { return false; };
                        eles[x].unselectable = "on"; // For IE and Opera
                        eles[x].style.userSelect = "none";
                        eles[x].style.webkitUserSelect = "none";
                        eles[x].style.MozUserSelect = "none";
                    }
                } catch (e) { };
                return this;
            },
            /**
            * Returns a CSS formatted string from the Oda.UI.Padding object.
            * @function
            * @type Native.Function
            * @name pad
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Padding} [padRect] The Oda.UI.Padding object to convert to a CSS string.
            * @param {Native.Number} [width] The maximum width of the element before clipping occurs.
            * @returns {Native.String} CSS padding string.
            * @public
            */
            pad: function (padRect) {
                return padRect.t + 'px ' + padRect.r + 'px ' + padRect.b + 'px ' + padRect.l + 'px';
            },
            /**
            * Returns a CSS formatted string from the Oda.UI.Border object.
            * @function
            * @type Native.Function
            * @name border
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Border} [padRect] The Oda.UI.Border to turn into a CSS string.
            * @param {Native.Number} [width] The maximum width of the element before clipping occurs.
            * @returns {Native.String} CSS border string.
            * @public
            */
            border: function (obj) {
                return obj.style + ' ' + obj.size + 'px ' + obj.color;
            },
            /**
            * Appends the result of a function, an element a HTML string or an array of elements, function and strings to a HTML Element.
            * @function
            * @type Native.Function
            * @name appendObj
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.HTMLElement} [parentNode] The HTML Element to append to..
            * @param {Native.Object} [obj] The function, element, string or array of functions elements and strings to append.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            appendObj: function (parentNode, obj) {
                if (typeof obj === 'string'
                    || typeof obj === 'number'
                    || typeof obj === 'boolean') { // string, number, boolean goes in as Html
                    parentNode.innerHTML = obj;
                    return this;
                } else if (obj.nodeType !== undefined) { // Element goes in as child node
                    parentNode.appendChild(obj);
                    return this;
                } else if (typeof obj === 'function') { // function gets applied recursively
                    this.appendObj(parentNode, obj.apply(this, []));
                    return this;
                } else if (Oda.isArray(obj)) { // array elements get applied recursively
                    var l = obj.length;
                    for (var x = 0; l > x; x++) {
                        this.appendObj(parentNode, obj[x]);
                    }
                    return this;
                } else { // null, undefined, blah don't do anything
                    return this;
                }
            },
            /**
            * Gets the Oda.UI.Rect defined by the element.
            * @function
            * @type Native.Function
            * @name getRect
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.HTMLElement} [obj] The HTML Element to get the Oda.UI.Rect for.
            * @returns {Oda.UI.Rect} The Oda.UI.Rect for the HTML element passed to the function.
            * @public
            */
            getRect: function (obj) {
                return { x: obj.offsetLeft, y: obj.offsetTop, w: obj.offsetWidth, h: obj.offsetHeight };
            },
            /**
            * Gets the position of the mouse within the HTML element in the second parameter.
            * @function
            * @type Native.Function
            * @name mouse
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Object} [e] The browser's event object.
            * @param {Oda.UI.HTMLElement} [obj] The HTML Element to get the mouse's position on.
            * @returns {Oda.UI.Point} Oda.UI.Point that represents the position of the mouse.
            * @public
            */
            mouse: function (e, obj) {
                e = e || window.event;
                var objPos = this.position(obj);
                var mousePos = this.mouseLiteral(e);
                return { x: mousePos.x - objPos.x, y: mousePos.y - objPos.y };
            },
            /**
            * Creates an HTML Element.  Same as document.createElement() but with slightly less typing.
            * @function
            * @type Native.Function
            * @name createElement
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.String} [eleType] The type of element to create.
            * @returns {Oda.UI.HTMLElement} The HTML element created.
            * @public
            */
            createElement: function (eleType) {
                return document.createElement(eleType);
            },
            /**
            * Gets position of an element relevant to the browser window.
            * @function
            * @type Native.Function
            * @name position
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.HTMLElement} [e] The element to get the position of.
            * @returns {Oda.UI.Point} Oda.UI.Point that represents the position of the HTML element passed to the function.
            * @public
            */
            position: function (e) {
                var x = 0;
                var y = 0;
                while (e.offsetParent) {
                    x += e.offsetLeft;
                    y += e.offsetTop;
                    x -= e.scrollLeft;
                    y -= e.scrollTop;
                    e = e.offsetParent;
                }
                return { x: x, y: y };
            },
            /**
            * Gets the document.documentElement's height and width.
            * @function
            * @type Native.Function
            * @name client
            * @memberOf Oda.UI.Widget
            * @returns {Oda.UI.Rect} Oda.UI.Rect that represents the dimensions of the browser window. The x and y members are always zero.
            * @public
            */
            client: function () {
                return {
                    x: 0,
                    y: 0,
                    w: document.documentElement.clientWidth,
                    h: document.documentElement.clientHeight
                };
            },
            /**
            * Extracts the literal mouse position from the browser's event object.
            * @function
            * @type Native.Function
            * @name mouseLiteral
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.HTMLElement} [e] The browser event object.
            * @returns {Oda.UI.Point} Oda.UI.Point that represents the position of the mouse relative to the browser window.
            * @public
            */
            mouseLiteral: function (e) {
                return { x: window.document.documentElement.scrollLeft + e.clientX, y: window.document.documentElement.scrollTop + e.clientY };
            },
            /**
            * Safely creates a message in the debugging console by checking to see if it exists first then sending the message.
            * @function
            * @type Native.Function
            * @name log
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.HTMLElement} [message] The message to display in the box.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            log: function (message) {
                if (console) {
                    console.log(message);
                }
                return this;
            },
            /**
            * Creates a nifty little floating HTML box to display debugging data if you can't be bothered to use Console.log or this.log.
            * @function
            * @type Native.Function
            * @name debug
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.String} [message] The message to display in the box.
            * @param {Oda.UI.Boolean} [persist] When true the previous message will stay in the box and be moved down.  When false the previous message will be removed.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            debug: function (message, persist) {
                window.__debugMessageBox = window.__debugMessageBox || this.createElement('div');
                var d = window.__debugMessageBox;
                d.style.position = 'absolute';
                d.style.zIndex = '99999';
                d.style.padding = '5px';
                d.style.height = '250px';
                d.style.overflow = 'scroll';
                d.style.top = '25px';
                d.style.color = 'black';
                d.style.background = '#CCC';
                d.style.border = 'solid 1px #555';
                var resize = function() {
                    d.style.left = parseInt(Oda.UI.Widget.client().w) - 300 + 'px';
                };
                if (d.parentNode === null) {
                    document.body.appendChild(d);
                    window.addEventListener('resize', resize, false);
                    resize();
                }
                if (persist) {
                    d.innerHTML = message + d.innerHTML;
                } else {
                    d.innerHTML = message;
                }
                return this;
            },
            /**
            * Used by Oda.UI.Widget implementers to fire event procedures internally.
            * @function
            * @type Native.Function
            * @name raiseEvent
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.String} [eventName] Name of the event to execute.
            * @param {Oda.UI.Object} [e] The event object defined by the browser.
            * @param {Oda.UI.HTMLElement} [element] The HTML Element associated with this event or undefined if there is no element.
            * @param {Oda.UI.Array} [eventArguments] An array list of additional arguments to provide the function subscribed to the event.
            * @returns {Native.Boolean} When true, cancelDefault was set or called.
            * @public
            */
            raiseEvent: function (eventName, e, element, eventArguments) {
                return this.executeEvent(this.events[eventName], e, element, eventArguments);
            },
            /**
            * Adds a function to an event for this widget.  When the event occurs the function will execute in the context of this widget.
            * Setting this.cancelDefault = true; will cancel the default event.
            * @function
            * @type Native.Function
            * @name addEventListener
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.String} [eventName] The name of the event, such as 'click', 'resize' or 'init'.
            * @param {Oda.UI.Function} [procedure] The function to execute when the event occurs.
            * @throws {Exception} If the event name cannot be found.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            addEventListener: function (eventName, procedure) {
                var errMsg = 'Cannot attach to event handler ' + eventName + ' to ' + this.type;
                if (Oda.isArray(this.events[eventName])) {
                    if (this.events[eventName].indexOf(procedure) === -1) {
                        this.events[eventName].push(procedure);
                    }
                } else {
                    throw (errMsg);
                }
                return this;
            },
            /**
            * Removes a function from an event for this widget.  This function must reference the function to remove.
            * @function
            * @type Native.Function
            * @name removeEventListener
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.String} [eventName] The name of the event, such as 'click', 'resize' or 'init'.
            * @param {Oda.UI.Function} [procedure] The function to remove.
            * @throws {Exception} If the event procedure cannot be found.
            * @returns {Native.Object} The widget to which this function belongs.
            * @public
            */
            removeEventListener: function (eventName, procedure) {
                var events = this.events[eventName];
                var errMsg = 'Cannot detach from event handler ' + eventName + ' on ' + this.type + '.  No such event.';
                if (Oda.isArray(this.events[eventName])) {
                    var l = this.events.length;
                    for (var x = 0; l > x; x++) {
                        if (events[x] === procedure) {
                            events.splice(x, 1);
                            return this;
                        }
                    }
                    throw (errMsg);
                } else {
                    throw (errMsg);
                }
            },
            /**
            * Used by Oda.UI.Widget implementers to fire event procedures internally.
            * @function
            * @type Native.Function
            * @name executeEvent
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Array} [eventArray] An array list of functions subscribed to the event listener.
            * @param {Oda.UI.Object} [e] The event object defined by the browser.
            * @param {Oda.UI.HTMLElement} [element] The HTML Element associated with this event or undefined if there is no element.
            * @param {Oda.UI.Array} [eventArguments] An array list of additional arguments to provide the function subscribed to the event.
            * @returns {Native.Boolean} When true, cancelDefault was set or called.
            * @public
            */
            executeEvent: function (eventArray, e, element, eventArguments) {
                this.cancelDefault = false;
                if (eventArguments === undefined) {
                    eventArguments = [undefined, this.interface, undefined];
                } else {
                    eventArguments.unshift(e, this.interface, element);
                }
                var l = eventArray.length;
                for (var x = 0; l > x; x++) {
                    eventArray[x].apply(this.interface, eventArguments);
                }
                return this.cancelDefault;
            },
            /**
            * Used to create event subscriptions from the arguments passed to the 
            * Oda.UI.Widget's constructor arguments.  Returns an array
            * containing the event argument or and empty array.
            * @function
            * @type Native.Function
            * @name addInitalEvents
            * @memberOf Oda.UI.Widget
            * @param {Oda.UI.Function} [eventProcedure] The function passed to the constructor.
            * @returns {Native.Array} Array of event procedures passed for this event.
            * @public
            */
            addInitalEvents: function (eventProcedure) {
                if (typeof eventProcedure === 'function') {
                    return [eventProcedure];
                } else {
                    return [];
                }
            }
        }
    }
}