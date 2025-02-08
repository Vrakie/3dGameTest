using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public static class GizmosExtensions
    {
        /// <summary>
        /// Draws a dashed line starting at from towards to, with a set number of dashes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lines"></param>
        public static void DrawDashedLine(Vector3 from, Vector3 to, int lines = 10)
        {
            DrawDottedLine(from, to, lines, false);
        }

        /// <summary>
        /// Draws a line of arrows starting at from towards to, with a set number of arrows.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="arrows"></param>
        public static void DrawArrowLine(Vector3 from, Vector3 to, int arrows = 10)
        {
            DrawDottedLine(from, to, arrows, true);
        }
        
        private static void DrawDottedLine(Vector3 from, Vector3 to, int lines = 10, bool arrows = false)
        {
            // calculate variables
            float sections = lines + (lines - 1);
            Vector3 direction = (to - from).normalized;
            float totalDistance = Vector3.Distance(from, to);
            float lineDistance = totalDistance / sections;
            
            // quick draw: single line / single arrow
            if (lines < 1)
            {
                if (arrows) { DrawWireArrow(from, direction, totalDistance); }
                else { Gizmos.DrawLine(from, to); }
                return;
            }
            
            // multi-line: get points to draw from and to
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i <= sections; i++)
            {
                points.Add(from + (direction * lineDistance * i));
            }
            
            // draw multiple lines
            for (int i = 0; i < points.Count - 1; i = i+2)
            {
                if (arrows) { DrawWireArrow(points[i], direction, lineDistance); }
                else { Gizmos.DrawLine(points[i], points[i + 1]); }
            }
        }

        public static void DrawCircle(Vector3 origin, Vector3 dir, float radius)
        {
#if UNITY_EDITOR
            Color32 defaultColor = UnityEditor.Handles.color;
        
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawWireDisc(origin, dir, radius);
        
            // reset to default values
            UnityEditor.Handles.color = defaultColor;
#endif
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dir"></param>
        /// <param name="anglesRange"></param>
        /// <param name="radius"></param>
        /// <param name="maxSteps"></param>
        /// <param name="centerLine"></param>
        public static void DrawWireArc(Vector3 origin, Vector3 dir, float anglesRange, float radius, float maxSteps = 20, bool centerLine = false)
        {
            float srcAngles = GetAnglesFromDir(origin, dir);
            Vector3 posA = origin;
            float stepAngles = anglesRange / maxSteps;
            float angle = srcAngles - anglesRange / 2;
            for (var i = 0; i <= maxSteps; i++)
            {
                float rad = Mathf.Deg2Rad * angle;
                Vector3 posB = origin + new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                if (centerLine || (!centerLine && 0 < i))
                {
                    Gizmos.DrawLine(posA, posB);
                }

                angle += stepAngles;
                posA = posB;
            }

            if (centerLine)
            {
                Gizmos.DrawLine(posA, origin);
            }
        }

        private static float GetAnglesFromDir(Vector3 position, Vector3 dir)
        {
            var forwardLimitPos = position + dir;
            var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

            return srcAngles;
        }

        /// <summary>
        /// Draws an arrow from the origin in a direction, with a set magnitude. The head's size is based on a multiplier of the arrows magnitude (length)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="magnitude"></param>
        /// <param name="headMultiplier"></param>
        /// <param name="arrowHeadAngle"></param>
        public static void DrawWireArrow(Vector3 origin, Vector3 direction, float magnitude = 1f, float headMultiplier = 0.25f, float arrowHeadAngle = 30.0f)
        {
            // arrow body
            Vector3 normalisedDirection = direction.normalized;
            Vector3 arrowTip = origin + (normalisedDirection * magnitude);
            Gizmos.DrawLine(origin, arrowTip);

            // arrow head
            Quaternion lookRotation = normalisedDirection.Equals(Vector3.zero) ? new Quaternion() : Quaternion.LookRotation(normalisedDirection);
            Vector3 leftDirection = (lookRotation * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1)).normalized;
            Vector3 rightDirection = (lookRotation * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1)).normalized;
            Vector3 arrowTipLeft = arrowTip + (leftDirection * magnitude * headMultiplier);
            Vector3 arrowTipRight = arrowTip + (rightDirection * magnitude * headMultiplier);
            Gizmos.DrawLine(arrowTip, arrowTipLeft);
            Gizmos.DrawLine(arrowTip, arrowTipRight);
        }

        /// <summary>
        /// Draws a plane at a given point (position) and two relative vectors (up and right)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="up"></param>
        /// <param name="right"></param>
        /// <param name="size"></param>
        /// <param name="lines"></param>
        public static void DrawWirePlane(Vector3 point, Vector3 up, Vector3 right, Vector2 size, int lines = 3)
        {
            // calculate scale
            Vector3 scaleX = Vector3.one * size.x / (lines * 2.0f);
            Vector3 scaleY = Vector3.one * size.y / (lines * 2.0f);

            // calculate gaps between lines
            Vector3 lineGapX = Vector3.Scale(right, scaleX);
            Vector3 lineGapY = Vector3.Scale(up, scaleY);

            // calculate base start and end point for lines
            Vector3 startX = point - (lineGapY * lines);
            Vector3 endX = point + (lineGapY * lines);
            Vector3 startY = point - (lineGapX * lines);
            Vector3 endY = point + (lineGapX * lines);

            // draw lines in ...
            for (int i = -lines; i <= lines; i++)
            {
                // line gaps
                Vector3 xGap = lineGapX * i;
                Vector3 yGap = lineGapY * i;

                // ... X axis
                Gizmos.DrawLine(startX + xGap, endX + xGap);

                // ... Y axis
                Gizmos.DrawLine(startY + yGap, endY + yGap);
            }
        }

        /// <summary>
        /// Draws a line from a point to the normal intersection point between the line and the point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        public static void DrawPointLineIntersection(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 intersectionPoint = GgMaths.ClosestPointOnLine(point, lineStart, lineEnd);
            Gizmos.DrawLine(intersectionPoint, point);
        }

        /// <summary>
        /// Draws a line from a point to the normal intersection point between the plane and the point.
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="point"></param>
        public static void DrawPointPlaneIntersection(Vector3 point, Plane plane)
        {
            Vector3 closestPoint = plane.ClosestPointOnPlane(point);
            Gizmos.DrawLine(point, closestPoint);
        }
    
        /// <summary>
        /// Draws the min and max distance visualisers from an AudioSource
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="alpha"></param>
        /// <param name="thickness"></param>
        public static void DrawAudioSource(Transform transform, float minDistance, float maxDistance, float alpha = 0.2f, float thickness = 1)
        {
            DrawRadius_Handles(transform, minDistance, alpha, thickness);
            DrawRadius_Handles(transform, maxDistance, alpha, thickness);
        }

        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
