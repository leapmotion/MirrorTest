using UnityEngine;
using System.Collections;
using Leap;

public class ButtonLogic : MonoBehaviour {

	Vector3 m_touchOffset;
	float m_depth;
	GameObject m_touchObject;
	Vector3 m_startingPos;
	Controller m_leapController;

	void Start() {
		m_depth = transform.position.z;
		m_startingPos = transform.position;
		m_leapController = new Controller();
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Tip" && other.GetComponent<TipLogic>().m_touching == true) return;

		if (m_touchObject == null && other.tag == "Tip") {
			m_touchObject = other.gameObject;
			m_touchObject.GetComponent<TipLogic>().m_touching = true;
			m_touchOffset = other.transform.position - transform.position;
			renderer.material = Resources.Load("Materials/GlowMat") as Material;
		}
	}

	bool AnyHandClosed(Frame f) {
		int fistCount = 0;
		for (int i = 0; i < f.Hands.Count; ++i) {
			if (f.Hands[i].GrabStrength > 0.4f) {
				fistCount++;
			}
		}
		return fistCount > 1;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject == m_touchObject) {
			m_touchObject.GetComponent<TipLogic>().m_touching = false;
			m_touchObject = null;
			renderer.material = Resources.Load("Materials/TransparentMat") as Material;
		}
	}

	void Update() {

		if (AnyHandClosed(m_leapController.Frame())) {
			transform.position = Vector3.Lerp(transform.position, m_startingPos, Time.deltaTime * 3.0f);
			renderer.material = Resources.Load("Materials/GlowMat") as Material;
		} else if (m_touchObject == null) {
			renderer.material = Resources.Load("Materials/TransparentMat") as Material;
		}

		if (m_touchObject != null) {
			Vector3 newPos = m_touchObject.transform.position - m_touchOffset;
			newPos.z = m_depth;
			transform.position = newPos;
		}

	}
}
