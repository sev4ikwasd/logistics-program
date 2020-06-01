using System.Globalization;
using LogisticsProgram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Text;

namespace LogisticsProgramTests
{
    [TestClass]
    public class MainModelTests
    {
        private readonly RoutesModel model;

        public MainModelTests()
        {
            model = new RoutesModel();
        }

        [TestMethod]
        public void GenerateTest()
        {
            model.StartPosition.Address.AddressValue = "55.88318,37.51557";
            model.StartPosition.TimeFrom =
                LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("07:00").Value;
            model.StartPosition.TimeTo =
                LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("20:00").Value;
            model.AmountOfVehicles = 2;
            model.DelayPeriod = Period.FromMinutes(30);
            var pos1 = new Position();
            pos1.Address.AddressValue = "55.70985,37.50378";
            pos1.TimeFrom = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("10:00").Value;
            pos1.TimeTo = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("12:00").Value;
            model.Positions.Add(pos1);
            var pos2 = new Position();
            pos2.Address.AddressValue = "55.72325,37.52783";
            pos2.TimeFrom = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("18:00").Value;
            pos2.TimeTo = LocalTimePattern.Create("HH:mm", CultureInfo.InvariantCulture).Parse("19:00").Value;
            model.Positions.Add(pos2);
            model.GenerateRoute().GetAwaiter().GetResult();
        }

        /*[TestMethod]
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
        }*/

        /*[TestMethod]
        public void TestFindingShortestWay()
        {
            Position startPos = new Position(null, new LocalTime(), new LocalTime(7, 0));
            Position pos1 = new Position(null, new LocalTime(9, 0), new LocalTime(9, 30));
            Position pos2 = new Position(null, new LocalTime(15, 0), new LocalTime(15, 30));
            Position pos3 = new Position(null, new LocalTime(18, 0), new LocalTime(18, 30));

            List<Position> positions = new List<Position> {startPos, pos1, pos2, pos3};
            List<RoutesModel.Path> paths = new List<RoutesModel.Path> {new RoutesModel.Path(startPos, pos1, Period.FromMinutes(100)), new RoutesModel.Path(startPos, pos2, Period.FromMinutes(120)), new RoutesModel.Path(startPos, pos3, Period.FromMinutes(120)), new RoutesModel.Path(pos1, pos2, Period.FromMinutes(100)), new RoutesModel.Path(pos1, pos3, Period.FromMinutes(240)), new RoutesModel.Path(pos2, pos3, Period.FromMinutes(180))};
            List<RoutesModel.Path> actualPaths = model.AddReversePaths(paths);
            List<Position> result = model.FindShortestPossibleWay(actualPaths, startPos, new List<Position>(), startPos.TimeTo);
            List<Position> expectedResult = new List<Position>{startPos, pos1, pos2, pos3};
            CollectionAssert.AreEqual(result, expectedResult);
        }*/
    }
}