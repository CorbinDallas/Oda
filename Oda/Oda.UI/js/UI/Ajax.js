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
Oda.Ajax = function (args, options) {
    // map is defined as one of these two patterns
    //
    //    {
    //        method: '',
    //        parameters: [],
    //        procedure: function(thisResult, allResults){ },
    //        context: this
    //    }
    //
    //    or...
    //   
    //    [
    //        {
    //            method: '',
    //            parameters: [],
    //            procedure: function(thisResult, allResults){ },
    //            context: this
    //        },
    //        <additonal request blocks>
    //    ]
    //
    //
    Oda.assert(args !== undefined, 'Oda.Ajax missing parameter.');
    var self = {};
    options = options || { };
    var createRequest = function() {
        try { return new XMLHttpRequest(); } catch(e) {}
        try { return new ActiveXObject('Msxml2.XMLHTTP'); } catch(e) {}
        try { return new ActiveXObject('Microsoft.XMLHTTP'); } catch(e) {}
        return null;
    };
    self.beginRequest = function () {
        self.map = [];
        self.methodInstances = { };
        if(Oda.isArray(args)) {
            // array of methods
            for (var x = 0; args.length > x; x++) {
                // assign this method an instance number in case it occurs more than once
                if (self.methodInstances[args[x].method] === undefined) {
                    self.methodInstances[args[x].method] = 1;
                }else {
                    self.methodInstances[args[x].method]++;
                }
                self.map.push([
                    args[x].method,
                    args[x].parameters
                ]);
                // assgin the instance number back to the mapper object
                args[x].instanceNumber = self.methodInstances[args[x].method];
            }
        } else {
            // a single method
            self.map.push([
                args.method,
                args.parameters
            ]);
        }
        self.id = Oda.createUUID();
        self.request = createRequest();
        self.request.onreadystatechange = self.readyStateChange;
        self.request.open(options.method || 'POST', (options.responderUrl || '/responder?id=') + self.id, options.async || true);
        self.request.setRequestHeader('Content-Type', options.contentType || 'application/json; charset=utf-8');
        self.request.send(JSON.stringify(self.map));
    };
    self.readyStateChange = function() {
        if (self.request.readyState != 4 || self.request.status == 0) { return; }
        if (self.request.status != 200) {
            // an error occured
            Oda.UI.Widget.log('Oda.Ajax request returned a status of ' + self.request.status + ':\n' + self.request.responseText);
            return;
        }
        var allResults = JSON.parse(self.request.responseText);
        // seperate responses into map collection and call callback functions
        if (Oda.isArray(args)) {
            // array of methods
            for (var x = 0; args.length > x; x++) {
                var instanceNumber = self.methodInstances[args[x].method] > 1 ? '_' + args[x].instanceNumber : '';
                args[x].procedure.apply(args[x].context, [allResults[args[x].method + instanceNumber], allResults]);
            }
        } else {
            // a single method
            args.procedure.apply(args.context, [allResults[args.method], allResults]);
        }
    };
    self.beginRequest();
    return self;
}