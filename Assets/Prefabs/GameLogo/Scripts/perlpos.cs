using UnityEngine;

public class perlpos : MonoBehaviour
{
	[SerializeField]
	bool rotation = false; // set whether to affect rotation or position
	[SerializeField]
	float speed = 3; // set speed of movement
	[SerializeField]
	Vector3 amount = new Vector3(.15f,.15f,.15f);
	Vector3 y;
	Vector3 startpos;

	void Start()
	{
		if (rotation) startpos = transform.localEulerAngles; // if rotation is set to be affected, transform rotation
		else startpos = transform.localPosition; // otherwise, transform position
		y = new Vector3(
			Random.Range(0f, 100f), // random range as float value between 0 and 100
			Random.Range(0f, 100f),
			Random.Range(0f, 100f));
	}

	void Update()
	{
		Vector3 bob = startpos + new Vector3( //new vector 3 called "bob"
			(Mathf.PerlinNoise(Time.time * speed, y.x) - 0.5f)*amount.x, // generate perlin noise to affect movement
			(Mathf.PerlinNoise(Time.time * speed, y.y) - 0.5f)*amount.y,
			(Mathf.PerlinNoise(Time.time * speed, y.z) - 0.5f)*amount.z);
		if (rotation) transform.localRotation = Quaternion.Euler(bob); // if rotation is set to be affected, apply "bob" to rotation
		else transform.localPosition = bob; // otherwise, apply the "bob" to position
	}
}
