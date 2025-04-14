using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHeadCollision : MonoBehaviour
{
    [SerializeField] private float detectionDist = 0.2f;
    [SerializeField] private LayerMask detectLayers;
    public List<RaycastHit> colliderHits {  get; private set; }
    
    private List<RaycastHit> Detection(Vector3 pos, float dist, LayerMask mask)
    {
        List<RaycastHit> detectedHits = new();
        List<Vector3> directions = new() { transform.forward, transform.right, -transform.right };
        RaycastHit hit;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(pos, dir, out hit, dist, mask)){
                detectedHits.Add(hit);
            }
        }
        return detectedHits;
    }

    // Update is called once per frame
    public void CallDetection()
    {
        colliderHits = Detection(transform.position, detectionDist, detectLayers);
    }
}
