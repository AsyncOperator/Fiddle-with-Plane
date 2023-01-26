using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Plane : MonoBehaviour {
    [Header( "Target" )]
    [SerializeField] private Transform sphereTf;

    [Space( 10 )]

    #region Helper Canvas
    [Header( "Helper Texts" )]
    [SerializeField] private TextMeshProUGUI getSideTmp;
    [SerializeField] private TextMeshProUGUI closestPointTmp;
    [SerializeField] private TextMeshProUGUI distanceToPointTmp;
    #endregion

    [Space( 10 )]

    [Header( "Visual Settings" )]
    [Range( 0f, 1f ), SerializeField] private float faceColorAlpha;
    [Range( 100f, 1000f ), SerializeField] private float planeSize;

    private Color GetSide() {
        if ( sphereTf == null )
            return default;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        bool side = plane.GetSide( sphereTf.position );
        Color color = side ? Color.green : Color.red;

        getSideTmp?.SetText( $"GetSide: {side}" );

        return color;
    }

    private Vector3 ClosestPoint() {
        Vector3 closestPoint = default;
        if ( sphereTf == null )
            return closestPoint;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        closestPoint = plane.ClosestPointOnPlane( sphereTf.position );

        closestPointTmp?.SetText( $"ClosestPoint: {closestPoint}" );

        return closestPoint;

        /*
         * The direction vector that is from plane.ClosestPointOnPlane( sphereTf.position )
         * to sphereTf.position is always same as plane's normal vector
         */
    }

    private float DistanceToPoint() {
        float distanceToPoint = default;
        if ( sphereTf == null )
            return distanceToPoint;

        UnityEngine.Plane plane = new( transform.forward, transform.position );
        distanceToPoint = plane.GetDistanceToPoint( sphereTf.position );

        distanceToPointTmp?.SetText( $"GetDistanceToPoint: {distanceToPoint}" );

        return distanceToPoint;

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
                Debug.DrawRay( ray.origin, ray.direction * signDistance, Color.red );
            }
        }
        /*
         * This function sets enter to the distance along the ray, where it intersects the plane. If the ray is parallel to the plane, function returns false and sets enter to zero.
         * If the ray is pointing in the opposite direction than the plane, function returns false/ and sets enter to the distance along the ray (negative value).
         */
    }

    private void OnDrawGizmos() {
        Vector3 size = new( planeSize, planeSize, 0f );

        Vector3 closestPoint = ClosestPoint();
        _ = DistanceToPoint();
        PlaneRaycast();

#if UNITY_EDITOR
        Handles.matrix = transform.localToWorldMatrix;
        //Handles.color = Color
        Handles.DrawSolidRectangleWithOutline( new Rect( position: -size * 0.5f, size ), GetSide().A( faceColorAlpha ), Color.black );

        Handles.DrawDottedLine( transform.InverseTransformPoint( sphereTf.position ), transform.InverseTransformPoint( closestPoint ), 2f );

        Handles.color = Color.blue;
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