﻿using Geo;
using Geo.Abstractions.Interfaces;
using Geo.Geometries;
using Raven.Client.Spatial.Geo;
using Xunit;

namespace Raven.Client.Spatial.Tests.Geo
{
	public class GeoJsonTests : SpatialTestBase
	{
		public GeoJsonTests() : base(serializer => serializer.Converters.Add(new GeoJsonConverter()))
		{
		}

		[Fact]
		public void Null()
		{
			Assertion<IGeometry>(() => (IGeometry) null);
		}

		[Fact]
		public void Point()
		{
			Assertion<IGeometry>(() => new Point(56.543, 32.657));
		}

		[Fact]
		public void LineString()
		{
			Assertion<IGeometry>(() => new LineString(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343)));
		}

		[Fact]
		public void Polygon()
		{
			Assertion<IGeometry>(() => new Polygon(new LinearRing(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343), new Coordinate(-64.456, -23.345), new Coordinate(56.543, 32.657))));
		}

		[Fact]
		public void MultiPoint()
		{
			Assertion<IGeometry>(() => new MultiPoint(
				                new Point(56.543, 32.657),
				                new Point(56.543, 32.657)
				                ));
		}

		[Fact]
		public void MultiLineString()
		{
			Assertion<IGeometry>(() => new MultiLineString(
				                new LineString(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343)),
				                new LineString(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343))
				                ));
		}

		[Fact]
		public void MultiPolygon()
		{
			Assertion<IGeometry>(() => new MultiPolygon(
				                new Polygon(new LinearRing(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343), new Coordinate(-64.456, -23.345), new Coordinate(56.543, 32.657))),
				                new Polygon(new LinearRing(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343), new Coordinate(-64.456, -23.345), new Coordinate(56.543, 32.657)))
				                ));
		}

		[Fact]
		public void GeometryCollection()
		{
			Assertion<IGeometry>(() => new GeometryCollection(
				                new Point(56.543, 32.657),
				                new LineString(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343)),
				                new Polygon(new LinearRing(new Coordinate(56.543, 32.657), new Coordinate(23.463, 23.343), new Coordinate(-64.456, -23.345), new Coordinate(56.543, 32.657)))
				                ));
		}
	}
}