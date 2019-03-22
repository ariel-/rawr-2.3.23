// Name:        SilverlightControl.debug.js
// Assembly:    System.Web.Silverlight
// Version:     3.0.0.0
// FileVersion: 3.0.40210.0
//-----------------------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------
// SilverlightControl.js
Type.registerNamespace('Sys.UI.Silverlight');

Sys.UI.Silverlight.Control = function Sys$UI$Silverlight$Control(domElement) {
    /// <summary locid="M:J#Sys.UI.Silverlight.Control.#ctor" />
    /// <param name="domElement" domElement="true"></param>
    var e = Function._validateParams(arguments, [
        {name: "domElement", domElement: true}
    ]);
    if (e) throw e;
                            Sys.UI.Silverlight.Control.initializeBase(this, [this._findObject(domElement) || domElement]);
    this._scaleMode = Sys.UI.Silverlight.ScaleMode.none;
}











    function Sys$UI$Silverlight$Control$add_pluginError(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginError" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginError", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginError(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginError", handler);
    }

    function Sys$UI$Silverlight$Control$add_pluginSourceDownloadComplete(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginSourceDownloadComplete" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginSourceDownloadComplete", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginSourceDownloadComplete(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginSourceDownloadComplete", handler);
    }

    function Sys$UI$Silverlight$Control$add_pluginSourceDownloadProgressChanged(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginSourceDownloadProgressChanged" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginSourceDownloadProgressChanged", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginSourceDownloadProgressChanged(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginSourceDownloadProgressChanged", handler);
    }

    function Sys$UI$Silverlight$Control$add_pluginFullScreenChanged(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginFullScreenChanged" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginFullScreenChanged", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginFullScreenChanged(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginFullScreenChanged", handler);
    }

    function Sys$UI$Silverlight$Control$add_pluginLoaded(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginLoaded" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginLoaded", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginLoaded(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginLoaded", handler);
    }

    function Sys$UI$Silverlight$Control$add_pluginResized(handler) {
    /// <summary locid="E:J#Sys.UI.Silverlight.Control.pluginResized" />
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().addHandler("pluginResized", handler);
    }
    function Sys$UI$Silverlight$Control$remove_pluginResized(handler) {
    var e = Function._validateParams(arguments, [{name: "handler", type: Function}]);
    if (e) throw e;
        this.get_events().removeHandler("pluginResized", handler);
    }

    function Sys$UI$Silverlight$Control$get_scaleMode() {
        /// <value type="Sys.UI.Silverlight.ScaleMode" locid="P:J#Sys.UI.Silverlight.Control.scaleMode"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._scaleMode;
    }
    function Sys$UI$Silverlight$Control$set_scaleMode(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: Sys.UI.Silverlight.ScaleMode}]);
        if (e) throw e;
        if (value !== this.get_scaleMode()) {
            this._scaleMode = value;
            if (this._loaded) {
                this._ensureTransform();
            }
        }
    }

    function Sys$UI$Silverlight$Control$get_source() {
        /// <value type="String" locid="P:J#Sys.UI.Silverlight.Control.source"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._source || "";
    }
    function Sys$UI$Silverlight$Control$set_source(value) {
        var e = Function._validateParams(arguments, [{name: "value", type: String}]);
        if (e) throw e;
        if (this._lockSource) {
            throw Error.invalidOperation(Sys.UI.Silverlight.ControlRes.cannotChangeSource);
        }
        this._source = value;
        if (value && this._setOnLoad) {
            this._lockSource = true;
            this.get_element().Source = value;
        }
    }

    function Sys$UI$Silverlight$Control$get_splashScreenSource() {
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._splashScreenSource || "";
    }
    function Sys$UI$Silverlight$Control$set_splashScreenSource(value) {
        if (this._lockSource) {
            throw Error.invalidOperation(Sys.UI.Silverlight.ControlRes.cannotChangeSplash);
        }
        this._splashScreenSource = value;
        if (this._setOnLoad) {
            this.get_element().SplashScreenSource = value;
        }
    }

    function Sys$UI$Silverlight$Control$addEventListener(element, eventName, handler) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.addEventListener" />
        /// <param name="element"></param>
        /// <param name="eventName" type="String"></param>
        /// <param name="handler" type="Function"></param>
        /// <returns type="Number"></returns>
        var e = Function._validateParams(arguments, [
            {name: "element"},
            {name: "eventName", type: String},
            {name: "handler", type: Function}
        ]);
        if (e) throw e;
        var token = element.addEventListener(eventName, handler);
        var events = this._getEventsForElement(element, true);
        events[events.length] = { eventName: eventName, token: token };
        return token;
    }

    function Sys$UI$Silverlight$Control$dispose() {
        if (this._loaded) {
            this.pluginDispose();
            this._loaded = false;
        }

        var host = this.get_element();
        if (host) {
            if (this._setOnLoad) {
                host.OnLoad = null;
                host.OnSourceDownloadProgressChanged = null;
                host.OnSourceDownloadComplete = null;
            }
            if (this._setOnError) {
                host.OnError = null;
            }
            if (this._setOnFullScreenChange) {
                host.content.OnFullScreenChange = null;
            }
            if (this._setOnResize) {
                host.content.OnResize = null;
            }
        }

        var boundEvents = this._boundEvents;
        if (boundEvents) {
            for (var i = 0, l = boundEvents.length; i < l; i++) {
                var e = boundEvents[i];
                var element = e.element;
                var events = e.events;
                for (var j = 0, m = events.length; j < m; j++) {
                    var event = events[j];
                    element.removeEventListener(event.eventName, event.token);
                }
            }
            this._boundEvents = null;
        }

        this._doCheck = null;
        Sys.UI.Silverlight.Control.callBaseMethod(this, "dispose");
    }

    function Sys$UI$Silverlight$Control$_ensureTransform() {
                        var root = this.get_element().content.root;
                var scale = Sys.UI.Silverlight.Control._computeScale(root, this.get_scaleMode());
        Sys.UI.Silverlight.Control._applyMatrix(root, scale.horizontal, scale.vertical, 0, 0);
    }

    function Sys$UI$Silverlight$Control$_findObject(element) {
        if (this._isSilverlight(element)) return element;
        var i, l;
        var object;
        var objects = element.getElementsByTagName("object");
        for (i = 0, l = objects.length; i < l; i++) {
            object = objects[i];
            if (this._isSilverlight(object)) return object;
        }
        
        objects = element.getElementsByTagName("embed");
        for (i = 0, l = objects.length; i < l; i++) {
            object = objects[i];
            if (this._isSilverlight(object)) return object;
        }
                return null;
    }

    function Sys$UI$Silverlight$Control$_getEventsForElement(element, create) {
        var e;
        var boundEvents = this._boundEvents;
        if (!boundEvents) {
            this._boundEvents = boundEvents = [];
        }
        for (var i = 0, l = boundEvents.length; i < l; i++) {
            e = boundEvents[i];
            if (e.element === element) {
                return e.events;
            }
        }
        if (!create) return null;
        e = { element: element, events: [] };
        boundEvents[boundEvents.length] = e;
        return e.events;
    }

    function Sys$UI$Silverlight$Control$initialize() {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.initialize" />
        if (arguments.length !== 0) throw Error.parameterCount();
        Sys.UI.Silverlight.Control.callBaseMethod(this, "initialize");

        var host = this.get_element();
        
        if (this._isSilverlight(host)) {
            if (!host.OnError) {
                host.OnError = Function.createDelegate(this, this._pluginError);
                this._setOnError = true;
            }
            
            if (host.Source && this.get_source()) {
                                                throw Error.invalidOperation(Sys.UI.Silverlight.ControlRes.sourceAlreadySet);
            }
            else if (host.SplashScreenSource && this.get_splashScreenSource()) {
                                throw Error.invalidOperation(Sys.UI.Silverlight.ControlRes.splashAlreadySet);
            }
            if (host.IsLoaded) {
                                this._lockSource = true;
                this._pluginLoaded();
            }
            else {
                                                host.OnLoad = Function.createDelegate(this, this._pluginLoaded);
                host.OnSourceDownloadProgressChanged = Function.createDelegate(this, this._pluginSourceProgress);
                host.OnSourceDownloadComplete = Function.createDelegate(this, this._pluginSourceComplete);
                this._setOnLoad = true;
                
                if (!host.Source) {
                                        
                    if (!host.SplashScreenSource) {
                                                var splash = this.get_splashScreenSource();
                        if (splash) {
                            host.SplashScreenSource = splash;
                        }
                    }
                    
                    var source = this.get_source();
                    if (source) {
                        this._lockSource = true;
                        host.Source = source;
                        if ((Sys.Browser.agent === Sys.Browser.Firefox) && (Sys.Browser.version >= 3) && (Sys.Browser.version < 3.1)) {
                            if (!this._doCheck) {
                                                                                                                                                                this._ensureSet();
                            }
                            else {
                                this._doCheck = null;
                            }
                        }
                    }
                                                        }
                else {
                                                                                this._lockSource = true;
                }
            }
        }
    }
    function Sys$UI$Silverlight$Control$_ensureSet() {
                        this._doCheck = Function.createDelegate(this, this._checkSet);
        window.setTimeout(this._doCheck, 50);
    }
    function Sys$UI$Silverlight$Control$_checkSet() {
        if (!this._doCheck) return;
        if (!this.get_element().Source) {
            this.initialize();
            return;
        }
        window.setTimeout(this._doCheck, 200);
    }
    function Sys$UI$Silverlight$Control$_isSilverlight(element) {
        if (!element) return false;
        
                var tagName = element.tagName.toLowerCase();
        if (tagName === "object" || tagName === "embed") {
                        var type = (element.type ? element.type.toLowerCase() : "");
            if ((type.indexOf("application/x-silverlight") === 0) ||
                (type.indexOf("application/silverlight") === 0)) {
                                                
                                                element.innerHTML;
                
                return !!(element.settings);
            }
        }
        return false;
    }

    function Sys$UI$Silverlight$Control$_onFullScreen() {
                if (!this.get_element()) return;
        this.onPluginFullScreenChanged(Sys.EventArgs.Empty);
        this._raiseEvent("pluginFullScreenChanged");
    }

    function Sys$UI$Silverlight$Control$onPluginError(errorEventArgs) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginError" />
        /// <param name="errorEventArgs" type="Sys.UI.Silverlight.ErrorEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "errorEventArgs", type: Sys.UI.Silverlight.ErrorEventArgs}
        ]);
        if (e) throw e;
    }

    function Sys$UI$Silverlight$Control$onPluginFullScreenChanged(args) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginFullScreenChanged" />
        /// <param name="args" type="Sys.EventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.EventArgs}
        ]);
        if (e) throw e;
    }

    function Sys$UI$Silverlight$Control$onPluginLoaded(args) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginLoaded" />
        /// <param name="args" type="Sys.EventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.EventArgs}
        ]);
        if (e) throw e;
    }

    function Sys$UI$Silverlight$Control$onPluginResized(args) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginResized" />
        /// <param name="args" type="Sys.EventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.EventArgs}
        ]);
        if (e) throw e;
        if (this.get_scaleMode() !== Sys.UI.Silverlight.ScaleMode.none) {
            this._ensureTransform();
        }
    }

    function Sys$UI$Silverlight$Control$onPluginSourceDownloadComplete(args) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginSourceDownloadComplete" />
        /// <param name="args" type="Sys.EventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.EventArgs}
        ]);
        if (e) throw e;
    }

    function Sys$UI$Silverlight$Control$onPluginSourceDownloadProgressChanged(args) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.onPluginSourceDownloadProgressChanged" />
        /// <param name="args" type="Sys.UI.Silverlight.DownloadProgressEventArgs"></param>
        var e = Function._validateParams(arguments, [
            {name: "args", type: Sys.UI.Silverlight.DownloadProgressEventArgs}
        ]);
        if (e) throw e;
    }

    function Sys$UI$Silverlight$Control$_onResize() {
                if (!this.get_element()) return;
        this.onPluginResized(Sys.EventArgs.Empty);
        this._raiseEvent("pluginResized");
    }

    function Sys$UI$Silverlight$Control$pluginDispose() {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.pluginDispose" />
        if (arguments.length !== 0) throw Error.parameterCount();
    }

    function Sys$UI$Silverlight$Control$_pluginError(slSender, e) {
                if (!this.get_element()) return;
        var args = new Sys.UI.Silverlight.ErrorEventArgs(e);
        this.onPluginError(args);
        this._doCheck = null;
        
        if (!args.get_cancel()) {
                                                if (!this._raiseEvent("pluginError", args)) {
                var errorType = e.errorType,
                    errorCode = e.errorCode,
                    errorMessage = e.errorMessage,
                    id = this.get_id(),
                    lineNumber = "", charPosition = "", source = "", methodName = "",
                    errorFormat = Sys.UI.Silverlight.ControlRes.otherError;
                    
                                if (errorType === "ParserError") {
                    errorFormat = Sys.UI.Silverlight.ControlRes.parserError;
                    lineNumber = e.lineNumber;
                    charPosition = e.charPosition;
                    source = e.xamlFile;
                }
                else if (((errorType === "ImageError") || (errorType === "MediaError")) &&
                        (errorMessage === "AG_E_NOT_FOUND")) {
                        errorFormat = Sys.UI.Silverlight.ControlRes.mediaError_NotFound;
                        errorMessage = slSender.Source;
                }
                else if (errorType === "RuntimeError") {
                    if (e.lineNumber) {
                        errorFormat = Sys.UI.Silverlight.ControlRes.runtimeErrorWithPosition;
                        lineNumber = e.lineNumber;
                        charPosition = e.charPosition;
                        methodName = e.methodName;
                    }
                    else {
                        errorFormat = Sys.UI.Silverlight.ControlRes.runtimeErrorWithoutPosition;
                        methodName = e.methodName;
                    }
                }
                throw Error.invalidOperation(String.format(errorFormat,
                        id,
                        errorType,
                        errorCode,
                        errorMessage,
                        lineNumber,
                        charPosition,
                        methodName,
                        source));
            }
        }
    }

    function Sys$UI$Silverlight$Control$_pluginLoaded() {
                var element = this.get_element();
        if (element) {
                                                
                        if (!element.content.OnFullScreenChange) {
                element.content.OnFullScreenChange = Function.createDelegate(this, this._onFullScreen);
                this._setOnFullScreenChange = true;
            }
            if (!element.content.OnResize) {
                element.content.OnResize = Function.createDelegate(this, this._onResize);
                this._setOnResize = true;
            }
        
            this._doCheck = null;        
            this._raisepluginLoaded();
                                    this._onResize();
        }
    }

    function Sys$UI$Silverlight$Control$_pluginSourceProgress(sender, args) {
        var progressArgs = new Sys.UI.Silverlight.DownloadProgressEventArgs(args.progress);
        this.onPluginSourceDownloadProgressChanged(progressArgs);
        this._raiseEvent("pluginSourceDownloadProgressChanged", progressArgs);
    }

    function Sys$UI$Silverlight$Control$_pluginSourceComplete() {
        this.onPluginSourceDownloadComplete(Sys.EventArgs.Empty);
        this._raiseEvent("pluginSourceDownloadComplete");
    }

    function Sys$UI$Silverlight$Control$_raiseEvent(name, args) {
                        var handler = this.get_events().getHandler(name);
        if (handler) {
            handler(this, args || Sys.EventArgs.Empty);
            return true;
        }
        return false;
    }

    function Sys$UI$Silverlight$Control$_raisepluginLoaded() {
        this._loaded = true;
        this.onPluginLoaded(Sys.EventArgs.Empty);
        this._raiseEvent("pluginLoaded");
    }

    function Sys$UI$Silverlight$Control$removeEventListener(element, eventName, token) {
        /// <summary locid="M:J#Sys.UI.Silverlight.Control.removeEventListener" />
        /// <param name="element"></param>
        /// <param name="eventName" type="String"></param>
        /// <param name="token" type="Number"></param>
        var e = Function._validateParams(arguments, [
            {name: "element"},
            {name: "eventName", type: String},
            {name: "token", type: Number}
        ]);
        if (e) throw e;
        element.removeEventListener(eventName, token);
        var events = this._getEventsForElement(element, false);
        if (!events) return;
        
        for (var i = 0, l = events.length; i < l; i++) {
            var e = events[i];
            if ((e.token === token) && (e.eventName === eventName)) {
                Array.removeAt(events, i);
                return;
            }
        }
    }
