(function() {
    window.angular = {
        ieCreateHttpBackend: function ($browser, XHR, $browserDefer, callbacks, rawDocument, locationProtocol, msie, xhr) {
            if (!msie || msie > 9) return null;

            var getHostName = function(path) {
                var a = document.createElement('a');
                a.href = path;
                return a.hostname;
            };

            var isLocalCall = function(reqUrl) {
                var reqHost = getHostName(reqUrl);
                if (reqHost === "")
                    return true;

                var localHost = getHostName($browser.url());

                patt = new RegExp(localHost + "$", 'i');
                return patt.test(reqHost);
            };

            function completeRequest(callback, status, response, headersString) {
                var url = url || $browser.url(),
                  URL_MATCH = /^([^:]+):\/\/(\w+:{0,1}\w*@)?(\{?[\w\.-]*\}?)(:([0-9]+))?(\/[^\?#]*)?(\?([^#]*))?(#(.*))?$/;


                // URL_MATCH is defined in src/service/location.js
                var protocol = (url.match(URL_MATCH) || ['', locationProtocol])[1];

                // fix status code for file protocol (it's always 0)
                status = (protocol == 'file') ? (response ? 200 : 404) : status;

                // normalize IE bug (http://bugs.jquery.com/ticket/1450)
                status = status == 1223 ? 204 : status;

                callback(status, response, headersString);
                $browser.$$completeOutstandingRequest(angular.noop);
            }

            var pmHandler = function(method, url, post, callback, headers, timeout, withCredentials) {
                var win = document.getElementsByName(getHostName(url))[0].id;
                console.log('ie postMessage for url : ' + url);
                console.log('iframe window ' + win);
                pm({
                    target: window.frames[win],
                    type: 'xhrRequest',
                    data: {
                        headers: headers,
                        method: method,
                        data: post,
                        url: url
                    },
                    success: function(respObj) {
                        completeRequest(callback, 200, respObj.responseText, 'Content-Type: ' + respObj.contentType);
                    },
                    error: function(data) {
                        completeRequest(callback, 500, 'Error', 'Content-Type: text/plain');
                    }
                });
            };
            return function(method, url, post, callback, headers, timeout, withCredentials) {
                $browser.$$incOutstandingRequestCount();
                url = url || $browser.url();

                if (isLocalCall(url)) {
                    xhr(method, url, post, callback, headers, timeout, withCredentials);
                } else {
                    pmHandler(method, url, post, callback, headers, timeout, withCredentials);
                }
                if (timeout > 0) {
                    $browserDefer(function() {
                        status = -1;
                        xdr.abort();
                    }, timeout);
                }
            };

        }
    };
})();