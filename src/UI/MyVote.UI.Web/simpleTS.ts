//Modules act as naming containers that encapsulate other things like 
//'classes, 'interfaces', etc.
module Test
{
    export class MyTest {
        name = "hello";
        private ab = new PrivateTest();
        
        public myPublicMethod(): string {
            //Use instance of non exported TS class, but still within scope of this module
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

class MyTestWithInterface implements ISimple {
    name = "hello";
}

//var a = new Test.PrivateTest(); //Not accessible - it's not exported on 'test'
var b = new Test.MyTest();