Sys.UI.Silverlight.Control.prototype = {
    _source: null,
    _splashScreenSource: null,
    _loaded: false,
    _boundEvents: null,
    _setOnLoad: false,
    _setOnFullScreenChange: false,
    _setOnResize: false,
    _setOnError: false,
    _lockSource: false,
    add_pluginError: Sys$UI$Silverlight$Control$add_pluginError,
    remove_pluginError: Sys$UI$Silverlight$Control$remove_pluginError,
    add_pluginSourceDownloadComplete: Sys$UI$Silverlight$Control$add_pluginSourceDownloadComplete,
    remove_pluginSourceDownloadComplete: Sys$UI$Silverlight$Control$remove_pluginSourceDownloadComplete,
    add_pluginSourceDownloadProgressChanged: Sys$UI$Silverlight$Control$add_pluginSourceDownloadProgressChanged,
    remove_pluginSourceDownloadProgressChanged: Sys$UI$Silverlight$Control$remove_pluginSourceDownloadProgressChanged,
    add_pluginFullScreenChanged: Sys$UI$Silverlight$Control$add_pluginFullScreenChanged,
    remove_pluginFullScreenChanged: Sys$UI$Silverlight$Control$remove_pluginFullScreenChanged,
    add_pluginLoaded: Sys$UI$Silverlight$Control$add_pluginLoaded,
    remove_pluginLoaded: Sys$UI$Silverlight$Control$remove_pluginLoaded,
    add_pluginResized: Sys$UI$Silverlight$Control$add_pluginResized,
    remove_pluginResized: Sys$UI$Silverlight$Control$remove_pluginResized,
    get_scaleMode: Sys$UI$Silverlight$Control$get_scaleMode,
    set_scaleMode: Sys$UI$Silverlight$Control$set_scaleMode,
    get_source: Sys$UI$Silverlight$Control$get_source,
    set_source: Sys$UI$Silverlight$Control$set_source,
    get_splashScreenSource: Sys$UI$Silverlight$Control$get_splashScreenSource,
    set_splashScreenSource: Sys$UI$Silverlight$Control$set_splashScreenSource,
    addEventListener: Sys$UI$Silverlight$Control$addEventListener,
    dispose: Sys$UI$Silverlight$Control$dispose,
    _ensureTransform: Sys$UI$Silverlight$Control$_ensureTransform,
    _findObject: Sys$UI$Silverlight$Control$_findObject,
    _getEventsForElement: Sys$UI$Silverlight$Control$_getEventsForElement,    
    initialize: Sys$UI$Silverlight$Control$initialize,
    _ensureSet: Sys$UI$Silverlight$Control$_ensureSet,
    _checkSet: Sys$UI$Silverlight$Control$_checkSet,        
    _isSilverlight: Sys$UI$Silverlight$Control$_isSilverlight,    
    _onFullScreen: Sys$UI$Silverlight$Control$_onFullScreen,
    onPluginError: Sys$UI$Silverlight$Control$onPluginError,
    onPluginFullScreenChanged: Sys$UI$Silverlight$Control$onPluginFullScreenChanged,
    onPluginLoaded: Sys$UI$Silverlight$Control$onPluginLoaded,
    onPluginResized: Sys$UI$Silverlight$Control$onPluginResized,
    onPluginSourceDownloadComplete: Sys$UI$Silverlight$Control$onPluginSourceDownloadComplete,
    onPluginSourceDownloadProgressChanged: Sys$UI$Silverlight$Control$onPluginSourceDownloadProgressChanged,
    _onResize: Sys$UI$Silverlight$Control$_onResize,
    pluginDispose: Sys$UI$Silverlight$Control$pluginDispose,
    _pluginError: Sys$UI$Silverlight$Control$_pluginError,
    _pluginLoaded: Sys$UI$Silverlight$Control$_pluginLoaded,
    _pluginSourceProgress: Sys$UI$Silverlight$Control$_pluginSourceProgress,
    _pluginSourceComplete: Sys$UI$Silverlight$Control$_pluginSourceComplete,
    _raiseEvent: Sys$UI$Silverlight$Control$_raiseEvent,
    _raisepluginLoaded: Sys$UI$Silverlight$Control$_raisepluginLoaded,
    removeEventListener: Sys$UI$Silverlight$Control$removeEventListener    
}

