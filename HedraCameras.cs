﻿/*
 * Hedra's partial class for screen and camera management.
 *
 * @author  Olsyx (Olatz Castaño)
 * @source https://github.com/Darkatom/Hedra-Library
 * @since   Unity 2017.1.0p4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static partial class Hedra {
        
    public static bool IsPointInsideScreen(Camera camera, Vector2 point) {
        Vector2 screenPosition = camera.WorldToScreenPoint(point);
        return screenPosition.x >= 0f && screenPosition.x <= Screen.width && screenPosition.y >= 0f && screenPosition.y <= Screen.height;
    }

    public static bool IsGameObjectInsideCameraBounds(Camera camera, Collider2D collider) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

    public static bool IsGameObjectInsideCameraBounds(Camera camera, Collider collider) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

    public static bool IsGameObjectInsideCameraBounds(Camera camera, Renderer renderer) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
        
}
