//Modules act as naming containers that encapsulate other things like
//'classes, 'interfaces', etc.
var test;
(function (test) {
    var MyTest = (function () {
        function MyTest() {
            this.name = "hello";
        }
        return MyTest;
    })();
    test.MyTest = MyTest;

    var PrivateTest = (function () {
        function PrivateTest() {
            this.name = "hi";
        }
        return PrivateTest;
    })();
    test.PrivateTest = PrivateTest;
})(test || (test = {}));

var MyTest = (function () {
    function MyTest() {
        this.name = "hello";
    }
    return MyTest;
})();

var a = new test.PrivateTest();
var b = new MyTest();
//# sourceMappingURL=simpleTS.js.map
