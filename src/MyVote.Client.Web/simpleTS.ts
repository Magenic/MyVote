//Modules act as naming containers that encapsulate other things like 
//'classes, 'interfaces', etc.
module test
{
    export class MyTest {
        name = "hello";
    }

    export class PrivateTest {
        name = "hi";
    }
}

interface ISimple {
    name: string;
}

class MyTest implements ISimple {
    name = "hello";
}

var a = new test.PrivateTest();
var b = new MyTest();


