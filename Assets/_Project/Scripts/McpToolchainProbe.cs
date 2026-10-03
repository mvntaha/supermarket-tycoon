using UnityEngine;

namespace Stockwell.Diagnostics
{
    /// <summary>
    /// Phase 0 toolchain probe. Confirms the MCP can create, edit and compile a
    /// script in this project. Safe to delete once Phase 1 begins.
    /// </summary>
    public class McpToolchainProbe : MonoBehaviour
    {
        [SerializeField] private string phase = "0";

        private void Start()
        {
            Debug.Log($"[Stockwell] Toolchain probe alive. Phase {phase}, Unity {Application.unityVersion}.");
        }
    }
}
