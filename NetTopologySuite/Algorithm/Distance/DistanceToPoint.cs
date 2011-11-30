using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.Algorithm.Distance
{
    ///<summary>
    /// Computes the Euclidean distance (L2 metric) from a Point to a Geometry.
    ///</summary>
    /// <remarks>
    /// Also computes two points which are separated by the distance.
    /// </remarks>
    public class DistanceToPoint
    {
        public static void ComputeDistance(IGeometry geom, Coordinate pt, PointPairDistance ptDist)
        {
            if (geom is ILineString)
            {
                ComputeDistance((ILineString) geom, pt, ptDist);
            }
            else if (geom is IPolygon)
            {
                ComputeDistance((IPolygon) geom, pt, ptDist);
            }
            else if (geom is IGeometryCollection)
            {
                var gc = (IGeometryCollection) geom;
                for (int i = 0; i < gc.NumGeometries; i++)
                {
                    IGeometry g = gc.GetGeometryN(i);
                    ComputeDistance(g, pt, ptDist);
                }
            }
            else
            {
                // assume geom is Point
                ptDist.SetMinimum(geom.Coordinate, pt);
            }
        }

        public static void ComputeDistance(ILineString line, Coordinate pt, PointPairDistance ptDist)
        {
            Coordinate[] coords = line.Coordinates;
            for (int i = 0; i < coords.Length - 1; i++)
            {
                // used for point-line distance calculation
                LineSegment temp = new LineSegment();
                temp.SetCoordinates(coords[i], coords[i + 1]);
                // this is somewhat inefficient - could do better
                Coordinate closestPt = temp.ClosestPoint(pt);
                ptDist.SetMinimum(closestPt, pt);
            }
        }

        public static void ComputeDistance(LineSegment segment, Coordinate pt, PointPairDistance ptDist)
        {
            Coordinate closestPt = segment.ClosestPoint(pt);
            ptDist.SetMinimum(closestPt, pt);
        }

        public static void ComputeDistance(IPolygon poly, Coordinate pt, PointPairDistance ptDist)
        {
            ComputeDistance(poly.ExteriorRing, pt, ptDist);
            for (int i = 0; i < poly.NumInteriorRings; i++)
            {
                ComputeDistance(poly.GetInteriorRingN(i), pt, ptDist);
            }
        }
    }
}