using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder
{
    public static void Building(Vector3 pos, int x, int y, uint stories)
    {
        Vector3[] vertices;

        int xSize = 1;
        int ySize = 1;
        int zSize = 1;

        int cornerVtx = 8;
        int edgeVtx = (xSize + ySize + zSize - 3) * 4;
        int faceVtx = ((xSize - 1) * (ySize - 1) +
                       (xSize - 1) * (zSize - 1) +
                       (ySize - 1) * (zSize - 1)) * 2;


        vertices = new Vector3[cornerVtx + edgeVtx + faceVtx];

        for (int i = 0; i < xSize; i++)
        {
            vertices[i] = new Vector3(i, 0, 0);
        }
    }
}
