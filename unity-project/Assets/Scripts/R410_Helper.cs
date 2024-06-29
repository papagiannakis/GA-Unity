using R41;
using System;
using UnityEngine;

/// <summary>
/// A singleton class that provides helper methods for the R410 class.
/// </summary>
public class R410_Helper : MonoBehaviour
{
    private static R410_Helper instance;

    /// <summary>
    /// Static instance of the R410_Helper class.
    /// </summary>
    public static R410_Helper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<R410_Helper>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("R410_Helper");
                    instance = singletonObject.AddComponent<R410_Helper>();
                    instance.Initialize(); 
                }
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    public string[] _basis;
    // The basis blades
    public R410 e1;
    public R410 e2;
    public R410 e3;
    public R410 e4;
    public R410 e5;
    public R410 e12;
    public R410 e13;
    public R410 e14;
    public R410 e15;
    public R410 e23;
    public R410 e24;
    public R410 e25;
    public R410 e34;
    public R410 e35;
    public R410 e45;
    public R410 e123;
    public R410 e124;
    public R410 e125;
    public R410 e134;
    public R410 e135;
    public R410 e145;
    public R410 e234;
    public R410 e235;
    public R410 e245;
    public R410 e345;
    public R410 e1234;
    public R410 e1235;
    public R410 e1245;
    public R410 e1345;
    public R410 e2345;
    public R410 e12345;
    public R410 eInf;
    public R410 eOrigin;

    public void Initialize()
    {
        _basis = new[] { "1", "e1", "e2", "e3", "e4", "e5", "e12", "e13", "e14", "e15", "e23", "e24", "e25", "e34", "e35", "e45", "e123", "e124", "e125", "e134", "e135", "e145", "e234", "e235", "e245", "e345", "e1234", "e1235", "e1245", "e1345", "e2345", "e12345" };
        // The basis blades
        e1 = new R410(1f, 1);
        e2 = new R410(1f, 2);
        e3 = new R410(1f, 3);
        e4 = new R410(1f, 4);
        e5 = new R410(1f, 5);
        e12 = new R410(1f, 6);
        e13 = new R410(1f, 7);
        e14 = new R410(1f, 8);
        e15 = new R410(1f, 9);
        e23 = new R410(1f, 10);
        e24 = new R410(1f, 11);
        e25 = new R410(1f, 12);
        e34 = new R410(1f, 13);
        e35 = new R410(1f, 14);
        e45 = new R410(1f, 15);
        e123 = new R410(1f, 16);
        e124 = new R410(1f, 17);
        e125 = new R410(1f, 18);
        e134 = new R410(1f, 19);
        e135 = new R410(1f, 20);
        e145 = new R410(1f, 21);
        e234 = new R410(1f, 22);
        e235 = new R410(1f, 23);
        e245 = new R410(1f, 24);
        e345 = new R410(1f, 25);
        e1234 = new R410(1f, 26);
        e1235 = new R410(1f, 27);
        e1245 = new R410(1f, 28);
        e1345 = new R410(1f, 29);
        e2345 = new R410(1f, 30);
        e12345 = new R410(1f, 31);
        eInf = e4 + e5;
        eOrigin = (e5 - e4) * 0.5f;

    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }


    public static R410 CreateSphere(Vector3 center, float radius)
    {
        R410 sphere = 
            (center.x * R410_Helper.Instance.e1) + 
            (center.y * R410_Helper.instance.e2) + 
            (center.z * R410_Helper.Instance.e3) +
            (0.5f * (center.x * center.x + center.y * center.y + center.z * center.z - radius* radius) * R410_Helper.instance.eInf) +
            R410_Helper.Instance.eOrigin;
        return sphere;
    }
    
    public static R410 ScaleToDelation(float scale_factor)
    {
        R410 result = R410Pool.GetFromPool();
        float t = ((1 - scale_factor) / (1 + scale_factor));

        result = 1 + (t) * (R410_Helper.Instance.e45);
        return result;
    }

    public static R410 InverseDilation(float scale_factor)
    {
        R410 eInfeOrg = R410_Helper.Instance.eInf ^ R410_Helper.Instance.eOrigin;

        R410 result =
            (
            (1 + scale_factor) * (1 + scale_factor) + (scale_factor * scale_factor - 1)
            * (eInfeOrg)
            )
            *
            (1.0f / (4 * scale_factor));
        eInfeOrg.Dispose();
        return result;
    }

    public static (R410 tr, float radius) Extract_T_R_D_From_TRD(R410 M)
    {
        R410 U = CreateSphere(Vector3.zero, 1);
        R410 MU = M * U;
        R410 invM = ~M;

        R410 U_2 = MU * invM;

        if (U_2._mVec[5] - U_2._mVec[4] != 1.0f)
        {
            float z = U_2._mVec[5] - U_2._mVec[4];
            U_2 *= (1.0f / z);
        }
        (Vector3 center, float radius) = ExtractSphereData(U_2);
        R410 TR = M * InverseDilation(radius);

        U.Dispose();
        MU.Dispose();
        invM.Dispose();
        U_2.Dispose();

        return (TR, radius);
    }

    public static Quaternion Rotor_to_quaternion(R410 mv)
    {
        Quaternion q = Quaternion.identity;
        R410 _tmpMv = (R410_Helper.Instance.e123 * mv);
        q[0] = mv[0];
        q[1] = _tmpMv[1];
        q[2] = _tmpMv[2];
        q[3] = _tmpMv[3];
        _tmpMv.Dispose();
        return q;
    }

    public static R410 Quaternion_to_rotor(Quaternion q)
    {
        R410 result = R410Pool.GetFromPool();

        result._mVec[1] = q[1];
        result._mVec[2] = q[2];
        result._mVec[3] = q[3];

        R410 temp = R410_Helper.Instance.e123 * result;
        result.Dispose();
        result = temp;
        temp = result * (-1.0f);
        result.Dispose();
        result = temp;
        result._mVec[0] = q[0];
        return result;
    }

    public static R410 Generate_translation_rotor(Vector3 euc_vector_a)
    {
        R410 T = R410Pool.GetFromPool();
        T._mVec[1] = euc_vector_a[0];
        T._mVec[2] = euc_vector_a[1];
        T._mVec[3] = euc_vector_a[2];
        R410 temp = T * R410_Helper.Instance.eInf;
        T.Dispose();
        R410 scaledTemp = temp * 0.5f;
        temp.Dispose();
        R410 one = R410Pool.GetFromPool();
        one._mVec[0] = 1;
        R410 result = one - scaledTemp;
        scaledTemp.Dispose();
        one.Dispose();
        return result;
    }

    public static R410 Quaternion_and_vector_and_scale_to_rotor(Quaternion q, Vector3 v, float scale)
    {
        R410 rotation = Quaternion_to_rotor(q);
        R410 translation = Generate_translation_rotor(v);
        R410 dilator = ScaleToDelation(scale);
        R410 product = translation * rotation * dilator;
        translation.Dispose();
        rotation.Dispose();
        dilator.Dispose();
        R410 result = product.Normalized();
        product.Dispose();
        return result;
    }

    public static R410 Quaternion_and_vector_to_rotor(Quaternion q, Vector3 v)
    {
        R410 rotation = Quaternion_to_rotor(q);
        R410 translation = Generate_translation_rotor(v);
        R410 product = translation * rotation;
        translation.Dispose();
        rotation.Dispose();
        R410 result = product.Normalized();
        product.Dispose();
        return result;
    }

    public static Vector3 Rotor_to_vector(R410 T)
    {
        R410 t2 = T.Normalized();
        R410 eOriginNeg = -1.0f * R410_Helper.Instance.eOrigin;  // Explicitly manage this instance
        R410 innerProduct = t2 | eOriginNeg;
        R410 scaled = -2.0f * innerProduct;
        Vector3 vec = new Vector3(scaled[1], scaled[2], scaled[3]);

        t2.Dispose();
        eOriginNeg.Dispose();  // Dispose the explicitly managed instance
        innerProduct.Dispose();
        scaled.Dispose();

        return vec;
    }

    public static R410 LerpTR(R410 TR_start, R410 TR_end, float factor)
    {
        //using (R410 scaledStart = TR_start * (1 - factor))
        //using (R410 scaledEnd = TR_end * factor)
        //{
        //	R410 result = scaledStart + scaledEnd;
        //	Debug.Log(result);
        //	//return result;
        //}

        R410 res = R410Pool.GetFromPool();
        for (int i = 0; i < 32; i++)
        {
            if (i == 0 || i == 1 || i == 2 || i == 3 || i == 6 || i == 7 || i == 8
                || i == 9 || i == 10 || i == 11 || i == 12 || i == 13 || i == 14
                || i == 26 || i == 27)
                res._mVec[i] = TR_start._mVec[i] * (1 - factor) + TR_end._mVec[i] * factor;
            else
                res._mVec[i] = 0;
        }
        return res;

    }

    public static R410 R_at_ei(int i, R410 R)
    {
        R410 _r = R410Pool.GetFromPool();
        if (i == 0)
        {
            _r[0] = R[0];
        }
        if (i == 1)
        {
            _r[1] = R[1];
            _r[2] = R[2];
            _r[3] = R[3];
            _r[4] = R[4];
            _r[5] = R[5];
        }
        if (i == 2)
        {
            _r[6] = R[6];
            _r[7] = R[7];
            _r[8] = R[8];
            _r[9] = R[9];
            _r[10] = R[10];
            _r[11] = R[11];
            _r[12] = R[12];
            _r[13] = R[13];
            _r[14] = R[14];
            _r[15] = R[15];
        }
        if (i == 3)
        {
            _r[16] = R[16];
            _r[17] = R[17];
            _r[18] = R[18];
            _r[19] = R[19];
            _r[20] = R[20];
            _r[21] = R[21];
            _r[22] = R[22];
            _r[23] = R[23];
            _r[24] = R[24];
            _r[25] = R[25];
        }
        if (i == 4)
        {
            _r[26] = R[26];
            _r[27] = R[27];
            _r[28] = R[28];
            _r[29] = R[29];
            _r[30] = R[30];
            _r[31] = R[31];
        }
        return _r;

    }

    public static void TR_to_vectorT_quaternionQ(R410 TR, ref Vector3 t, ref Quaternion q)
    {
        R410 R_only = R410Pool.GetFromPool();
        R_only[0] = TR[0];
        R_only[1] = TR[1];
        R_only[2] = TR[2];
        R_only[3] = TR[3];
        R_only[6] = TR[6];
        R_only[7] = TR[7];
        R_only[10] = TR[10];
        R_only[16] = TR[16];
        q = Rotor_to_quaternion(R_only);

        R410 invertedR = ~R_only;
        R410 TR_mult_invR = TR * invertedR;
        R410 T_only = TR_mult_invR.Normalized();
        t = Rotor_to_vector(T_only);

        R_only.Dispose();
        invertedR.Dispose();
        TR_mult_invR.Dispose();
        T_only.Dispose();
    }

    public static (Vector3 center, float radius) ExtractSphereData(R410 sphere)
    {
        float z = sphere[4] + 0.5f;
        float r = sphere[1] * sphere[1] +
            sphere[2] * sphere[2] +
            sphere[3] * sphere[3] -
            2 * z;
        r = (float)Mathf.Sqrt(r);
        Vector3 c = new Vector3(sphere[1], sphere[2], sphere[3]);
        return (c, r);
    }

    public static R410 CreatePlane(Vector3 normal, float distance)
    {
        return R410Pool.GetFromPool();
    }

    public static R410 CreatePoint(Vector3 position)
    {
        return R410Pool.GetFromPool();
    }

    public static R410 CreateLine(Vector3 point, Vector3 direction)
    {
        return R410Pool.GetFromPool();
    }

    public static R410 CreateCircle(Vector3 center, Vector3 normal, float radius)
    {
        return R410Pool.GetFromPool();
    }
}
