using UnityEngine;

public class BotoDividir : MonoBehaviour
{
    public Vector3 ubicMeitat1 = Vector3.zero;
    public Vector3 ubicMeitat2 = Vector3.one;
    public GameObject meitat1;
    public GameObject meitat2;

    bool summoned = false;

    // Mètode que cridarà el rectangle quan està sobre el botó
    public void ActivarDividir(moveRectangle rect)
    {
        if (summoned) return;          // cooldown simple
        if (rect == null) return;

        // Només si està vertical (state == 0)
        if (rect.state != 0) return;

        summoned = true;
        rect.selfdestroy();

        Quaternion rot180Y = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

        Instantiate(meitat1, ubicMeitat1 + new Vector3(0, 0.55f, 0), rot180Y);
        Instantiate(meitat2, ubicMeitat2 + new Vector3(0, 0.55f, 0), rot180Y);


        Invoke(nameof(Reset), 1f);     // poder reutilitzar el bloque separador
    }

    void Reset()
    {
        summoned = false;
    }
}
