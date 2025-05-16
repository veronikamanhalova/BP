using UnityEngine;
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
    int difficultyIdx;
    Dirrection dirrection;

    public void GenerateWall(Difficulty diff)
    {
        DestroyWall();
        difficultyIdx = (int)diff;
        dirrection = Dirrection.Straight;
        int generatedParts = 1;
        int lastWallSideIdx = 0;

        GenerateFirstWallPard(lastWallSideIdx);

        while (generatedParts < wallParts)
        {
            int wallSideIdx;
            if (dirrection == Dirrection.Straight)
            {
                Dirrection wallSideDirrection = (Dirrection)Random.Range(-1, 2);

                if (wallSideDirrection == Dirrection.Straight)
                {
                    wallSideIdx = lastWallSideIdx;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnSameWallSide(newPos, wallSideIdx);

                }else if (wallSideDirrection == Dirrection.Right)
                {
                    wallSideIdx = (lastWallSideIdx == wallSides - 1) ? 0 : lastWallSideIdx + 1;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnRightWallSide(newPos, wallSideIdx);
                }
                else
                {
                    wallSideIdx = (lastWallSideIdx == 0) ? wallSides - 1 : lastWallSideIdx - 1;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnLeftWallSide(newPos, wallSideIdx);
                }

                lastWallSideIdx = wallSideIdx;
                generatedParts++;
            }
            else if (dirrection == Dirrection.Right)
            {
                Dirrection wallSideDirrection = (Dirrection)Random.Range(0, 2);
                if(wallSideDirrection == Dirrection.Straight)
                {
                    wallSideIdx = lastWallSideIdx;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnSameWallSide(newPos, wallSideIdx);
                }
                else
                {
                    wallSideIdx = (lastWallSideIdx == wallSides - 1) ? 0 : lastWallSideIdx + 1;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnRightWallSide(newPos, wallSideIdx);
                }

                lastWallSideIdx = wallSideIdx;
                generatedParts++;
            }
            else
            {
                Dirrection wallSideDirrection = (Dirrection)Random.Range(-1, 1);
                if (wallSideDirrection == Dirrection.Straight)
                {
                    wallSideIdx = lastWallSideIdx;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnSameWallSide(newPos, wallSideIdx);
                }
                else
                {
                    wallSideIdx = (lastWallSideIdx == 0) ? wallSides - 1 : lastWallSideIdx - 1;
                    float YPos = wallStart + (generatedParts * wallPartSize);
                    Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
                    GenerateOnLeftWallSide(newPos, wallSideIdx);
                }

                lastWallSideIdx = wallSideIdx;
                generatedParts++;
            }

        }
    }

    void GenerateFirstWallPard(int wallSideIdx)
    {
        float YPos = wallStart;
        Vector3 newPos = transform.parent.transform.position + new Vector3(position[wallSideIdx].x, YPos, position[wallSideIdx].z);
        GenerateOnSameWallSide(newPos, wallSideIdx);
    }

    void GenerateOnRightWallSide(Vector3 newPos, int wallSideIdx)
    {
        Dirrection groupIdx = (Dirrection)Random.Range(0, 2);

        if (groupIdx == Dirrection.Right)
        {
            //generate Right Group
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, rightDiagonalGroup.Length);
            
            rightDiagonalGroup[randIdx].SetActive(true);
            Instantiate(rightDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            rightDiagonalGroup[randIdx].SetActive(false);
        }
        else
        {
            //generate Square Group 
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, squareGroup.Length);

            squareGroup[randIdx].SetActive(true);
            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            squareGroup[randIdx].SetActive(false);
        }
        dirrection = groupIdx;
    }

    void GenerateOnLeftWallSide(Vector3 newPos, int wallSideIdx)
    {
        Dirrection groupIdx = (Dirrection)Random.Range(-1, 1);

        if (groupIdx == Dirrection.Left)
        {
            //generate Left Group
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, leftDiagonalGroup.Length);

            leftDiagonalGroup[randIdx].SetActive(true);
            Instantiate(leftDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            leftDiagonalGroup[randIdx].SetActive(false);
        }
        else
        {
            //generate Square Group 
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, squareGroup.Length);

            squareGroup[randIdx].SetActive(true);
            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            squareGroup[randIdx].SetActive(false);
        }
        dirrection = groupIdx;
    }

    void GenerateOnSameWallSide(Vector3 newPos, int wallSideIdx)
    {
        Dirrection groupIdx = (Dirrection)Random.Range(-1, 2);
        if (groupIdx == Dirrection.Right)
        {
            //generate Right Group
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, rightDiagonalGroup.Length);

            rightDiagonalGroup[randIdx].SetActive(true);
            Instantiate(rightDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            rightDiagonalGroup[randIdx].SetActive(false);
        }
        else if (groupIdx == Dirrection.Left)
        {
            //generate Left Group
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, leftDiagonalGroup.Length);

            leftDiagonalGroup[randIdx].SetActive(true);
            Instantiate(leftDiagonalGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            leftDiagonalGroup[randIdx].SetActive(false);
        }
        else
        {
            //generate Square Group 
            int randIdx = (Random.Range(0, 101) < 76) ? difficultyIdx : Random.Range(0, squareGroup.Length);

            squareGroup[randIdx].SetActive(true);
            Instantiate(squareGroup[randIdx], newPos, Quaternion.Euler(rotations[wallSideIdx]), holds);
            squareGroup[randIdx].SetActive(false);
        }
        dirrection = groupIdx;
    }
    void DestroyWall()
    {
        while (holds.childCount > 0)
        {
            DestroyImmediate(holds.GetChild(0).gameObject);
        }
    }
}
