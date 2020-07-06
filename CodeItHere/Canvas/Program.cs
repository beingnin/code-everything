using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canvas
{
    class Program
    {
        static unsafe void Main(string[] args)
        {


            //Pointers();
            //FlaggedFeatureEnums();
            FlaggedPermissionEnums();
            Console.ReadKey();
        }
        static unsafe void Pointers()
        {
            DateTime[] arr = new DateTime[3]
            {
                new DateTime(2020, 01, 01),
                new DateTime(2020, 01, 02),
                new DateTime(2020, 01, 03)
            };
            fixed (DateTime* p = &arr[0])
            {
                DateTime* p2 = p;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                p2++;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                p2++;
                Console.WriteLine($"memory address : { (int)p2} and value is {*p2}");

                *p = (*p).AddDays(10);
                Console.WriteLine($"memory address : { (int)p} and value is {*p}");
            }


        }

        #region Feature Flags
        [Flags]
        enum Feature
        {
            None = 0,
            GeneralTasks = 1,
            IdeationTasks = 2,
            ImplementationTasks = 4,
            Documents = 8,
            Info = 16,
            Discussions = 32,
            LinkedLists = 64

        }
        static Feature features = Feature.None;


        static void addFeature(Feature ftr)
        {
            features |= ftr;
        }
        static void removeFeature(Feature ftr)
        {
            features = features & ~ftr;
        }
        static void hasFeature(Feature ftr)
        {
            Console.WriteLine("Value is " + (int)features);
            Console.WriteLine((features & ftr) == ftr ? ftr.ToString() + " Feature is present" : ftr.ToString() + " Feature not present");
        }
        static void RunFeatureActivities()
        {

            addFeature(Feature.IdeationTasks);
            addFeature(Feature.GeneralTasks);
            addFeature(Feature.ImplementationTasks);
            removeFeature(Feature.ImplementationTasks);

            hasFeature(Feature.ImplementationTasks);


        }
        #endregion
        #region Permission Flag
        [Flags]
        enum Permission
        {
            None = 0,
            View = 1,
            Add = 2,
            Edit = 4,
            Delete = 8,
            Approve = 16
        }
        static Permission permissions = Permission.None;
        static void FlaggedPermissionEnums()
        {
            permissions |= Permission.Delete;
            permissions |= Permission.Edit;
            permissions = permissions & ~Permission.View;
            hasPermission(Permission.Edit);
            hasPermissionFromDb(Permission.Edit, 12);
            void hasPermission(Permission pms)
            {
                Console.WriteLine("Value is " + (int)permissions);
                Console.WriteLine((permissions & pms) == pms ? pms.ToString() + " Permission is present" : pms.ToString() + " Permission not present");
            }
            void hasPermissionFromDb(Permission pms, int dbValue)
            {
                Permission value = (Permission)dbValue;
                Console.WriteLine((value & pms) == pms ? pms.ToString() + " Permission is present" : pms.ToString() + " Permission not present");
            }
        }

        #endregion
    }
}
