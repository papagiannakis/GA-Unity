using R41;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the interpolation pipeline to interopolate object with multi vectors.
/// </summary>
public class MultiVectorLerp : MonoBehaviour
{
    /// <summary>
    /// Contains the start and end position, rotation and scale of the object.
    /// </summary>
    public Transform start, end;

    /// <summary>
    /// The interpolation factor.
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float factor = 0;

    private R410 TR_start, TR_end;

    private float s1, s2;

    void Start()
    {
        /// Convert the start and end position, rotation and scale to rotor.
        R410 tmp_TR_start = R410_Helper.Quaternion_and_vector_and_scale_to_rotor(
                start.rotation,
                start.position, 
                start.localScale.x);

        /// Convert the start and end position, rotation and scale to rotor.
        R410 tmp_TR_end = R410_Helper.Quaternion_and_vector_and_scale_to_rotor(
                end.rotation,
                end.position, 
                end.localScale.x);
        (TR_start, s1) = R410_Helper.Extract_T_R_D_From_TRD(tmp_TR_start);
        (TR_end, s2) = R410_Helper.Extract_T_R_D_From_TRD(tmp_TR_end);
    }

    void Update()
    {
        /// Interpolate the position, rotation and scale of the object.

        Vector3 v = Vector3.zero;
        Quaternion q = Quaternion.identity;

        using (R410 lerp_TR = R410_Helper.LerpTR(TR_start, TR_end, factor))
        {
            R410_Helper.TR_to_vectorT_quaternionQ(lerp_TR, ref v, ref q);
        }

        transform.position = v;
        transform.rotation = q;
        transform.localScale = ((s2 * factor) + (s1 * (1.0f - factor))) * Vector3.one;       
    }
}