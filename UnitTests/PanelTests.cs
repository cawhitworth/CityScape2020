using System.Linq;
using CityScape2020.Geometry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDX;

namespace CityScape2020.UnitTests
{
    [TestClass]
    public class PanelTests
    {
        private static readonly Vector2 Origin = new Vector2(0.0f);
        private static readonly Color Mod = Color.White;

        [TestMethod]
        public void XY_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XY, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 0, -1);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,2,0));
            v[2].Position.Should().Be(new Vector3(1,0,0));
            v[3].Position.Should().Be(new Vector3(1,2,0));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }

        [TestMethod]
        public void XY_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XY, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 0, 1);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,2,0));
            v[2].Position.Should().Be(new Vector3(1,0,0));
            v[3].Position.Should().Be(new Vector3(1,2,0));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }

        [TestMethod]
        public void XZ_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XZ, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 1, 0);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,0,2));
            v[2].Position.Should().Be(new Vector3(1,0,0));
            v[3].Position.Should().Be(new Vector3(1,0,2));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }

        [TestMethod]
        public void XZ_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XZ, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, -1, 0);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,0,2));
            v[2].Position.Should().Be(new Vector3(1,0,0));
            v[3].Position.Should().Be(new Vector3(1,0,2));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }

        [TestMethod]
        public void YZ_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.YZ, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(1, 0, 0);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,2,0));
            v[2].Position.Should().Be(new Vector3(0,0,1));
            v[3].Position.Should().Be(new Vector3(0,2,1));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }

        [TestMethod]
        public void YZ_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.YZ, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(-1, 0, 0);

            v[0].Position.Should().Be(new Vector3(0,0,0));
            v[1].Position.Should().Be(new Vector3(0,2,0));
            v[2].Position.Should().Be(new Vector3(0,0,1));
            v[3].Position.Should().Be(new Vector3(0,2,1));

            v[0].Normal.Should().Be(norm);
            v[1].Normal.Should().Be(norm);
            v[2].Normal.Should().Be(norm);
            v[3].Normal.Should().Be(norm);
        }
    }
}
