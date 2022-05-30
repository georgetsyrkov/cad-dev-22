using System;
using System.Collections.Generic;

namespace CadDev
{
    public static class OtherPoints
    {
        public static float[] GetParal(float lengthX, float lengthY, float lengthZ)
        {
            List<float> total = new List<float>();

            float[] ret = new float[12];

            ret[0] = 0; ret[1] = lengthY / 2; ret[2] = lengthZ / 2;
            ret[3] = 0; ret[4] = -lengthY / 2; ret[5] = lengthZ / 2;
            ret[6] = 0; ret[7] = -lengthY / 2; ret[8] = -lengthZ / 2;
            ret[9] = 0; ret[10] = lengthY / 2; ret[11] = -lengthZ / 2;

            total.AddRange(ret);
            

            ret[0] = lengthX; ret[1] = lengthY / 2; ret[2] = lengthZ / 2;
            ret[3] = lengthX; ret[4] = lengthY / 2; ret[5] = -lengthZ / 2;
            ret[6] = lengthX; ret[7] = -lengthY / 2; ret[8] = -lengthZ / 2;
            ret[9] = lengthX; ret[10] = -lengthY / 2; ret[11] = lengthZ / 2;

            total.AddRange(ret);

            ret[0] = 0; ret[1] = lengthY / 2; ret[2] = lengthZ / 2;
            ret[3] = lengthX; ret[4] = lengthY / 2; ret[5] = lengthZ / 2;
            ret[6] = lengthX; ret[7] = -lengthY / 2; ret[8] = lengthZ / 2;
            ret[9] = 0; ret[10] = -lengthY / 2; ret[11] = lengthZ / 2;

            total.AddRange(ret);

            ret[0] = lengthX; ret[1] = lengthY / 2; ret[2] = -lengthZ / 2;
            ret[3] = 0; ret[4] = lengthY / 2; ret[5] = -lengthZ / 2;
            ret[6] = 0; ret[7] = -lengthY / 2; ret[8] = -lengthZ / 2;
            ret[9] = lengthX; ret[10] = lengthY / 2; ret[11] = -lengthZ / 2;

            total.AddRange(ret);

            ret[0] = 0; ret[1] = lengthY / 2; ret[2] = lengthZ / 2;
            ret[3] = 0; ret[4] = lengthY / 2; ret[5] = -lengthZ / 2;
            ret[6] = lengthX; ret[7] = lengthY / 2; ret[8] = -lengthZ / 2;
            ret[9] = lengthX; ret[10] = lengthY / 2; ret[11] = lengthZ / 2;

            total.AddRange(ret);

            ret[0] = 0; ret[1] = -lengthY / 2; ret[2] = lengthZ / 2;
            ret[3] = lengthX; ret[4] = -lengthY / 2; ret[5] = lengthZ / 2;
            ret[6] = lengthX; ret[7] = -lengthY / 2; ret[8] = -lengthZ / 2;
            ret[9] = 0; ret[10] = -lengthY / 2; ret[11] = -lengthZ / 2;

            total.AddRange(ret);

            return total.ToArray();
        }
    }
}