Sys.UI.Silverlight.Control._computeScale = function Sys$UI$Silverlight$Control$_computeScale(element, scaleMode) {
            if (scaleMode === Sys.UI.Silverlight.ScaleMode.none) {
                return { horizontal: 1, vertical: 1 };
    }
    
    var width = element.width, height = element.height,
        host = element.getHost(),
        scale = { horizontal: width ? (host.content.ActualWidth / width) : 0,
             vertical: height ? (host.content.ActualHeight / height) : 0 };
    if (scaleMode === Sys.UI.Silverlight.ScaleMode.zoom) {
        scale.horizontal = scale.vertical = Math.min(scale.horizontal, scale.vertical);
    }
    return scale;    
}

Sys.UI.Silverlight.Control._applyMatrix = function Sys$UI$Silverlight$Control$_applyMatrix(e, scaleX, scaleY, offsetX, offsetY) {
    var transform = e.RenderTransform;
    if (!transform) {
        e.RenderTransform = transform =
            e.getHost().content.createFromXaml('<MatrixTransform Matrix="1,0 0,1 0,0"/>');
    }
    else if (transform.toString() !== "MatrixTransform") {
        throw Error.invalidOperation(Sys.UI.Silverlight.ControlRes.scaleModeRequiresMatrixTransform);
    }
    var original = { horizontal: transform.Matrix.M11, vertical: transform.Matrix.M22 };
    transform.Matrix.M11 = scaleX;
    transform.Matrix.M22 = scaleY;
    transform.Matrix.OffsetX = offsetX;
    transform.Matrix.OffsetY = offsetY;
    return original;
}

