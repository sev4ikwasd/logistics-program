using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Globalization;
using LogisticsProgram;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgramTests
{
    [TestClass]
    public class MainModelTests
    {
        MainModel model;
        public MainModelTests()
        {
            model = new MainModel();
        }

        [TestMethod]
        public void GenerateTest()
        {
            model.StartAddress = "test";
            Position pos1 = new Position();
            pos1.Address = "Test1";
            //pos1.Time = DateTime.ParseExact("14:00", "HH:mm", null);
            pos1.Time = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("14:00").Value;
            model.Positions.Add(pos1);
            Position pos2 = new Position();
            pos2.Address = "Test2";
            //pos2.Time = DateTime.ParseExact("13:00", "HH:mm", null);
            pos2.Time = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("13:00").Value;
            model.Positions.Add(pos2);
            Route route = model.GenerateRoute();

            Route checkRoute = new Route();
            checkRoute.Positions.Add(pos2);
            checkRoute.Positions.Add(pos1);

            Assert.IsTrue(route.Equals(checkRoute));
        }
    }
}
