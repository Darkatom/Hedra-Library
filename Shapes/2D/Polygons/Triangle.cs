﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HedraLibrary.Shapes.Polygons {
    public partial class Triangle : Polygon {

        // 0: A | 1: B | 2: C

        public float[] Angles { get; protected set; }
        public Segment2D[] Heights { get; protected set; }

        #region Init
        public Triangle(Triangle other) {
            center = other.center;
            rotation = other.rotation;
            Area = other.Area;
            Collider = other.Collider;
            Vertices = other.Vertices;
            Edges = other.Edges;
            Normals = other.Normals;
            Angles = other.Angles;
            Heights = other.Heights;
        }

        public Triangle(Vector2 a, float ab, float ac, float alpha) {
            // https://www.mathsisfun.com/algebra/trig-solving-sas-triangles.html
            Vertices = new Vector2[3];
            Angles = new float[3];

            Vertices[0] = a;
            Angles[0] = alpha;

            float radAlpha = alpha * Mathf.Rad2Deg;
            float bc = Mathf.Sqrt(Mathf.Pow(ac, 2) + Mathf.Pow(ab, 2) - 2 * ab * ac * Mathf.Cos(radAlpha));

            Angles[1] = Mathf.Asin((Mathf.Sin(radAlpha) * ac) / bc) * Mathf.Deg2Rad;
            Angles[2] = 180 - Angles[1] - Angles[0];

            Vertices[2] = Vertices[0] + Vector2.right * ac;
            Vertices[1] = Hedra.Rotate(Vertices[0], Vertices[2], Angles[0]).normalized * ab;

            CalculateCenter();

            CreateEdges();
            CreateNormals();
            StoreHeights();
            CalculateArea();
            rotation = 0;
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            Vertices = new Vector2[] { a, b, c };
            rotation = 0;

            Init();
        }

        public Triangle(Segment2D a, Segment2D b, Segment2D c) {
            Vertices = new Vector2[3];
            Vertices[0] = a.PointA;
            Vertices[1] = a.PointB;
            if (b.PointA == a.PointA) {
                Vertices[2] = b.PointB;
            } else {
                Vertices[2] = b.PointA;
            }

            Init();
            rotation = 0;
        }

        public Triangle(Vector2 position, float radius) {
            Vector2 a = position + Vector2.up * radius;
            Vector2 b = Hedra.Rotate(position, a, 120f);
            Vector2 c = Hedra.Rotate(position, a, -120f);

            Vertices = new Vector2[] { a, b, c };

            rotation = 0;
            Init();
        }

        protected override void Init() {
            CalculateCenter();
            SortVertices();
            CreateEdges();
            CreateNormals();
            StoreHeights();

            CalculateAngles();
            CalculateArea();
        }

        protected override void CreateVertices() { /* Not needed */ }

        protected virtual void CalculateCenter() {
            center = Vector2.zero;
            center.x = (Vertices[0].x + Vertices[1].x + Vertices[2].x) / 3f;
            center.y = (Vertices[0].y + Vertices[1].y + Vertices[2].y) / 3f;
        }

        protected override void CreateEdges() {
            Edges = new Segment2D[3];
            Edges[0] = new Segment2D(Vertices[0], Vertices[1]); // AB
            Edges[1] = new Segment2D(Vertices[1], Vertices[2]); // BC
            Edges[2] = new Segment2D(Vertices[2], Vertices[0]); // AC
        }

        protected virtual void CalculateAngles() {
            Angles = new float[3];
            Angles[0] = Hedra.Angle(Edges[0].Vector, Edges[2].Vector); // Alpha
            Angles[1] = Hedra.Angle(Edges[0].Vector, Edges[1].Vector); // Beta
            Angles[2] = 180 - Angles[0] - Angles[1]; // Gamma
        }

        protected virtual void StoreHeights() {
            Heights = new Segment2D[3];
            Heights[0] = new Segment2D(Vertices[0], Edges[1].PerpendicularPoint(Vertices[0]));    // Height A
            Heights[1] = new Segment2D(Vertices[1], Edges[2].PerpendicularPoint(Vertices[1]));    // Height B
            Heights[2] = new Segment2D(Vertices[2], Edges[0].PerpendicularPoint(Vertices[2]));    // Height C
        }

        protected override void CalculateArea() {
            // Heron's formula            
            float a = Edges[1].Vector.magnitude;
            float b = Edges[2].Vector.magnitude;

            Area = 0.5f * a * b * Mathf.Sin(Angles[2] * Mathf.Rad2Deg);
        }
        #endregion

        #region Control
        public override void Translate(Vector2 direction) {
            base.Translate(direction);
            for (int i = 0; i < Heights.Length; i++) {
                Heights[i].Translate(direction);
            }
        }

        public override void Rotate(float degrees) {
            base.Rotate(degrees);
            for (int i = 0; i < Heights.Length; i++) {
                Heights[i].Rotate(Center, degrees);
            }
        }

        public override string ToString() {
            string s = "";
            s += "Vertices: " + Vertices.Join(", ") + "\n";
            s += "Edges: " + Edges.Join(", ") + "\n";
            s += "Normals: " + Normals.Join(", ") + "\n";
            s += "Angles: " + Angles.Join(", ") + "\n";
            s += "Heights: " + Heights.Join(", ") + "\n";
            return s;
        }
        #endregion

        /// <summary>
        /// Returns the deepest Vertex of this box in another box.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>The closest Vertex of this box to another box.</returns>
        public virtual Vector2 DeepestVertexIn(Rectangle other) {
            List<Vector2> containedPoints = VerticesInside(other);

            Vector2 Vertex = new Vector2(float.NaN, float.NaN);
            float greatestDistance = float.MinValue;
            for (int i = 0; i < containedPoints.Count; i++) {
                float distance = Vector2.Distance(containedPoints[i], other.ClosestPerpendicularPointTo(containedPoints[i]));
                if (distance >= greatestDistance) {
                    Vertex = containedPoints[i];
                    greatestDistance = distance;
                }
            }

            return Vertex;
        }

        #region Collision
        public override Vector2 CalculateCollisionOffset(Polygon pastSelf, Polygon obstacle) {
            return Vector2.zero;
        }

        public override Collider2D[] CheckCollisions(LayerMask mask) {
            throw new NotImplementedException();
        }

        public override Collider2D[] CheckCollisionsAt(Vector2 position, LayerMask mask) {
            throw new NotImplementedException();
        }
        #endregion

        #region Debug   
        public virtual void DrawGizmos(bool drawData, float size = 0.2f) {
            base.DrawGizmos(drawData, size);

            if (!drawData) {
                return;
            }

            DrawHeights(size / 3);
            DrawInnerTriangles();
        }

        public virtual void DrawHeights(float size = 0.2f) {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < Heights.Length; i++) {
                Heights[i].DrawGizmos(true, size);
            }
        }

        public virtual void DrawInnerTriangles() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Center, Vertices[0]);
            Gizmos.DrawLine(Center, Vertices[1]);
            Gizmos.DrawLine(Center, Vertices[2]);
        }
        #endregion
    }
}