Sys.UI.Silverlight.Control.createObject = function Sys$UI$Silverlight$Control$createObject(parentId, html) {
    /// <summary locid="M:J#Sys.UI.Silverlight.Control.createObject" />
    /// <param name="parentId" type="String"></param>
    /// <param name="html" type="String"></param>
    var e = Function._validateParams(arguments, [
        {name: "parentId", type: String},
        {name: "html", type: String}
    ]);
    if (e) throw e;
            var parent = document.getElementById(parentId);
    if (!parent) {
        throw Error.invalidOperation(String.format(Sys.UI.Silverlight.ControlRes.parentNotFound, parentId));
    }
    parent.innerHTML = html;
}

Sys.UI.Silverlight.Control.registerClass("Sys.UI.Silverlight.Control", Sys.UI.Control);
Sys.UI.Silverlight.ErrorEventArgs = function Sys$UI$Silverlight$ErrorEventArgs(error) {
    /// <summary locid="M:J#Sys.UI.Silverlight.ErrorEventArgs.#ctor" />
    /// <param name="error"></param>
    var e = Function._validateParams(arguments, [
        {name: "error"}
    ]);
    if (e) throw e;
    this._error = error;
    Sys.UI.Silverlight.ErrorEventArgs.initializeBase(this);
}

    function Sys$UI$Silverlight$ErrorEventArgs$get_error() {
        /// <value locid="P:J#Sys.UI.Silverlight.ErrorEventArgs.error"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._error;
    }
