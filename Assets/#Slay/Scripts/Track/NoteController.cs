using UnityEngine;

namespace Slay
{
    public class NoteController : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private Rigidbody m_RigidBody;

        [SerializeField]
        private MeshRenderer m_MeshRenderer;

        [SerializeField]
        private Material m_MaterialNormal;

        [SerializeField]
        private Material m_MaterialHighlight;
        #endregion

        #region State
        private bool m_IsAtMidline = false;
        public AcapellaTimeseriesPoint Point { get; set; }
        #endregion

        #region API
        public bool IsAtMidline => m_IsAtMidline;
        public Rigidbody Rigidbody => m_RigidBody;
        #endregion

        #region Physics Events
        private void OnTriggerEnter(Collider other)
        {
            m_IsAtMidline = true;
            m_MeshRenderer.material = m_MaterialHighlight;
        }

        private void OnTriggerExit(Collider other)
        {
            m_IsAtMidline = false;
            m_MeshRenderer.material = m_MaterialNormal;
        }
        #endregion
    }
}
