using System;

namespace learn
{
    public class Human
    {
        
        public string firstname;
        private int age;

        public Human(string name)
        {
            firstname = name;
        }

        public void IntroduceMyself()
        {
            Console.WriteLine("Hi, I am "+firstname+" and I am "+age);
        }
                

        public int Age
        {
            get => age;
            set => age = value;
        }
    }
}