/// <reference path="../_refs.ts" />
var MyVote;
(function (MyVote) {
    (function (Models) {
        'use strict';

        var AuthProvider = (function () {
            function AuthProvider(id, name, image, color) {
                this.id = id;
                this.name = name;
                this.image = image;
                this.color = color;
            }
            return AuthProvider;
        })();
        Models.AuthProvider = AuthProvider;
    })(MyVote.Models || (MyVote.Models = {}));
    var Models = MyVote.Models;
})(MyVote || (MyVote = {}));
//# sourceMappingURL=authProvider.js.map
