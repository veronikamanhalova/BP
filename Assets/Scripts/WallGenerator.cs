using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum Dirrection
{
    Right = 1,
    Straight = 0,
    Left = -1
}
public class WallGenerator : MonoBehaviour
{
    [SerializeField] Transform holds;
    [SerializeField] Vector3[] position;
    [SerializeField] Vector3[] rotations;

    [SerializeField] float wallStart;
    [SerializeField] float wallPartSize;
    [SerializeField] GameObject[] squareGroup;
    [SerializeField] GameObject[] rightDiagonalGroup;
    [SerializeField] GameObject[] leftDiagonalGroup;
    const int wallParts = 8;
    const int wallSides = 4;
    // Start is called before the first frame update

    public void GenerateWall()
    {
        DestroyWall();
        int generatedParts = 0;
        int lastWallSideIdx = 0;
        Dirrection dirrection = Dirrection.Straight;
        while (generatedParts < wallParts)
        {
            int wallSideIdx;
            if (dirrection == Dirrection.Straight)
            {
                wallSideIdx = Random.Range(0, wallSides - 1);
                int sidesDiff = Mathf.Abs(wallSideIdx - lastWallSideIdx);

                if (2 > sidesDiff || sidesDiff == 3)
                {
                    //generate wall part
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);

                    if (wallSideIdx > lastWallSideIdx || (wallSideIdx == 0 && lastWallSideIdx == 3))
                    {
                        // Right
                        GenerateOnRightWallSide(newPos,wallSideIdx,ref dirrection);
                    }
                    else if (wallSideIdx < lastWallSideIdx)
                    {
                        // Left
                        GenerateOnLeftWallSide(newPos,wallSideIdx, ref dirrection);
                    }
                    else
                    {
                        // Straight
                        int groupIdx = Random.Range(-1, 1);
                        if (groupIdx == (int)Dirrection.Right)
                        {
                            //generate Right Group
                            int randIdx = Random.Range(0, rightDiagonalGroup.Length - 1);
                            Instantiate(rightDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

                            dirrection = Dirrection.Right;
                        }
                        else if(groupIdx == (int)Dirrection.Left)
                        {
                            //generate Left Group
                            int randIdx = Random.Range(0, leftDiagonalGroup.Length - 1);
                            Instantiate(leftDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

                            dirrection = Dirrection.Left;
                        }
                        else
                        {
                            //generate Square Group 
                            int randIdx = Random.Range(0, squareGroup.Length - 1);
                            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

                            dirrection = Dirrection.Straight;
                        }
                    }

                    lastWallSideIdx = wallSideIdx;
                    generatedParts++;
                }
            }
            else if (dirrection == Dirrection.Right)
            {
                //Generate on right side of the wall
                wallSideIdx = (lastWallSideIdx == wallSides - 1) ? 0 : lastWallSideIdx - 1;

                float YPos = wallStart + (generatedParts * wallPartSize);
                Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);

                GenerateOnRightWallSide(newPos, wallSideIdx, ref dirrection);

                lastWallSideIdx = wallSideIdx;
                generatedParts++;
            }
            else
            {
                //Generate on left side of the wall
                wallSideIdx = (lastWallSideIdx == 0) ? wallSides - 1 : lastWallSideIdx - 1;

                float YPos = wallStart + (generatedParts * wallPartSize);
                Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);

                GenerateOnLeftWallSide(newPos, wallSideIdx, ref dirrection);

                lastWallSideIdx = wallSideIdx;
                generatedParts++;
            }

        }
    }

    void GenerateOnRightWallSide(Vector3 newPos, int wallSideIdx, ref Dirrection dirrection)
    {
        int groupIdx = Random.Range(0, 1);

        if (groupIdx == (int)Dirrection.Right)
        {
            //generate Right Group
            int randIdx = Random.Range(0, rightDiagonalGroup.Length - 1);
            Instantiate(rightDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

            dirrection = Dirrection.Right;
        }
        else
        {
            //generate Square Group 
            int randIdx = Random.Range(0, squareGroup.Length - 1);
            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

            dirrection = Dirrection.Straight;
        }
    }

    void GenerateOnLeftWallSide(Vector3 newPos, int wallSideIdx, ref Dirrection dirrection)
    {
        int groupIdx = Random.Range(-1, 0);

        if (groupIdx == (int)Dirrection.Left)
        {
            //generate Left Group
            int randIdx = Random.Range(0, leftDiagonalGroup.Length - 1);
            Instantiate(leftDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

            dirrection = Dirrection.Left;
        }
        else
        {
            //generate Square Group 
            int randIdx = Random.Range(0, squareGroup.Length - 1);
            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);

            dirrection = Dirrection.Straight;
        }
    }
    void DestroyWall()
    {
        while (holds.childCount > 0)
        {
            DestroyImmediate(holds.GetChild(0).gameObject);
        }
    }
}
