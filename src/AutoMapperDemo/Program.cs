using System;
using System.Linq.Expressions;
using AutoMapperDemo.Demos;
using AutoMapperDemo.Demos.AutoMappers;

namespace AutoMapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Config.InitAutoMapper();

            //ConvertDemo.Run();


            CreateMemberInitExpression();
            Console.Read();
        }



        public static void CreateMemberInitExpression()
        {
            System.Linq.Expressions.NewExpression newAnimal =
                System.Linq.Expressions.Expression.New(typeof(Animal));

            System.Reflection.MemberInfo speciesMember =
                typeof(Animal).GetMember("Species")[0];
            System.Reflection.MemberInfo ageMember =
                typeof(Animal).GetMember("Age")[0];

            // Create a MemberBinding object for each member
            // that you want to initialize.
            System.Linq.Expressions.MemberBinding speciesMemberBinding =
                System.Linq.Expressions.Expression.Bind(
                    speciesMember,
                    System.Linq.Expressions.Expression.Constant("horse"));
            System.Linq.Expressions.MemberBinding ageMemberBinding =
                System.Linq.Expressions.Expression.Bind(
                    ageMember,
                    System.Linq.Expressions.Expression.Constant(12));

            // Create a MemberInitExpression that represents initializing
            // two members of the 'Animal' class.
            System.Linq.Expressions.MemberInitExpression memberInitExpression =
                System.Linq.Expressions.Expression.MemberInit(
                    newAnimal,
                    speciesMemberBinding,
                    ageMemberBinding);


            //// Create a MemberInitExpression that represents initializing
            //// two members of the 'Animal' class.
            //System.Linq.Expressions.MemberInitExpression memberInitExpression =
            //    System.Linq.Expressions.Expression.MemberInit(
            //        newAnimal,
            //        speciesMemberBinding);

            Console.WriteLine(memberInitExpression.ToString());

            var expression = Expression.Lambda<Func<Dog, Animal>>(memberInitExpression);
            var compile = expression.Compile();

            var dog = new Dog() { Species = "DOG" };

            var test = compile(dog);
            Console.WriteLine(test.ToJson());
            // This code produces the following output:
            //
            // new Animal() {Species = "horse", Age = 12}
        }
    }

    public class Animal
    {
        public string Species { get; set; }
        public int Age { get; set; }
    }
    public class Dog
    {
        public string Species { get; set; }
    }
}
