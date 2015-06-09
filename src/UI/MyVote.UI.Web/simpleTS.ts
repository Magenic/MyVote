//Modules act as naming containers that encapsulate other things like 
//'classes, 'interfaces', etc.
module test
{
    export class MyTest {
        name = "hello";
        private ab = new PrivateTest();
        
        public myPublicMethod() : string {
            return this.ab.myMethod();
        }

    }

    class PrivateTest {
        //Exposing private value on private method only accessible within the scope of this module
        public myMethod() {
            return this.name1;
        }

        private name1 = "hi";
    }
}

interface ISimple {
    name: string;
}

class MyTest implements ISimple {
    name = "hello";
}

//var a = new test.PrivateTest(); //Not accessible - it's not exported on 'test'
var b = new MyTest();


