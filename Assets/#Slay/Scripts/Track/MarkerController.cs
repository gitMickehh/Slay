using UnityEngine;

namespace Slay
{
    public class MarkerController : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private Transform m_TrackTopLeft;

        [SerializeField]
        private Transform m_TrackTopRight;

        [SerializeField]
        private Transform m_TrackBottomLeft;

        [SerializeField]
        private Transform m_TrackBottomRight;
        #endregion

        #region API
        public float TrackWidth =>
            Mathf.Abs(m_TrackBottomRight.position.x - m_TrackBottomRight.position.x);
        public float TrackHeight =>
            Mathf.Abs(m_TrackBottomRight.position.y - m_TrackTopRight.position.y);
        public float TrackStartX => m_TrackBottomRight.position.x;
        public float TrackStartY => m_TrackBottomRight.position.y;
        public float TrackZ => m_TrackBottomRight.position.z;
        public float TrackEndX => m_TrackBottomLeft.position.x;
        #endregion
    }
}