Sys.UI.Silverlight.ErrorEventArgs.prototype = {
    get_error: Sys$UI$Silverlight$ErrorEventArgs$get_error
}
Sys.UI.Silverlight.ErrorEventArgs.registerClass("Sys.UI.Silverlight.ErrorEventArgs", Sys.CancelEventArgs);
Sys.UI.Silverlight.DownloadProgressEventArgs = function Sys$UI$Silverlight$DownloadProgressEventArgs(progress) {
    /// <summary locid="M:J#Sys.UI.Silverlight.DownloadProgressEventArgs.#ctor" />
    /// <param name="progress" type="Number"></param>
    var e = Function._validateParams(arguments, [
        {name: "progress", type: Number}
    ]);
    if (e) throw e;
    this._progress = progress;
    Sys.UI.Silverlight.DownloadProgressEventArgs.initializeBase(this);
}

    function Sys$UI$Silverlight$DownloadProgressEventArgs$get_progress() {
        /// <value type="Number" locid="P:J#Sys.UI.Silverlight.DownloadProgressEventArgs.progress"></value>
        if (arguments.length !== 0) throw Error.parameterCount();
        return this._progress;
    }
Sys.UI.Silverlight.DownloadProgressEventArgs.prototype = {
    get_progress: Sys$UI$Silverlight$DownloadProgressEventArgs$get_progress
}
Sys.UI.Silverlight.DownloadProgressEventArgs.registerClass("Sys.UI.Silverlight.DownloadProgressEventArgs", Sys.EventArgs);
Sys.UI.Silverlight.ScaleMode = function Sys$UI$Silverlight$ScaleMode() {
    /// <summary locid="M:J#Sys.UI.Silverlight.ScaleMode.#ctor" />
    /// <field name="none" type="Number" integer="true" static="true" locid="F:J#Sys.UI.Silverlight.ScaleMode.none"></field>
    /// <field name="zoom" type="Number" integer="true" static="true" locid="F:J#Sys.UI.Silverlight.ScaleMode.zoom"></field>
    /// <field name="stretch" type="Number" integer="true" static="true" locid="F:J#Sys.UI.Silverlight.ScaleMode.stretch"></field>
    if (arguments.length !== 0) throw Error.parameterCount();
    throw Error.notImplemented();
}




