using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Plane : MonoBehaviour {
    [SerializeField] private Transform sphereTf;

    [Range( 0f, 1f ), SerializeField] private float faceColorAlpha;
    [Range( 100f, 1000f ), SerializeField] private float planeSize;

    private Color GetSide() {
        if ( sphereTf == null )
            return default;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        return plane.GetSide( sphereTf.position ) ? Color.green : Color.red;
    }

    private Vector3 ClosestPoint() {
        if ( sphereTf == null )
            return default;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        return plane.ClosestPointOnPlane( sphereTf.position );

        /*
         * Dont forget that direction vector that is from plane.ClosestPointOnPlane( sphereTf.position )
         * to sphereTf.position is always same as plane's normal vector
         */
    }

    private float DistanceToPoint() {
        if ( sphereTf == null )
            return default;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        return plane.GetDistanceToPoint( sphereTf.position );

        /*
         * Same as
         * Vector3 closestPoint = ClosestPoint();
         * Vector3.Distance( closestPoint, sphereTf.position ); => Only difference this one is always positive
         * Plane.GetDistanceToPoint( sphereTf.position ); => This is represent side distance (-,+) => Formula is => Plane.GetSide( sphereTf.position ) ? +Distance : -Distance will be returned
         */
    }

    private void PlaneRaycast() {
        if ( sphereTf == null )
            return;

        Ray ray = new( sphereTf.position, sphereTf.forward );

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        bool hitInfo = plane.Raycast( ray, out float signDistance );

        if ( hitInfo ) {
            Debug.DrawRay( ray.origin, ray.direction * signDistance, Color.green );
        }
        else {
            if ( Mathf.Approximately( signDistance, 0f ) ) {
                Debug.DrawRay( ray.origin, ray.direction * 100f, Color.white );
            }
            else {
                Debug.DrawRay( ray.origin, ray.direction * Mathf.Abs( signDistance ), Color.red );
            }
        }

        /*
         * This function sets enter to the distance along the ray, where it intersects the plane. If the ray is parallel to the plane, function returns false and sets enter to zero.
         * If the ray is pointing in the opposite direction than the plane, function returns false/ and sets enter to the distance along the ray (negative value).
         */
    }

    private void OnDrawGizmos() {
        Vector3 size = new( planeSize, planeSize, 0f );

        Color faceColor = default;
        if ( sphereTf != null ) {
            faceColor = GetSide();
        }

        Vector3 closestPoint = ClosestPoint();
        PlaneRaycast();

#if UNITY_EDITOR
        Handles.matrix = transform.localToWorldMatrix;
        Handles.DrawSolidRectangleWithOutline( new Rect( position: -size * 0.5f, size ), faceColor.A( faceColorAlpha ), Color.black );

        Handles.DrawDottedLine( transform.InverseTransformPoint( sphereTf.position ), transform.InverseTransformPoint( closestPoint ), 2f );
        Handles.DrawSolidDisc( transform.InverseTransformPoint( closestPoint ), Vector3.forward, radius: 2f );
#endif
    }
}

public static class ColorExtensionMethods {
    public static Color A( this Color c, float a ) => new( c.r, c.g, c.b, a );
}

// Notes

/*
 * Plane.flipped returns the same plane with opposite inNormal Vector
 * Plane.Flip() method just overrides inNormal Vector rather than creating new whole Plane, just doing something with a bit performant way compare to Plane.flipped
 */