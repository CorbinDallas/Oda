/*
 * Copyright (c) 2012 Tony Germaneri
 * Permission is hereby granted,  free of charge, to any person 
 * obtaining a copy of this software and associated documentation files 
 * (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARSING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
*/
/**
* Style for the Oda.UI.TaskBar
* @constructor
* @name TaskBar
* @version 0.1.0 beta release
* @author Tony Germaneri (TonyGermaneri@gmail.com)
* @augments Oda.UI.WidgetStyle
* @requires Oda
* @requires Oda.UI
* @memberOf Oda.UI.Style
* @param {Native.String} [args.background] The background of the taskbar.
* @param {Native.String} [args.item.background] The background of an item.
* @param {Native.String} [args.item.hover.background] The background of an item when hovered over.
* @param {Native.String} [args.item.hover.textColor] The text color of an item when hovered over.
* @param {Native.String} [args.item.textColor] The text color of an item.
* @param {Oda.UI.Border} [args.item.border] The border of an item.
* @param {Oda.UI.String} [args.item.font] The font of an item.
* @param {Oda.UI.Rect} [args.rect] The rect of the task bar.
* @param {Oda.UI.Border} [args.border] The top border of the task bar.
* @param {Oda.UI.Rect} [args.item.rect] The spaceing of the items.
* @param {Oda.UI.Rect} [args.item.spacing] The spaceing of the items.
* @param {Oda.UI.Padding} [args.item.padding] The spaceing of the items.
* @param {Native.Object} [args] Parameters for the style.
*/
Oda.UI.Style.TaskBar = function(args){
    var self = Oda.beget(Oda.UI.WidgetStyle);
    // make sure objects exist
    args = args || { item: {} };
    args.item.hover = args.item.hover || { };
    // create an id for this widget
    /**
    * The background of the taskbar.
    * @field
    * @name id
    * @type Native.Integer
    * @memberOf Oda.UI.Style.TaskBar
    */
    self.id = Oda.UI.widgetIdCount++;
    self.item = { hover: {} };
    /**
    * The background of the taskbar.
    * @field
    * @name type
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar
    */
    self.type = "TaskBarStyle";
    /**
    * The background of the taskbar.
    * @field
    * @name background
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar
    */
    self.background = args.background || '#DDD';
    /**
    * The background of an item.
    * @field
    * @name background
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.background = args.item.background || '#EFEFEF';
    /**
    * The background of an item when hovered over.
    * @field
    * @name background
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar.item.hover
    */
    self.item.hover.background = args.item.hover.background || '#dddddd';
    /**
    * The text color of an item when hovered over.
    * @field
    * @name textColor
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar.item.hover
    */
    self.item.hover.textColor = args.item.hover.textColor || '#ffffff';
    /**
    * The text color of an item.
    * @field
    * @name textColor
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.textColor = args.item.textColor || '#000';
    /**
    * The border of an item.
    * @field
    * @name border
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.border = args.item.border || { size: 1, style: 'solid', color: '#FFF' };
    /**
    * The font of an item.
    * @field
    * @name font
    * @type Native.String
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.font = args.item.font || self.defaultFontFamily;
    /**
    * The rect of the task bar.
    * @field
    * @name rect
    * @type Native.Rect
    * @memberOf Oda.UI.Style.TaskBar
    */
    self.rect = args.rect || { h: 29, w: 0, x: 0, y: 0 };
    /**
    * The top border of the task bar.
    * @field
    * @name border
    * @type Oda.UI.Border
    * @memberOf Oda.UI.Style.TaskBar
    */
    self.border = args.border || { size: 1, style: 'solid', color: '#00568f' };
    /**
    * The spaceing of the items.
    * @field
    * @name spacing
    * @type Native.Integer
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.spacing = args.item.spacing || 2;
    /**
    * The spaceing of the items.
    * @field
    * @name rect
    * @type Oda.UI.Rect
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.rect = args.item.rect || { h: 27, w: 155, x: 0, y: 0 };
    /**
    * The spaceing of the items.
    * @field
    * @name padding
    * @type Oda.UI.Padding
    * @memberOf Oda.UI.Style.TaskBar.item
    */
    self.item.padding = args.item.padding || { t: 2, r: 4, b: 2, l: 2 };
    return self;
};