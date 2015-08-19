using UnityEngine;
using System.Collections;

public class UdpReceiver : MonoBehaviour {
	public int listenPort = 2002; // ポートはサーバ・クライアントで合わせる必要がある
	private static bool received = false;

	private struct UdpState {
		public System.Net.IPEndPoint e;
		public System.Net.Sockets.UdpClient u;
	}

	public static void ReceiveCallback(System.IAsyncResult ar) {
		System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)((UdpState)(ar.AsyncState)).u;
		System.Net.IPEndPoint e = (System.Net.IPEndPoint)((UdpState)(ar.AsyncState)).e;
		var receiveBytes = u.EndReceive(ar, ref e);
		var receiveString = System.Text.Encoding.ASCII.GetString(receiveBytes);

		Debug.Log(string.Format("Received: {0}", receiveString)); // ここに任意の処理を書く

		received = true;
	}

	IEnumerator receive_loop() {
		var e = new System.Net.IPEndPoint(System.Net.IPAddress.Any, listenPort);
		var u = new System.Net.Sockets.UdpClient(e);
		var s = new UdpState();
		s.e = e;
		s.u = u;
		for (;;) {
			received = false;
			u.BeginReceive(new System.AsyncCallback(ReceiveCallback), s);
			while (!received) {
				yield return null;
			}
		}
	}

	void Start () {
		StartCoroutine(receive_loop());
	}
}