Sys.UI.Silverlight.ScaleMode.prototype = {
    none: 0,
    zoom: 1,
    stretch: 2
}
Sys.UI.Silverlight.ScaleMode.registerEnum('Sys.UI.Silverlight.ScaleMode');



Type.registerNamespace('Sys.UI.Silverlight');
Sys.UI.Silverlight.ControlRes={
"scaleModeRequiresMatrixTransform":"When ScaleMode is set to zoom or stretch, only a MatrixTransform is permitted on the root Canvas.",
"cannotChangeSource":"The source cannot be changed after initialization.",
"cannotChangeSplash":"The splashScreenSource cannot be changed after initialization.",
"runtimeErrorWithoutPosition":"Runtime error {2} in control \u0027{0}\u0027, method {6}: {3}",
"mediaError_NotFound":"Media \"{3}\" in control \"{0}\" could not be found.",
"runtimeErrorWithPosition":"Runtime error {2} in control \u0027{0}\u0027, method {6} (line {4}, col {5}): {3}",
"sourceAlreadySet":"The source cannot be set because the Silverlight host already has a source.",
"otherError":"{1} error #{2} in control \u0027{0}\u0027: {3}",
"splashAlreadySet":"The splashScreenSource cannot be set because the Silverlight host already has a splashScreenSource.",
"parentNotFound":"An element with id \"{0}\" could not be found.",
"parserError":"Invalid XAML for control \u0027{0}\u0027. [{7}] (line {4}, col {5}): {3}"
};

if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();