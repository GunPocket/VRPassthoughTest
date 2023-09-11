using UnityEngine;
using UnityEngine.VFX;

public class RiverHandler : MonoBehaviour {
    private Transform firstBubble;
    private Transform secondBubble;
    private VisualEffect river;

    public void Initializer(Transform _firstBubble, Transform _secondBubble, VisualEffect _river) {
        firstBubble = _firstBubble;
        secondBubble = _secondBubble;
        river = _river;

        MakeRiver();
    }

    private void MakeRiver() {
        transform.position = new Vector3 (firstBubble.position.x, 0, firstBubble.position.z);
        transform.LookAt(new Vector3(secondBubble.position.x, transform.position.y, secondBubble.position.z));
        transform.rotation *= Quaternion.FromToRotation(Vector3.right, Vector3.forward);
        var p1 = Vector3.Distance(firstBubble.position, secondBubble.position);

        var ratio = p1 / 9;

        river.SetVector3("p1", new Vector3(p1 - 1   , 0,  0        ));
        river.SetVector3("p2", new Vector3(6 * ratio, 0,  3 * ratio));
        river.SetVector3("p3", new Vector3(3 * ratio, 0, -3 * ratio));



        /*
        river.SetVector3("p4", Vector3.zero);

        var difference = secondBubble.position - firstBubble.position;
        var p1 = new Vector3(difference.x, 0, difference.z * Mathf.PI);

        river.SetVector3("p1", p1);
        
        Vector3 p2 = p1 * 2 / 3;
        Vector3 p3 = p1 / 3;
        river.SetVector3("p2", p2);
        river.SetVector3("p3", p3);
        */
    }
}
