using System;
using GraphQL;
using GraphQL.Types;
namespace Canvas.GraphQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var schema = Schema.For(
                @"type Query {
                hello:String
                }");

            var root = new { Hello = "first resopnse" };

            var json = schema.Execute(x =>
            {
                x.Query = "{hello}";
                x.Root = root;
            });

            Console.WriteLine(json);
            Console.ReadKey();
        }
    }
}
