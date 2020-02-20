using System;
using System.Collections.Generic;
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
            /*model.StartPosition.Address = "test";
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

            Assert.IsTrue(route.Equals(checkRoute));*/
        }

        [TestMethod]
        public void AddressTest()
        {
            Address address = new Address();
            address.StringAddressValue = "мосфильмовская 74";
            Assert.AreEqual("55.71054:37.50511", address.AddressValue);
        }

        [TestMethod]
        [ExpectedException(typeof(Address.AddressException))]
        public void WrongAddressTest()
        {
            Address address = new Address();
            address.StringAddressValue = "мghuasfgdghlaksdflg";
        }

        /*[TestMethod]
        public void TestFindingShortestWay()
        {
            Position startPos = new Position(null, new LocalTime(), new LocalTime(7, 0));
            Position pos1 = new Position(null, new LocalTime(9, 0), new LocalTime(9, 30));
            Position pos2 = new Position(null, new LocalTime(15, 0), new LocalTime(15, 30));
            Position pos3 = new Position(null, new LocalTime(18, 0), new LocalTime(18, 30));

            List<Position> positions = new List<Position> {startPos, pos1, pos2, pos3};
            List<MainModel.Path> paths = new List<MainModel.Path> {new MainModel.Path(startPos, pos1, Period.FromMinutes(100)), new MainModel.Path(startPos, pos2, Period.FromMinutes(120)), new MainModel.Path(startPos, pos3, Period.FromMinutes(120)), new MainModel.Path(pos1, pos2, Period.FromMinutes(100)), new MainModel.Path(pos1, pos3, Period.FromMinutes(240)), new MainModel.Path(pos2, pos3, Period.FromMinutes(180))};
            List<MainModel.Path> actualPaths = model.AddReversePaths(paths);
            List<Position> result = model.FindShortestPossibleWay(actualPaths, startPos, new List<Position>(), startPos.TimeTo);
            List<Position> expectedResult = new List<Position>{startPos, pos1, pos2, pos3};
            CollectionAssert.AreEqual(result, expectedResult);
        }*/
    }
}