#if UNITY_EDITOR
            // cache defaults
            Color32 defaultColor = UnityEditor.Handles.color;
        
            UnityEditor.Handles.color = Gizmos.color;
            UnityEditor.Handles.DrawSolidArc(center, normal, from, angle, radius);
        
            // reset to default values
            UnityEditor.Handles.color = defaultColor;
#endif
        }

        #region Handles [Editor Wrapped Inside Method]
        
        private static void DrawRadius_Handles(Transform transform, float distance, float alpha = 0.2f, float thickness = 1)
        {
#if UNITY_EDITOR
            // cache defaults
            Matrix4x4 defaultMatrix = UnityEditor.Handles.matrix;
            CompareFunction defaultZTest = UnityEditor.Handles.zTest;
            Color32 defaultColor = UnityEditor.Handles.color;

            // set variable values
            UnityEditor.Handles.matrix = transform.localToWorldMatrix;
            Vector3 center;
            Vector3 normal;
            float radius;
            if (Camera.current.orthographic)
            {
                normal = Vector3.zero - UnityEditor.Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
                radius = distance;
                center = Vector3.zero;
            }
            else
            {
                normal = Vector3.zero - UnityEditor.Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                float sqrMagnitude = normal.sqrMagnitude;
                float a = distance * distance;
                float b = a * a / sqrMagnitude;

                radius = Mathf.Sqrt(a - b);
                center = Vector3.zero - a * normal / sqrMagnitude;
            }

            // right (x)
            DrawHandles_DepthTestedWireDisk(Vector3.zero, Vector3.right, distance, thickness);

            // up (y)
            DrawHandles_DepthTestedWireDisk(Vector3.zero, Vector3.up, distance, thickness);

            // forward (z)
            DrawHandles_DepthTestedWireDisk(Vector3.zero, Vector3.forward, distance, thickness);
            
            // camera
            DrawHandles_DepthTestedWireDisk(center, normal, radius, thickness);
            DrawHandles_DepthTestedDisk(center, normal, radius, alpha);

            // reset to default values
            UnityEditor.Handles.matrix = defaultMatrix;
            UnityEditor.Handles.zTest = defaultZTest;
            UnityEditor.Handles.color = defaultColor;
#endif
        }

        private static void DrawHandles_DepthTestedWireDisk(Vector3 center, Vector3 normal, float radius, float thickness = 1)
        {
#if UNITY_EDITOR
            // in front of object
            UnityEditor.Handles.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
            UnityEditor.Handles.zTest = CompareFunction.LessEqual;
            UnityEditor.Handles.DrawWireDisc(center, normal, radius, thickness);

            // behind object
            UnityEditor.Handles.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            UnityEditor.Handles.zTest = CompareFunction.Greater;
            UnityEditor.Handles.DrawWireDisc(center, normal, radius, thickness);
#endif
        }

        private static void DrawHandles_DepthTestedDisk(Vector3 center, Vector3 normal, float radius, float alpha = 0.2f)
        {
#if UNITY_EDITOR
            // in front of object
            UnityEditor.Handles.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha);
            UnityEditor.Handles.zTest = CompareFunction.LessEqual;
            UnityEditor.Handles.DrawSolidDisc(center, normal, radius);

            // behind object
            UnityEditor.Handles.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha * 0.5f);
            UnityEditor.Handles.zTest = CompareFunction.Greater;
            UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
#endif
        }
        
        #endregion

    } // class end
}